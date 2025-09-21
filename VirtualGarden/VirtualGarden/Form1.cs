#nullable enable

using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection;
using static VirtualGarden.Form1;

namespace VirtualGarden
{
    public partial class Form1 : Form
    {
        internal Garden? _garden;
        readonly string _picturesPath;
        private UIState _state;
        private TableLayoutPanel _gamePanel = new TableLayoutPanel();
        private TableLayoutPanel _mainMenuPanel = new TableLayoutPanel();
        private TableLayoutPanel _gameMenuPanel = new TableLayoutPanel();
        private TableLayoutPanel _gardenPanel = new TableLayoutPanel();
        private TableLayoutPanel _errorPanel = new TableLayoutPanel();
        private TableLayoutPanel _tableWithBackButtonAndMoneyLabel = new TableLayoutPanel();
        private TableLayoutPanel _tileInfoPanel = new TableLayoutPanel();
        private TableLayoutPanel _flowerCataloguePanel = new TableLayoutPanel();
        private Label _moneyLabel = new Label();
        private Label _moneyLabel2 = new Label();
        private Label _dayLabel = new Label();
        private Button[,] _tileButtons;
        private Label _errorMessage = new Label();
        private const string _noErrorText = "No errors";
        private Button _clearErrorMessageButton = new Button();
        private PlantedFlowerControls _plantedFlowerControls = PlantedFlowerControls.CreatePlantedFlowerControls();
        private TileUserButtons _tileUserButtons = TileUserButtons.CreateTileUserButtons();
        private List<Button> _buyAndPlantButtons = new List<Button>();
        private Tile? _chosenTile {  get; set; }

        private readonly Color _defaultTileColor = Color.SaddleBrown;

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
            TILE_INFO,
            FLOWER_CATALOGUE
        }

        private void UpdateUI()
        {
            switch (_state)
            {
                case UIState.MAINMENU:
                    _mainMenuPanel.Visible = true;
                    _gameMenuPanel.Visible = false;
                    _gardenPanel.Visible = false;
                    _tableWithBackButtonAndMoneyLabel.Visible = false;
                    break;
                case UIState.GAMEMENU:
                    _mainMenuPanel.Visible = false;
                    _gameMenuPanel.Visible = true;
                    _gardenPanel.Visible = false;
                    _tableWithBackButtonAndMoneyLabel.Visible = false;
                    break;
                case UIState.GARDEN:
                    _mainMenuPanel.Visible= false;
                    _gameMenuPanel.Visible= false;
                    _gardenPanel.Visible = true;
                    _tableWithBackButtonAndMoneyLabel.Visible = false;
                    _moneyLabel.Text = $"Money: {_garden.Player.Money}";
                    _dayLabel.Text = $"Day: {_garden.Player.NumberOfDay}";
                    UpdateTilesUI();
                    break;
                case UIState.TILE_INFO:
                    _mainMenuPanel.Visible = false;
                    _gameMenuPanel.Visible = false;
                    _gardenPanel.Visible = false;
                    _tableWithBackButtonAndMoneyLabel.Visible = true;
                    _tileInfoPanel.Visible = true;
                    _flowerCataloguePanel.Visible = false;
                    _moneyLabel2.Text = $"Money: {_garden.Player.Money}";
                    UpdateTileControls(_chosenTile);
                    UpdatePlantedFlowerControls(_chosenTile);
                    break;
                case UIState.FLOWER_CATALOGUE:
                    _mainMenuPanel.Visible = false;
                    _gameMenuPanel.Visible = false;
                    _gardenPanel.Visible = false;
                    _tableWithBackButtonAndMoneyLabel.Visible = true;
                    _tileInfoPanel.Visible = false;
                    _flowerCataloguePanel.Visible = true;
                    if (_chosenTile is not null)
                    {
                        SetControlsToVisible(_buyAndPlantButtons);
                    }
                    else
                    {
                        SetControlsToNotVisible(_buyAndPlantButtons);
                    }
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

            //Add game menu
            _gamePanel.Controls.Add(CreateGameMenu(), 0, 0);

            //Add error panel
            CreateErrorPanel();
            _gamePanel.Controls.Add(_errorPanel, 0, 1);
            
            this.Controls.Add(_gamePanel);
        }

        private static class TileControls
        {
            internal static Label RowLabel { get; } = CreateLabel();
            internal static Label ColumnLabel { get; } = CreateLabel();
            internal static Label FlowerLabel { get; } = CreateLabel();
            internal static Label WeedLabel { get; } = CreateLabel();
            internal static Label BugsLabel { get; } = CreateLabel();
            internal static Label CoinsLabel { get; } = CreateLabel();

            internal static void Initialize()
            {
                RowLabel.TextAlign = ContentAlignment.MiddleCenter;
                ColumnLabel.TextAlign = ContentAlignment.MiddleCenter;
                FlowerLabel.TextAlign = ContentAlignment.MiddleCenter;
                WeedLabel.TextAlign = ContentAlignment.MiddleCenter;
                BugsLabel.TextAlign = ContentAlignment.MiddleCenter;
                CoinsLabel.TextAlign = ContentAlignment.MiddleCenter;
            }

        }

        private void UpdateTileControls(Tile? tile)
        {
            if (tile is null)
            {
                throw new UnableToUpdateUIException($"Cannot update tile controls. No tile is celected.");
            }
            int row = tile.Row;
            int column = tile.Column;
            TileControls.RowLabel.Text = $"Row: {row}";
            TileControls.ColumnLabel.Text = $"Column: {column}";

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

        internal class PlantedFlowerControls
        {
            internal Label GrowthDaysLabel { get; } = CreateLabel();
            internal Label BloomDaysLabel { get; } = CreateLabel();
            internal Label DaysSinceLastWateredLabel { get; } = CreateLabel();
            internal Label StateLabel { get; } = CreateLabel();
            private PlantedFlowerControls()
            {
                GrowthDaysLabel.TextAlign = ContentAlignment.MiddleCenter;
                BloomDaysLabel.TextAlign = ContentAlignment.MiddleCenter;
                DaysSinceLastWateredLabel.TextAlign = ContentAlignment.MiddleCenter;
                StateLabel.TextAlign = ContentAlignment.MiddleCenter;
            }
            internal static PlantedFlowerControls CreatePlantedFlowerControls()
            {
                return new PlantedFlowerControls();
            }
        }

        private void UpdatePlantedFlowerControls(Tile? tile)
        {
            if (tile is null)
            {
                throw new UnableToUpdateUIException($"Cannot update planted flower controls. No tile is selected.");
            }
            if (tile.Flower is not null)
            {
                _plantedFlowerControls.GrowthDaysLabel.Text = $"Days grown: {tile.Flower.GrowthDays}";
                _plantedFlowerControls.BloomDaysLabel.Text = $"Days bloomed: {tile.Flower.BloomDays}";
                _plantedFlowerControls.DaysSinceLastWateredLabel.Text = $"Days since last watered: {tile.Flower.DaysSinceLastWatered}";
                _plantedFlowerControls.StateLabel.Text = $"State: {tile.Flower.State}";
            }
        }

        internal class TileUserButtons
        {
            internal Button WaterFlowerButton { get; set; }
            internal Button RemoveWeedButton { get; set; }
            internal Button RemoveBugsButton { get; set; }
            internal Button CollectCoinsButton { get; set; }
            internal Button PlantFlowerButton {  get; set; }
            internal Button RemoveFlowerButton { get; set; }
            private TileUserButtons()
            {
                WaterFlowerButton = CreateButton("Water");
                RemoveWeedButton = CreateButton("Remove weed");
                RemoveBugsButton = CreateButton("Remove bugs");
                CollectCoinsButton = CreateButton("Collect coins");
                PlantFlowerButton = CreateButton("Plant Flower");
                RemoveFlowerButton = CreateButton("Remove flower");
            }
            internal static TileUserButtons CreateTileUserButtons()
            {
                return new TileUserButtons();
            }
        }

        internal class FlowerTypeControls
        {
            internal Label NameLabel { get; } = CreateLabel();
            internal Label GrowthDaysLabel { get; } = CreateLabel();
            internal Label BloomDaysLabel { get; } = CreateLabel();
            internal Label DaysBetweenWateringLabel { get; } = CreateLabel();
            internal Label DailyBloomIncomeLabel { get; } = CreateLabel();
            internal Label MaximumIncomeLabel { get; } = CreateLabel();
            internal Label SeedPriceLabel { get; } = CreateLabel();
            private FlowerTypeControls()
            {
                NameLabel.TextAlign = ContentAlignment.MiddleLeft;
                GrowthDaysLabel.TextAlign = ContentAlignment.MiddleLeft;
                BloomDaysLabel.TextAlign = ContentAlignment.MiddleLeft;
                DaysBetweenWateringLabel.TextAlign = ContentAlignment.MiddleLeft;
                DailyBloomIncomeLabel.TextAlign = ContentAlignment.MiddleLeft;
                MaximumIncomeLabel.TextAlign = ContentAlignment.MiddleLeft;
                SeedPriceLabel.TextAlign = ContentAlignment.MiddleLeft;
            }

            internal static FlowerTypeControls CreateFlowerTypeControls(FlowerType flower)
            {
                FlowerTypeControls controls = new FlowerTypeControls();
                controls.NameLabel.Text = $"Name: {flower.Name}";
                controls.GrowthDaysLabel.Text = $"Growth days: {flower.GrowthDays}";
                controls.BloomDaysLabel.Text = $"Bloom days: {flower.BloomDays}";
                controls.DaysBetweenWateringLabel.Text = $"Maximum days between watering: {flower.DaysBetweenWatering}";
                controls.DailyBloomIncomeLabel.Text = $"Daily bloom income: {flower.DailyBloomIncome}";
                controls.MaximumIncomeLabel.Text = $"Maximum income: {flower.MaximumIncome}";
                controls.SeedPriceLabel.Text = $"Seed price: {flower.SeedPrice}";
                return controls;
            }
        }

        private TableLayoutPanel CreateMenu()
        {
            TableLayoutPanel menu = new TableLayoutPanel();
            menu.Dock = DockStyle.Fill;
            menu.RowCount = 3;
            menu.ColumnCount = 3;

            menu.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            menu.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            menu.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            return menu;
        }

        private Panel CreateMainMenu()
        {
            _mainMenuPanel = CreateMenu();

            Button newGameButton = CreateButton("New game");
            newGameButton.Click += NewGameButton_Click;

            Button loadGameButton = CreateButton("Load game");
            loadGameButton.Click += LoadGameButton_Click;

            Button exitGameButton = CreateButton("Exit");
            exitGameButton.Click += ExitGameButton_Click;

            List<Button> menuButtons = new List<Button> { newGameButton, loadGameButton, exitGameButton };
            Panel buttonPanel = CreateButtonPanel(menuButtons);

            _mainMenuPanel.Controls.Add(buttonPanel, 1, 1);
            return _mainMenuPanel;
        }

        private Panel CreateGameMenu()
        {
            _gameMenuPanel = CreateMenu();
            Button resumeButton = CreateButton("Resume");
            resumeButton.Click += ResumeButton_Click;

            Button saveButton = CreateButton("Save game");
            saveButton.Click += SaveButton_Click;

            Button exitButton = CreateButton("Exit");
            exitButton.Click += ExitGameButton_Click;

            List<Button> menuButtons = new List<Button> { resumeButton, saveButton, exitButton };
            Panel buttonPanel = CreateButtonPanel(menuButtons);

            _gameMenuPanel.Controls.Add(buttonPanel, 1, 1);
            return _gameMenuPanel;
        }


        private Panel CreateButtonPanel(List<Button> buttons)
        {
            TableLayoutPanel buttonPanel = new TableLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.RowCount = buttons.Count();
            buttonPanel.ColumnCount = 1;
            AddSameSizeRowStyle(buttonPanel);
            for (int i = 0; i < buttonPanel.RowCount; i++)
            {
                buttonPanel.Controls.Add(buttons[i], 0, i);
            }
            return buttonPanel;
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

            _errorMessage.TextAlign = ContentAlignment.MiddleLeft;
            _errorMessage.Text = _noErrorText;
            _errorPanel.Controls.Add(_errorMessage, 0, 0);
            _errorMessage.Dock = DockStyle.Fill;
            _errorMessage.AutoSize = false;
            _errorMessage.AutoEllipsis = false;

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
            _gardenPanel.Controls.Add(CreatePlayerPanel(), 2, 0);

            //Add the grid of tiles
            _gardenPanel.Controls.Add(CreateTilesGridPanel(), 1, 1);

            _gamePanel.Controls.Add(_gardenPanel, 0, 0);

            _gamePanel.Controls.Add(CreateTableWithBackButtonAndMoneyLabel(), 0, 0);
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

        private Panel CreatePlayerPanel()
        {
            TableLayoutPanel playerStatsPanel = new TableLayoutPanel();
            playerStatsPanel.Dock = DockStyle.Fill;
            playerStatsPanel.RowCount = 3;
            playerStatsPanel.ColumnCount = 1;

            AddSameSizeRowStyle(playerStatsPanel);

            TableLayoutPanel buttonRow = new TableLayoutPanel();
            buttonRow.Dock = DockStyle.Fill;
            buttonRow.RowCount = 1;
            buttonRow.ColumnCount = 2;
            AddSameSizeColumnStyle(buttonRow);
            Button menuButton = CreateButton("Menu");
            menuButton.Click += menuButton_Click;
            buttonRow.Controls.Add(menuButton, 1, 0);
            Button flowerCatalogueButton = CreateButton("Flower catalogue");
            flowerCatalogueButton.Click += FlowerCatalogueButton_Click;
            buttonRow.Controls.Add(flowerCatalogueButton, 0, 0);
            playerStatsPanel.Controls.Add(buttonRow, 0, 0);

            Color playerStatsBackColor = Color.PowderBlue;
            _moneyLabel.Dock = DockStyle.Fill;
            _moneyLabel.BackColor = playerStatsBackColor;
            _moneyLabel.TextAlign = ContentAlignment.TopLeft;
            playerStatsPanel.Controls.Add(_moneyLabel, 0, 1);

            _dayLabel.Dock = DockStyle.Fill;
            _dayLabel.BackColor = playerStatsBackColor;
            _dayLabel.TextAlign = ContentAlignment.TopLeft;
            playerStatsPanel.Controls.Add(_dayLabel, 0, 2);

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
                    _tileButtons[i, j].BackColor = _defaultTileColor;

                    int row = i;
                    int column = j;
                    _tileButtons[row, column].Click += (s, e) => Tile_Click(row, column);
                    tilesPanel.Controls.Add(_tileButtons[i, j], j, i);
                }
            }

            return tilesPanel;
        }

        private TableLayoutPanel CreateTableWithBackButtonAndMoneyLabel()
        {
            _tableWithBackButtonAndMoneyLabel.Dock = DockStyle.Fill;
            _tableWithBackButtonAndMoneyLabel.RowCount = 2;
            _tableWithBackButtonAndMoneyLabel.ColumnCount = 1;
            _tableWithBackButtonAndMoneyLabel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _tableWithBackButtonAndMoneyLabel.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            _tableWithBackButtonAndMoneyLabel.BackColor = Color.Beige;

            _tableWithBackButtonAndMoneyLabel.Controls.Add(CreateBackButtonAndMoneyLabelRow(), 0, 0);
            _tableWithBackButtonAndMoneyLabel.Controls.Add(CreateTileInfoPanel(), 0, 1);
            _tableWithBackButtonAndMoneyLabel.Controls.Add(CreateFlowerCataloguePanel(), 0, 1);
            return _tableWithBackButtonAndMoneyLabel;
        }

        private Panel CreateBackButtonAndMoneyLabelRow()
        {
            Button backButton = CreateButton("Back to garden");
            backButton.Click += BackButton_Click;

            TableLayoutPanel backButtonTable = new TableLayoutPanel();
            backButtonTable.Dock = DockStyle.Fill;
            backButtonTable.RowCount = 1;
            backButtonTable.ColumnCount = 3;
            backButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            backButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            backButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            backButtonTable.Controls.Add(backButton, 0, 0);

            backButtonTable.Controls.Add(_moneyLabel2, 2, 0);
            return backButtonTable;
        }

        private Panel CreateTileInfoPanel()
        {
            _tileInfoPanel.Dock = DockStyle.Fill;
            _tileInfoPanel.RowCount = 2;
            _tileInfoPanel.ColumnCount = 1;
            _tileInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            _tileInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));

            TableLayoutPanel tileInfo = new TableLayoutPanel();
            tileInfo.Dock = DockStyle.Fill;
            tileInfo.RowCount = 1;
            tileInfo.ColumnCount = 2;
            tileInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tileInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            tileInfo.Controls.Add(CreateTileInfoWithoutFlowerPanel(), 0, 0);
            tileInfo.Controls.Add(CreateTileFlowerInfoPanel(), 1, 0);

            _tileInfoPanel.Controls.Add(tileInfo, 0, 0);
            _tileInfoPanel.Controls.Add(CreateTileUserButtonsPanel(), 0, 1);
            return _tileInfoPanel;
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

            TileControls.Initialize();
            tileInfo.Controls.Add(TileControls.RowLabel, 0, 0);
            tileInfo.Controls.Add(TileControls.ColumnLabel, 0, 1);
            tileInfo.Controls.Add(TileControls.FlowerLabel, 0, 2);
            tileInfo.Controls.Add(TileControls.WeedLabel, 0, 3);
            tileInfo.Controls.Add(TileControls.BugsLabel, 0, 4);
            tileInfo.Controls.Add(TileControls.CoinsLabel, 0, 5);
            return tileInfo;
        }

        private Panel CreateTileFlowerInfoPanel()
        {
            TableLayoutPanel tileFlowerInfo = new TableLayoutPanel();
            tileFlowerInfo.Dock = DockStyle.Fill;
            tileFlowerInfo.RowCount = 4;
            tileFlowerInfo.ColumnCount = 1;
            for (int i = 0; i < tileFlowerInfo.RowCount; i++)
            {
                tileFlowerInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tileFlowerInfo.RowCount));
            }
            tileFlowerInfo.Controls.Add(_plantedFlowerControls.GrowthDaysLabel, 0, 0);
            tileFlowerInfo.Controls.Add(_plantedFlowerControls.BloomDaysLabel, 0, 1);
            tileFlowerInfo.Controls.Add(_plantedFlowerControls.DaysSinceLastWateredLabel, 0, 2);
            tileFlowerInfo.Controls.Add(_plantedFlowerControls.StateLabel, 0, 3);
            return tileFlowerInfo;
        }

        private Panel CreateTileUserButtonsPanel()
        {
            TableLayoutPanel tileUserButtonsPanel = new TableLayoutPanel();
            tileUserButtonsPanel.RowCount = 1;
            tileUserButtonsPanel.ColumnCount = 6;
            tileUserButtonsPanel.Dock = DockStyle.Fill;
            AddSameSizeColumnStyle(tileUserButtonsPanel);

            _tileUserButtons.WaterFlowerButton.Click += WaterFlowerButton_Click;
            _tileUserButtons.RemoveWeedButton.Click += RemoveWeedButton_Click;
            _tileUserButtons.RemoveBugsButton.Click += RemoveBugsButton_Click;
            _tileUserButtons.CollectCoinsButton.Click += CollectCoinsButton_Click;
            _tileUserButtons.PlantFlowerButton.Click += PlantFlowerButton_Click;
            _tileUserButtons.RemoveFlowerButton.Click += RemoveFlowerButton_Click;

            tileUserButtonsPanel.Controls.Add(_tileUserButtons.WaterFlowerButton, 0, 0);
            tileUserButtonsPanel.Controls.Add(_tileUserButtons.RemoveWeedButton, 1, 0);
            tileUserButtonsPanel.Controls.Add(_tileUserButtons.RemoveBugsButton, 2, 0);
            tileUserButtonsPanel.Controls.Add(_tileUserButtons.CollectCoinsButton, 3, 0);
            tileUserButtonsPanel.Controls.Add(_tileUserButtons.PlantFlowerButton, 4, 0);
            tileUserButtonsPanel.Controls.Add(_tileUserButtons.RemoveFlowerButton, 5, 0);

            return tileUserButtonsPanel;
        }

        private Panel CreateFlowerCataloguePanel()
        {
            
            _flowerCataloguePanel.Dock = DockStyle.Fill;

            Type flowerTypes = typeof(FlowerTypes);
            FieldInfo[] flowerTypesFields = flowerTypes.GetFields(BindingFlags.Public | BindingFlags.Static);
            List<TableLayoutPanel> flowerTypePanels = new List<TableLayoutPanel>();
            
            for ( int i = 0; i < flowerTypesFields.Length; i++ )
            {
                object? fieldValue = flowerTypesFields[i].GetValue(null);
                if (fieldValue is FlowerType flower)
                flowerTypePanels.Add(CreateFlowerTypePanel(flower));
            }

            //More catalogue pages are needed if more flowers are added
            _flowerCataloguePanel.ColumnCount = 5;
            _flowerCataloguePanel.RowCount = 2;
            AddSameSizeColumnStyle(_flowerCataloguePanel);
            AddSameSizeRowStyle(_flowerCataloguePanel);
            int flowerTypePanelIndex = 0;
            for (int i = 0; i < _flowerCataloguePanel.RowCount; i++)
            {
                for (int j = 0; j < _flowerCataloguePanel.ColumnCount; j++)
                {
                    if (flowerTypePanelIndex >= flowerTypePanels.Count)
                    {
                        //all flower panels have been put into the table
                        break;
                    }
                    _flowerCataloguePanel.Controls.Add(flowerTypePanels[flowerTypePanelIndex], j, i);
                    flowerTypePanelIndex++;
                }
            }
            return _flowerCataloguePanel;
        }

        private TableLayoutPanel CreateFlowerTypePanel(FlowerType flower)
        {
            TableLayoutPanel flowerTypePanel = new TableLayoutPanel();
            flowerTypePanel.Dock = DockStyle.Fill;
            
            FlowerTypeControls flowerTypeControls = FlowerTypeControls.CreateFlowerTypeControls(flower);
            Type type = typeof(FlowerTypeControls);
            PropertyInfo[] flowerTypeControlsProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            flowerTypePanel.RowCount = flowerTypeControlsProperties.Length + 1; //+1 fot the buy button
            flowerTypePanel.ColumnCount = 1;
            AddSameSizeRowStyle(flowerTypePanel);
            for ( int i = 0; i < flowerTypeControlsProperties.Length; i++)
            {
                if (flowerTypeControlsProperties[i].GetValue(flowerTypeControls) is Label label)
                flowerTypePanel.Controls.Add(label, 0, i);
            }
            Button buyAndPlantButton = CreateButton("Buy seeds and plant");
            buyAndPlantButton.Click += (s, e) => buyAndPlantButton_Click(flower);
            _buyAndPlantButtons.Add(buyAndPlantButton);
            flowerTypePanel.Controls.Add(buyAndPlantButton, 0, flowerTypeControlsProperties.Length);

            return flowerTypePanel;
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
            try
            {
                _garden.NewDay();
            }
            catch(GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void BackButton_Click(object? sender, EventArgs e)
        {
            _state = UIState.GARDEN;
            _chosenTile = null;
            UpdateUI();
        }

        private void Tile_Click(int row, int column)
        {
            _state = UIState.TILE_INFO;
            _chosenTile = _garden.GetTile(row, column);
            UpdateUI();
        }

        private void ClearErrorMessageButton_Click(object? sender, EventArgs e)
        {
            _errorMessage.Text = _noErrorText;
            _errorPanel.Visible = false;
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
                    _tileButtons[i, j].Image = null;
                    Tile tile = _garden.GetTile(i, j);

                    if (tile.HasWeed)
                    {
                        _tileButtons[i, j].BackColor = Color.Olive;
                    }
                    else
                    {
                        _tileButtons[i, j].BackColor = _defaultTileColor;
                    }

                    if (tile.Flower is not null)
                    {
                        FlowerState flowerState = tile.Flower.State;
                        string flowerImageFilename = "";
                        switch (flowerState)
                        {
                            case FlowerState.Growing:
                                flowerImageFilename = tile.Flower.FlowerType.GrowingImageFilename;
                                break;
                            case FlowerState.Blooming:
                                flowerImageFilename = tile.Flower.FlowerType.BloomingImageFilename;
                                break;
                            case FlowerState.Dead:
                                flowerImageFilename = tile.Flower.FlowerType.DeadImageFilename;
                                break;
                        }
                        string flowerImagePath = Path.Combine(_picturesPath, flowerImageFilename);

                        string? bugsImageFilename = null;
                        if (tile.Bugs is not null)
                        {
                            bugsImageFilename = tile.Bugs.Bugs.ImageFilename;
                        }
                        try
                        {
                            Image finalImage;
                            Image flowerImage = FileHandler.GetImageFromFile(flowerImagePath);
                            Image bugsImage;
                            if (bugsImageFilename is not null)
                            {
                                string bugsImagePath = Path.Combine(_picturesPath, bugsImageFilename);
                                bugsImage = FileHandler.GetImageFromFile(bugsImagePath);
                                finalImage = CombineImages(flowerImage, bugsImage);
                            }
                            else
                            {
                                finalImage = flowerImage;
                            }
                            PutImageOnTile(finalImage, _tileButtons[i, j]);
                        }
                        catch (FileException ex)
                        {
                            WriteErrorMessage(ex.Message);
                            _tileButtons[i, j].Text = "Image not loaded.";
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

        private static Button CreateButton(string text)
        {
            Button button = CreateButton();
            button.Text = text;
            return button;
        }

        private static Button CreateButton()
        {
            Button button = new Button();
            button.Dock = DockStyle.Fill;
            button.BackColor = Color.MistyRose;
            button.FlatStyle = FlatStyle.Flat;
            return button;
        }

        private static Label CreateLabel(string text)
        {
            Label label = CreateLabel();
            label.Text = text;
            return label;
        }

        private static Label CreateLabel()
        {
            Label label = new Label();
            label.Dock = DockStyle.Fill;
            label.AutoSize = false;
            label.AutoEllipsis = false;
            return label;
        }

        private void WaterFlowerButton_Click(object? sender, EventArgs e)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException("Cannot water flower. No tile is selected.");
            }
            try
            {
                _garden.WaterFlower(_chosenTile);
            }
            catch(GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void RemoveWeedButton_Click(object? sender, EventArgs e)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException($"Cannot remove weed on a tile. No tile is selected.");
            }
            try
            {
                _garden.RemoveWeed(_chosenTile);
            }
            catch(GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void RemoveBugsButton_Click(object? sender, EventArgs e)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException($"Cannot remove bugs on a tile. No tile is selected.");
            }
            try
            {
                _garden.RemoveBugs(_chosenTile);
            }
            catch (GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void CollectCoinsButton_Click(object? sender, EventArgs e)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException("Cannot collect coins on a tile. No tile is selected.");
            }
            try
            {
                _garden.CollectCoins(_chosenTile);
            }
            catch (GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void PlantFlowerButton_Click(object? sender, EventArgs e)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException($"Cannot plant flower on a tile. No tile is selected");
            }
            _state = UIState.FLOWER_CATALOGUE;
            UpdateUI();
        }

        private void RemoveFlowerButton_Click(object? sender, EventArgs e)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException($"Cannot remove flower on a tile. No tile is selected");
            }
            try
            {
                _garden.RemoveFlower(_chosenTile);
            }
            catch (GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void AddSameSizeColumnStyle(TableLayoutPanel panel)
        {
            for (int i = 0; i < panel.ColumnCount; i++)
            {
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / panel.ColumnCount));
            }
        }

        private void AddSameSizeRowStyle(TableLayoutPanel panel)
        {
            for (int i = 0; i < panel.RowCount; i++)
            {
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / panel.RowCount));
            }
        }

        private void WriteErrorMessage(string message)
        {
            _errorMessage.Text = message;
            _errorPanel.Visible = true;
            UpdateUI();
        }

        /// <summary>
        /// Buys flower seeds and plants them on the chosen tile.
        /// </summary>
        /// <param name="flower">The type of the flower to be planted on the chosen tile.</param>
        private void buyAndPlantButton_Click(FlowerType flower)
        {
            if (_chosenTile is null)
            {
                throw new UnableToUpdateUIException("Cannot plant flower on a tile. No tile is selected.");
            }
            try
            {
                _garden.Player.BuySeeds(flower);
                try
                {
                    _garden.PlantFlower(_chosenTile, flower);
                    _state = UIState.GARDEN;
                }
                catch(GardenException ex)
                {
                    //Planting failed, return money spent on seeds to player.
                    _garden.Player.AddMoney(flower.SeedPrice);
                }
            }
            catch(GardenException ex)
            {
                WriteErrorMessage(ex.Message);
            }
            UpdateUI();
        }

        private void menuButton_Click(object? sender, EventArgs e)
        {
            _state = UIState.GAMEMENU;
            UpdateUI();
        }

        private void FlowerCatalogueButton_Click(object? sender, EventArgs e)
        {
            _state = UIState.FLOWER_CATALOGUE;
            UpdateUI();
        }

        private void SetControlsToVisible(IEnumerable<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Visible = true;
            }
        }

        private void SetControlsToNotVisible(IEnumerable<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Visible = false;
            }
        }

        private Image CombineImages(Image image1, Image image2)
        {
            Bitmap bitmap = new Bitmap(image1.Width, image1.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image1, 0, 0, image1.Width, image1.Height);
                g.DrawImage(image2, 0, 0, image2.Width, image2.Height);
            }
            return bitmap;
        }

        private void ResumeButton_Click(object? sender, EventArgs e)
        {
            _state = UIState.GARDEN;
            UpdateUI();
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            //save the garden using reflection
            throw new NotImplementedException();
        }
    }
}
