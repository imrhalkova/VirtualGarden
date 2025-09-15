#nullable enable

using System.Diagnostics.Tracing;

namespace VirtualGarden
{
    public partial class Form1 : Form
    {
        private Garden _garden;
        readonly string _picturesPath;
        UIState _state = UIState.GARDEN;
        private TableLayoutPanel _gardenPanel = new TableLayoutPanel();
        private Label _moneyLabel = new Label();
        private Label _dayLabel = new Label();
        private Button[,] _tileButtons;
        private (int, int)? _chosenTile;

        public Form1()
        {
            InitializeComponent();
            _garden = new Garden(4, 4, new Player(20, 20));
            _picturesPath = Path.Combine(Application.StartupPath, "pictures");
            _tileButtons = new Button[_garden.Grid.GetLength(0), _garden.Grid.GetLength(1)];
            GardenUIInitialization();
            UpdateUI();
        }

        public enum UIState
        {
            GARDEN,
            TILE_INFO
        }

        private void UpdateUI()
        {
            switch (_state)
            {
                case UIState.GARDEN:
                    _gardenPanel.Visible = true;
                    _moneyLabel.Text = $"Money: {_garden.Player.Money}";
                    _dayLabel.Text = $"Day: {_garden.Player.playerStatistics.numberOfDay}";
                    UpdateTilesUI();
                    break;
            }
        }

        private void GardenUIInitialization()
        {
            _gardenPanel.Dock = DockStyle.Fill;
            _gardenPanel.BackColor = Color.YellowGreen;
            _gardenPanel.RowCount = 2;
            _gardenPanel.ColumnCount = 3;
            
            _gardenPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            _gardenPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 80));

            _gardenPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            _gardenPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _gardenPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            //Add a new day button
            _gardenPanel.Controls.Add(CreatePanelWithNewDayButton(), 1, 0);

            //Add panel with player stats
            _gardenPanel.Controls.Add(CreatePlayerStatsPanel(), 2, 0);

            //Add the grid of tiles
            _gardenPanel.Controls.Add(CreateTilesPanel(), 1, 1);


            this.Controls.Add(_gardenPanel);
        }

        private Panel CreatePanelWithNewDayButton()
        {
            Button newDayButton = new Button();
            newDayButton.Text = "New Day";
            newDayButton.BackColor = Color.Gold;
            newDayButton.FlatStyle = FlatStyle.Flat;
            newDayButton.Anchor = AnchorStyles.Top;
            newDayButton.Click += NewDayButton_Click;

            Panel newDayButtonCell = new Panel();
            newDayButtonCell.Dock = DockStyle.Fill;
            const double newDayButtoncellPercentageWidth = 0.5;
            const double newDayButtoncellPercentageHeight = 0.7;
            ResizeControl(newDayButton, newDayButtonCell, newDayButtoncellPercentageWidth, newDayButtoncellPercentageHeight);
            newDayButtonCell.Resize += (s, e) => ResizeControl(newDayButton, newDayButtonCell, newDayButtoncellPercentageWidth, newDayButtoncellPercentageHeight);
            newDayButtonCell.Controls.Add(newDayButton);

            return newDayButtonCell;
        }

        private Panel CreatePlayerStatsPanel()
        {
            TableLayoutPanel playerStatsPanel = new TableLayoutPanel();
            playerStatsPanel.Dock = DockStyle.Fill;
            playerStatsPanel.RowCount = 2;
            playerStatsPanel.ColumnCount = 1;

            playerStatsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            playerStatsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            Color playerStatsBackColor = Color.PowderBlue;
            _moneyLabel.Dock = DockStyle.Fill;
            _moneyLabel.BackColor = playerStatsBackColor;
            _moneyLabel.TextAlign = ContentAlignment.TopLeft;
            playerStatsPanel.Controls.Add(_moneyLabel, 0, 0);

            _dayLabel.Dock = DockStyle.Fill;
            _dayLabel.BackColor = playerStatsBackColor;
            _dayLabel.TextAlign = ContentAlignment.TopLeft;
            playerStatsPanel.Controls.Add(_dayLabel, 0, 1);

            return playerStatsPanel;
        }

        private Panel CreateTilesPanel()
        {
            TableLayoutPanel tilesPanel = new TableLayoutPanel();
            tilesPanel.Dock = DockStyle.Fill;
            tilesPanel.RowCount = _garden.Grid.GetLength(0);
            tilesPanel.ColumnCount = _garden.Grid.GetLength(1);

            for (int i = 0; i < tilesPanel.RowCount; i++)
            {
                tilesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tilesPanel.RowCount));
            }

            for (int i = 0; i < tilesPanel.ColumnCount; i++)
            {
                tilesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / tilesPanel.ColumnCount));
            }

            for (int i = 0; i < tilesPanel.RowCount; i++)
            {
                for (int j = 0; j < tilesPanel.ColumnCount; j++)
                {
                    _tileButtons[i, j] = new Button();
                    _tileButtons[i, j].Dock = DockStyle.Fill;
                    _tileButtons[i, j].BackColor = Color.SaddleBrown;
                    _tileButtons[i, j].Click += (s, e) => Tile_Click(i, j);
                    tilesPanel.Controls.Add(_tileButtons[i, j], j, i);
                }
            }

            return tilesPanel;
        }

        private void ResizeControl(Control control, Panel panel, double widthPercentage, double heightPercentage)
        {
            control.Size = new Size(
                (int)(panel.Width * widthPercentage),
                (int)(panel.Height * heightPercentage)
            );
            control.Location = new Point(
                (panel.Width - control.Width) / 2,
                (panel.Height - control.Height) / 2
            );
        }

        private void NewDayButton_Click(object? sender, EventArgs e)
        {
            _garden.NewDay();
            UpdateUI();
        }

        private void Tile_Click(int row, int column)
        {
            _state = UIState.TILE_INFO;
            _chosenTile = (row, column);
            UpdateUI();
        }

        private void PutImageOnTile(Image image, Button buttonTile)
        {
            buttonTile.Image = image;
            buttonTile.BackgroundImageLayout = ImageLayout.Stretch;
            buttonTile.ImageAlign = ContentAlignment.MiddleCenter;
        }

        private void UpdateTilesUI()
        {
            for (int i = 0; i < _tileButtons.GetLength(0); i++)
            {
                for (int j = 0; j < _tileButtons.GetLength(1); j++)
                {
                    _tileButtons[i, j].Text = "";
                    Tile tile = _garden.GetTile(i, j);
                    if (tile.Flower is not null){
                        FlowerState flowerState = tile.Flower.State;
                        switch (flowerState)
                        {
                            case FlowerState.Growing:
                                //show growing image on tile
                                break;
                            case FlowerState.Blooming:
                                string imageFilename = tile.Flower.FlowerType.ImageFilename;
                                string path = Path.Combine(_picturesPath, imageFilename);
                                try
                                {
                                    Image image = FileHandler.GetImageFromFile(path);
                                    PutImageOnTile(image, _tileButtons[i, j]);
                                }
                                catch (FileException e)
                                {
                                    //write e.message into error output
                                    _tileButtons[i, j].Text = "Image not loaded.";
                                }
                                
                                break;
                            case FlowerState.Dead:
                                //show dead image on tile
                                break;
                        }
                    }
                }
            }
        }
    }
}
