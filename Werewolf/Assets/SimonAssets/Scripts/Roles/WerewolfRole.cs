using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.SimonAssets.Scripts.Roles
{
    class WerewolfRole : AbstractRole
    {

        #region Properties

        private List<Card> middleCards;
        private List<Card> players;
        
        private Card chosenCard = null;
        
        #endregion 

        #region Inherited

        public override void InitNightEvent()
        {
            players = BoardManager.instance.otherPlayerCards;
            middleCards = BoardManager.instance.middleCards;
            
            int nrOfWerewolfs = 0;
            foreach (Card player in players) {
                if(player.role == "WEREWOLF"){
                    nrOfWerewolfs++;
                }
            }
            if(nrOfWerewolfs == 0){
                foreach (Card c in middleCards)
                {
                    Button b = c.GetComponent<Button>();
                    b.onClick.AddListener(() => LookAtCard(c));
                    b.interactable = true;
                }
            }
        }

        public override void EndTurn()
        {
            base.DisableCards();
        }
    
        
        #endregion

        #region Listeners

        private void LookAtCard(Card card){
            base.DisableCards();
        }

        #endregion
    }
}
