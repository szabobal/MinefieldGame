namespace Minefield.Persistence
{
    public class Bomb
    {
        #region Fields

        private Int32 _sinkRate;
        private Double lastUpdated;

        #endregion

        #region Properties

        public Int32 X {get; set;}
        public Int32 Y {get; set;}

        public Weight Weight { get; private set; }

        #endregion

        #region Constructor

        public Bomb(Int32 x, Int32 y, Weight weight)
        {
            X = x;
            Y = y;
            Weight = weight;
            SetSinkRate();
        }

        #endregion

        #region Public methods

        public Boolean UpdatePosition(Double dt)
        {
            lastUpdated += dt;
            if (lastUpdated >= _sinkRate)
            {
                X += 1;
               lastUpdated = 0;
                return true;
            }
            return false;
        }

        #endregion

        #region Private methods

        private void SetSinkRate()
        {
            switch (Weight)
            {
                case Weight.LIGHT:
                    _sinkRate = 1300;
                    break;
                case Weight.MEDIUM:
                    _sinkRate = 1000;
                    break;
                case Weight.HEAVY:
                    _sinkRate = 800;
                    break;
            }
        }

        #endregion
    }
}
