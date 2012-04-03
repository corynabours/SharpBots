using System.Collections.Generic;

namespace Ente
{
    internal class RobotMath
    {
        internal static Data WeightedLinearRegression(List<WeightedData> data)
        {
            var w = new List<double>();
            var x = new List<double>();
            var y = new List<double>();
            var x2 = new List<double>();
            var xy = new List<double>();
            var xs = 0.0;
            var ys = 0.0;
            var x2S = 0.0;
            var xys = 0.0;
            var one = 0.0;
            foreach (var datum in data)
            {
                w.Add(datum.W);
                x.Add(datum.X);
                y.Add(datum.Y);
                x2.Add(datum.X*datum.X);
                xy.Add(datum.X*datum.Y);
                xs += datum.X*datum.W;
                ys += datum.Y*datum.W;
                x2S += datum.X*datum.X*datum.W;
                xys += datum.X*datum.Y*datum.W;
                one += datum.W;
            }

            var div = xs*xs - one*x2S;

            var a = (xs*ys - one*xys)/div;
            var b = (xs*xys - x2S*ys)/div;
            return new Data(a, b);
        }

        internal static double ZeroFixedLinearRegression(List<Data> data)
        {
            var x = new List<double>();
            var y = new List<double>();
            double x2 = 0.0;
            double xy = 0.0;
            foreach (var datum in data)
            {
                x.Add(datum.X);
                y.Add(datum.Y);
                x2 += datum.X*datum.X;
                xy += datum.X*datum.Y;
            }

            return xy/x2;
        }

        internal static double Offset(double headingA, double headingB = 0)
        {
            var myOffset = (headingA - headingB) % 360;
            if (myOffset > 180) myOffset = myOffset - 360;
            return myOffset;
        }

        internal static double ToRad(double x)
        {
            return x*0.0174532925199433;
        }
  
        internal static double ToDeg(double x)
        {
            return x*57.2957795130823;
        }
    }
}
