import sys
from typing import List, Final


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Image info
    width: Final[int] = 25
    height: Final[int] = 6
    stride: Final[int] = width * height

    # File read stub
    with open(args[1], "r") as f:
        data: str = f.readline().strip()

    # Separate the data into layers
    layers: List[str] = [data[i:i + stride] for i in range(0, len(data), stride)]

    # Find the layer with the least zeroes
    zeroes: int = stride
    h: int = 0
    for layer in layers:
        z = layer.count("0")
        if z < zeroes:
            # Calculate the hash of 1's and 2's
            zeroes = z
            h = layer.count("1") * layer.count("2")

    print(h)

    # Create the array for the final image
    image: List[str] = ["2"] * stride
    for i in range(stride):
        # Check through the layers sequentially
        for layer in layers:
            # If the pixel is not transparent, write it to the layer
            if layer[i] < "2":
                image[i] = layer[i]
                break

    # Replace all numerical values by colours
    image = ["░" if s == "0" else s for s in image]
    image = ["▓" if s == "1" else " " for s in image]
    # Join the image into the right format
    image = ["".join(image[i:i + width]) for i in range(0, len(image), width)]
    print("\n".join(image))


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
