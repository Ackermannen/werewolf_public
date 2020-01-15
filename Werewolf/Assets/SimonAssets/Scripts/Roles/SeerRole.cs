using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.SimonAssets.Scripts.Roles
{
    class SeerRole : AbstractRole
    {
        #region Properties
        
        private List<Card> players;
        private List<Card> middleCards;

        private Card firstCard = null;
        private Card secondCard = null;

        #endregion

        #region Inherited

        public override void InitNightEvent()
        {
            players = BoardManager.instance.otherPlayerCards;
            middleCards = BoardManager.instance.middleCards;

            foreach (Card c in players)
            {
                Button b = c.GetComponent<Button>();
                b.onClick.AddListener(() => DisplayPlayerCard(c));
                b.interactable = true;
            }

            foreach (Card c in middleCards)
            {
                Button b = c.GetComponent<Button>();
                b.onClick.AddListener(() => DisplayMiddleCard(c));
                b.interactable = true;
            }

        }

        public override void EndTurn()
        {
            base.DisableCards();
        }

        #endregion

        #region Listeners

        private void DisplayPlayerCard(Card card)
        {
            
            firstCard = card;
            base.DisableCards();
        }

        private void DisplayMiddleCard(Card card)
        {
            if(firstCard == null)
            {
                firstCard = card;
                foreach (Card c in players)
                {
                    Button b = c.GetComponent<Button>();
                    b.onClick.RemoveAllListeners();
                    b.interactable = false;
                }

                Button firstCardButton = card.GetComponent<Button>();
                firstCardButton.onClick.RemoveAllListeners();
                firstCardButton.interactable = false;
            } else
            {
                secondCard = card;
                base.DisableCards();
            }
        }

        #endregion
    }
}
