namespace Minefield.View
{
    internal class GridButton : Button
    {
        private Int32 _x;
        private Int32 _y;
        public Int32 CoordX { get { return _x; } }
        public Int32 CoordY { get { return _y; } }

        public GridButton(Int32 x, Int32 y)
        {
            _x = x;
            _y = y;
        }
    }
}
