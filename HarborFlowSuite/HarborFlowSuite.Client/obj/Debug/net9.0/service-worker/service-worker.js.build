/* Manifest version: t17XgC5U */
const CACHE_NAME = 'harborflow-map-tiles-v1';
const TILE_SERVER_URL = 'https://tiles.openseamap.org/';

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                // Optionally, pre-cache some initial tiles if known, or just let fetch handle it
                console.log('Service Worker: Cache opened');
                return cache.addAll([]); // No pre-caching for now, fetch will handle it
            })
    );
});

self.addEventListener('fetch', event => {
    // Check if the request is for a map tile from OpenSeaMap
    if (event.request.url.startsWith(TILE_SERVER_URL)) {
        event.respondWith(
            caches.match(event.request)
                .then(response => {
                    // Cache hit - return response
                    if (response) {
                        return response;
                    }

                    // No cache hit - fetch from network, then cache and return
                    return fetch(event.request)
                        .then(networkResponse => {
                            if (networkResponse.ok) {
                                caches.open(CACHE_NAME)
                                    .then(cache => {
                                        cache.put(event.request, networkResponse.clone());
                                    });
                            }
                            return networkResponse;
                        })
                        .catch(error => {
                            console.error('Service Worker: Fetch failed for map tile:', error);
                            // Optionally, return a fallback tile or an error response
                            return new Response('Map tile fetch failed', { status: 404, statusText: 'Map tile not found' });
                        });
                })
        );
    } else {
        // For all other requests, use network-first strategy or default behavior
        event.respondWith(fetch(event.request));
    }
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('Service Worker: Deleting old cache', cacheName);
                        return caches.delete(cacheName);
                    }
                    return null;
                })
            );
        })
    );
});