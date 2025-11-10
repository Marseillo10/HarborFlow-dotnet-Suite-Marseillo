var map;
var vesselMarkers = {};
var vesselMarkerOrder = []; // To keep track of the order of vessels
const MAX_VESSELS = 200; // Maximum number of vessels to display

window.HarborFlowMap = {
    initMap: function (mapElementId) {
        try {
            var mapContainer = document.getElementById(mapElementId);
            if (!mapContainer) {
                console.error('Map container not found:', mapElementId);
                return;
            }

            console.log('Initializing map on:', mapElementId);
            map = L.map(mapElementId).setView([-7.797, 110.37], 10); // Yogyakarta, Indonesia, zoom level 10

            // Using OpenStreetMap as the base layer
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            console.log('Map initialized successfully.');

        } catch (error) {
            console.error('Error initializing map:', error);
        }
    },
    addMarker: function (lat, lng, title) {
        if (map) {
            L.marker([lat, lng]).addTo(map)
                .bindPopup(title)
                .openPopup();
        } else {
            console.error('Map not initialized. Cannot add marker.');
        }
    },
    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name) {
        if (map) {
            var popupContent = `<b>${name}</b><br>MMSI: ${mmsi}<br>Speed: ${speed} knots<br>Heading: ${heading}°`;
            if (vesselMarkers[mmsi]) {
                // Vessel exists, update its position
                vesselMarkers[mmsi].setLatLng([lat, lng]);
                vesselMarkers[mmsi].setPopupContent(popupContent);
            } else {
                // New vessel, check limit
                if (vesselMarkerOrder.length >= MAX_VESSELS) {
                    // Remove the oldest vessel
                    var oldestMmsi = vesselMarkerOrder.shift(); // Remove from the beginning of the array
                    if (vesselMarkers[oldestMmsi]) {
                        map.removeLayer(vesselMarkers[oldestMmsi]);
                        delete vesselMarkers[oldestMmsi];
                        console.log('Removed oldest vessel:', oldestMmsi);
                    }
                }

                // Add new vessel marker
                console.log('Adding new vessel marker for:', mmsi);
                vesselMarkers[mmsi] = L.marker([lat, lng], {
                    icon: L.icon({
                        iconUrl: 'images/vessel-icon.png', // Using a local icon
                        iconSize: [25, 41],
                        iconAnchor: [12, 41],
                        popupAnchor: [1, -34],
                        shadowSize: [41, 41]
                    })
                }).addTo(map)
                    .bindPopup(popupContent);
                vesselMarkerOrder.push(mmsi); // Add new vessel to the end of the order array
            }
        } else {
            console.error('Map not initialized. Cannot update vessel marker.');
        }
    },
    getVesselMarkerCount: function() {
        return Object.keys(vesselMarkers).length;
    }
};
