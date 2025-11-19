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
                                <h5 style="margin: 0 0 8px 0; color: #0d6efd; border-bottom: 1px solid #dee2e6; padding-bottom: 4px;">${port.CITY}</h5>
                                <div style="font-size: 14px; line-height: 1.4;">
                                    <div><strong>State:</strong> ${port.STATE || 'N/A'}</div>
                                    <div><strong>Country:</strong> ${port.COUNTRY}</div>
                                    <div style="margin-top: 4px; font-size: 12px; color: #6c757d;">
                                        Lat: ${port.LATITUDE.toFixed(4)}, Lng: ${port.LONGITUDE.toFixed(4)}
                                    </div>
                                </div>
                            </div>
                        `;

                        marker.bindPopup(popupContent);
                        this.portMarkers.push(marker);

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

    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name, type = 'HSC', metadata) {
        if (!this.map) return;

        const vesselData = {
            vesselId: mmsi,
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

            this.vesselMarkers[mmsi] = newMarker;
        }

        // Throttle the reporting to avoid spamming .NET
        this.reportVesselData();
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
    }
};
