namespace CSBot.Robots
{
    public class Explosion
    {
        private int _x;
        private int _y;
        private int _t;
        private Battlefield _battlefield;
        private bool _dead;

        public bool Dead 
        {
            get { return _dead; }
        }

        public void Tick()
        {
            _t += 1;
            _dead = (_t > 15);
        }

        public Explosion(Battlefield bf, int x, int y)
        {
            _x = x;
            _y = y;
            _t = 0;
            _battlefield = bf;
            _dead = false;
        }
    }
}
