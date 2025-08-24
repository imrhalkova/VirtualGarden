using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class GardenException : Exception
    {
        public GardenException(string message) : base(message) { }
    }

    public class WeedNotPresentException : GardenException
    {
        public WeedNotPresentException() : base("There is no weed on this tile.") { }
    }

    
    public class FlowerNotPresentException : GardenException
    {
        public FlowerNotPresentException() : base("There is no flower on this tile.") { }
    }

    public class FlowerAlreadyPresentException : GardenException
    {
        public FlowerAlreadyPresentException() : base("There is already a flower on this tile") { }
    }

    public class CoinsNotPresentException : GardenException
    {
        public CoinsNotPresentException() : base("There are no coins on this tile") { }
    }
    
}
