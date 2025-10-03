namespace Minefield.Model
{
    public class Ship
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Int32 ShipDirection { get; set; }

        public Ship(Int32 x, Int32 y, Int32 dir)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "X coordinate must be non-negative.");
            }
            if (y < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y coordinate must be non-negative.");
            }
            X = x;
            Y = y;
            ShipDirection = dir;
        }
    }
}