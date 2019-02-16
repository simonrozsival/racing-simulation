using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace RaceCircuitGenerator.Output
{
    internal sealed class OccupancyGridGenerator
    {
        private double resolution;

        public OccupancyGridGenerator(double resolution)
        {
            this.resolution = resolution;
        }

        public string[]GenerateOccupancyGrid(Bitmap image)
        {
            var grid = calculateOccupationGrid(image);
            return printOccupationGrid(grid);
        }

        private bool[,] calculateOccupationGrid(Bitmap image)
        {
            var backgroundColor = image.GetPixel(0, 0); // we always assume the top left corner is the background color
            var width = (int)(image.Width / resolution);
            var height = (int)(image.Height / resolution);

            var grid = new bool[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[y, x] = isFree(image, backgroundColor, x, y);
                }
            }

            return grid;
        }

        private bool isFree(Bitmap image, Color backgroundColor, int tileX, int tileY)
        {
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    var pixelX = (int)(tileX * resolution + i);
                    var pixelY = (int)(tileY * resolution + j);
                    var color = image.GetPixel(pixelX, pixelY);
                    if (color == backgroundColor)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static string[] printOccupationGrid(bool[,] grid)
        {
            var lines = new List<string>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                var builder = new StringBuilder();
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var symbol = grid[i, j] ? ' ' : '#';
                    builder.Append(symbol);
                }

                lines.Add(builder.ToString());
            }

            return lines.ToArray();
        }
    }
}
