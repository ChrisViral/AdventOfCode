using System.Text;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 20
/// </summary>
public sealed class Day20 : Solver<Day20.Tile[]>
{
    /// <summary>
    /// Image tile
    /// </summary>
    public sealed class Tile
    {
        /// <summary>
        /// Character representing a block pixel
        /// </summary>
        private const char BLACK = '#';
        /// <summary>
        /// Position of every monster "pixel"
        /// </summary>
        private static readonly Vector2<int>[] Monster =
        [
            new(0,  1),
            new(1,  2),
            new(4,  2),
            new(5,  1),
            new(6,  1),
            new(7,  2),
            new(10, 2),
            new(11, 1),
            new(12, 1),
            new(13, 2),
            new(16, 2),
            new(17, 1),
            new(18, 0),
            new(18, 1),
            new(19, 1)
        ];

        private readonly char[][] image;
        private char[] top;
        private char[] bottom;
        private char[] left;
        private char[] right;
        private readonly bool ignoreBorders;

        /// <summary>
        /// Tile ID
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Tile size length
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets the character at the given position in the image
        /// </summary>
        /// <param name="pos">Position to get</param>
        /// <returns>The character at the specified position</returns>
        public char this[in Vector2<int> pos] => this.image[pos.Y][pos.X];

        /// <summary>
        /// Creates a new tile from the given data and ID
        /// </summary>
        /// <param name="id">Tile ID</param>
        /// <param name="lines">Tile image data</param>
        public Tile(int id, IReadOnlyList<string> lines)
        {
            //Setup
            this.ID = id;
            this.Size = lines.Count;
            this.image = new char[this.Size][];
            this.top = new char[this.Size];
            this.bottom = new char[this.Size];
            this.left = new char[this.Size];
            this.right = new char[this.Size];

            //Copy input over
            foreach (int i in ..this.Size)
            {
                this.image[i] = lines[i].ToCharArray();
            }

            //Get borders
            this.image[0].CopyTo(this.top, 0);
            this.image[^1].CopyTo(this.bottom, 0);
            foreach (int j in ..this.Size)
            {
                this.left[j] = this.image[j][0];
                this.right[j] = this.image[j][^1];
            }
        }

        /// <summary>
        /// Creates a new tile from the given image
        /// </summary>
        /// <param name="image">Image data to create the tile for</param>
        public Tile(char[][] image)
        {
            this.image = image;
            this.Size = image.Length;
            this.ignoreBorders = true;
            this.top = this.bottom = this.left = this.right = [];
        }

        /// <summary>
        /// Flips the image vertically
        /// </summary>
        private void FlipVertical()
        {
            for (int i = 0; i < this.Size / 2; /*i++*/)
            {
                AoCUtils.Swap(ref this.image[i++], ref this.image[^i]);
            }

            if (this.ignoreBorders) return;

            this.left.ReverseInPlace();
            this.right.ReverseInPlace();
            (this.top, this.bottom) = (this.bottom, this.top);
        }

        /// <summary>
        /// Flips the image horizontally
        /// </summary>
        private void FlipHorizontal()
        {
            this.image.ForEach(i => i.ReverseInPlace());

            if (this.ignoreBorders) return;

            this.top.ReverseInPlace();
            this.bottom.ReverseInPlace();
            (this.left, this.right) = (this.right, this.left);
        }

        /// <summary>
        /// Rotates the image counter clockwise
        /// </summary>
        private void Rotate()
        {
            foreach (int j in ..(this.Size / 2))
            {
                foreach (int i in ..(this.Size / 2))
                {
                    ref char topLeft = ref this.image[j][i];
                    AoCUtils.Swap(ref topLeft, ref this.image[^(i + 1)][j]);
                    AoCUtils.Swap(ref topLeft, ref this.image[^(j + 1)][^(i + 1)]);
                    AoCUtils.Swap(ref topLeft, ref this.image[i][^(j + 1)]);
                }
            }

            if (this.ignoreBorders) return;

            (this.left, this.top) = (this.top, this.left);
            (this.bottom, this.top) = (this.top, this.bottom);
            (this.right, this.top) = (this.top, this.right);

            this.left.ReverseInPlace();
            this.right.ReverseInPlace();
        }

        /// <summary>
        /// Modifies the image to try all layouts and yields after adopting a new one
        /// </summary>
        /// <returns>An enumerable of <see langword="null"/> that yields after adopting a new layout</returns>
        private IEnumerable<object?> AllLayouts()
        {
            yield return null;
            FlipVertical();
            yield return null;
            FlipHorizontal();
            yield return null;
            FlipVertical();
            yield return null;
            Rotate();
            yield return null;
            FlipVertical();
            yield return null;
            FlipHorizontal();
            yield return null;
            FlipVertical();
            yield return null;
        }

        /// <summary>
        /// Checks if the image is a corner image
        /// </summary>
        /// <param name="others">Other image to use to check for corner status</param>
        /// <returns>True if the image is a corner, false otherwise</returns>
        /// ReSharper disable once CognitiveComplexity
        public bool IsCorner(IEnumerable<Tile> others)
        {
            int adjacent = 0;
            foreach (Tile tile in others)
            {
                if (tile == this) continue;

                foreach (int _ in ..4)
                {
                    if (AdjacentBottom(tile))
                    {
                        if (adjacent is 2)
                        {
                            return false;
                        }

                        adjacent++;
                        break;
                    }

                    Rotate();
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if another tile can be adjacent to it on the right
        /// </summary>
        /// <param name="tile">Tile to check against</param>
        /// <returns>True if the other tile can be adjacent to the right, false otherwise</returns>
        public bool AdjacentRight(Tile tile) => tile.AllLayouts().Any(_ => this.right.SequenceEqual(tile.left));

        /// <summary>
        /// Checks if another tile can be adjacent to it on the bottom
        /// </summary>
        /// <param name="tile">Tile to check against</param>
        /// <returns>True if the other tile can be adjacent to the bottom, false otherwise</returns>
        public bool AdjacentBottom(Tile tile) => tile.AllLayouts().Any(_ => this.bottom.SequenceEqual(tile.top));

        /// <summary>
        /// Strips the image from it's borders
        /// </summary>
        /// <returns></returns>
        public char[][] RemoveBorders()
        {
            char[][] stripped = this.image[1..^1];
            for (int i = 0; i < stripped.Length; i++)
            {
                stripped[i] = stripped[i][1..^1];
            }
            return stripped;
        }

        /// <summary>
        /// Calculates the roughness of the water by arranging the image correctly and counting the monsters found
        /// </summary>
        /// <returns>The roughness of the water for the given image</returns>
        public int CalculateRoughness()
        {
            int monsters = AllLayouts().Select(_ => CountMonsters()).FirstOrDefault(m => m is not 0) * Monster.Length;
            return this.image.Sum(l => l.Count(c => c is BLACK)) - monsters;
        }

        /// <summary>
        /// Counts the amount of monsters in the image
        /// </summary>
        /// <returns>How many monsters we're found</returns>
        private int CountMonsters()
        {
            int monsters = 0;
            foreach (int j in ..(this.Size - 3))
            {
                char[][] slice = this.image[j..(j + 3)];
                foreach (int i in ..(this.Size - 20))
                {
                    int end = i + 20;
                    char[][] box = slice[..];
                    box[0] = box[0][i..end];
                    box[1] = box[1][i..end];
                    box[2] = box[2][i..end];
                    if (Monster.All(m => box[m.Y][m.X] is BLACK))
                    {
                        monsters++;
                    }
                }
            }
            return monsters;
        }

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => this.ID;

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            StringBuilder builder = new(this.Size * this.Size);
            this.image.ForEach(l => builder.Append(l).AppendLine());
            return builder.ToString();
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Tile"/>[] fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Tile topLeftCorner = this.Data.First(t => t.IsCorner(this.Data));
        HashSet<Tile> notPlaced = new(this.Data);

        Tile? adjacent = topLeftCorner;
        List<Tile> row = [];
        while (adjacent is not null)
        {
            Tile left = adjacent;
            row.Add(left);
            notPlaced.Remove(left);
            adjacent = notPlaced.FirstOrDefault(left.AdjacentRight);
        }

        List<List<Tile>> image = [row];
        while (!notPlaced.IsEmpty)
        {
            adjacent = notPlaced.First(row[0].AdjacentBottom);
            row = new List<Tile>(row.Count);
            while (adjacent is not null)
            {
                Tile left = adjacent;
                row.Add(left);
                notPlaced.Remove(left);
                adjacent = notPlaced.FirstOrDefault(left.AdjacentRight);
            }
            image.Add(row);
        }
        AoCUtils.LogPart1((long)image[0][0].ID * image[0][^1].ID * image[^1][0].ID * image[^1][^1].ID);

        int strippedSize = topLeftCorner.Size - 2;
        char[][] fullImage = new char[image.Count * strippedSize][];
        foreach (int i in ..fullImage.Length)
        {
            fullImage[i] = new char[image[0].Count * strippedSize];
        }
        for (int j = 0; j < image.Count; j++)
        {
            row = image[j];
            int verticalOffset = j * strippedSize;
            for (int i = 0; i < row.Count; i++)
            {
                Tile tile = row[i];
                int y = 0;
                int horizontalOffset = i * strippedSize * sizeof(char);
                foreach (char[] line in tile.RemoveBorders())
                {
                    Buffer.BlockCopy(line, 0, fullImage[verticalOffset + y++], horizontalOffset, strippedSize * sizeof(char));
                }
            }
        }

        Tile fullTile = new(fullImage);
        AoCUtils.LogPart2(fullTile.CalculateRoughness());
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Tile[] Convert(string[] rawInput)
    {
        Tile[] tiles = new Tile[rawInput.Length / 11];
        for (int i = 0, j = 0; i < rawInput.Length; i += 10, j++)
        {
            int id = int.Parse(rawInput[i++][5..^1]);
            tiles[j] = new Tile(id, rawInput[i..(i + 10)]);
        }
        return tiles;
    }
}
