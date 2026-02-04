#!/usr/bin/env python3
import math

size = 1024
radius = 352
cx = cy = size // 2

with open("icon-1024.ppm", "w") as f:
    f.write("P3\n")
    f.write(f"{size} {size}\n")
    f.write("255\n")
    for y in range(size):
        for x in range(size):
            dx = x - cx
            dy = y - cy
            if dx*dx + dy*dy <= radius*radius:
                r, g, b = 119, 0, 255
            else:
                r, g, b = 255, 255, 255
            f.write(f"{r} {g} {b} ")
        f.write("\n")
print("Wrote icon-1024.ppm")
