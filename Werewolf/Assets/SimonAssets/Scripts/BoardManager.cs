using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public List<Card> middleCards;

    public List<Card> otherPlayerCards;

    public Card myCard;

    public Dictionary<ushort, Card> allCards = new Dictionary<ushort, Card>();

    [SerializeField] public StartGameAsLobbyLeader startGame;

    /* Boards */
    [SerializeField] List<GameObject> boards;

    public static BoardManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    public void setBoard(ushort numOfPlayers) {
        boards[numOfPlayers - 3].SetActive(true);
        setMiddleCards();
        setOtherPlayerCards();
        setPlayerCard();
        setAllCards();
        startGame.gameObject.SetActive(true);
    }

    public Card getFirstFreeCard() {
        foreach (Card c in otherPlayerCards)
        {
            Debug.Log("Testing: " + c.id);
            if (!c.playerActive)
            {
                Debug.Log("Returning: " + c);
                return c;
            }
        }
        return null;
    }

    public void setAllCards()
    {
        allCards = new Dictionary<ushort, Card>();

        allCards.Add(myCard.id,myCard);

        foreach (Card c in otherPlayerCards)
        {
            allCards.Add(c.id,c);
        }

        foreach (Card c in middleCards)
        {
            allCards.Add(c.id,c);
        }
    }

    public void updateCardSprites()
    {
        foreach (Card c in allCards.Values)
        {
            c.UpdateImage();
        }
    }



    public void setOtherPlayerCards() {
        Object[] o = GameObject.FindGameObjectsWithTag("OtherPlayerCard");
        List<Card> Cards = new List<Card>();

        foreach (GameObject obj in o) {
            Cards.Add(obj.GetComponent<Card>());
        }
        otherPlayerCards = Cards;
    }

    public void setMiddleCards()
    {
        Object[] o = GameObject.FindGameObjectsWithTag("MiddleCard");
        List<Card> Cards = new List<Card>();

        foreach (GameObject obj in o)
        {
            Cards.Add(obj.GetComponent<Card>());
        }
        middleCards = Cards;
    }

    public void setPlayerCard()
    {
        Object[] o = GameObject.FindGameObjectsWithTag("PlayerCard");
        myCard = ((GameObject)o[0]).GetComponent<Card>();
    }
}
