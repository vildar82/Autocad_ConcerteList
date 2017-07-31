using System;

namespace Autocad_ConcerteList.Lib.Blocks.Dublicate.Tree
{
    public struct PointTree : IEquatable<PointTree>
    {
        public static readonly double tolerance = CheckDublicateBlocks.Tolerance.EqualPoint;
        public readonly double X;
        public readonly double Y;
        private readonly int hX;
        private readonly int hY;

        public PointTree(double x, double y)
        {
            X = x;
            Y = y;
            hX = X.GetHashCode();
            hY = Y.GetHashCode();
        }

        public bool Equals(PointTree other)
        {
            return Math.Abs(X - other.X) < tolerance &&
                   Math.Abs(Y - other.Y) < tolerance;
        }

        public override int GetHashCode()
        {
            return hX ^ hY;
        }
    }
}
