#nullable enable

using System.Diagnostics.Tracing;

namespace VirtualGarden
{
    public partial class Form1 : Form
    {
        private Garden _garden;
        readonly string _picturesPath;
        UIState _state;
        private TableLayoutPanel _gardenPanel = new TableLayoutPanel();
        private TableLayoutPanel _errorPanel = new TableLayoutPanel();
        private TableLayoutPanel[,] _tilePanels;
        private Label _moneyLabel = new Label();
        private Label _dayLabel = new Label();
        private Button[,] _tileButtons;
        private Label _errorMessage = new Label();
        const string _noErrorText = "No errors";
        private Button _clearErrorMessageButton = new Button();
        private (int, int)? _chosenTile;

        public Form1()
        {
            InitializeComponent();
            _garden = new Garden(4, 4, new Player(20, 20));
            _picturesPath = Path.Combine(Application.StartupPath, "pictures");
            _tileButtons = new Button[_garden.Rows, _garden.Columns];
            _tilePanels = new TableLayoutPanel[_garden.Rows, _garden.Columns];
            GameUIInitialization();
            //GardenUIInitialization();
            //TilesInfoUIInitialization();
            _state = UIState.MAINMENU;
            UpdateUI();
        }

        public enum UIState
        {
            MAINMENU,
            GAMEMENU,
            GARDEN,
            TILE_INFO
        }

        private void UpdateUI()
        {
            switch (_state)
            {
                case UIState.MAINMENU:

                    break;
                case UIState.GARDEN:
                    /*
                    _gardenPanel.Visible = true;
                    _moneyLabel.Text = $"Money: {_garden.Player.Money}";
                    _dayLabel.Text = $"Day: {_garden.Player.playerStatistics.numberOfDay}";
                    UpdateTilesUI();
                    */
                    break;
                case UIState.TILE_INFO:
                    break;
            }
        }

        private void GameUIInitialization()
        {
            TableLayoutPanel gamePanel = new TableLayoutPanel();
            gamePanel.RowCount = 2;
            gamePanel.ColumnCount = 1;
            gamePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 95));
            gamePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            gamePanel.BackColor = Color.YellowGreen;
            gamePanel.Dock = DockStyle.Fill;
            gamePanel.Visible = true;

            //Add error panel
            CreateErrorPanel();
            gamePanel.Controls.Add(_errorPanel, 0, 1);
            
            this.Controls.Add(gamePanel);
        }

        /// <summary>
        /// Creates a panel on the buttom of the screen for displaying error messages raised by the program.
        /// The exceptions belong to the program's exception hierarchy.
        /// </summary>
        private void CreateErrorPanel()
        {
            _errorPanel.Dock = DockStyle.Fill;
            _errorPanel.BackColor = Color.White;
            _errorPanel.RowCount = 1;
            _errorPanel.ColumnCount = 2;
            _errorPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90));
            _errorPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _errorPanel.Visible = false;

            _errorMessage.Dock = DockStyle.Fill;
            _errorMessage.Anchor = AnchorStyles.Left;
            _errorMessage.Text = _noErrorText;
            _errorPanel.Controls.Add(_errorMessage, 0, 0);

            _clearErrorMessageButton.Dock = DockStyle.Fill;
            _clearErrorMessageButton.Text = "Clear";
            _clearErrorMessageButton.BackColor = Color.MistyRose;
            _clearErrorMessageButton.FlatStyle = FlatStyle.Flat;
            _clearErrorMessageButton.Click += ClearErrorMessageButton_Click;
            _errorPanel.Controls.Add(_clearErrorMessageButton, 1, 0);
        }

        private void GardenUIInitialization()
        {
            _gardenPanel.Dock = DockStyle.Fill;
            //_gardenPanel.BackColor = Color.YellowGreen;
            _gardenPanel.RowCount = 2;
            _gardenPanel.ColumnCount = 3;
            _gardenPanel.Visible = false;
            
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
            _gardenPanel.Controls.Add(CreateTilesGridPanel(), 1, 1);

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

        private Panel CreateTilesGridPanel()
        {
            TableLayoutPanel tilesPanel = new TableLayoutPanel();
            tilesPanel.Dock = DockStyle.Fill;
            tilesPanel.RowCount = _garden.Rows;
            tilesPanel.ColumnCount = _garden.Columns;

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

        private void TilesInfoUIInitialization()
        {
            Button tileInfoBackButton = CreateTileInfoBackButton();
            for (int i = 0; i < _garden.Rows; i++)
            {
                for (int j = 0; j < _garden.Columns; j++)
                {
                    _tilePanels[i, j] = new TableLayoutPanel();
                    _tilePanels[i, j].RowCount = 3;
                    _tilePanels[i, j].ColumnCount = 1;
                    _tilePanels[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 15));
                    _tilePanels[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 70));
                    _tilePanels[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 15));
                    _tilePanels[i, j].Dock = DockStyle.Fill;
                    _tilePanels[i, j].Visible = false;

                    TableLayoutPanel firstRow = new TableLayoutPanel();
                    firstRow.RowCount = 1;
                    firstRow.ColumnCount = 3;
                    firstRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
                    firstRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
                    firstRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

                    firstRow.Controls.Add(tileInfoBackButton, 0, 0);
                    firstRow.Controls.Add(CreatePlayerStatsPanel(), 2, 0);
                }
            }
        }

        private Button CreateTileInfoBackButton()
        {
            Button tileInfoBackButton = new Button();
            tileInfoBackButton.Text = "Back";
            tileInfoBackButton.BackColor = Color.MistyRose;
            tileInfoBackButton.Click += TileInfoBackButton_Click;
            tileInfoBackButton.Dock = DockStyle.Fill;
            return tileInfoBackButton;
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

        private void TileInfoBackButton_Click(object? sender, EventArgs e)
        {
            _state = UIState.GARDEN;
            UpdateUI();
        }

        private void Tile_Click(int row, int column)
        {
            _state = UIState.TILE_INFO;
            _chosenTile = (row, column);
            UpdateUI();
        }

        private void ClearErrorMessageButton_Click(object? sender, EventArgs e)
        {
            _errorMessage.Text = _noErrorText;
            _errorPanel.Visible = false;
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
