let map;
let vesselMarkers = [];

// --- Map Initialization and Markers ---
window.initMap = (options) => {
    map = L.map('map').setView([options.lat, options.lng], options.zoom);
    setTileLayer('OpenStreetMap'); // Set default layer
};

window.addVesselMarkers = (vessels) => {
    if (!map) return;
    vesselMarkers.forEach(marker => marker.remove());
    vesselMarkers = [];

    vessels.forEach(vessel => {
        const icon = L.icon({
            iconUrl: `/images/vessel-icons/${vessel.type}.svg`,
            iconSize: [32, 32],
            iconAnchor: [16, 16]
        });
        const markerOptions = { imo: vessel.imo, icon: icon };
        const marker = L.marker([vessel.lat, vessel.lng], markerOptions)
            .addTo(map)
            .bindPopup(`<b>${vessel.name}</b><br>IMO: ${vessel.imo}<br>Speed: ${vessel.speed} knots`);
        vesselMarkers.push(marker);
    });
};

// --- Map View and Layers ---
window.flyToBookmark = (bounds) => {
    if (map) {
        map.flyToBounds([
            [bounds.North, bounds.East],
            [bounds.South, bounds.West]
        ]);
    }
};

window.getMapBounds = () => {
    return map ? map.getBounds() : null;
};

window.setTileLayer = (layerName) => {
    if (!map) return;

    map.eachLayer(layer => {
        if (layer instanceof L.TileLayer) {
            map.removeLayer(layer);
        }
    });

    let tileUrl;
    let options = { attribution: '' };

    switch (layerName) {
        case 'EsriWorldImagery':
            tileUrl = 'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}';
            options.attribution = 'Tiles &copy; Esri';
            break;
        case 'NOAA':
            tileUrl = 'https://tileservice.charts.noaa.gov/tiles/50000_1/{z}/{x}/{y}.png';
            options.attribution = 'NOAA';
            break;
        case 'OpenStreetMap':
        default:
            tileUrl = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
            options.attribution = '&copy; OpenStreetMap contributors';
            break;
    }

    L.tileLayer(tileUrl, options).addTo(map);
};

window.updateVesselPosition = (position) => {
    if (!map) return;

    let vesselMarker = vesselMarkers.find(m => m.options.imo === position.vesselImo);

    if (vesselMarker) {
        // Marker exists, update its position
        vesselMarker.setLatLng([position.latitude, position.longitude]);
    } else {
        // Marker does not exist, create a new one
        const icon = L.icon({
            iconUrl: '/images/vessel-icons/vessel.svg',
            iconSize: [32, 32],
            iconAnchor: [16, 16]
        });
        const markerOptions = { imo: position.vesselImo, icon: icon };
        const newMarker = L.marker([position.latitude, position.longitude], markerOptions)
            .addTo(map)
            .bindPopup(`<b>IMO: ${position.vesselImo}</b><br>Name: Loading...`);
        vesselMarkers.push(newMarker);
    }
};


window.scrollToElement = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};
