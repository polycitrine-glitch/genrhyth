# Genrhyth PWA assets

Drop these files into your WebGL build folder:
- manifest.json
- service-worker.js
- icons/icon-192.png
- icons/icon-512.png

Then edit the generated index.html to include:

<link rel="manifest" href="manifest.json">
<meta name="theme-color" content="#7700FF">

<script>
if ("serviceWorker" in navigator) {
  window.addEventListener("load", () => {
    navigator.serviceWorker.register("service-worker.js");
  });
}
</script>

Bump the CACHE_NAME in service-worker.js whenever you rebuild to force updates.
