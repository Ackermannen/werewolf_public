using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimonAssets.Scripts.Roles
{
    class DrunkRole : AbstractRole
    {
        #region Properties

        private Card ownCard;

        private List<Card> middleCards;

        #endregion

        #region Inherited

        public override void InitNightEvent()
        {
            middleCards = BoardManager.instance.middleCards;

            ownCard = BoardManager.instance.myCard;
            foreach (Card c in middleCards)
            {
                Button b = c.GetComponent<Button>();
                b.onClick.AddListener(() => SwitchCard(c));
                b.interactable = true;
            }

        }

        public override void EndTurn()
        {

            base.DisableCards();
        }

        #endregion

        #region Listeners

        private void SwitchCard(Card card)
        {
            string temp = ownCard.role;

            ownCard.role = card.role;

            card.role = temp;

            BoardManager.instance.updateCardSprites();
            base.DisableCards();
        }

        #endregion
    }
}
