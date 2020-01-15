using Assets.OneNight.Scripts.Network;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ClientBoardManager : MonoBehaviour
{

    [SerializeField]
    private MasterClientManager masterManager;

    [SerializeField]
    private NetworkManager networkManager;

    private Card myCard;

    void Awake()
    {
        myCard = BoardManager.instance.myCard;
        masterManager.clientReference.MessageReceived += MessageReceived;
    }

    #region Answers

    void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using(Message message = e.GetMessage() as Message)
        {
            if (message.Tag == Tags.BoardHasChangedTag)
            {
                BoardChanged(sender, e);
            }
            else if (message.Tag == Tags.DoNightEventTag)
            {
                DoNightEvent(sender, e);
            }
            else if (message.Tag == Tags.GetRoleTag)
            {
                GetRole(sender, e);
            }
            else if (message.Tag == Tags.middleCardRolesTag)
            {
                SetMiddleCards(sender, e);
            }
        }
    }

    void SetMiddleCards(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            int counter = 0;
            while (reader.Position < reader.Length)
            {
                string role = reader.ReadString();

                BoardManager.instance.middleCards[counter].role = role;
                counter++;
            }
        }
        BoardManager.instance.updateCardSprites();
    }

    void BoardChanged(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Changing the board");
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            while (reader.Position < reader.Length)
            {
                ushort id = reader.ReadUInt16();
                string role = reader.ReadString();

                BoardManager.instance.allCards[id].role = role;
            }
        }
    }

    void DoNightEvent(object sender, MessageReceivedEventArgs e)
    {
        AbstractRole role = networkManager.getMyRole();

        role.InitNightEvent();

        StartCoroutine(networkManager.waitInSecondsToEndTurn(10));

        /*
         * Here should be code to return the updated playfield including all cards
         */

    }

    void GetRole(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            while (reader.Position < reader.Length)
            {
                ushort id = reader.ReadUInt16();
                string role = reader.ReadString();

                BoardManager.instance.allCards[id].role = role;

                if (BoardManager.instance.myCard.id == id)
                {
                    Debug.Log("Your role is: " + role);
                    networkManager.SetRole(role);
                }
            }
        }
        BoardManager.instance.updateCardSprites();
    }
    #endregion

    #region Queries

    public void ChangeName(string newName) {
        
        using (DarkRiftWriter changeNameWriter = DarkRiftWriter.Create())
        {
            changeNameWriter.Write(myCard.id);
            changeNameWriter.Write(newName);
            myCard.name = newName;

            using (Message changeNameMessage = Message.Create(Tags.NameUpdateTag, changeNameWriter))
            {
                masterManager.clientReference.SendMessage(changeNameMessage, SendMode.Reliable);
            }
        }
    }


    //DEPRACATED
    public void SetupGame(string[] roles)
    {
        using (DarkRiftWriter gameSetupWriter = DarkRiftWriter.Create())
        {
            foreach (string s in roles)
            {
                gameSetupWriter.Write(s);
            }
            
            using (Message changeNameMessage = Message.Create(Tags.GameSetupTag, gameSetupWriter))
            {
                masterManager.clientReference.SendMessage(changeNameMessage, SendMode.Reliable);
            }
        }
    }

    

    #endregion
}
