window.HarborFlowMap = {
    map: null,
    vesselMarkers: {}, // Object to store markers by MMSI
    dotNetHelper: null,

    initMap: function (elementId = 'map', dotNetHelper) {
        if (this.map) {
            this.map.remove();
        }
        this.dotNetHelper = dotNetHelper;
        this.map = L.map(elementId, {
            center: [1.352083, 103.819836],
            zoom: 12
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(this.map);

        this.map.on('moveend zoomend', () => this.reportVesselData());
        window.addEventListener('resize', () => this.invalidateMapSize());
        this.reportVesselData(); // Initial report
    },

    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name, type = 'HSC', metadata) {
        if (!this.map) return;

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
        } else {
            // Create new marker
            const newMarker = L.marker([lat, lng], { icon: vesselIcon }).addTo(this.map);
            newMarker.bindPopup(popupContent);
            newMarker.vesselType = type; // Store vessel type
            this.vesselMarkers[mmsi] = newMarker;
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
