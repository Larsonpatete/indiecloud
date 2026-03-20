const CACHE_NAME = 'indiecloud-v2';
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

// Activate event - clear old caches
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(
                keys.filter(key => key !== CACHE_NAME)
                    .map(key => caches.delete(key))
            );
        })
    );
});

// Fetch event - network first for API, network first for static assets to ensure updates during dev
self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    // For API calls
    if (event.request.url.includes('/Stream')) {
        event.respondWith(fetch(event.request));
        return;
    }

    // For static files (HTML, JS, CSS, images), try network first, then fallback to cache
    // This helps tremendously during development so you aren't stuck on old versions
    event.respondWith(
        fetch(event.request).catch(() => caches.match(event.request))
    );
});