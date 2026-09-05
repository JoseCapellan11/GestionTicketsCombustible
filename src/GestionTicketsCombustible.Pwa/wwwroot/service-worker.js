const CACHE_NAME = "gestion-combustible-pwa-v5";
const ARCHIVOS_APP_SHELL = [
  "/",
  "/index.html",
  "/despacho.html",
  "/manifest.json",
  "/icons/icon.svg",
  "/css/estilos.css",
  "/js/api.js",
  "/js/login.js",
  "/js/despacho.js",
  "/js/registro-sw.js"
];

self.addEventListener("install", (evento) => {
  evento.waitUntil(
    caches.open(CACHE_NAME).then((cache) => cache.addAll(ARCHIVOS_APP_SHELL))
  );
  self.skipWaiting();
});

self.addEventListener("activate", (evento) => {
  evento.waitUntil(
    caches.keys().then((nombresCache) =>
      Promise.all(
        nombresCache
          .filter((nombre) => nombre !== CACHE_NAME)
          .map((nombre) => caches.delete(nombre))
      )
    )
  );
  self.clients.claim();
});

self.addEventListener("fetch", (evento) => {
  const url = new URL(evento.request.url);

  if (url.origin !== self.location.origin) {
    return;
  }

  evento.respondWith(
    caches.match(evento.request).then((respuestaCache) => respuestaCache || fetch(evento.request))
  );
});