window.HarborFlowMap = {
    map: null,
    vesselMarkers: {}, // Object to store markers by MMSI
    portMarkers: [], // Array to store port markers
    dotNetHelper: null,
    currentTileLayer: null,

    tileLayers: {
        openstreetmap: L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }),
        nasa_gibs: L.tileLayer('https://gibs.earthdata.nasa.gov/wmts/epsg3857/best/BlueMarble_ShadedRelief_Bathymetry/default/GoogleMapsCompatible_Level8/{z}/{y}/{x}.jpeg', {
            attribution: '&copy; <a href="https://earthdata.nasa.gov/eosdis/science-system-description/eosdis-components/global-imagery-browse-services-gibs">NASA GIBS</a>',
            maxNativeZoom: 8
        }),
        esri_worldimagery: L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Tiles &copy; Esri — Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community'
        })
    },

    dispose: function () {
        if (this.reportTimeout) {
            clearTimeout(this.reportTimeout);
            this.reportTimeout = null;
        }
        this.dotNetHelper = null;
        if (this.map) {
            this.map.remove();
            this.map = null;
        }
    },

    initMap: function (elementId = 'map', dotNetHelper) {
        if (this.map) {
            this.map.remove();
        }
        this.dotNetHelper = dotNetHelper;
        try {
            this.map = L.map(elementId, {
                center: [1.352083, 103.819836],
                zoom: 12
            });

            // Add Scale Control
            L.control.scale({ position: 'bottomleft' }).addTo(this.map);

            // Add Coordinates Control
            const CoordsControl = L.Control.extend({
                onAdd: function (map) {
                    const div = L.DomUtil.create('div', 'leaflet-control-coordinates');
                    div.innerHTML = `<div id="map-coords-box" class="map-coords-box">
                        <div id="mouse-coords-dms" class="coords-dms">0° 00.00' N <br> 0° 00.00' E</div>
                        <div id="mouse-coords-decimal" class="coords-decimal">(0.0000, 0.0000)</div>
                        <div id="map-zoom-level" class="map-zoom-level">Zoom: 12</div>
                    </div>`;
                    return div;
                }
            });
            new CoordsControl({ position: 'bottomright' }).addTo(this.map);

            this.map.on('mousemove', (e) => {
                const dmsDiv = document.getElementById('mouse-coords-dms');
                const decimalDiv = document.getElementById('mouse-coords-decimal');

                if (dmsDiv && decimalDiv) {
                    const lat = e.latlng.lat;
                    const lng = e.latlng.lng;

                    // Format Decimal
                    decimalDiv.innerText = `(${lat.toFixed(6)}, ${lng.toFixed(6)})`;

                    // Format DMS (Degrees Decimal Minutes)
                    const formatDMS = (val, isLat) => {
                        const absVal = Math.abs(val);
                        const degrees = Math.floor(absVal);
                        const minutes = ((absVal - degrees) * 60).toFixed(2);
                        const direction = isLat ? (val >= 0 ? 'N' : 'S') : (val >= 0 ? 'E' : 'W');
                        return `${degrees}° ${minutes}' ${direction}`;
                    };

                    dmsDiv.innerHTML = `${formatDMS(lat, true)} <br> ${formatDMS(lng, false)}`;
                }
            });

            this.map.on('zoomend', () => {
                const zoomDiv = document.getElementById('map-zoom-level');
                if (zoomDiv) {
                    zoomDiv.innerText = `Zoom: ${this.map.getZoom()}`;
                }
            });

            // Coordinate Grid Layer
            this.gridLayer = L.layerGroup();

            this.drawGrid = function () {
                this.gridLayer.clearLayers();
                const zoom = this.map.getZoom();
                const bounds = this.map.getBounds();
                const north = bounds.getNorth();
                const south = bounds.getSouth();
                const west = bounds.getWest();
                const east = bounds.getEast();

                // Determine step size based on zoom (Nautical Standards - Optimized for Clutter)
                let step = 20; // Default for very low zoom (0-2)

                if (zoom >= 15) step = 0.5 / 60;    // 30 seconds
                else if (zoom >= 13) step = 1 / 60; // 1 minute
                else if (zoom >= 11) step = 5 / 60; // 5 minutes
                else if (zoom >= 10) step = 10 / 60;// 10 minutes
                else if (zoom >= 9) step = 0.5;     // 30 minutes
                else if (zoom >= 7) step = 1;       // 1 degree
                else if (zoom >= 6) step = 2;       // 2 degrees
                else if (zoom >= 5) step = 5;       // 5 degrees
                else if (zoom >= 3) step = 10;      // 10 degrees
                // Zoom < 3 uses step = 20

                // Helper to format DMS with Zero Padding (Nautical Style)
                // Longitude: 3 digits for degrees (000-180)
                // Latitude: 2 digits for degrees (00-90)
                // Minutes: 2 digits (00-59)
                const formatDMS = (val, isLat) => {
                    const absVal = Math.abs(val);
                    const degrees = Math.floor(absVal);
                    const minutes = (absVal - degrees) * 60;

                    // Round minutes to avoid floating point errors for display
                    const roundedMinutes = Math.round(minutes);

                    // Handle case where rounding pushes minutes to 60
                    let finalDegrees = degrees;
                    let finalMinutes = roundedMinutes;
                    if (finalMinutes === 60) {
                        finalDegrees++;
                        finalMinutes = 0;
                    }

                    const degStr = isLat
                        ? finalDegrees.toString().padStart(2, '0')
                        : finalDegrees.toString().padStart(3, '0');

                    const minStr = finalMinutes.toString().padStart(2, '0');
                    const direction = isLat ? (val >= 0 ? 'N' : 'S') : (val >= 0 ? 'E' : 'W');

                    if (step >= 1 && finalMinutes === 0) return `${degStr}° ${direction}`;
                    return `${degStr}° ${minStr}' ${direction}`;
                };

                // Draw Longitude lines (Vertical)
                const startLng = Math.ceil(west / step) * step;
                for (let lng = startLng; lng <= east; lng += step) {
                    // Line
                    L.polyline([[south, lng], [north, lng]], {
                        weight: 1,
                        className: 'leaflet-grid-line' // Color handled by CSS
                    }).addTo(this.gridLayer);

                    // Labels - placed slightly inside bounds
                    const labelHtml = `<div class="leaflet-grid-label lng-label">${formatDMS(lng, false)}</div>`;

                    // Top Label
                    L.marker([north, lng], {
                        icon: L.divIcon({
                            className: 'grid-label-icon',
                            html: labelHtml,
                            iconSize: [80, 20],
                            iconAnchor: [40, 0] // Anchor top-center
                        })
                    }).addTo(this.gridLayer);

                    // Bottom Label
                    L.marker([south, lng], {
                        icon: L.divIcon({
                            className: 'grid-label-icon',
                            html: labelHtml,
                            iconSize: [80, 20],
                            iconAnchor: [40, 20] // Anchor bottom-center
                        })
                    }).addTo(this.gridLayer);
                }

                // Draw Latitude lines (Horizontal)
                const startLat = Math.ceil(south / step) * step;
                for (let lat = startLat; lat <= north; lat += step) {
                    // Line
                    L.polyline([[lat, west], [lat, east]], {
                        weight: 1,
                        className: 'leaflet-grid-line' // Color handled by CSS
                    }).addTo(this.gridLayer);

                    // Labels
                    const labelHtml = `<div class="leaflet-grid-label lat-label">${formatDMS(lat, true)}</div>`;

                    // Left Label
                    L.marker([lat, west], {
                        icon: L.divIcon({
                            className: 'grid-label-icon',
                            html: labelHtml,
                            iconSize: [80, 20],
                            iconAnchor: [0, 10] // Anchor left-center
                        })
                    }).addTo(this.gridLayer);

                    // Right Label
                    L.marker([lat, east], {
                        icon: L.divIcon({
                            className: 'grid-label-icon',
                            html: labelHtml,
                            iconSize: [80, 20],
                            iconAnchor: [80, 10] // Anchor right-center
                        })
                    }).addTo(this.gridLayer);
                }
            };

            this.map.on('moveend zoomend', () => {
                if (this.map.hasLayer(this.gridLayer)) {
                    this.drawGrid();
                }
            });

            // Grid Toggle Control
            const GridControl = L.Control.extend({
                onAdd: (map) => {
                    const container = L.DomUtil.create('div', 'leaflet-bar leaflet-control leaflet-control-custom grid-toggle-control');
                    container.title = "Toggle Coordinate Grid";
                    container.innerHTML = '<span>#</span>';

                    container.onclick = () => {
                        if (this.map.hasLayer(this.gridLayer)) {
                            this.map.removeLayer(this.gridLayer);
                            container.classList.remove('active');
                        } else {
                            this.drawGrid();
                            this.map.addLayer(this.gridLayer);
                            container.classList.add('active');
                        }
                    };
                    return container;
                }
            });
            new GridControl({ position: 'topright' }).addTo(this.map);

            // Fullscreen Control
            const FullscreenControl = L.Control.extend({
                onAdd: (map) => {
                    const container = L.DomUtil.create('div', 'leaflet-bar leaflet-control leaflet-control-custom fullscreen-control');
                    container.title = "Toggle Fullscreen";

                    // Simple Expand Icon (Corners)
                    const expandIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3"/></svg>`;

                    // Simple Compress Icon (Inward Corners)
                    const compressIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M8 3v3a2 2 0 0 1-2 2H3m18 0h-3a2 2 0 0 1-2-2V3m0 18v-3a2 2 0 0 1 2-2h3M3 16h3a2 2 0 0 1 2 2v3"/></svg>`;

                    container.innerHTML = expandIcon;

                    container.onclick = () => {
                        const mapContainer = map.getContainer();
                        if (!document.fullscreenElement) {
                            mapContainer.requestFullscreen().catch(err => {
                                console.error(`Error attempting to enable fullscreen: ${err.message}`);
                            });
                        } else {
                            document.exitFullscreen();
                        }
                    };

                    // Listen for fullscreen changes to update icon
                    document.addEventListener('fullscreenchange', () => {
                        if (document.fullscreenElement) {
                            container.innerHTML = compressIcon;
                        } else {
                            container.innerHTML = expandIcon;
                        }
                        // Give browser a moment to resize, then invalidate map size
                        setTimeout(() => {
                            map.invalidateSize();
                        }, 100);
                    });

                    return container;
                }
            });
            new FullscreenControl({ position: 'topright' }).addTo(this.map);

            this.currentTileLayer = this.tileLayers.openstreetmap;
            this.currentTileLayer.addTo(this.map);

            this.map.on('moveend zoomend', () => this.reportVesselData());
            window.addEventListener('resize', () => this.invalidateMapSize());
            this.reportVesselData(); // Initial report
        } catch (error) {
            console.error("Error initializing map:", error);
        }
    },

    switchLayer: function (layerName) {
        if (this.map && this.tileLayers[layerName]) {
            if (this.currentTileLayer) {
                this.map.removeLayer(this.currentTileLayer);
            }
            this.currentTileLayer = this.tileLayers[layerName];
            this.currentTileLayer.addTo(this.map);
        }
    },

    addPortMarkers: function (ports) {
        if (!this.map) {
            console.error("Map is not initialized. Cannot add port markers.");
            return;
        }

        try {
            this.portMarkers.forEach(marker => this.map.removeLayer(marker));
            this.portMarkers = [];

            const portIcon = L.icon({
                iconUrl: '/images/port-icon.png',
                iconSize: [24, 24], // Slightly larger for better visibility
                iconAnchor: [12, 12],
                popupAnchor: [0, -12]
            });

            if (ports && ports.length > 0) {
                ports.forEach(port => {
                    if (port.LATITUDE && port.LONGITUDE) {
                        const marker = L.marker([port.LATITUDE, port.LONGITUDE], { icon: portIcon }).addTo(this.map);

                        const popupContent = `
                            <div style="min-width: 200px;">
                                <h5 style="margin: 0 0 8px 0; border-bottom: 1px solid #00f2ff; padding-bottom: 4px;">${port.CITY}</h5>
                                <div style="font-size: 14px; line-height: 1.4;">
                                    <div><strong>State:</strong> ${port.STATE || 'N/A'}</div>
                                    <div><strong>Country:</strong> ${port.COUNTRY}</div>
                                    <div style="margin-top: 4px; font-size: 12px; opacity: 0.8;">
                                        Lat: ${port.LATITUDE.toFixed(4)}, Lng: ${port.LONGITUDE.toFixed(4)}
                                    </div>
                                </div>
                            </div>
                        `;

                        marker.bindPopup(popupContent);
                        this.portMarkers.push(marker);

                        marker.on('mouseover', (e) => {
                            this.dotNetHelper.invokeMethodAsync('ShowPortTooltip',
                                port.CITY,
                                port.STATE || '',
                                port.COUNTRY,
                                port.LATITUDE,
                                port.LONGITUDE,
                                e.originalEvent.clientX,
                                e.originalEvent.clientY,
                                window.innerWidth,
                                window.innerHeight
                            );
                        });

                        marker.on('mouseout', () => {
                            this.dotNetHelper.invokeMethodAsync('HidePortTooltip');
                        });

                        marker.on('click', function () {
                            // Future: Invoke DotNet method to show detailed modal
                        });
                    } else {
                        console.warn("Skipping port with invalid coordinates:", port);
                    }
                });
            }
        } catch (error) {
            console.error("Error in addPortMarkers:", error);
        }
    },

    reportTimeout: null,

    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name, type = 'HSC', metadata, vesselId) {
        if (!this.map) return;

        const vesselData = {
            vesselId: vesselId || mmsi,
            vesselName: name,
            vesselType: type,
            imo: metadata ? metadata.imoNumber : 'N/A',
            vesselStatus: 'Active',
            latitude: lat,
            longitude: lng,
            heading: heading,
            speed: speed,
            recordedAt: new Date().toISOString()
        };

        // Optimization: Only create icon if needed or if rotation changed significantly
        // For now, we'll keep the icon update but ensure we throttle the reporting
        const iconUrl = this.getIconUrl(type);
        const vesselIcon = L.divIcon({
            className: 'custom-vessel-icon',
            html: `<img src="${iconUrl}" style="transform: rotate(${heading}deg); width: 48px; height: 48px;" />`,
            iconSize: [48, 48],
            iconAnchor: [24, 24],
            popupAnchor: [0, -24]
        });

        let popupContent = `<b>Vessel:</b> ${name}<br>
                              <b>Type:</b> ${type}<br>
                              <b>Lat:</b> ${lat}<br>
                              <b>Lng:</b> ${lng}<br>
                              <b>Speed:</b> ${speed} knots<br>
                              <b>Heading:</b> ${heading}°`;

        if (metadata) {
            popupContent += `<br><b>Flag:</b> ${metadata.flag}<br>
                             <b>Length:</b> ${metadata.length}m<br>
                             <b>IMO:</b> ${metadata.imoNumber}`;
        }

        if (this.vesselMarkers[mmsi]) {
            // Update existing marker
            const marker = this.vesselMarkers[mmsi];
            marker.setLatLng([lat, lng]);
            marker.setIcon(vesselIcon);
            marker.setPopupContent(popupContent);
            marker.vesselType = type;
            marker.vesselData = vesselData;
        } else {
            // Create new marker
            const newMarker = L.marker([lat, lng], { icon: vesselIcon }).addTo(this.map);
            newMarker.bindPopup(popupContent);
            newMarker.vesselType = type;
            newMarker.vesselData = vesselData;

            newMarker.on('click', () => {
                this.dotNetHelper.invokeMethodAsync('OnVesselSelected', mmsi);
            });

            newMarker.on('mouseover', (e) => {
                this.dotNetHelper.invokeMethodAsync('ShowVesselTooltip',
                    mmsi,
                    name,
                    metadata ? metadata.imoNumber : 'N/A',
                    'Active', // Status
                    type,
                    speed,
                    heading,
                    lat,
                    lng,
                    e.originalEvent.clientX,
                    e.originalEvent.clientY,
                    window.innerWidth,
                    window.innerHeight,
                    vesselId
                );
            });

            newMarker.on('mouseout', () => {
                this.dotNetHelper.invokeMethodAsync('HideVesselTooltip');
            });

            this.vesselMarkers[mmsi] = newMarker;
        }

        // Throttle the reporting to avoid spamming .NET
        this.reportVesselData();
    },

    updateVesselMetadata: function (mmsi, metadata) {
        if (!this.vesselMarkers[mmsi]) return;

        const marker = this.vesselMarkers[mmsi];
        if (marker.vesselData) {
            // Update internal data
            marker.vesselData.metadata = metadata;
            if (metadata.shipName) {
                marker.vesselData.name = metadata.shipName;
            }
        }

        // Re-generate popup content
        const vesselData = marker.vesselData;
        let popupContent = `<h5>${vesselData.name || 'Unknown Vessel'}</h5>
                            <b>MMSI:</b> ${mmsi}<br>
                            <b>Type:</b> ${vesselData.vesselType || 'Other'}<br>
                            <b>Speed:</b> ${vesselData.speed ? vesselData.speed.toFixed(1) : 0} kn<br>
                            <b>Heading:</b> ${vesselData.heading ? vesselData.heading.toFixed(0) : 0}°`;

        if (metadata) {
            popupContent += `<br><b>Flag:</b> ${metadata.flag || 'N/A'}<br>
                             <b>Length:</b> ${metadata.length ? metadata.length + 'm' : 'N/A'}<br>
                             <b>IMO:</b> ${metadata.imoNumber || 'N/A'}`;
        }

        marker.setPopupContent(popupContent);

        // If popup is open, update it immediately
        if (marker.isPopupOpen()) {
            marker.getPopup().setContent(popupContent);
        }
    },

    reportVesselData: function () {
        if (this.reportTimeout) return;

        this.reportTimeout = setTimeout(() => {
            this._executeReportVesselData();
            this.reportTimeout = null;
        }, 1000); // Update stats max once per second
    },

    _executeReportVesselData: function () {
        if (!this.map || !this.dotNetHelper) return;

        const allVesselTypes = {};
        let totalVesselCount = 0;

        for (const mmsi in this.vesselMarkers) {
            totalVesselCount++;
            const vessel = this.vesselMarkers[mmsi];
            const type = vessel.vesselType || 'Other';
            allVesselTypes[type] = (allVesselTypes[type] || 0) + 1;
        }

        const allVesselTypeSummary = Object.keys(allVesselTypes).map(key => {
            return { VesselType: key, Count: allVesselTypes[key] };
        }).sort((a, b) => a.VesselType.localeCompare(b.VesselType));

        this.dotNetHelper.invokeMethodAsync('UpdateTotalMapVesselCount', totalVesselCount);
        this.dotNetHelper.invokeMethodAsync('UpdateVisibleVesselTypeSummary', allVesselTypeSummary);
    },

    getIconUrl: function (vesselType) {
        let type = vesselType ? vesselType.toLowerCase() : 'other';
        switch (type) {
            case 'cargo':
                return '/images/vessels/cargo.png';
            case 'tanker':
                return '/images/vessels/tanker.png';
            case 'passenger':
                return '/images/vessels/passenger.png';
            case 'fishing':
                return '/images/vessels/fishing.png';
            case 'hsc':
                return '/images/vessels/hsc.png';
            case 'tug':
                return '/images/vessels/tug.png';
            case 'sailing':
                return '/images/vessels/sailing.png';
            default:
                return '/images/vessels/other.png';
        }
    },

    invalidateMapSize: function () {
        if (this.map) {
            this.map.invalidateSize();
        }
    },

    filterVessels: function (query) {
        if (!this.map) return;

        const lowerQuery = query ? query.toLowerCase().trim() : '';

        for (const mmsi in this.vesselMarkers) {
            const marker = this.vesselMarkers[mmsi];
            const data = marker.vesselData;

            if (!lowerQuery) {
                // Reset to full visibility
                marker.setOpacity(1.0);
                continue;
            }

            const name = data.vesselName ? data.vesselName.toLowerCase() : '';
            const imo = data.imo ? data.imo.toString().toLowerCase() : '';
            const type = data.vesselType ? data.vesselType.toLowerCase() : '';

            if (name.includes(lowerQuery) || imo.includes(lowerQuery) || type.includes(lowerQuery)) {
                // Match found
                marker.setOpacity(1.0);
                marker.setZIndexOffset(1000); // Bring to front
            } else {
                // No match - dim
                marker.setOpacity(0.2);
                marker.setZIndexOffset(0); // Reset z-index
            }
        }
    },

    setVesselsStale: function (isStale) {
        if (!this.map) return;

        for (const mmsi in this.vesselMarkers) {
            const marker = this.vesselMarkers[mmsi];
            // Get the icon element (divIcon)
            // Note: marker.getElement() might return null if marker is not currently visible (e.g. clustered)
            // But here we are not clustering yet.
            const icon = marker.getElement();
            if (icon) {
                // The image is inside the divIcon
                const img = icon.querySelector('img');
                if (img) {
                    if (isStale) {
                        img.style.filter = 'grayscale(100%) opacity(0.7)';
                    } else {
                        img.style.filter = '';
                    }
                }
            }
        }
    }
};
