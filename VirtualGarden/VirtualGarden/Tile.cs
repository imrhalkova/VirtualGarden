#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class Tile
    {
        public Flower? Flower;
        public bool Weed {  get; set; } = false;
        public bool Bugs { get; set; } = false;

        public Tile() { }
    }
}
