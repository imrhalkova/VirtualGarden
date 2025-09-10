#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class Garden
    {
        //a grid of tiles in which the user can grow flowers
        public Tile[,] Grid {  get; private set; }

        //an instance of the Player class containing the current players info (amount of coins, statistics, ...)
        public Player Player {  get; private set; }

        //the chance of a weed spawning on an empty tile with no active event affecting weed spawning
        private double _normalWeedChance = 0.3;

        //the chance of a weed spawning on an empty tile with an active event affecting weed spawning
        double? EventWeedChance { get; set; } = null;

        //the probability actually used for spawning weeds on empty tiles
        public double WeedChance
        {
            get
            {
                if (EventWeedChance is null)
                {
                    return _normalWeedChance;
                }
                return (double)EventWeedChance;
            }
        }

        //the chance of bugs spawning on a tile with a flower with no active event affecting bugs spawning
        private double _normalBugsChance = 0.05;

        //the chance of bugs spawning on a tile with a flower with an active event affecting bugs spawning
        double? EventBugsChance { get; set; } = null;

        //the probability actually used for spawning bugs on tiles with flowers
        public double BugsChance
        {
            get
            {
                if (EventBugsChance is null)
                {
                    return _normalBugsChance;
                }
                return (double)EventBugsChance;
            }
        }

        //the chance of weed spreading to this tile if one adjacent tile has weed
        public double WeedSpreadChance { get; private set; } = 0.5;

        public IRandomNumberGenerator Rand { get; } = new DefaultRandomGenerator();

        //the number of coins the player automatically receives with each new day
        public int NewDayIncome { get; private set; } = 10;

        //Enables/Disables events.
        public bool EventsEnabled = false;

        public Garden(int rows, int columns, Player player) 
        {
            InitializeGrid(rows, columns);
            Player = player;
        }
        public Garden(int rows, int columns, Player player, double weedChance, double bugsChance, int newDayIncome, bool eventsEnabled)
        {
            InitializeGrid(rows, columns);
            Player = player;
            _normalWeedChance = weedChance;
            _normalBugsChance = bugsChance;
            NewDayIncome = newDayIncome;
            EventsEnabled = eventsEnabled;
        }

        public Garden(int rows, int columns, Player player, double weedChance, double bugsChance, int newDayIncome, bool eventsEnabled, IRandomNumberGenerator randomGenerator)
        {
            Rand = randomGenerator;
            InitializeGrid(rows, columns);
            Player = player;
            _normalWeedChance = weedChance;
            _normalBugsChance = bugsChance;
            NewDayIncome = newDayIncome;
            EventsEnabled = eventsEnabled;
        }

        public void InitializeGrid(int rows, int columns)
        {
            Grid = new Tile[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Grid[i, j] = new Tile(i, j);
                }
            }
        }

        public Tile GetTile(int i, int j)
        {
            if (i < 0 || i >=  Grid.GetLength(0) || j < 0 || j >= Grid.GetLength(1))
            {
                throw new TileIndexOutOfRangeException($"The tile indexes [{i}, {j}] are out of range.");
            }
            return Grid[i, j];
        }

        public Tile GetTile((int, int) index)
        {
            return GetTile(index.Item1, index.Item2);
        }

        //Updates the whole garden for a new day
        public void Update()
        {
            UpdateExistingBugInfestations();
            UpdateFlowers();
            UpdateWateredStateOfFlowers();
            UpdateWeed();
            TrySpawningBugInfestations();
            UpdatePlayer();
        }

        //Lowers the countdowns until flowers die from their current bug infestations.
        public void UpdateExistingBugInfestations()
        {
            foreach (Tile tile in Grid)
            {
                if (tile.Bugs is not null)
                tile.UpdateBugInfestation();
            }
        }

        /*Sets the state of dead flowers to dead.
         * Updates growth days and bloom days of flowers that are not affected by weed or bugs. 
         Puts coins on tiles with blooming flowers.
         */
        public void UpdateFlowers()
        {
            foreach (Tile tile in Grid)
            {
                if (tile.Flower is not null)
                tile.UpdateFlower();
            }
        }

        //Updates the number of days since the flower was last watered.
        public void UpdateWateredStateOfFlowers()
        {
            foreach (Tile tile in Grid)
            {
                if (tile.Flower is not null)
                tile.UpdateWateredStateOfFlower();
            }
        }

        //Updates weed for a new day - tries spreading weed from previous days and spawning new weed to empty tiles
        public void UpdateWeed()
        {
            bool[,] currentPosOfWeed = GetCurrentPosOfWeed();

            TrySpreadingWeed(currentPosOfWeed);

            foreach (Tile tile in Grid)
            {
                if (tile.Flower is null && Rand.NextDouble() < WeedChance)
                tile.SpawnWeed();
            }
        }

        //Tries spawning new bug infestations on tiles with live flowers without bug infestations.
        public void TrySpawningBugInfestations()
        {
            foreach (Tile tile in Grid)
            {
                if (tile.CanSpawnBugs())
                {
                    if (Rand.NextDouble() < BugsChance)
                    {
                        SpawnInfestation(tile);
                    }
                }
            }
        }

        //Updates player's info for a new day.
        public void UpdatePlayer()
        {
            Player.AddMoney(NewDayIncome);
        }

        public void SpawnInfestation(Tile tile)
        {
            if (tile.Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot spawn bugs on tile {tile.PrintCoordinates()}. No flower present on this tile.");
            }
            var bugWeights = tile.Flower.FlowerType.BugWeights;
            Bugs bugs = WeightedRandom.ChooseWeightedRandom<Bugs, BugsWeight<Bugs>>(bugWeights, Rand);
            tile.SpawnBugInfestation(new BugInfestation(bugs));

        }

        public bool[,] GetCurrentPosOfWeed()
        {
            bool[,] currentWeedPositions = new bool[Grid.GetLength(0), Grid.GetLength(1)];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    currentWeedPositions[i, j] = GetTile(i, j).HasWeed;
                }
            }
            return currentWeedPositions;
        }

        //Takes each tile in the grid without weed and tries spreading weed to it from adjacent tiles with weed
        public void TrySpreadingWeed(bool[,] currentPosOfWeed)
        {
            foreach (Tile tile in Grid)
            {
                if (!tile.HasWeed)
                {
                    var adjacentIndexes = GetIndexesOfAdjacentTiles(tile.Row, tile.Column);
                    foreach ((int, int) index in adjacentIndexes)
                    {
                        if (currentPosOfWeed[index.Item1, index.Item2] && Rand.NextDouble() < WeedSpreadChance)
                        tile.SpawnWeed();
                        break;
                    }
                }
            }
        }
        public List<(int, int)> GetIndexesOfAdjacentTiles(int row, int column)
        {
            List<(int, int)> adjacentIndexes = new List<(int, int)>();
            (int, int)[] possibleAdjacentIndexes = new (int, int)[] { (row + 1, column), (row, column + 1), (row - 1, column), (row, column - 1) };
            foreach ((int, int) index in possibleAdjacentIndexes)
            {
                if (( 0 <= index.Item1 && index.Item1 < Grid.GetLength(0)) &&  ( 0 <= index.Item2 && index.Item2 < Grid.GetLength(1))){
                    adjacentIndexes.Add(index);
                }
            }
            return adjacentIndexes;
        }

        public List<(int, int)> GetIndexesOfAdjacentTiles((int, int) pos)
        {
            return GetIndexesOfAdjacentTiles(pos.Item1, pos.Item2);
        }

        public void PlantFlower(Tile tile, Flower flower)
        {
            tile.PlantFlower(flower);
        }

        public void PlantFlower(int row, int column, Flower flower)
        {
            PlantFlower(GetTile(row, column), flower);
        }

        public void CollectCoins(Tile tile)
        {
            tile.CollectCoins();
            Player.AddMoney(tile.Coins);
        }

        public void CollectCoins(int row, int column)
        {
            CollectCoins(GetTile(row, column));
        }

        public void WaterTile(Tile tile)
        {
            tile.WaterTile();
        }

        public void WaterTile(int row, int column)
        {
            WaterTile(GetTile(row, column));
        }
         
    }
}
