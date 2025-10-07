using Minefield.Model;
using Minefield.Persistence;
using System.Windows.Forms;

namespace Minefield.View
{
	public partial class MinefieldForm : Form
	{
		#region Fields

		private readonly MinefieldModel _model;
		private Button[,] _grid = null!;
		private readonly Dictionary<string, Image> _images = new Dictionary<string, Image>
		{
			["submarine"] = Image.FromFile("Resources/anothersub.png"),
			["bomb"] = Image.FromFile("Resources/bomb.png"),
			["boat"] = Image.FromFile("Resources/boat.png"),
			["explosion"] = Image.FromFile("Resources/explosion.png")
		};

		#endregion

		#region Constructor

		public MinefieldForm()
		{
			InitializeComponent();

			IMinefieldDataAccess dataAccess = new MinefieldFileDataAccess();
			_model = new MinefieldModel(dataAccess);
			_model.NewGame();
			_model.GameOver += Model_GameOver;
			_model.GameAdvanced += Model_GameAdvanced;

			KeyDown += new KeyEventHandler(MinefieldForm_KeyDown);

			SetupTableLayout();
			GenerateButtonGrid();
		}

		#endregion

		#region Game event handlers

		private void Model_GameAdvanced(object? sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() => Model_GameAdvanced(sender, e)));
				return;
			}
			UpdateGridImages();
		}
		
		private void Model_GameOver(object? sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() => Model_GameOver(sender, e)));
				return;
			}

			MessageBox.Show("Game Over! Submarine hit a bomb!", "Game Over", MessageBoxButtons.OK);
			Application.Exit();
		}
		
		private void MinefieldForm_KeyDown(object? sender, KeyEventArgs e)
		{
			Int32 dx = 0, dy = 0;
			switch (e.KeyCode)
			{
				case Keys.Up: dx = -1; break;
				case Keys.Down: dx = 1; break;
				case Keys.Left: dy = -1; break;
				case Keys.Right: dy = 1; break;
				default: return;
			}

			Int32 previousX = _model.Submarine.X;
			Int32 previousY = _model.Submarine.Y;
			_grid[previousX, previousY].BackgroundImage = null;

			_model.MoveSubmarine(dx, dy);

			Submarine sub = _model.Submarine;
			_grid[sub.X, sub.Y].BackgroundImageLayout = ImageLayout.Zoom;
			_grid[sub.X, sub.Y].BackgroundImage = _images["submarine"];
		}

		#endregion

		#region Menu event handlers

		private void MenuFileNewGame_Click(Object sender, EventArgs e)
		{
			menuFileNewGame.Enabled = true;
			_model.NewGame();
		}

		private async void MenuFileSaveGame_Click(Object sender, EventArgs e)
		{
			Boolean shouldRestart = !_model.IsGameOver;
			_model.PauseGame();
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					await _model.SaveGameAsync(saveFileDialog.FileName);
                }
				catch (Exception)
				{
					MessageBox.Show("Could not save the game!", "Error", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

			if (shouldRestart)
				_model.ResumeGame();
        }

		private async void MenuFileLoadGame_ClickAsync(object sender, EventArgs e)
		{
            Boolean restartTimer = !_model.IsGameOver;
            _model.PauseGame();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        await _model.LoadGameAsync(fileStream);
                    }
                    menuFileSaveGame.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "Játék betöltése sikertelen!" + Environment.NewLine +
                        "Hibás az elérési út, vagy a fájlformátum.", "Hiba!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    _model.NewGame();
                    menuFileSaveGame.Enabled = true;
                }
            }

            if (restartTimer)
                _model.ResumeGame();
        }

		private void MenuFileExitGame_Click(object sender, EventArgs e)
		{
			Boolean shouldRestart = !_model.IsGameOver;
			_model.PauseGame();
			
			if (MessageBox.Show("Are you sure you want to exit?", "Minefield game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				Close();
			else if (shouldRestart)
				_model.ResumeGame();
		}

		#endregion

		#region Private methods

		private void GenerateButtonGrid()
		{
			_grid = new Button[_model.Rows, _model.Columns];
			for (int i = 0; i < _model.Rows; i++)
				for (int j = 0; j < _model.Columns; j++)
				{
					Button b = new GridButton(i, j);
					b.Enabled = false;
					b.TabStop = false;
					b.FlatStyle = FlatStyle.Flat;
					b.BackColor = Color.LightBlue;
					b.ForeColor = Color.White;
					b.FlatStyle = FlatStyle.Flat;
					b.FlatAppearance.BorderColor = Color.LightBlue;
					b.Dock = DockStyle.Fill;

					_grid[i, j] = b;
					tableLayoutPanel.Controls.Add(_grid[i, j]);
				}
		}

		private void SetupTableLayout()
		{
			BackColor = Color.LightBlue;
			tableLayoutPanel.RowCount = _model.Rows;
			tableLayoutPanel.ColumnCount = _model.Columns;

			tableLayoutPanel.ColumnStyles.Clear();
			tableLayoutPanel.RowStyles.Clear();

			for (Int32 i = 0; i < _model.Rows; i++)
				tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / _model.Rows));
			for (Int32 i = 0; i < _model.Columns; i++)
				tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / _model.Columns));
		}

		private void UpdateGridImages()
		{
			for (Int32 i = 0; i < _model.Rows; i++)
				for (Int32 j = 0; j < _model.Columns; j++)
				{
					_grid[i, j].BackgroundImageLayout = ImageLayout.Zoom;
					_grid[i, j].BackgroundImage = null;
				}

			for (Int32 i = 0; i < _model.Rows; i++)
				for (Int32 j = 0; j < _model.Columns; j++)
				{
					if (_model.ContainsBomb(i, j))
					{
						_grid[i, j].BackgroundImage = _images["bomb"];
					}
				}

			for (Int32 i = 0; i < _model.Ships.Length; i++)
			{
				Ship ship = _model.Ships[i];
				_grid[0, ship.Y].BackgroundImage = _images["boat"];
			}

			Submarine sub = _model.Submarine;
			if (_model.ContainsBomb(sub.X, sub.Y))
			{
				_grid[sub.X, sub.Y].BackgroundImage = _images["explosion"];
			}
			else
			{
				_grid[sub.X, sub.Y].BackgroundImage = _images["submarine"];
			}
		}

		#endregion	
	}
}
