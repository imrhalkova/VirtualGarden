#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    /// <summary>
    /// Represents the virtual garden. It contains the whole game.
    /// </summary>
    public class Garden
    {
        //a grid of tiles in which the user can grow flowers
        public Tile[,] Grid {  get; private set; }

        //an instance of the Player class containing the current players info (amount of coins, statistics, ...)
        public Player Player {  get; private set; }

        //The probability that weeds will be spawned on one empty tile the next day with no active event affecting weed spawning.
        private double _normalWeedChance = 0.08;

        /// <summary>
        /// The probability that weeds will be spawned on one empty tile the next day.
        /// </summary>
        public double WeedChance
        {
            get
            {
                if (Event is HighTemperature highTemperature)
                {
                    return _normalWeedChance * highTemperature.WeedChanceMultiply;
                }
                return _normalWeedChance;
            }
        }

        //The probability that a bug infestation will be spawned on one tile with a flower the next day with no active event affecting bugs spawning.
        private double _normalBugsChance = 0.05;

        /// <summary>
        /// The probability that a bug infestation will be spawned on one tile with a flower the next day.
        /// </summary>
        public double BugsChance
        {
            get
            {
                if (Event is HumidWeatherEvent humidWeatherEvent)
                {
                    return _normalBugsChance * humidWeatherEvent.BugChanceMultiply;
                }
                return _normalBugsChance;
            }
        }

        /// <summary>
        /// The probability of weed spreading to this tile if one adjacent tile has weed.
        /// </summary>
        public double WeedSpreadChance { get; private set; } = 0.35;

        /// <summary>
        /// A random number generator used for making decisions in the garden that have certain probabilities - spawning and spreading
        /// of weed, spawning and picking insects, events generating.
        /// </summary>
        public IRandomNumberGenerator Rand { get; } = new DefaultRandomGenerator();

        //Enables/Disables events in the game.
        private bool _eventsEnabled = false;

        //The current active event or null if no event is currently active.
        public Event? Event { get; private set; }

        /// <summary>
        /// The probability of a random event starting on a day with no active event.
        /// </summary>
        public double EventChance { get; private set; } = 0.35;

        /// <summary>
        /// The number of flowers currently planted in the garden that are alive (growing or blooming).
        /// </summary>
        public int FlowerCount
        {
            get
            {
                int count = 0;
                foreach (Tile tile in Grid)
                {
                    if (tile.Flower is not null && tile.Flower.State != FlowerState.Dead)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public int Rows { get { return this.Grid.GetLength(0); } }
        public int Columns { get { return this.Grid.GetLength(1); } }

        public Garden(int rows, int columns, Player player) 
        {
            InitializeGrid(rows, columns);
            Player = player;
        }
        public Garden(int rows, int columns, Player player, double weedChance, double bugsChance, double eventChance, bool eventsEnabled)
        {
            InitializeGrid(rows, columns);
            Player = player;
            _normalWeedChance = weedChance;
            _normalBugsChance = bugsChance;
            EventChance = eventChance;
            _eventsEnabled = eventsEnabled;
        }

        public Garden(int rows, int columns, Player player, double weedChance, double bugsChance, double eventChance, bool eventsEnabled, IRandomNumberGenerator randomGenerator)
        {
            Rand = randomGenerator;
            InitializeGrid(rows, columns);
            Player = player;
            _normalWeedChance = weedChance;
            _normalBugsChance = bugsChance;
            EventChance = eventChance;
            _eventsEnabled = eventsEnabled;
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
            if (i < 0 || i >=  Rows || j < 0 || j >= Columns)
            {
                throw new TileIndexOutOfRangeException($"The tile indexes [{i}, {j}] are out of range.");
            }
            return Grid[i, j];
        }

        public Tile GetTile((int, int) index)
        {
            return GetTile(index.Item1, index.Item2);
        }

        /// <summary>
        /// Updates the whole garden for a new day.
        /// </summary>
        public void NewDay()
        {
            UpdateOngoingEvent();
            UpdateExistingBugInfestations();
            UpdateFlowers();
            UpdateWateredStateOfFlowers();
            UpdateWeed();
            TrySpawningBugInfestations();
            UpdatePlayer();
            TryStartingAnEvent();
        }

        private void UpdateOngoingEvent()
        {
            if (Event is not null)
            {
                Event.Update();
                if (Event.DaysLeft == 0)
                {
                    Event = null;
                }
            }
        }

        //Lowers the countdowns until flowers die from their current bug infestations.
        private void UpdateExistingBugInfestations()
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
        private void UpdateFlowers()
        {
            foreach (Tile tile in Grid)
            {
                if (tile.Flower is not null)
                tile.UpdateFlower(Player);
            }
        }

        //Updates the number of days since the flower was last watered.
        private void UpdateWateredStateOfFlowers()
        {
            foreach (Tile tile in Grid)
            {
                if (tile.Flower is not null)
                {
                    if (Event is RainEvent)
                    {
                        tile.WaterFlower();
                    }
                    else
                    {
                        tile.UpdateWateredStateOfFlower();
                    }  
                }
            }
        }

        //Updates weed for a new day - tries spreading weed from previous days and spawning new weed to empty tiles
        private void UpdateWeed()
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
        private void TrySpawningBugInfestations()
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
        private void UpdatePlayer()
        {
            Player.Update();
        }

        private void TryStartingAnEvent()
        {
            
            if (Event is null)
            {
                if (Rand.NextDouble() < EventChance)
                {
                    Event = EventGenerator.GenerateEvent(Rand, Rows * Columns);
                }
            }
        }

        private void SpawnInfestation(Tile tile)
        {
            if (tile.Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot spawn bugs on tile {tile.PrintCoordinates()}. No flower present on this tile.");
            }
            var bugWeights = tile.Flower.FlowerType.BugWeights;
            Bugs bugs = WeightedRandom.ChooseWeightedRandom<Bugs, BugsWeight>(bugWeights, Rand);
            tile.SpawnBugInfestation(new BugInfestation(bugs));

        }

        private bool[,] GetCurrentPosOfWeed()
        {
            bool[,] currentWeedPositions = new bool[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    currentWeedPositions[i, j] = GetTile(i, j).HasWeed;
                }
            }
            return currentWeedPositions;
        }

        //Takes each tile in the grid without weed and tries spreading weed to it from adjacent tiles with weed
        private void TrySpreadingWeed(bool[,] currentPosOfWeed)
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
        private List<(int, int)> GetIndexesOfAdjacentTiles(int row, int column)
        {
            List<(int, int)> adjacentIndexes = new List<(int, int)>();
            (int, int)[] possibleAdjacentIndexes = new (int, int)[] { (row + 1, column), (row, column + 1), (row - 1, column), (row, column - 1) };
            foreach ((int, int) index in possibleAdjacentIndexes)
            {
                if (( 0 <= index.Item1 && index.Item1 < Rows) &&  ( 0 <= index.Item2 && index.Item2 < Columns)){
                    adjacentIndexes.Add(index);
                }
            }
            return adjacentIndexes;
        }

        private List<(int, int)> GetIndexesOfAdjacentTiles((int, int) pos)
        {
            return GetIndexesOfAdjacentTiles(pos.Item1, pos.Item2);
        }

        public void PlantFlower(Tile tile, FlowerType flower)
        {
            tile.PlantFlower(flower);
            if (Event is RainEvent)
            {
                tile.WaterFlower();
            }
        }

        public void PlantFlower(int row, int column, FlowerType flower)
        {
            PlantFlower(GetTile(row, column), flower);
        }

        public void CollectCoins(Tile tile)
        {
            Player.AddMoney(tile.Coins);
            tile.CollectCoins();
        }

        public void CollectCoins(int row, int column)
        {
            CollectCoins(GetTile(row, column));
        }

        public void WaterFlower(Tile tile)
        {
            if (Event is DroughtEvent)
            {
                DroughtEvent droughtEvent = (DroughtEvent) Event;
                if (!droughtEvent.CanWater)
                {
                    throw new NoWaterLeftException($"Cannot water flower on tile {tile.PrintCoordinates()}. There is no water left.");
                }
                droughtEvent.IncrementNumberOfWateringsDoneToday();
            }
            tile.WaterFlower();
        }

        public void WaterFlower(int row, int column)
        {
            WaterFlower(GetTile(row, column));
        }

        public void RemoveWeed(int row, int column)
        {
            RemoveWeed(GetTile(row, column));
        }

        public void RemoveWeed(Tile tile)
        {
            if (Event is BrokenToolsEvent)
            {
                throw new BrokenToolsException($"Cannot remove weed from tile {tile.PrintCoordinates()}. The tools are broken.");
            }
            tile.RemoveWeed();
        }

        public void RemoveBugs(int row, int column)
        {
            RemoveBugs(GetTile(row, column));
        }

        public void RemoveBugs(Tile tile)
        {
            if (tile.Bugs is null)
            {
                throw new BugsNotPresentException($"Cannot remove bugs from tile {tile.PrintCoordinates()}. There are no bugs on this tile.");
            }
            if (tile.Bugs.Bugs.SprayPrice > Player.Money)
            {
                throw new InsufficientFundsException($"Cannot remove bugs from tile {tile.PrintCoordinates()}. Not enought money.");
            }
            tile.RemoveBugs();
        }

        public void RemoveFlower(int row, int column)
        {
            RemoveFlower(GetTile(row, column));
        }

        public void RemoveFlower(Tile tile)
        {
            tile.RemoveFlower();
        }

        private void EndEvent()
        {
            Event = null;
        }

        public void KillFlower(int numberOfFlowerToBeKilled)
        {
            int counter = 0;
            foreach (Tile tile in Grid)
            {
                if (tile.Flower is not null && tile.Flower.State != FlowerState.Dead)
                {
                    counter++;
                    if (counter == numberOfFlowerToBeKilled)
                    {
                        tile.KillFlower();
                    }
                }
            }
        }
    }
}
