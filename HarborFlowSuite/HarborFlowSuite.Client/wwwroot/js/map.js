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
            attribution: '&copy; <a href="https://earthdata.nasa.gov/eosdis/science-system-description/eosdis-components/global-imagery-browse-services-gibs">NASA GIBS</a>'
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
        this.map = L.map(elementId, {
            center: [1.352083, 103.819836],
            zoom: 12
        });

        this.currentTileLayer = this.tileLayers.openstreetmap;
        this.currentTileLayer.addTo(this.map);

        this.map.on('moveend zoomend', () => this.reportVesselData());
        window.addEventListener('resize', () => this.invalidateMapSize());
        this.reportVesselData(); // Initial report
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
        if (!this.map) return;

        console.log("Received ports:", ports);

        this.portMarkers.forEach(marker => this.map.removeLayer(marker));
        this.portMarkers = [];

        const portIcon = L.icon({
            iconUrl: '/images/port-icon.png',
            iconSize: [20, 20],
            iconAnchor: [10, 10],
            popupAnchor: [0, -10]
        });

                    ports.forEach(port => {
                        console.log("Processing port:", port);
                        const marker = L.marker([port.LATITUDE, port.LONGITUDE], { icon: portIcon }).addTo(this.map);
                        marker.bindPopup(`<b>City:</b> ${port.CITY}<br><b>STATE:</b> ${port.STATE}<br><b>Country:</b> ${port.COUNTRY}`);
                        this.portMarkers.push(marker);
        
                        marker.on('click', function() {
                            console.log('Port marker clicked:', port.CITY);
                        });
                    });    },

    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name, type = 'HSC', metadata) {
        if (!this.map) return;

        const vesselData = {
            vesselId: mmsi, // Using MMSI as a unique ID for now
            vesselName: name,
            vesselType: type,
            imo: metadata ? metadata.imoNumber : 'N/A',
            vesselStatus: 'Active', // Placeholder, actual status would come from AIS data
            latitude: lat,
            longitude: lng,
            heading: heading,
            speed: speed,
            recordedAt: new Date().toISOString() // Placeholder for current time
        };

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
            this.vesselMarkers[mmsi].setLatLng([lat, lng]);
            this.vesselMarkers[mmsi].setIcon(vesselIcon);
            this.vesselMarkers[mmsi].setPopupContent(popupContent);
            this.vesselMarkers[mmsi].vesselType = type; // Update vessel type
            this.vesselMarkers[mmsi].vesselData = vesselData; // Store full vessel data
        } else {
            // Create new marker
            const newMarker = L.marker([lat, lng], { icon: vesselIcon }).addTo(this.map);
            newMarker.bindPopup(popupContent);
            newMarker.vesselType = type; // Store vessel type
            newMarker.vesselData = vesselData; // Store full vessel data
            this.vesselMarkers[mmsi] = newMarker;

            newMarker.on('mouseover', (e) => {
                if (this.dotNetHelper) {
                    this.dotNetHelper.invokeMethodAsync('ShowVesselTooltip', newMarker.vesselData, e.originalEvent.clientX, e.originalEvent.clientY);
                }
            });
            newMarker.on('mouseout', () => {
                if (this.dotNetHelper) {
                    this.dotNetHelper.invokeMethodAsync('HideVesselTooltip');
                }
            });
        }
        this.reportVesselData();
    },

    reportVesselData: function () {
        if (!this.map || !this.dotNetHelper) return;

        const bounds = this.map.getBounds();
        const allVesselTypes = {};

        for (const mmsi in this.vesselMarkers) {
            const vessel = this.vesselMarkers[mmsi];
            const type = vessel.vesselType || 'Other';
            allVesselTypes[type] = (allVesselTypes[type] || 0) + 1;
        }

        const allVesselTypeSummary = Object.keys(allVesselTypes).map(key => {
            return { VesselType: key, Count: allVesselTypes[key] };
        }).sort((a, b) => a.VesselType.localeCompare(b.VesselType));

        const totalVesselCount = Object.keys(this.vesselMarkers).length;

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
