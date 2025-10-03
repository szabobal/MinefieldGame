using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minefield.Persistence
{
    public class Bomb
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Weight Weight { get; }
        public int FallCounter { get; set; }

        public Bomb(Int32 x, Int32 y, Weight weight)
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
            Weight = weight;
            FallCounter = 0;
        }
    }
}
