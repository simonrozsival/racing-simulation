namespace Racing.Mathematics
{
    public static class Distance
    {
        public static double Between(Vector a, Vector b)
            => (a - b).CalculateLength();
    }
}
