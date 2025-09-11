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
        public void Update()
        {
            DaysLeft--;
        }
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
                if (NumberOfWateringsDoneToday >= NumberOfWateringsADay)
                {
                    return false;
                }
                return true;
            }
        }
        public DroughtEvent(int daysLeft, int numberOfWateringsADay) : base(daysLeft)
        {
            NumberOfWateringsADay = numberOfWateringsADay;
            NumberOfWateringsDoneToday = 0;
        }

        public new void Update()
        {
            NumberOfWateringsDoneToday = 0;
            base.Update();
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
    }

    public class BrokenToolsEvent : Event
    {
        public override string Name { get; } = "Broken Tools";
        public override string Description { get; } = "Your tools have broken down. It will take some time to repair them. You won't be able to remove any weed while they are broken.";
        public BrokenToolsEvent(int daysLeft) : base(daysLeft) {}
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
    }

    public class WildRabbitEvent : Event
    {
        public override string Name { get; } = "A Wild Rabbit";
        public override string Description { get; } = "A wild rabbit has wandered into your garden. If you don't do anything about it, it might eat one of your flowers. You can leave the rabbit be, give it food, chase it away or take care of the problem more permanently...";

        public static bool IsAlive = true;
        private Garden _garden;
        public string? rabbitName { get; private set; }
        public WildRabbitEvent(int daysLeft, Garden garden) : base(daysLeft)
        {
            _garden = garden;
        }

        public new void Update()
        {
            double flowerEatenChance = _garden.Rand.NextDouble();
            if (_garden.Rand.NextDouble() < flowerEatenChance)
            {
                int numberOfFlowerToBeEaten = _garden.Rand.Next(1, _garden.FlowerCount);
                _garden.KillFlower(numberOfFlowerToBeEaten);
            }
            base.Update();
        }

        public void KillTheRabbit()
        {
            if (!IsAlive)
            {
                throw new KillingAlreadyDeadRabbitException($"Cannot kill the wild rabbit. It is already dead.");
            }
            IsAlive = false;
            //update event chances to 0
        }

        public void RenameTheRabbit(string name)
        {
            rabbitName = name;
        }
    }
}
