using System;

namespace Kite2
{
    public class DoubleMath
    {
        public static double deg2rad(double value)
        {
            return value*0.0174532925199433;
        }

        public static double rad2deg(double value)
        {
            return value*57.2957795130823;
        }

        public static double sign(double value)
        {
            return value/Math.Abs(value);
        }
    }
}
