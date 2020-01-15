using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.OneNight.Scripts.Network
{
    class Player
    {
        public ushort ID { get; set; }
        public string name { get; set; }
        public string role { get; set; }

        public Player(ushort ID, string name, string role)
        {
            this.ID = ID;
            this.name = name;
            this.role = role;
        }

        public Player(ushort ID, string name)
        {
            this.ID = ID;
            this.name = name;
        }

        public Player(ushort ID)
        {
            this.ID = ID;
            name = "Player";
        }
    }
}
