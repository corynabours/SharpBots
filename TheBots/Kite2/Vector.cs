using System;

namespace Kite2
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator *(Vector v, double x)
        {
            return new Vector(v.X * x, v.Y * x);
        }

        public static Vector operator -(Vector x, Vector y)
        {
            return new Vector(x.X - y.X, x.Y - y.Y);
        }

        public Vector normalize()
        {
            return this * (1.0 / r());
        }

        public double r()
        {
            return Math.Sqrt(X * X + Y * Y);
        }
    }
}
