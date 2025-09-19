#nullable enable

using System.Diagnostics.Tracing;

namespace VirtualGarden
{
    public partial class Form1 : Form
    {
        internal Garden? _garden;
        readonly string _picturesPath;
        private UIState _state;
        TableLayoutPanel _gamePanel = new TableLayoutPanel();
        TableLayoutPanel _mainMenuPanel = new TableLayoutPanel();
        private TableLayoutPanel _gardenPanel = new TableLayoutPanel();
        private TableLayoutPanel _errorPanel = new TableLayoutPanel();
        TableLayoutPanel _tileInfoPanel = new TableLayoutPanel();
        private Label _moneyLabel = new Label();
        private Label _moneyLabel2 = new Label();
        private Label _dayLabel = new Label();
        private Button[,] _tileButtons;
        private Label _errorMessage = new Label();
        private const string _noErrorText = "No errors";
        private Button _clearErrorMessageButton = new Button();

        public Form1()
        {
            InitializeComponent();
            _picturesPath = Path.Combine(Application.StartupPath, "pictures");
            GameUIInitialization();
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

        internal static class TileControls
        {
            internal static (int, int) _chosenTile {  get; set; }

            internal static Label RowLabel {  get; } = new Label();
            internal static Label ColumnLabel { get; } = new Label();
            internal static Label FlowerLabel { get; } = new Label();
            internal static Label WeedLabel { get; } = new Label();
            internal static Label BugsLabel { get; } = new Label();
            internal static Label CoinsLabel { get; } = new Label();
            
            internal static void Inicialize()
            {
                RowLabel.TextAlign = ContentAlignment.MiddleCenter;
                ColumnLabel.TextAlign = ContentAlignment.MiddleCenter;
                FlowerLabel.TextAlign = ContentAlignment.MiddleCenter;
                WeedLabel.TextAlign = ContentAlignment.MiddleCenter;
                BugsLabel.TextAlign = ContentAlignment.MiddleCenter;
                CoinsLabel.TextAlign = ContentAlignment.MiddleCenter;
            }
            
        }

        private void UpdateTileControls()
        {
            int row = TileControls._chosenTile.Item1;
            int column = TileControls._chosenTile.Item2;
            TileControls.RowLabel.Text = $"Row: {row}";
            TileControls.ColumnLabel.Text = $"Column: {column}";

            Tile tile = _garden.GetTile(row, column);
            if (tile.Flower is null)
            {
                TileControls.FlowerLabel.Text = $"No flower";
            }
            else
            {
                TileControls.FlowerLabel.Text = $"Flower: {tile.Flower.FlowerType.Name}";
            }

            if (tile.HasWeed)
            {
                TileControls.WeedLabel.Text = "Has weed";
            }
            else
            {
                TileControls.WeedLabel.Text = "No weed";
            }

            if (tile.Bugs is null)
            {
                TileControls.BugsLabel.Text = "No bugs";
            }
            else
            {
                TileControls.BugsLabel.Text = $"Has bugs (spray price: {tile.Bugs.Bugs.SprayPrice})";
            }

            TileControls.CoinsLabel.Text = $"Coins: {tile.Coins}";
        }

        private void UpdateUI()
        {
            switch (_state)
            {
                case UIState.MAINMENU:
                    _mainMenuPanel.Visible = true;
                    _gardenPanel.Visible = false;
                    _tileInfoPanel.Visible = false;
                    break;
                case UIState.GARDEN:
                    _mainMenuPanel.Visible= false;
                    _gardenPanel.Visible = true;
                    _tileInfoPanel.Visible = false;
                    _moneyLabel.Text = $"Money: {_garden.Player.Money}";
                    _dayLabel.Text = $"Day: {_garden.Player.playerStatistics.numberOfDay}";
                    UpdateTilesUI();
                    break;
                case UIState.TILE_INFO:
                    _mainMenuPanel.Visible = false;
                    _gardenPanel.Visible = false;
                    _tileInfoPanel.Visible = true;
                    _moneyLabel2.Text = $"Money: {_garden.Player.Money}";
                    UpdateTileControls();
                    break;
            }
        }

        private void GameUIInitialization()
        {
            _gamePanel.RowCount = 2;
            _gamePanel.ColumnCount = 1;
            _gamePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 95));
            _gamePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            _gamePanel.BackColor = Color.YellowGreen;
            _gamePanel.Dock = DockStyle.Fill;
            _gamePanel.Visible = true;

            //Add main menu
            _gamePanel.Controls.Add(CreateMainMenu(), 0, 0);

            //Add error panel
            CreateErrorPanel();
            _gamePanel.Controls.Add(_errorPanel, 0, 1);
            
            this.Controls.Add(_gamePanel);
        }

        private Panel CreateMainMenu()
        {
            _mainMenuPanel.Dock = DockStyle.Fill;
            _mainMenuPanel.RowCount = 3;
            _mainMenuPanel.ColumnCount = 3;

            _mainMenuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            _mainMenuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            _mainMenuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            _mainMenuPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            _mainMenuPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            _mainMenuPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            TableLayoutPanel buttonPanel = new TableLayoutPanel();
            buttonPanel.RowCount = 3;
            buttonPanel.ColumnCount = 1;
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));

            Button newGameButton = new Button();
            newGameButton.Dock = DockStyle.Fill;
            newGameButton.Click += NewGameButton_Click;
            newGameButton.Text = "New Game";
            newGameButton.BackColor = Color.MistyRose;
            buttonPanel.Controls.Add(newGameButton, 1, 0);

            Button loadGameButton = new Button();
            loadGameButton.Dock = DockStyle.Fill;
            loadGameButton.Click += LoadGameButton_Click;
            loadGameButton.Text = "Load Game";
            loadGameButton.BackColor = Color.MistyRose;
            buttonPanel.Controls.Add(loadGameButton, 1, 1);

            Button exitGameButton = new Button();
            exitGameButton.Dock = DockStyle.Fill;
            exitGameButton.Click += ExitGameButton_Click;
            exitGameButton.Text = "Exit";
            exitGameButton.BackColor = Color.MistyRose;
            buttonPanel.Controls.Add(exitGameButton, 1, 2);

            _mainMenuPanel.Controls.Add(buttonPanel, 1, 1);
            return _mainMenuPanel;
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
            _tileButtons = new Button[_garden.Rows, _garden.Columns];

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

            _gamePanel.Controls.Add(_gardenPanel, 0, 0);

            
            TilesInfoUIInitialization();
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

                    int row = i;
                    int column = j;
                    _tileButtons[row, column].Click += (s, e) => Tile_Click(row, column);
                    tilesPanel.Controls.Add(_tileButtons[i, j], j, i);
                }
            }

            return tilesPanel;
        }

        private void TilesInfoUIInitialization()
        {
            _tileInfoPanel.Dock = DockStyle.Fill;
            _tileInfoPanel.RowCount = 3;
            _tileInfoPanel.ColumnCount = 1;
            _tileInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            _tileInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            _tileInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            _tileInfoPanel.BackColor = Color.Beige;

            _tileInfoPanel.Controls.Add(CreateTileInfoBackButton(), 0, 0);
            _tileInfoPanel.Controls.Add(CreateTileInfoPanel(), 0, 1);
            _gamePanel.Controls.Add(_tileInfoPanel, 0, 0);
        }

        private Panel CreateTileInfoBackButton()
        {
            Button tileInfoBackButton = new Button();
            tileInfoBackButton.Text = "Back";
            tileInfoBackButton.BackColor = Color.MistyRose;
            tileInfoBackButton.Click += TileInfoBackButton_Click;
            tileInfoBackButton.Dock = DockStyle.Fill;
            tileInfoBackButton.FlatStyle = FlatStyle.Flat;

            TableLayoutPanel backButtonTable = new TableLayoutPanel();
            backButtonTable.Dock = DockStyle.Fill;
            backButtonTable.RowCount = 1;
            backButtonTable.ColumnCount = 3;
            backButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            backButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            backButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            backButtonTable.Controls.Add(tileInfoBackButton, 0, 0);

            backButtonTable.Controls.Add(_moneyLabel2, 2, 0);
            return backButtonTable;
        }

        private Panel CreateTileInfoPanel()
        {
            TableLayoutPanel tileInfo = new TableLayoutPanel();
            tileInfo.Dock = DockStyle.Fill;
            tileInfo.RowCount = 1;
            tileInfo.ColumnCount = 2;
            tileInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tileInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            tileInfo.Controls.Add(CreateTileInfoWithoutFlowerPanel(), 0, 0);

            return tileInfo;
        }

        private Panel CreateTileInfoWithoutFlowerPanel()
        {
            TableLayoutPanel tileInfo = new TableLayoutPanel();
            tileInfo.Dock = DockStyle.Fill;
            tileInfo.ColumnCount = 1;
            tileInfo.RowCount = 6;
            for (int i = 0; i < tileInfo.RowCount; i++)
            {
                tileInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tileInfo.RowCount));
            }

            TileControls.Inicialize();
            tileInfo.Controls.Add(TileControls.RowLabel, 0, 0);
            tileInfo.Controls.Add(TileControls.ColumnLabel, 0, 1);
            tileInfo.Controls.Add(TileControls.FlowerLabel, 0, 2);
            tileInfo.Controls.Add(TileControls.WeedLabel, 0, 3);
            tileInfo.Controls.Add(TileControls.BugsLabel, 0, 4);
            tileInfo.Controls.Add(TileControls.CoinsLabel, 0, 5);
            return tileInfo;
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
            TileControls._chosenTile = (row, column);
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

        private void NewGameButton_Click(object? sender, EventArgs e)
        {
            _garden = new Garden(4, 4, new Player(20, 20));
            _state = UIState.GARDEN;
            GardenUIInitialization();
            UpdateUI();
        }

        private void LoadGameButton_Click(object? sender, EventArgs e)
        {

        }

        private void ExitGameButton_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
