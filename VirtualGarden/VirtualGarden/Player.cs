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
        public int Coins { get; private set; } = 0;

        /// <summary>
        /// The number of coins the player automatically receives with each new day
        /// </summary>
        public int NewDayIncome { get; private set; } = 10;

        public PlayerStatistics playerStatistics = new PlayerStatistics();

        public Player(int startingCoins, int newDayIncome)
        {
            Coins = startingCoins;
            NewDayIncome = newDayIncome;
        }
        public Player() { }

        public void Update()
        {
            Coins += NewDayIncome;
            playerStatistics.numberOfDay++;
        }

        public void AddMoney(int money)
        {
            Coins += money;
        }

        public void SpendMoney(int money)
        {
            Coins -= money;
        }

        public class PlayerStatistics
        {
            public int numberOfDay { get; set; } = 0;
            public int FlowersWatered { get; set; } = 0;
            public PlayerStatistics() { }
        }
    }
}
