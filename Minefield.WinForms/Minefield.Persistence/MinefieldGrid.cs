namespace Minefield.Persistence
{
    public class MinefieldGrid
    {
        #region Fields

        private Bomb?[,] _fields; // Játék mezői (aknákkal vagy anélkül).
        private Submarine? _submarine;
        private Ship[]? _ships;
        private List<Bomb> _bombsPlaced;

        #endregion

        #region Properties

        public List<Bomb> BombsPlaced
        {
            get => _bombsPlaced;
            set => _bombsPlaced = value;
        }

        public Submarine Submarine
        {
            get => _submarine!;
            set => _submarine = value;
        }

        public Ship[] Ships
        {
            get => _ships!;
            set => _ships = value;
        }

        /// <summary>
        /// Játékmező oszlopainak száma.
        /// </summary>
        public Int32 Columns => _fields!.GetLength(0);

        /// <summary>
        /// Játékmező sorainak száma.
        /// </summary>
        public Int32 Rows => _fields!.GetLength(1);

        /// <summary>
        /// Mező értékének lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>Bomba példánya vagy null.</returns>
        public Bomb? this[Int32 x, Int32 y]
        {
            get => GetBombAt(x, y);

            set
            {
                if (value == null)
                {
                    _fields![x, y] = null;
                }
                else
                {
                    AddBombAt(new Bomb(x, y, value.Weight));
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Aknamező játéktábla példányosítása
        /// </summary>
        public MinefieldGrid() : this(15, 15)
        {
        }

        /// <summary>
        /// Aknamező játéktábla példányosítása
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public MinefieldGrid(Int32 width, Int32 height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be non-negative.");
            }
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be non-negative.");
            }

            _fields = new Bomb[width, height];
            _submarine = null;
            _ships = null;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Mezőn akna lekérdezése.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Bomb? GetBombAt(Int32 x, Int32 y)
        {
            if (x < 0 || x >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The x coordinate is out of range!");
            }
            if (y < 0 || y >= Columns)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The y coordinate is out of range!");
            }

            return IsBomb(x, y) ? _fields![x, y]! : null;
        }

        /// <summary>
        /// Mezőre akna elhelyezése.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddBombAt(Bomb bomb)
        {
            if (IsBomb(bomb.X, bomb.Y))
            {
                throw new InvalidOperationException("A bomb is already present at the specified coordinates.");
            }
            //_fields![x, y] = new Bomb(x,y,weight);
            _fields[bomb.X, bomb.Y] = bomb;
        }

        /// <summary>
        /// Tartalmaz-e aknát a megadott koordinátán.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean ContainsBomb(Int32 x, Int32 y)
        {
            return IsBomb(x, y);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tartalmaz-e aknát a megadott koordinátán.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Boolean IsBomb(Int32 x, Int32 y)
        {
            return x < 0 || x >= _fields!.GetLength(0)
                ? throw new ArgumentOutOfRangeException(nameof(x), "The x coordinate is out of range!")
                : y < 0 || y >= _fields!.GetLength(1)
                ? throw new ArgumentOutOfRangeException(nameof(y), "The y coordinate is out of range!")
                : _fields[x, y] != null;
        }

        #endregion
    }
}
