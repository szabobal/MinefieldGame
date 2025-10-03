namespace TimerTest
{
    internal class Bomb
    {
        public int pos;
        private Weight weight;
        double lastUpdated;
        int sinkRate;

        public void GetSinkRate()
        {
            switch (weight)
            {
                case Weight.LIGHT:
                    sinkRate = 1000;
                    break;
                case Weight.MEDIUM:
                    sinkRate = 900;
                    break;
                case Weight.HEAVY:
                    sinkRate = 500;
                    break;
            }
        }

        public void Update(double dt)
        {
            lastUpdated += dt;
            if (lastUpdated >= sinkRate)
            {
                pos++;
                lastUpdated = 0;
            }
        }

        public Bomb(int pos, Weight weight)
        {
            this.pos = pos;
            this.weight = weight;
            lastUpdated = 0;
            GetSinkRate();
        }
    }
}
