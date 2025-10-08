using Minefield.Persistence;

namespace Minefield.Model
{
    public class MinefieldModel : IDisposable
    {
        #region Fields

        private readonly IMinefieldDataAccess _dataAccess;
        private MinefieldGrid _gameArea;
        private Double _timePlayed;
        private DateTime _lastUpdate;

        private readonly Random _random = new Random();
        private readonly System.Timers.Timer _timer;
        private readonly System.Timers.Timer _bombDropTimer;



        #endregion

        #region Properties

        public List<Bomb> BombsPlaced
        {
            get => _gameArea.BombsPlaced;
            private set => _gameArea.BombsPlaced = value;
        }

        public Ship[] Ships
        {
            get => _gameArea.Ships;
            private set => _gameArea.Ships = value;
        }

        public Submarine Submarine 
        {
            get => _gameArea.Submarine;
            private set => _gameArea.Submarine = value;
        }

        public Bomb? this[Int32 x, Int32 y] => _gameArea[x, y];

        public Boolean IsGameOver => Submarine.CollidedWithBomb;

        public Int32 Columns => _gameArea.Columns;

        public Int32 Rows => _gameArea.Rows;

        #endregion

        #region Events

        public event EventHandler? GameOver;
        public event EventHandler? ShipMoved;
        public event EventHandler? GameAdvanced;

        #endregion

        #region Constructor

        public MinefieldModel(IMinefieldDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _gameArea = new MinefieldGrid();
            _timePlayed = 0;
            _lastUpdate = DateTime.Now;
            BombsPlaced = new List<Bomb>();

            _timer = new System.Timers.Timer(200);
            _timer.Elapsed += Timer_Elapsed;


            _bombDropTimer = new System.Timers.Timer(2500);
            _bombDropTimer.Elapsed += BombDropTimer_Elapsed;
        }

        #endregion

        #region Public table accessors

        public void AddBombAt(Bomb bomb)
        {
            _gameArea.AddBombAt(bomb);
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
            _timer.Start();
            _bombDropTimer.Start();
            GenerateStartingPositions();
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

        public async Task LoadGameAsync(Stream stream)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("Data access not initialized.");
            }

            _gameArea = await _dataAccess.LoadAsync(stream);
        }

        public async Task SaveGameAsync(string path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("Data access not initialized.");
            }
            await _dataAccess.SaveAsync(path, _gameArea);
        }


        public void MoveSubmarine(Int32 dx, Int32 dy)
        {
            int newX = Submarine.X + dx;
            int newY = Submarine.Y + dy;
            if (newX >= 1 && newX < Rows && newY >= 0 && newY < Columns)
            {
                Submarine.X = newX;
                Submarine.Y = newY;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _bombDropTimer?.Dispose();
        }

        #endregion

        #region Private game methods

        private void GenerateStartingPositions()
        {
            Int32 x = _random.Next(2, Rows);
            Int32 y = _random.Next(0, Columns);
            Submarine = new Submarine(x, y);

            Ships = new Ship[4];
            Int32[] cols = Enumerable.Range(0, Columns).ToArray();
            for (Int32 i = 0; i < 4; i++)
            {
                Int32 j = _random.Next(i, Columns);
                (cols[i], cols[j]) = (cols[j], cols[i]);
            }

            for (Int32 i = 0; i < 4; i++)
            {
                Int32 dir = _random.Next(0, 2) == 0 ? -1 : 1;
                Ships[i] = new Ship(cols[i], dir);
            }
        }

        private void DropBomb(Int32 col)
        {
            Weight[] weights = { Weight.LIGHT, Weight.MEDIUM, Weight.HEAVY };
            Weight randomWeight = weights[_random.Next(0, weights.Length)];
            if (col >= 0 && col < Columns && !ContainsBomb(1, col))
            {
                AddBombAt(new Bomb(1, col, randomWeight));
                BombsPlaced.Add(this[1,col]!);
            }
        }

        private void DropBombsFromShips()
        {
            foreach (var ship in Ships)
            {
                DropBomb(ship.Y);
            }
        }

        private void ShipsMoving()
        {
            Int32[] nextY = new Int32[Ships.Length];

            for (Int32 i = 0; i < Ships.Length; i++)
            {
                nextY[i] = Ships[i].Y + Ships[i].ShipDirection;
                if (nextY[i] < 0 || nextY[i] >= Columns)
                {
                    Ships[i].SetDirection(Ships[i].ShipDirection * -1);
                    nextY[i] = Ships[i].Y + Ships[i].ShipDirection;
                }
            }

            for (Int32 i = 0; i < Ships.Length; i++)
                Ships[i].Y = nextY[i];
        }

        private void ClearFromBottomRow(Bomb bomb)
        {
            BombsPlaced.Remove(bomb);
            _gameArea[bomb.X, bomb.Y] = null;
            //for (Int32 j = 0; j < Columns; j++)
            //{
            //    if (ContainsBomb(Rows - 1, j))
            //    {
            //        _bombsPlaced.Remove(this[Rows - 1, j]!);
            //        _gameArea[Rows - 1, j] = null;
            //    }
            //}
        }

        private void SinkBomb(Double dt)
        {
            foreach (var bomb in BombsPlaced)
            {
                Int32 row = bomb.X;
                Int32 column = bomb.Y;
                if (row + 1 >= Rows)
                    ClearFromBottomRow(bomb);
                else if (!ContainsBomb(row + 1, column) && bomb.UpdatePosition(dt))
                {
                    _gameArea[row + 1, column] = bomb;
                    _gameArea[row, column] = null;
                    bomb.X += 1;
                }
                //if (bomb.X == Rows - 1) {
                //    OnBombSinked();
                //}
            }

            //for (Int32 column = 0; column < Columns; column++)
            //    for (Int32 row = Rows - 2; row >= 1; row--)
            //    {
            //        var bomb = this[row, column];
            //        if (bomb != null && !ContainsBomb(row + 1, column))
            //        {
            //            if (bomb.UpdatePosition(dt))
            //            {
            //                _gameArea[row + 1, column] = bomb;
            //                _gameArea[row, column] = null;
            //            }
            //        }

            //        if (bomb.X == Rows)
            //            ClearFromBottomRow(bomb);
            //    }
        }

        private void CheckForSubmarineCollision()
        {
            if (ContainsBomb(Submarine.X, Submarine.Y))
                Submarine.CollidedWithBomb = true;
        }

        private void UpdateInterval()
        {
            _bombDropTimer.Interval = Math.Max(750, _bombDropTimer.Interval - 25);
            Console.WriteLine(_bombDropTimer.Interval);
        }
        #endregion

        #region Private event methods

        private void OnGameAdvanced()
        {
            Double dt = (DateTime.Now - _lastUpdate).TotalMilliseconds;
            _lastUpdate = DateTime.Now;
            _timePlayed += dt;

            ShipsMoving();
            SinkBomb(dt);
            CheckForSubmarineCollision();

            GameAdvanced?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameOver()
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
                return;

            OnGameAdvanced();

            if (Submarine.CollidedWithBomb)
                OnGameOver();
        }

        private void BombDropTimer_Elapsed(Object? sender, EventArgs e)
        {
            DropBombsFromShips();
            UpdateInterval();
        }

        #endregion
    }
}
