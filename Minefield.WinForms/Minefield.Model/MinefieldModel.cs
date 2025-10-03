using Minefield.Persistence;
using System;
using System.Timers;

namespace Minefield.Model
{
    public class MinefieldModel
    {
        #region Fields

        private MinefieldGrid _gameArea;
        private Submarine? _submarine;
        
        private System.Timers.Timer _timer;
        private System.Timers.Timer _bombDropTimer;
        private Int32 _timePlayed;
        private Double _initialBombDropInterval = 1000;
        private Double _minBombDropInterval = 800;
        private Double _currentBombDropInterval;

        #endregion

        #region Properties

        public Ship[]? _ships;
        public Submarine GetSubmarine() => _submarine!;
        public Bomb? this[Int32 x, Int32 y] => _gameArea[x, y];

        public Boolean IsGameOver => _submarine!.CollidedWithBomb;

        public Int32 Columns => _gameArea.Columns;
        public Int32 Rows => _gameArea.Rows;

        #endregion

        #region Events

        public event EventHandler? GameOver;
        public event EventHandler? ShipMoved;

        #endregion

        #region Constructor

        public MinefieldModel()
        {
            _gameArea = new MinefieldGrid();
            _submarine = null;
            _timePlayed = 0;

            _timer = new System.Timers.Timer();
            _timer.Interval = 100;
            _timer.Elapsed += Timer_Elapsed;

            _bombDropTimer = new System.Timers.Timer();
            _currentBombDropInterval = _initialBombDropInterval;
            _bombDropTimer.Interval = _initialBombDropInterval;
            _bombDropTimer.Elapsed += BombDropTimer_Elapsed;

            _timer.Start();
            _bombDropTimer.Start();
        }

        #endregion

        #region Public table accessors

        public Bomb? GetBombAt(Int32 x, Int32 y)
        {
            return _gameArea.GetBombAt(x, y);
        }

        public void AddBombAt(Int32 x, Int32 y, Weight weight)
        {
            _gameArea.AddBombAt(x, y, weight);
        }

        public Boolean StepWouldCauseCollision(Int32 x, Int32 y)
        {
            return _gameArea.StepWouldCauseCollision(x, y);
        }

        public Boolean ContainsBomb(Int32 x, Int32 y)
        {
            return _gameArea.ContainsBomb(x, y);
        }

        #endregion

        #region Public game methods

        public void NewGame()
        {
            _gameArea = new MinefieldGrid();
            SetupGrid();
        }

        public void PauseGame()
        {
            _timer.Stop();
            _bombDropTimer.Stop();
        }

        public void ResumeGame()
        {
            if (!IsGameOver)
            {
                _timer.Start();
                _bombDropTimer.Start();
            }
        }

        public void MoveSubmarine(Int32 dx, Int32 dy)
        {
            int newX = _submarine!.X + dx;
            int newY = _submarine.Y + dy;
            if (newX >= 1 && newX < _gameArea.Rows && newY >= 0 && newY < _gameArea.Columns)
            {
                _submarine.X = newX;
                _submarine.Y = newY;
                CheckForSubmarineCollision();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _bombDropTimer?.Dispose();
        }

        #endregion

        #region Private game methods

        private void SetupGrid()
        {
            Random random = new();
            Int32 x = random.Next(2, _gameArea.Rows);
            Int32 y = random.Next(0, _gameArea.Columns);
            _submarine = new Submarine(x, y);

            _ships = new Ship[2];
            _ships[0] = new Ship(0, _gameArea.Columns / 2, -1);
            _ships[1] = new Ship(0, _gameArea.Columns - 1, -1);
        }

        private bool WouldShipsCollide()
        {
            int nextY0 = _ships![0].Y + _ships![0].ShipDirection;
            int nextY1 = _ships![1].Y + _ships![1].ShipDirection;

            return nextY0 == nextY1;
        }

        private void DropBomb(int col)
        {
            Weight[] weights = { Weight.LIGHT, Weight.MEDIUM, Weight.HEAVY };
            Random random = new();
            Weight randomWeight = weights[random.Next(0, weights.Length)];
            if (col >= 0 && col < _gameArea.Columns && !ContainsBomb(1, col))
            {
                AddBombAt(1, col, randomWeight);
                Console.WriteLine($"Bomb dropped at column {col}");
            }
        }

        private void DropBombsFromShips()
        {
            foreach (var ship in _ships!)
            {
                DropBomb(ship.Y);
            }
        }

        private void ShipsMoving()
        {
            bool wouldCollide = WouldShipsCollide();
            bool shouldReverse = false;

            for (int i = 0; i < _ships!.Length; i++)
            {
                if (_ships[i].Y + _ships[i].ShipDirection < 0 || _ships[i].Y + _ships[i].ShipDirection >= _gameArea.Columns)
                {
                    shouldReverse = true;
                    break;
                }
            }

            if (shouldReverse || wouldCollide)
            {
                for (int i = 0; i < _ships.Length; i++)
                {
                    _ships[i].ShipDirection *= -1;
                }
            }

            for (int i = 0; i < _ships.Length; i++)
            {
                _ships[i].Y += _ships[i].ShipDirection;
            }
            OnShipMoved();
        }
        private void UpdateBombDropFrequency()
        {
            Double reductionFactor = 0.98;
            _currentBombDropInterval = Math.Max(_minBombDropInterval, _currentBombDropInterval * reductionFactor);
            _bombDropTimer.Interval = _currentBombDropInterval;
        }

        private Int32 GetFallThreshold(Weight weight)
        {
            return weight switch
            {
                Weight.LIGHT => 12,
                Weight.MEDIUM => 10,
                Weight.HEAVY => 6,
                _ => 10
            };
        }

        private void ClearBottomRow()
        {
            for (int j = 0; j < _gameArea.Columns; j++)
            {
                if (ContainsBomb(_gameArea.Rows - 1, j))
                {
                    _gameArea[_gameArea.Rows - 1, j] = null;
                }
            }
        }

        //private void SinkBomb()
        //{
        //    for (int column = 0; column < _gameArea.Columns; column++)
        //    {
        //        for (int row = _gameArea.Rows - 2; row >= 1; row--)
        //        {
        //            if (ContainsBomb(row, column) && !ContainsBomb(row + 1, column))
        //            {
        //                _gameArea[row + 1, column] = _gameArea[row, column];
        //                _gameArea[row, column] = null;
        //            }
        //        }
        //    }
        //}

        private void SinkBomb()
        {
            for (int column = 0; column < _gameArea.Columns; column++)
            {
                for (int row = _gameArea.Rows - 2; row >= 1; row--)
                {
                    var bomb = _gameArea[row, column];
                    if (bomb != null && !ContainsBomb(row + 1, column))
                    {
                        bomb.FallCounter++;
                        if (bomb.FallCounter >= GetFallThreshold(bomb.Weight))
                        {
                            _gameArea[row + 1, column] = bomb;
                            _gameArea[row, column] = null;
                            bomb.X = row + 1;
                            bomb.FallCounter = 0;
                        }
                    }
                }
            }
        }

        private void CheckForSubmarineCollision()
        {
            if (ContainsBomb(_submarine!.X, _submarine.Y))
            {
                _submarine.CollidedWithBomb = true;
            }
        }

        #endregion

        #region Private event methods

        private void OnGameOver(Int32 timePlayed)
        {
            _timer.Stop();
            _bombDropTimer.Stop();
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        private void OnShipMoved()
        {
            ShipMoved?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Timer event handlers

        private void Timer_Elapsed(Object? sender, EventArgs e)
        {
            if (IsGameOver)
            {
                return;
            }

            _timePlayed += 100;
            if (_timePlayed % 700 == 0)
                ShipsMoving();

            SinkBomb();
            ClearBottomRow();

            if (_timePlayed % 1000 == 0)
            {
                UpdateBombDropFrequency();
            }

            CheckForSubmarineCollision();
            if (_submarine!.CollidedWithBomb)
            {

                OnGameOver(_timePlayed);
            }
        }

        private void BombDropTimer_Elapsed(Object? sender, EventArgs e)
        {
            if (IsGameOver)
                OnGameOver(_timePlayed);

            DropBombsFromShips();
            OnShipMoved();
        }


        #endregion
    }
}
