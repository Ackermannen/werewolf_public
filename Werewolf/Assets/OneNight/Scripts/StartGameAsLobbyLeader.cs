using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAsLobbyLeader : MonoBehaviour
{
    private List<Card> playerCards;

    [SerializeField] MasterClientManager masterClientManager;

    [SerializeField] Button buttonToEnable;

    void Start()
    {
        playerCards = BoardManager.instance.otherPlayerCards;
    }


    public void Update()
    {
        int active = 0;
        int all = 0;
        if(masterClientManager.isLeader)
        {
            foreach(Card c in playerCards)
            {
                if (c.playerActive)
                {
                    active++;
                }
                all++;
            }

            if (active == all)
            {
                buttonToEnable.gameObject.SetActive(true);
            }
            else {
                buttonToEnable.gameObject.SetActive(false);
            }
        }
    }
}
