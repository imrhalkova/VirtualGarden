using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class Player
    {
        /// <summary>
        /// The number of coins the player currently has.
        /// </summary>
        [Save]
        public int Money { get; private set; } = 0;

        /// <summary>
        /// The number of coins the player automatically receives with each new day.
        /// </summary>
        [Save]
        public int NewDayIncome { get; private set; } = 10;

        /// <summary>
        /// Number of the current day.
        /// </summary>
        [Save]
        public int NumberOfDay { get; set; } = 1;

        /// <summary>
        /// The count of flowers that have bloomed for each flower type.
        /// </summary>
        [Save]
        public Dictionary<FlowerType, int> FlowerTypesBloomCount { get; private set; }

        public Player(int startingCoins, int newDayIncome)
        {
            Money = startingCoins;
            NewDayIncome = newDayIncome;
            InitializeFlowerTypesBloomedDict();
        }
        public Player() { InitializeFlowerTypesBloomedDict(); }

        private void InitializeFlowerTypesBloomedDict()
        {
            FlowerTypesBloomCount = new Dictionary<FlowerType, int>();
            Type type = typeof(FlowerTypes);
            FieldInfo[] flowerTypesFields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo flowerTypeField in flowerTypesFields)
            {
                if(flowerTypeField.GetValue(null) is FlowerType flowerType)
                {
                    FlowerTypesBloomCount.Add(flowerType, 0);
                }
            }
        }

        public void Update()
        {
            Money += NewDayIncome;
            NumberOfDay++;
        }

        public void AddMoney(int money)
        {
            Money += money;
        }

        public void SpendMoney(int money)
        {
            Money -= money;
        }

        public void BuySeeds(FlowerType flower)
        {
            if (flower.SeedPrice > Money)
            {
                throw new InsufficientFundsException($"Cannot buy and plant {flower.Name} seeds. Not enought money to buy them.");
            }
            SpendMoney(flower.SeedPrice);
        }
    }
}
