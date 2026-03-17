const CACHE_NAME = 'indiecloud-v1';
const ASSETS = [
    '/',
    '/index.html',
    '/manifest.json',
    '/icon.svg'
];

// Install event - cache static assets
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            return cache.addAll(ASSETS);
        })
    );
});

// Fetch event - network first for API, cache first for static assets
self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    // For API calls, try network first, then cache (though we don't really cache API here)
    if (event.request.url.includes('/Stream')) {
        event.respondWith(fetch(event.request));
        return;
    }

    // For static files (HTML, JS, CSS, images), try cache first, then network
    event.respondWith(
        caches.match(event.request).then(response => {
            return response || fetch(event.request);
        })
    );
});