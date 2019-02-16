using System;
using System.DrawingCore;

namespace Racing.IO.Transformations
{
    internal static class OccupancyGridLoader
    {
        public static bool[,] GenerateOccupancyGrid(Bitmap image, double tileSize)
        {
            var backgroundColor = image.GetPixel(0, 0); // we always assume the top left corner is the background color
            var width = (int)(image.Width / tileSize);
            var height = (int)(image.Height / tileSize);

            var grid = new bool[height, width];

            bool tileIsFree(int x, int y)
                => isFree(image, backgroundColor, x, y, tileSize);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[y, x] = tileIsFree(x, y);
                }
            }

            return grid;
        }

        private static bool isFree(Bitmap image, Color backgroundColor, int tileX, int tileY, double tileSize)
        {
            for (int i = 0; i < tileSize; i++)
            {
                for (int j = 0; j < tileSize; j++)
                {
                    var pixelX = (int)(tileX * tileSize + i);
                    var pixelY = (int)(tileY * tileSize + j);
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
