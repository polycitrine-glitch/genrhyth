#!/usr/bin/env python3
import argparse
import os
import shutil
import sys

HEAD_TAGS = [
    '<link rel="manifest" href="manifest.json">',
    '<meta name="theme-color" content="#7700FF">',
]

BODY_SNIPPET = """
<script>
if ("serviceWorker" in navigator) {
  window.addEventListener("load", () => {
    navigator.serviceWorker.register("service-worker.js");
  });
}
</script>
""".strip()


def ensure_dir(path):
    os.makedirs(path, exist_ok=True)


def copy_pwa_assets(pwa_dir, build_dir):
    manifest_src = os.path.join(pwa_dir, "manifest.json")
    sw_src = os.path.join(pwa_dir, "service-worker.js")
    icons_src = os.path.join(pwa_dir, "icons")

    if not os.path.isfile(manifest_src) or not os.path.isfile(sw_src):
        raise FileNotFoundError("Missing PWA assets. Ensure pwa/manifest.json and pwa/service-worker.js exist.")

    shutil.copy2(manifest_src, os.path.join(build_dir, "manifest.json"))
    shutil.copy2(sw_src, os.path.join(build_dir, "service-worker.js"))

    if os.path.isdir(icons_src):
        icons_dst = os.path.join(build_dir, "icons")
        ensure_dir(icons_dst)
        for name in os.listdir(icons_src):
            if name.lower().endswith(".png"):
                shutil.copy2(os.path.join(icons_src, name), os.path.join(icons_dst, name))


def patch_index_html(build_dir):
    index_path = os.path.join(build_dir, "index.html")
    if not os.path.isfile(index_path):
        raise FileNotFoundError("index.html not found in build dir.")

    with open(index_path, "r", encoding="utf-8") as f:
        html = f.read()

    if "manifest.json" not in html:
        head_close = html.lower().find("</head>")
        if head_close == -1:
            raise RuntimeError("</head> tag not found.")
        head_insert = "\n" + "\n".join(HEAD_TAGS) + "\n"
        html = html[:head_close] + head_insert + html[head_close:]

    if "service-worker.js" not in html:
        body_close = html.lower().rfind("</body>")
        if body_close == -1:
            raise RuntimeError("</body> tag not found.")
        html = html[:body_close] + "\n" + BODY_SNIPPET + "\n" + html[body_close:]

    with open(index_path, "w", encoding="utf-8") as f:
        f.write(html)


def main():
    parser = argparse.ArgumentParser(description="Add PWA assets + patch index.html for Unity WebGL builds.")
    parser.add_argument("--build", required=True, help="Path to WebGL build folder (contains index.html)")
    parser.add_argument("--pwa", default="pwa", help="Path to pwa assets folder")
    args = parser.parse_args()

    build_dir = os.path.abspath(args.build)
    pwa_dir = os.path.abspath(args.pwa)

    if not os.path.isdir(build_dir):
        print("Build dir not found:", build_dir)
        return 1
    if not os.path.isdir(pwa_dir):
        print("PWA dir not found:", pwa_dir)
        return 1

    copy_pwa_assets(pwa_dir, build_dir)
    patch_index_html(build_dir)
    print("PWA post-build complete for:", build_dir)
    return 0


if __name__ == "__main__":
    sys.exit(main())
