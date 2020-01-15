using Assets.OneNight.Scripts.Network;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Utilities;

public class MasterClientManager : MonoBehaviour
{

    #region Inspector Variables

    /// <summary>
    /// Reference to the DarkRift client
    /// </summary>
    [SerializeField]
    public UnityClient clientReference;

    public bool isLeader = false;

    #endregion

    #region Non-Inspector Public Variables



    #endregion

    #region Private Variables

    #endregion

    #region Unity Lifecycle

    void Awake()
    {

        //////////////////
        /// Properties initialization
        clientReference = GetComponent<UnityClient>();

        clientReference.MessageReceived += MessageReceived;
    }
    #endregion

    #region Connection
    public void attemptConnection(string ip) {
        clientReference.ConnectInBackground(
            IPAddress.Parse(ip),
            4296,
            IPVersion.IPv4,
            null
            );
    }
    #endregion

    #region Answers

    /// <summary>
    /// Collection of actions to do when a message is received.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            if (message.Tag == Tags.UserConnectedTag)
            {
                AddAllOtherPlayers(sender, e);
            }
            else if (message.Tag == Tags.newUserConnectedTag)
            {
                AddNewPlayer(sender, e);
            }
            else if (message.Tag == Tags.UserLeftTag)
            {
                RemovePlayer(sender, e);
            }
            else if (message.Tag == Tags.NameUpdateTag)
            {
                NameChange(sender, e);
            }
            else if (message.Tag == Tags.UserLeaderTag)
            {
                BecomeLeader(sender, e);
            }
        }
    }

    /// <summary>
    /// When a client leaves it needs to be removed from the client-side as well. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void RemovePlayer(object sender, MessageReceivedEventArgs e) {
        Debug.Log("Removing Player");

        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            while (reader.Position < reader.Length)
            {
                ushort id = reader.ReadUInt16();
                BoardManager.instance.allCards[id].playerActive = false;
            }
        }
    }

    /// <summary>
    /// When a new player joins the server the client needs to add it to the Dictionary.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AddNewPlayer(object sender, MessageReceivedEventArgs e) {
        Debug.Log("Adding new Player");
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            while (reader.Position < reader.Length)
            {
                ushort id = reader.ReadUInt16();
                string name = reader.ReadString();


                Card newPlayer = BoardManager.instance.getFirstFreeCard();

                newPlayer.id = id;
                newPlayer.playerName = name;
                newPlayer.playerActive = true;
            }
            //Since we changed the cardlist above, update all the cards to use the proper id.
            //Prevents null exceptions when using the hashmap
            BoardManager.instance.setAllCards();
        }
    }

    /// <summary>
    /// When a user joins a server it is immediately greeted with a list of all other players already there.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AddAllOtherPlayers(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Adding other players in game");
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            //Set the amount of players according to server
            BoardManager.instance.setBoard(reader.ReadUInt16());

            //Set your own ID
            BoardManager.instance.myCard.id = reader.ReadUInt16();


            int counter = 0; 
            //Iterate through entire list of connected users
            while (reader.Position < reader.Length)
            {
                ushort id = reader.ReadUInt16();
                string name = reader.ReadString();

                if (id != BoardManager.instance.myCard.id)
                {
                    Card newPlayer = BoardManager.instance.otherPlayerCards[counter];

                    newPlayer.id = id;
                    newPlayer.playerName = name;
                    newPlayer.playerActive = true;
                } else
                {
                    Card newPlayer = BoardManager.instance.myCard;

                    newPlayer.playerName = name;
                    newPlayer.playerActive = true;
                }
                counter++;
            }

            //allCards needs to be updated with the new ID:s, since it uses default values
            BoardManager.instance.setAllCards();
        }
    }

    /// <summary>
    /// A user can change their name after they've joined a lobby.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void NameChange(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {

            while (reader.Position < reader.Length)
            {
                ushort id = reader.ReadUInt16();
                string name = reader.ReadString();
                BoardManager.instance.allCards[id].name = name;
            }
        }
    }

    void BecomeLeader(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            ushort id = reader.ReadUInt16();
            isLeader = true;
        }
    }
    #endregion

    #region Queries

    public void StartGame()
    {
        using (DarkRiftWriter gameStartWriter = DarkRiftWriter.Create())
        {

            using (Message gameStartMessage = Message.Create(Tags.StartGameTag, gameStartWriter))
            {
                clientReference.SendMessage(gameStartMessage, SendMode.Reliable);
            }
        }
    }

    #endregion
}
