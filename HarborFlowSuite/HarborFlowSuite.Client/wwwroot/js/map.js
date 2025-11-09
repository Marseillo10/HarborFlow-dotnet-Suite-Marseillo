var map;
var vesselMarkers = {};

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

            // Using OpenSeaMap as the base layer
            var seaMarkUrl = 'https://tiles.openseamap.org/seamark/{z}/{x}/{y}.png';
            L.tileLayer(seaMarkUrl, {
                maxZoom: 18,
                attribution: 'Nautical charts &copy; <a href="http://www.openseamap.org">OpenSeaMap</a> contributors'
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
                vesselMarkers[mmsi].setLatLng([lat, lng]);
                vesselMarkers[mmsi].setPopupContent(popupContent);
            } else {
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
            }
        } else {
            console.error('Map not initialized. Cannot update vessel marker.');
        }
    }
};
