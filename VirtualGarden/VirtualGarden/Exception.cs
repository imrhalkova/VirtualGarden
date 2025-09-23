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

    public class EventGenerationException : GardenException
    {
        internal EventGenerationException(string message) : base(message) { }
    }

    public class LoadStoreException : Exception
    {
        internal LoadStoreException(string message) : base(message) { }
    }

    public class UnableToLoadImageFromFileException : LoadStoreException
    {
        internal UnableToLoadImageFromFileException(string message) : base(message) { }
    }

    public class SerializationException : LoadStoreException
    {
        internal SerializationException(string message) : base(message) { }
    }

    public class DeserializationException : LoadStoreException
    {
        internal DeserializationException(string message) : base(message) { }
    }

    public class UIException : Exception
    {
        internal UIException(string message) : base(message) { }
    }

    public class NoTileIsSelectedUIException : UIException
    {
        internal NoTileIsSelectedUIException(string message) : base(message) { }
    }
}
