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
        public WeedNotPresentException(string message) : base(message) { }
    }

    
    public class FlowerNotPresentException : GardenException
    {
        public FlowerNotPresentException(string message) : base(message) { }
    }

    public class FlowerAlreadyPresentException : GardenException
    {
        public FlowerAlreadyPresentException(string message) : base(message) { }
    }

    public class CoinsNotPresentException : GardenException
    {
        public CoinsNotPresentException(string message) : base(message) { }
    }

    public class FailedRandomItemSelectionException : GardenException
    {
        public FailedRandomItemSelectionException(string message) : base(message) { }
    }

    public class BugsAlreadyPresentException : GardenException
    {
        public BugsAlreadyPresentException(string message) : base(message) { }
    }

    public class BugsNotPresentException : GardenException
    {
        public BugsNotPresentException(string message) : base(message) { }
    }

    public class TileIndexOutOfRangeException : GardenException
    {
        public TileIndexOutOfRangeException(string message) : base(message) { }
    }
}
