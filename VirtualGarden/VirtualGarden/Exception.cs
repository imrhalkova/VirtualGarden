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

    public class BugsAlreadyPresent : GardenException
    {
        public BugsAlreadyPresent(string message) : base(message) { }
    }

    public class BugsNotPresent : GardenException
    {
        public BugsNotPresent(string message) : base(message) { }
    }
    
}
