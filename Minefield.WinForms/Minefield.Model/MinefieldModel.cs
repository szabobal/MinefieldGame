using Minefield.Persistence;

namespace Minefield.Model
{
	public class MinefieldModel : IDisposable
	{
		#region Fields

		private readonly IMinefieldDataAccess _dataAccess;
		private MinefieldGrid _gameArea;
		private readonly System.Timers.Timer _timer;
		private Double _timePlayed;
		private DateTime _lastUpdate;
        private readonly System.Timers.Timer _bombDropTimer;
		private readonly Double _startingDropInterval = 2600;
		private readonly Double _minDropInterval = 750;

		#endregion

		#region Properties

		public Ship[] Ships
		{
			get => _gameArea.Ships ?? Array.Empty<Ship>(); 
			private set => _gameArea.Ships = value;
        }

        public Submarine Submarine 
		{
			get => _gameArea.Submarine; 
			private set => _gameArea.Submarine = value;
        }

        public Bomb? this[Int32 x, Int32 y] => _gameArea[x, y];

		public Boolean IsGameOver => Submarine!.CollidedWithBomb;

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

			_timer = new System.Timers.Timer(200);
			_timer.Elapsed += Timer_Elapsed;


			_bombDropTimer = new System.Timers.Timer(_startingDropInterval);
			_bombDropTimer.Elapsed += BombDropTimer_Elapsed;

		}

		#endregion

		#region Public table accessors

		public void AddBombAt(Int32 x, Int32 y, Weight weight)
		{
			_gameArea.AddBombAt(x, y, weight);
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
			int newX = Submarine!.X + dx;
			int newY = Submarine.Y + dy;
			if (newX >= 1 && newX < _gameArea.Rows && newY >= 0 && newY < _gameArea.Columns)
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
			Random random = new();
			Int32 x = random.Next(2, _gameArea.Rows);
			Int32 y = random.Next(0, _gameArea.Columns);
			Submarine = new Submarine(x, y);

			Ships = new Ship[4];
			int[] cols = Enumerable.Range(0, _gameArea.Columns).ToArray();
			for (int i = 0; i < 4; i++)
			{
				int j = random.Next(i, _gameArea.Columns);
				(cols[i], cols[j]) = (cols[j], cols[i]);
            }

			for (int i = 0; i < 4; i++)
			{
				int dir = random.Next(0, 2) == 0 ? -1 : 1;
				Ships[i] = new Ship(cols[i], dir);
            }
		}

		private void DropBomb(int col)
		{
			Weight[] weights = { Weight.LIGHT, Weight.MEDIUM, Weight.HEAVY };
			Random random = new();
			Weight randomWeight = weights[random.Next(0, weights.Length)];
			if (col >= 0 && col < _gameArea.Columns && !ContainsBomb(1, col))
			{
				AddBombAt(1, col, randomWeight);
			}
		}

		private void DropBombsFromShips()
		{
			foreach (var ship in Ships!)
			{
				DropBomb(ship.Y);
			}
		}

		private void ShipsMoving()
		{
			var ships = Ships!;
			int n = ships.Length;
			int[] nextY = new int[n];

			for (int i = 0; i < n; i++)
			{
				nextY[i] = ships[i].Y + ships[i].ShipDirection;
				if (nextY[i] < 0 || nextY[i] >= _gameArea.Columns)
				{
					ships[i].SetDirection(ships[i].ShipDirection * -1);
					nextY[i] = ships[i].Y + ships[i].ShipDirection;
				}
			}

            for (int i = 0; i < n; i++)
				ships[i].Y = nextY[i];

			OnShipMoved();
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

		private void SinkBomb(Double dt)
		{
			for (int column = 0; column < _gameArea.Columns; column++)
				for (int row = _gameArea.Rows - 2; row >= 1; row--)
				{
					var bomb = this[row, column];
					if (bomb != null && !ContainsBomb(row + 1, column))
					{
						if (bomb.UpdatePosition(dt))
						{
							_gameArea[row + 1, column] = bomb;
							_gameArea[row, column] = null;
						}
					}
				}
		}

		private void CheckForSubmarineCollision()
		{
			if (ContainsBomb(Submarine!.X, Submarine.Y))
				Submarine.CollidedWithBomb = true;
		}

		private void UpdateInterval()
		{
			_bombDropTimer.Interval = Math.Max(_minDropInterval, _bombDropTimer.Interval - 25);
			Console.WriteLine(_bombDropTimer.Interval);
		}
		#endregion

		#region Private event methods

		private void OnGameAdvanced()
		{
			Double dt = (DateTime.Now - _lastUpdate).TotalMilliseconds;
			_lastUpdate = DateTime.Now;

			_timePlayed += dt;
			//if (_timePlayed >= 200)
			//{
			//	ShipsMoving();
			//	_timePlayed = 0;
			//}

			ShipsMoving();
			SinkBomb(dt);

			CheckForSubmarineCollision();

			GameAdvanced?.Invoke(this, EventArgs.Empty);

			ClearBottomRow();
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
			{
				return;
			}

			OnGameAdvanced();

			if (Submarine!.CollidedWithBomb)
			{
				OnGameOver();
			}
		}

		private void BombDropTimer_Elapsed(Object? sender, EventArgs e)
		{
			DropBombsFromShips();
			UpdateInterval();
		}

		#endregion
	}
}
