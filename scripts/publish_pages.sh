#!/usr/bin/env bash
set -euo pipefail

BUILD_DIR="${1:-WebGLBuild}"
PUBLISH_DIR="docs"

if [ ! -d "$BUILD_DIR" ]; then
  echo "Build dir not found: $BUILD_DIR" >&2
  exit 1
fi

rm -rf "$PUBLISH_DIR"
mkdir -p "$PUBLISH_DIR"
cp -R "$BUILD_DIR"/* "$PUBLISH_DIR"/

# Add PWA assets and patch index.html
./scripts/pwa_postbuild.py --build "$PUBLISH_DIR"

echo "Published to $PUBLISH_DIR. Commit and push to GitHub, then enable Pages on /docs."
