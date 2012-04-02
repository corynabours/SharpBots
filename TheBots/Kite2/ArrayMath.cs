using System;
using System.Collections.Generic;
using System.Linq;

namespace Kite2
{
    public class ArrayMath
    {
        public double average(List<double> values)
        {
            var result = values.Sum();
            return result / values.Count;
        }
        public double sd(List<double> values)
        {
            var avg = average(values);
            var sum = values.Sum(value => Math.Pow(value - avg, 2));
            return Math.Sqrt(sum / (values.Count - 1.0));

        }
    }
}
