using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.SimonAssets.Scripts.Roles
{
    class InsomniacRole : AbstractRole
    {
        private Card ownCard;
        
        public override void InitNightEvent()
        {
            ownCard = BoardManager.instance.myCard;
        }

        public override void EndTurn()
        {
            ownCard.gameObject.GetComponentInChildren<Text>().text = ownCard.playerName;
        }
    }
}
