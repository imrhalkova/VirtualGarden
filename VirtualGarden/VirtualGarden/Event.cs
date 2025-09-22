using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public abstract class Event
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public int DaysLeft { get; protected set; }
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
        public int NumberOfWateringsADay {  get; private set; }
        public int NumberOfWateringsDoneToday { get; private set; }

        public bool CanWater
        {
            get
            {
                return NumberOfRemainingWaterings > 0;
            }
        }
        public int NumberOfRemainingWaterings
        {
            get
            {
                return NumberOfWateringsADay - NumberOfWateringsDoneToday;
            }
        }
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
        public double BugChanceMultiply { get; private set; }
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
        public double WeedChanceMultiply { get; private set; }
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
