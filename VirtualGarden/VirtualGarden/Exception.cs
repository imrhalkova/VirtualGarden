using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class GardenException : Exception
    {
        internal GardenException(string message) : base(message) { }
    }

    public class WeedNotPresentException : GardenException
    {
        internal WeedNotPresentException(string message) : base(message) { }
    }

    public class FlowerNotPresentException : GardenException
    {
        internal FlowerNotPresentException(string message) : base(message) { }
    }

    public class FlowerAlreadyPresentException : GardenException
    {
        internal FlowerAlreadyPresentException(string message) : base(message) { }
    }

    public class CoinsNotPresentException : GardenException
    {
        internal CoinsNotPresentException(string message) : base(message) { }
    }

    public class FailedRandomItemSelectionException : GardenException
    {
        internal FailedRandomItemSelectionException(string message) : base(message) { }
    }

    public class BugsAlreadyPresentException : GardenException
    {
        internal BugsAlreadyPresentException(string message) : base(message) { }
    }

    public class BugsNotPresentException : GardenException
    {
        internal BugsNotPresentException(string message) : base(message) { }
    }

    public class TileIndexOutOfRangeException : GardenException
    {
        internal TileIndexOutOfRangeException(string message) : base(message) { }
    }

    public class NoWaterLeftException : GardenException
    {
        internal NoWaterLeftException(string message) : base(message) { }
    }

    public class BrokenToolsException : GardenException
    {
        internal BrokenToolsException(string message) : base(message) { }
    }

    public class InsufficientFundsException : GardenException
    {
        internal InsufficientFundsException(string message) : base(message) { }
    }

    public class KillingAlreadyDeadFlowerException : GardenException
    {
        internal KillingAlreadyDeadFlowerException(string message) : base(message) { }
    }

    public class KillingAlreadyDeadRabbitException : GardenException
    {
        internal KillingAlreadyDeadRabbitException(string message) : base(message) { }
    }

    public class WildRabbitEventNotActiveException : GardenException
    {
        internal WildRabbitEventNotActiveException(string message) : base(message) { }
    }

    public class FileException : Exception
    {
        internal FileException(string message) : base(message) { }
    }

    public class UnableToLoadImageFromFileException : FileException
    {
        internal UnableToLoadImageFromFileException(string message) : base(message) { }
    }

    public class UIException : Exception
    {
        internal UIException(string message) : base(message) { }
    }

    public class UnableToUpdateUIException : Exception
    {
        internal UnableToUpdateUIException(string message) : base(message) { }
    }
}
