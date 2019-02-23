using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    internal sealed class Lidar : ILidar
    {
        public Angle AngularResolution { get; } = Angle.FromDegrees(30);
        public Distance MaximumDistance { get; } = Distance.FromMeters(15);
        public Distance[] Readings { get; }
         
        private Lidar(Distance[] readings)
        {
            Readings = readings;
        }

        public static Lidar From(ITrack track, Vector position)
        {
            return new Lidar(new Distance[] { });
        }
    }
}
