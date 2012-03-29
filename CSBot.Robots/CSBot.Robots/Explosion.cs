namespace CSBot.Robots
{
    public class Explosion
    {
        private Battlefield _battlefield;
        private bool _dead;

        public int X { get; private set; }
        public int Y { get; private set; }
        public int T { get; private set; }
        public int Size { get { return 60; } }

        public bool Dead 
        {
            get { return _dead; }
        }

        public void Tick()
        {
            T += 1;
            _dead = (T > 15);
        }

        public Explosion(Battlefield bf, int x, int y)
        {
            X = x;
            Y = y;
            T = 0;
            _battlefield = bf;
            _dead = false;
        }
    }
}
