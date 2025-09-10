using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class Player
    {
        public int Money { get; private set; }

        public Player()
        {
            Money = 20;
        }
        public Player(int money)
        {
            Money = money;
        }

        public void AddMoney(int money)
        {
            Money += money;
        }
    }
}
