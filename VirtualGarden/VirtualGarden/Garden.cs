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

        public Garden(int rows, int columns) 
        { 
            grid = new Tile[rows, columns];
        }
    }
}
