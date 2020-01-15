using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractRole
{
    #region Properties

    List<Card> players;
    private Card playerVote = null;

    #endregion

    #region Functions

    public abstract void InitNightEvent();

    public abstract void EndTurn();

    public void StartVote() 
    {
        players = BoardManager.instance.otherPlayerCards;
        foreach (Card c in players)
        {
            Button b = c.GetComponent<Button>();
            b.onClick.AddListener(() => ChoosePlayer(c));
            b.interactable = true;
        }
    }

    public void EndVote() 
    {
        //SendToServer(playerVote.id)
        DisableCards();
    }

    public void DisableCards()
    {
        List<Card> players = BoardManager.instance.otherPlayerCards;
        List<Card> middleCards = BoardManager.instance.middleCards;
        Card ownCard = BoardManager.instance.myCard;

        foreach (Card c in players)
        {
            Button b = c.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.interactable = false;
        }

        foreach (Card c in middleCards)
        {
            Button b = c.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.interactable = false;
        }

        Button ownCardButton = ownCard.GetComponent<Button>();
        ownCardButton.onClick.RemoveAllListeners();
        ownCardButton.interactable = false;
    }

    #endregion

    #region Listeners

    private void ChoosePlayer(Card player){
        if(playerVote != null){
            foreach(Card c in players){
                if(c.id == playerVote.id){
                    Button currentChoiceB = c.GetComponent<Button>();
                    currentChoiceB.onClick.AddListener(() => ChoosePlayer(c));
                    currentChoiceB.interactable = true;
                }
            }
        }
        
        playerVote = player;
        Button newChoiceB = player.GetComponent<Button>();
        newChoiceB.onClick.RemoveAllListeners();
        newChoiceB.interactable = false;
    }

    #endregion
}
