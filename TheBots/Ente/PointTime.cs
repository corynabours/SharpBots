namespace Ente
{
    class PointTime : Data
    {
        public int ktime { get; set; }
        public PointTime (double x, double y, int time) : base(x,y)
        {
            ktime = time;
        }

    }
}
