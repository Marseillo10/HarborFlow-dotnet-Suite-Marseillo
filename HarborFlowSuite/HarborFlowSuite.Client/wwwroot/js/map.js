window.HarborFlowMap = {
    map: null,
    vesselMarkers: {}, // Object to store markers by MMSI

    initMap: function (elementId = 'map') {
        if (this.map) {
            this.map.remove();
        }
        this.map = L.map(elementId).setView([1.352083, 103.819836], 12); // Centered on Singapore

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(this.map);

        window.addEventListener('resize', () => this.invalidateMapSize());
    },

    addVesselMarkers: function (vesselPositions) {
        if (this.map && vesselPositions) {
            vesselPositions.forEach(position => {
                this.updateVesselMarker(
                    position.vesselId.toString(),
                    position.latitude,
                    position.longitude,
                    position.heading,
                    position.speed,
                    position.vesselName,
                    position.vesselType
                );
            });
        }
    },

    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name, type = 'HSC') {
        if (!this.map) return;

        const iconUrl = this.getIconUrl(type);
        const vesselIcon = L.divIcon({
            className: 'custom-vessel-icon',
            html: `<img src="${iconUrl}" style="transform: rotate(${heading}deg); width: 48px; height: 48px;" />`,
            iconSize: [48, 48],
            iconAnchor: [24, 24],
            popupAnchor: [0, -24]
        });

        const popupContent = `<b>Vessel:</b> ${name}<br>
                              <b>Type:</b> ${type}<br>
                              <b>Lat:</b> ${lat}<br>
                              <b>Lng:</b> ${lng}<br>
                              <b>Speed:</b> ${speed} knots<br>
                              <b>Heading:</b> ${heading}°`;

        if (this.vesselMarkers[mmsi]) {
            // Update existing marker
            this.vesselMarkers[mmsi].setLatLng([lat, lng]);
            this.vesselMarkers[mmsi].setIcon(vesselIcon);
            this.vesselMarkers[mmsi].setPopupContent(popupContent);
        } else {
            // Create new marker
            const newMarker = L.marker([lat, lng], { icon: vesselIcon }).addTo(this.map);
            newMarker.bindPopup(popupContent);
            this.vesselMarkers[mmsi] = newMarker;
        }
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
