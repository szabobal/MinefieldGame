namespace Minefield.Model
{
    public class Submarine
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Boolean CollidedWithBomb { get; set; }
        public Int32 FallCounter { get; set; } = 0;

        public Submarine(Int32 x, Int32 y)
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
        }
    }
}