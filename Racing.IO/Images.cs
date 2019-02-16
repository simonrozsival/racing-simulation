using Racing.IO.Transformations;
using System.Diagnostics;
using System.DrawingCore;
using System.IO;

namespace Racing.IO
{
    internal static class Images
    {
        public static void Convert(string svgPath, string pngPath)
        {
            var info = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "svgexport",
                Arguments = $"{svgPath} {pngPath}",
                UseShellExecute = true
            };

            using (var process = Process.Start(info))
            {
                process.WaitForExit();
            }
        }

        public static bool[,] LoadOccupancyGrid(string pngFile, double tileSize)
        {
            using (var stream = File.OpenRead(pngFile))
            {
                using (var image = Image.FromStream(stream))
                {
                    var bitmap = new Bitmap(image);
                    return OccupancyGridLoader.GenerateOccupancyGrid(bitmap, tileSize);
                }
            }
        }
    }
}
