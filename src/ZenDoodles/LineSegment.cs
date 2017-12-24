using System;

namespace ZenDoodles
{
    class LineSegment
    {
        public DVector2 Start { get; set; }
        public DVector2 End { get; set; }

        public double Length()
        {
            return (End - Start).Length();
        }

        public double Angle()
        {
            return (End - Start).Angle();
        }

        public static bool Intersects(LineSegment line1, LineSegment line2, out DVector2 intersection)
        {
            intersection = DVector2.Zero;

            DVector2 l1Difference = line1.End - line1.Start;
            DVector2 l2Difference = line2.End - line2.Start;
            DVector2 startDifference = line2.Start - line1.Start;

            double cross = l1Difference.Cross(l2Difference);

            // Collinear or parallel
            if (IsZero(cross))
            {
                return false;
            }

            double t = startDifference.Cross(l2Difference) / cross;
            double u = startDifference.Cross(l1Difference) / cross;

            // Intersection, trace out the point from the start
            if ((0 < t && t < 1) && (0 < u && u < 1))
            {
                intersection = line1.Start + l1Difference * t;

                return true;
            }

            return false;
        }

        private static bool IsZero(double d)
        {
            return Math.Abs(d) < 1e-10;
        }
    }
}
