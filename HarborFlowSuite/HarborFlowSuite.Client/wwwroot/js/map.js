var map;
var vesselMarkers = {};

window.HarborFlowMap = {
    initMap: function (mapElementId) {
        map = L.map(mapElementId).setView([-7.797, 110.37], 10); // Yogyakarta, Indonesia, zoom level 10

        L.tileLayer('https://t2.openseamap.org/tile/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            subdomains: 'abc'
        }).addTo(map);

        L.tileLayer('http://t1.openseamap.org/seamark/{z}/{x}/{y}.png', {
            maxZoom: 18,
            attribution: 'Nautical charts &copy; <a href="http://www.openseamap.org">OpenSeaMap</a> contributors'
        }).addTo(map);
    },
    addMarker: function (lat, lng, title) {
        if (map) {
            L.marker([lat, lng]).addTo(map)
                .bindPopup(title)
                .openPopup();
        }
    },
    updateVesselMarker: function (mmsi, lat, lng, heading, speed, name) {
        if (map) {
            var popupContent = `<b>${name}</b><br>MMSI: ${mmsi}<br>Speed: ${speed} knots<br>Heading: ${heading}°`;
            if (vesselMarkers[mmsi]) {
                vesselMarkers[mmsi].setLatLng([lat, lng]);
                vesselMarkers[mmsi].setPopupContent(popupContent);
            } else {
                vesselMarkers[mmsi] = L.marker([lat, lng], {
                    icon: L.icon({
                        iconUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png',
                        iconSize: [25, 41],
                        iconAnchor: [12, 41],
                        popupAnchor: [1, -34],
                        shadowSize: [41, 41]
                    })
                }).addTo(map)
                    .bindPopup(popupContent);
            }
        }
    }
};

