using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class Player
    {
        /// <summary>
        /// The number of coins the player currently has.
        /// </summary>
        public int Money { get; private set; } = 0;

        /// <summary>
        /// The number of coins the player automatically receives with each new day
        /// </summary>
        public int NewDayIncome { get; private set; } = 10;

        public int NumberOfDay { get; set; } = 1;

        public Player(int startingCoins, int newDayIncome)
        {
            Money = startingCoins;
            NewDayIncome = newDayIncome;
        }
        public Player() { }

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
