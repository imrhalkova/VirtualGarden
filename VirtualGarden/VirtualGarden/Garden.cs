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

        private double _normalBugsChance = 0.3;

        double? EventBugsChance { get; set; } = null;

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

        public double WeedSpreadChance { get; private set; } = 0.5;

        public Random rand { get; } = new Random();

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
                    grid[i, j] = new Tile(this);
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
            foreach (Tile tile in grid)
            {
                tile.UpdateFlowers();
            }
            UpdateWeed();
            Player.Money += NewDayIncome;
        }
        public void UpdateWeed()
        {
            bool[,] previousWeedPositions = new bool[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    previousWeedPositions[i, j] = GetTile(i, j).HasWeed;
                }
            }

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Tile tile = GetTile(i, j);
                    if (!tile.HasWeed)
                    {
                        var adjacentIndexes = GetIndexesOfAdjacentTiles(i, j);
                        foreach ((int, int) index in adjacentIndexes)
                        {
                            GetTile(index).TrySpreadingWeed();
                        }
                    }
                
                }
            }

            foreach (Tile tile in grid)
            {
                tile.TrySpawningWeed();
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
    }
}
