namespace Minefield.Persistence
{
    public class Ship
    {
        #region Fields

        private Int32 _shipDirection;

        #endregion

        #region Properties

        public Int32 Y { get; set; }
        public Int32 ShipDirection => _shipDirection;

        #endregion

        #region Setters

        public void SetDirection(Int32 dir)
        {
            if (dir != 1 && dir != -1)
            {
                return;
            }

            _shipDirection = dir;
        }

        #endregion

        #region Constructor

        public Ship(Int32 y, Int32 dir)
        {
            if (y < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y coordinate must be non-negative.");
            }
            Y = y;
            _shipDirection = dir;
        }

        #endregion
    }
}