#!/usr/bin/env python3
import sys
from math import sqrt
from PIL import Image, ImageDraw


def main():
    out = sys.argv[1] if len(sys.argv) > 1 else "icon-1024.png"
    size = 1024
    img = Image.new("RGBA", (size, size), (255, 255, 255, 255))
    draw = ImageDraw.Draw(img)
    # circle with generous margin
    margin = 160
    draw.ellipse([margin, margin, size - margin, size - margin], fill=(119, 0, 255, 255))
    img.save(out)
    print("Wrote", out)


if __name__ == "__main__":
    main()
