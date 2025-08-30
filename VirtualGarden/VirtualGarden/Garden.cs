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
        Tile[,] grid;

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

        public Garden(int rows, int columns, Player player) 
        {
            InitializeGrid(rows, columns);
            Player = player;
        }
        public Garden(int rows, int columns, Player player, double weedChance, double bugsChance, int newDayIncome)
        {
            InitializeGrid(rows, columns);
            Player = player;
            _normalWeedChance = weedChance;
            _normalBugsChance = bugsChance;
            NewDayIncome = newDayIncome;
        }

        public void InitializeGrid(int rows, int columns)
        {
            grid = new Tile[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    grid[i, j] = new Tile(this, i, j);
                }
            }
        }

        public Tile GetTile(int i, int j)
        {
            return grid[i, j];
        }

        public Tile GetTile((int, int) index)
        {
            return GetTile(index.Item1, index.Item2);
        }

        public void Update()
        {
            UpdateFlowersStates();
            UpdateExistingBugInfestations();
            UpdateFlowers();
            UpdateWateredStateOfFlowers();
            UpdateWeed();
            TrySpawningBugInfestations();
            UpdatePlayer();
        }

        //Changes the state of fully grown flowers from growing to blooming and the state of faded flowers from blooming to dead.
        public void UpdateFlowersStates()
        {
            foreach (Tile tile in grid)
            {
                tile.UpdateFlowerState();
            }
        }

        //Lowers the countdowns until flowers die from their current bug infestations.
        public void UpdateExistingBugInfestations()
        {
            foreach (Tile tile in grid)
            {
                tile.UpdateBugInfestation();
            }
        }

        /*Sets the state of flowers not watered for too long and flowers not treated from bugs for too long to dead.
         * Updates growth days and bloom days of flowers that are not affected by weed or bugs. */
        public void UpdateFlowers()
        {
            foreach (Tile tile in grid)
            {
                tile.UpdateFlower();
            }
        }

        //Updates the number of days since the flower was last watered.
        public void UpdateWateredStateOfFlowers()
        {
            foreach (Tile tile in grid)
            {
                tile.UpdateWateredStateOfFlower();
            }
        }

        //Updates weed for a new day - tries spreading weed from previous days and spawning new weed to empty tiles
        public void UpdateWeed()
        {
            bool[,] currentPosOfWeed = GetCurrentPosOfWeed();

            TrySpreadingWeed();

            foreach (Tile tile in grid)
            {
                tile.TrySpawningWeed();
            }
        }

        //Tries spawning new bug infestations on tiles with live flowers without bug infestations.
        public void TrySpawningBugInfestations()
        {
            foreach (Tile tile in grid)
            {
                if (tile.CanSpawnBugs())
                {
                    if (Rand.NextDouble() <= BugsChance)
                    {
                        SpawnInfestation(tile);
                    }
                }
            }
        }

        //Updates player's info for a new day.
        public void UpdatePlayer()
        {
            Player.Money += NewDayIncome;
        }

        public void SpawnInfestation(Tile tile)
        {
            if (tile.Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot spawn bugs on tile [{tile.Row}, {tile.Column}]. No flower present on this tile.");
            }
            var bugWeights = tile.Flower.FlowerType.BugWeights;
            Bugs bugs = WeightedRandom.ChooseWeightedRandom<Bugs, BugsWeight<Bugs>>(bugWeights, Rand);
            tile.SpawnBugInfestation(new BugInfestation(bugs));

        }

        public bool[,] GetCurrentPosOfWeed()
        {
            bool[,] currentWeedPositions = new bool[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    currentWeedPositions[i, j] = GetTile(i, j).HasWeed;
                }
            }
            return currentWeedPositions;
        }

        //Takes each tile in the grid without weed and tries spreading weed to it from adjacent tiles with weed
        public void TrySpreadingWeed()
        {
            foreach (Tile tile in grid)
            {
                if (!tile.HasWeed)
                {
                    var adjacentIndexes = GetIndexesOfAdjacentTiles(tile.Row, tile.Column);
                    foreach ((int, int) index in adjacentIndexes)
                    {
                        if (GetTile(index).HasWeed)
                        tile.TrySpreadingWeed();
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
                if (( 0 <= index.Item1 && index.Item1 < grid.GetLength(0)) &&  ( 0 <= index.Item2 && index.Item2 < grid.GetLength(1))){
                    adjacentIndexes.Add(index);
                }
            }
            return adjacentIndexes;
        }

        public List<(int, int)> GetIndexesOfAdjacentTiles((int, int) pos)
        {
            return GetIndexesOfAdjacentTiles(pos.Item1, pos.Item2);
        }
         
    }
}
