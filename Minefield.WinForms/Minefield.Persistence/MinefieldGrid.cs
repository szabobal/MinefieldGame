using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minefield.Persistence
{
    public class MinefieldGrid
    {
        #region Fields

        private Bomb?[,] _fields; // Játékmező mezői (aknákkal vagy anélkül).

        #endregion

        #region Properties

        /// <summary>
        /// Játékmező oszlpainak száma.
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
                    AddBombAt(x, y, value.Weight);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Aknamező játéktábla példányosítása
        /// </summary>
        public MinefieldGrid() : this(10, 10)
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

            return ContainsBomb(x, y) ? _fields![x, y]! : null;
        }

        /// <summary>
        /// Mezőre akna elhelyezése.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddBombAt(Int32 x, Int32 y, Weight weight)
        {
            if (x < 0 || x >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The x coordinate is out of range!");
            }
            if (y < 0 || y >= Columns)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The y coordinate is out of range!");
            }

            if (ContainsBomb(x, y))
            {
                throw new InvalidOperationException("A bomb is already present at the specified coordinates.");
            }
            _fields![x, y] = new Bomb(x, y, weight);
        }

        /// <summary>
        /// Lépés ütközést okozna-e.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean StepWouldCauseCollision(Int32 x, Int32 y)
        {
            if (x < 0 || x >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The x coordinate is out of range!");
            }
            if (y < 0 || y >= Columns)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The y coordinate is out of range!");
            }
            return IsBomb(x, y);
        }

        /// <summary>
        /// Tartalmaz-e aknát a megadott koordinátán.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean ContainsBomb(Int32 x, Int32 y)
        {
            if (x < 0 || x >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The x coordinate is out of range!");
            }
            if (y < 0 || y >= Columns)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The y coordinate is out of range!");
            }

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