using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.OneNight.Scripts.Network
{
    class Tags
    {
        public static readonly ushort NameUpdateTag = 0;
        public static readonly ushort newUserConnectedTag = 1;
        public static readonly ushort UserConnectedTag = 2;
        public static readonly ushort UserLeftTag = 3;
        public static readonly ushort BoardHasChangedTag = 4;
        public static readonly ushort UserLeaderTag = 5;
        public static readonly ushort DoNightEventTag = 6;
        public static readonly ushort GetRoleTag = 7;
        public static readonly ushort StartGameTag = 8;
        public static readonly ushort GameSetupTag = 9;
        public static readonly ushort middleCardRolesTag = 10;
    }
}
