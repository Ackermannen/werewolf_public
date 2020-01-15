using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobberRole : AbstractRole
{
    #region Properties

    private Card ownCard;

    private List<Card> players;

    #endregion

    #region Inherited

    public override void InitNightEvent()
    {
        players = BoardManager.instance.otherPlayerCards;

        ownCard = BoardManager.instance.myCard;

        foreach (Card c in players)
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

    private void SwitchCard(Card card) {
        string temp = ownCard.role;

        ownCard.role = card.role;

        card.role = temp;

        //Show card to player

        BoardManager.instance.updateCardSprites();
        base.DisableCards();
    }

    #endregion
    
}
