namespace Ente
{
    internal class WeightedData : Data
    {
        public double W { get; set; }
        public WeightedData(double x, double y, double w) : base(x,y)
        {
            W = w;
        }
    }
}
