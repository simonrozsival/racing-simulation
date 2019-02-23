using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    internal sealed class Lidar : ILidar
    {
        public Angle AngularResolution { get; } = Angle.FromDegrees(30);
        public double MaximumDistance { get; } = 
        public double[] Readings { get; }

        private Lidar(double[] readings)
        {
            Readings = readings;
        }

        public static Lidar From(ITrack track, Vector position)
        {
            
        }
    }
}
