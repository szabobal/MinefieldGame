using Minefield.Model;

namespace Minefield.View
{
    public partial class MinefieldForm : Form
    {
        private Dictionary<string, Image> _images = new Dictionary<string, Image>
        {
            ["submarine"] = Image.FromFile("Resources/submarine.png"),
            ["bomb"] = Image.FromFile("Resources/bomb.png"),
            ["boat"] = Image.FromFile("Resources/boat.png")
        };

        private MinefieldModel _model;
        private Button[,]? _grid;

        public MinefieldForm()
        {
            InitializeComponent();

            _model = new MinefieldModel();  
            _model.NewGame();
            _model.ShipMoved += Model_ShipMoved;
            _model.GameOver += Model_GameOver;


            KeyDown += new KeyEventHandler(MinefieldForm_KeyDown);

            _grid = null;
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

        private void Model_ShipMoved(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Model_ShipMoved(sender, e)));
            }

            UpdateGridImages();
        }

        private void UpdateGridImages()
        {
            for (int i = 0; i < _model.Rows; i++)
            {
                for (int j = 0; j < _model.Columns; j++)
                {
                    _grid[i, j].BackgroundImage = null;
                }
            }

            for (int i = 0; i < _model.Columns; i++)
            {
                for (int j = 0; j < _model.Columns; j++)
                {
                    if (_model.ContainsBomb(i, j))
                    {
                        _grid[i, j].BackgroundImageLayout = ImageLayout.Zoom;
                        _grid[i, j].BackgroundImage = _images["bomb"];
                    }
                }
            }
            for (int i = 0; i < _model._ships.Length; i++)
            {
                var ship = _model._ships[i];
                _grid[ship.X, ship.Y].BackgroundImage = _images["boat"];
            }

            var sub = _model.GetSubmarine();
            _grid[sub.X, sub.Y].BackgroundImage = _images["submarine"];
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

            Int32 previousX = _model.GetSubmarine().X;
            Int32 previousY = _model.GetSubmarine().Y;
            _grid[previousX, previousY].BackgroundImage = null;


            _model.MoveSubmarine(dx, dy);
            _grid[_model.GetSubmarine().X, _model.GetSubmarine().Y].BackgroundImage = _images["submarine"];
        }

        private void LoadImage()
        {
            _grid[_model.GetSubmarine().X, _model.GetSubmarine().Y].BackgroundImage = _images["submarine"];
            for (int i = 0; i < _model._ships.Length; i++)
            {
                _grid[_model._ships[i].X, _model._ships[i].Y].BackgroundImage = _images["boat"];
            }
        }

        private void MinefieldForm_Load(object sender, EventArgs e)
        {
            BackColor = Color.LightBlue;
            Int32 rowCount = _model.Rows, colCount = _model.Columns;
            tableLayoutPanel.RowCount = rowCount;
            tableLayoutPanel.ColumnCount = colCount;

            _grid = new Button[rowCount, colCount];

            tableLayoutPanel.ColumnStyles.Clear();
            tableLayoutPanel.RowStyles.Clear();
            for (int i = 0; i < colCount; i++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / colCount));
            }
            for (int i = 0; i < rowCount; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    _grid[i, j] = new GridButton(i, j);
                    _grid[i, j].Enabled = false;
                    _grid[i, j].TabStop = false;
                    _grid[i, j].FlatStyle = FlatStyle.Flat;
                    _grid[i, j].BackColor = Color.LightBlue;
                    _grid[i, j].ForeColor = Color.White;
                    _grid[i, j].FlatStyle = FlatStyle.Flat;
                    _grid[i, j].FlatAppearance.BorderColor = Color.LightBlue;
                    _grid[i, j].Dock = DockStyle.Fill;
                    tableLayoutPanel.Controls.Add(_grid[i, j]);
                }
            }

            LoadImage();
        }
    }
}
