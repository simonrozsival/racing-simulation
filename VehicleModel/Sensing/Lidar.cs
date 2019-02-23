using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    internal sealed class Lidar : ILidar
    {
        private static readonly Angle angularResolution = Angle.FromDegrees(30);
        private static readonly Length maximumDistance = Length.FromMeters(15);

        public Angle AngularResolution { get; } = angularResolution;
        public Length MaximumDistance { get; } = maximumDistance;
        public Length[] Readings { get; }
         
        private Lidar(Length[] readings)
        {
            Readings = readings;
        }

        public static Lidar From(ITrack track, Vector position)
        {
            return new Lidar(new Distance[] { });
        }
    }
}
