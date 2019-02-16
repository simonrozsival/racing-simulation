using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace Racing.CircuitGenerator.Output
{
    internal sealed class OccupancyGridGenerator
    {
        private double resolution;

        public OccupancyGridGenerator(double resolution)
        {
            this.resolution = resolution;
        }

        public bool[,] GenerateOccupancyGrid(Bitmap image)
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
    }
}
