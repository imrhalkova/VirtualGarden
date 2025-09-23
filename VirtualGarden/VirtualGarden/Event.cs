using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    /// <summary>
    /// The abstract class from which all events inherit.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The description of the event.
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// The number of days left of the event.
        /// </summary>
        [Save]
        public int DaysLeft { get; protected set; }
        protected Event() { }
        protected Event(int daysLeft)
        {
            DaysLeft = daysLeft;
        }
        public abstract void Update();
    }

    public class DroughtEvent : Event
    {
        public override string Name { get; } = "Drought";
        public override string Description { get; } = "A Drought has come to town. You can water only a limited number of flowers.";
        /// <summary>
        /// How many flowers the player can water this day.
        /// </summary>
        [Save]
        public int NumberOfWateringsADay {  get; private set; }
        /// <summary>
        /// How many flowers have already been watered today.
        /// </summary>
        [Save]
        public int NumberOfWateringsDoneToday { get; private set; }
        /// <summary>
        /// True if the player can still water flowers today, false otherwise.
        /// </summary>
        public bool CanWater
        {
            get
            {
                return NumberOfRemainingWaterings > 0;
            }
        }
        /// <summary>
        /// The number of flowers the player can still water today.
        /// </summary>
        public int NumberOfRemainingWaterings
        {
            get
            {
                return NumberOfWateringsADay - NumberOfWateringsDoneToday;
            }
        }
        private DroughtEvent() : base() { }
        public DroughtEvent(int daysLeft, int numberOfWateringsADay) : base(daysLeft)
        {
            NumberOfWateringsADay = numberOfWateringsADay;
            NumberOfWateringsDoneToday = 0;
        }

        public override void Update()
        {
            NumberOfWateringsDoneToday = 0;
            DaysLeft--;
        }

        public void IncrementNumberOfWateringsDoneToday()
        {
            NumberOfWateringsDoneToday++;
        }
    }

    public class RainEvent : Event
    {
        public override string Name { get; } = "Rain";
        public override string Description { get; } = "It's raining today. All your flowers will be automatically watered.";
        private RainEvent() : base() { }
        public RainEvent(int daysLeft) : base(daysLeft) { }
        public override void Update()
        {
            DaysLeft--;
        }
    }

    public class BrokenToolsEvent : Event
    {
        public override string Name { get; } = "Broken Tools";
        public override string Description { get; } = "Your tools have broken down. It will take some time to repair them. You won't be able to remove any weed while they are broken.";
        private BrokenToolsEvent() : base() { }
        public BrokenToolsEvent(int daysLeft) : base(daysLeft) {}
        public override void Update()
        {
            DaysLeft--;
        }
    }

    public class HumidWeatherEvent : Event
    {
        public override string Name { get; } = "Humid Weather";
        public override string Description { get; } = "The weather is humid today. There is a higher chance of bugs appearing than normal.";
        [Save]
        public double BugChanceMultiply { get; private set; }
        private HumidWeatherEvent() : base() { }
        public HumidWeatherEvent(int daysLeft, double bugChanceMultiply) : base(daysLeft)
        {
            BugChanceMultiply = bugChanceMultiply;
        }
        public override void Update()
        {
            DaysLeft--;
        }
    }

    public class HighTemperature : Event
    {
        public override string Name { get; } = "High Temperature";
        public override string Description { get; } = "The temperature is high today. Weeds will grow more on your garden.";
        [Save]
        public double WeedChanceMultiply { get; private set; }
        private HighTemperature() : base() { }
        public HighTemperature(int  daysLeft, double weedChanceMultiply) : base(daysLeft)
        {
            WeedChanceMultiply = weedChanceMultiply;
        }
        public override void Update()
        {
            DaysLeft--;
        }
    }

    public class EventWeight : IWeightedItem<Type>
    {
        public Type Item { get; private set; }
        public int Weight { get; private set; }
        public EventWeight(Type item, int weight)
        {
            Item = item;
            Weight = weight;
        }
    }

    /// <summary>
    /// Fulfills the function of generating new events.
    /// </summary>
    public static class EventGenerator
    {
        private static List<EventWeight> EventWeights { get; } = new List<EventWeight>()
        {
            new EventWeight(typeof(DroughtEvent), 30),
            new EventWeight(typeof(RainEvent), 50),
            new EventWeight (typeof(BrokenToolsEvent), 5),
            new EventWeight(typeof (HumidWeatherEvent), 15),
            new EventWeight(typeof (HighTemperature), 15)
        };
        
        public static Event GenerateEvent(IRandomNumberGenerator random, int numOfTiles)
        {
            Type eventType = WeightedRandom.ChooseWeightedRandom<Type, EventWeight>(EventWeights, random);
            Event chosenEvent;
            if (eventType == typeof(DroughtEvent))
            {
                chosenEvent = new DroughtEvent(random.Next(1, 4), random.Next(1, numOfTiles));
            }
            else if (eventType == typeof(RainEvent))
            {
                chosenEvent = new RainEvent(random.Next(1, 4));
            }
            else if (eventType == typeof(BrokenToolsEvent))
            {
                chosenEvent = new BrokenToolsEvent(random.Next(1, 4));
            }
            else if (eventType == typeof(HumidWeatherEvent))
            {
                chosenEvent = new HumidWeatherEvent(random.Next(1, 4), 2);
            }
            else if (eventType == typeof(HighTemperature))
            {
                chosenEvent = new HighTemperature(random.Next(1, 4), 2);
            }
            else
            {
                throw new EventGenerationException($"Cannot generate an unknown event {eventType.Name}");
            }
            return chosenEvent;
        }
    }
}
