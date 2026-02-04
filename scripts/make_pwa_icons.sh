#!/usr/bin/env bash
set -euo pipefail

SRC_PNG="${1:-icon-1024.png}"
OUT_DIR="${2:-pwa/icons}"

if [ ! -f "$SRC_PNG" ]; then
  echo "Source PNG not found: $SRC_PNG" >&2
  exit 1
fi

mkdir -p "$OUT_DIR"

sips -z 192 192 "$SRC_PNG" --out "$OUT_DIR/icon-192.png" >/dev/null
sips -z 512 512 "$SRC_PNG" --out "$OUT_DIR/icon-512.png" >/dev/null

echo "Wrote icons to $OUT_DIR"
