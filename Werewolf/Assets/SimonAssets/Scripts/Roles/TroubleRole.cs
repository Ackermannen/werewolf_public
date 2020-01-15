using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.SimonAssets.Scripts.Roles
{
    /* Kan byta två andras kort */

    class TroubleRole : AbstractRole
    {
        #region Properties
        
        private List<Card> players;
        
        private Card firstCard = null;
        private Card secondCard = null;

        #endregion

        #region Inherited

        public override void InitNightEvent()
        {
            players = BoardManager.instance.otherPlayerCards;

            foreach (Card c in players)
            {
                Button b = c.GetComponent<Button>();
                b.onClick.AddListener(() => SelectCard(c));
                b.interactable = true;
            }
        }

        public override void EndTurn()
        {
            base.DisableCards();
        }

        #endregion

        #region Listeners

        private void SelectCard(Card card)
        {
            if(firstCard == null)
            {
                firstCard = card;

                Button firstCardButton = card.GetComponent<Button>();
                firstCardButton.onClick.RemoveAllListeners();
                firstCardButton.interactable = false;
            } else
            {
                secondCard = card;
                SwitchCards(firstCard, secondCard);
                base.DisableCards();
            }
        }

        #endregion

        #region Functions 

        private void SwitchCards(Card c1, Card c2)
        {
            string temp = c1.role;

            c1.role = c2.role;

            c2.role = temp;
        }

        #endregion
    }
}
