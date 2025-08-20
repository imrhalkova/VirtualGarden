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
        Tile[,] grid;

        public Player Player {  get; private set; }

        private double _normalWeedChance = 0.3;

        double? EventWeedChance { get; set; } = null;

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

        public Random rand { get; } = new Random();
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

        public void Update()
        {
            foreach (Tile tile in grid)
            {
                tile.Update();
            }
            Player.Money += NewDayIncome;
        }
    }
}
