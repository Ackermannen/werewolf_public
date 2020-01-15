using Assets.OneNight.Scripts.Network;
using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class MasterServerManager : MonoBehaviourSingletonPersistent<MasterServerManager>
{
    #region Inspector Variables

    /// <summary>
    /// Reference to the DarkRift server
    /// </summary>
    [SerializeField]
    public XmlUnityServer serverReference;

    #endregion

    #region Non-Inspector Public Variables

    Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();



    #endregion


    #region Private Variables

    private Player LobbyLeader;

    ushort numOfPlayers = 3;

    List<string> roles = new List<string>();
    List<string> assignroles;

    string[] order = { "WEREWOLF", "MINION", "MASON", "SEER", "ROBBER", "TROUBLE", "DRUNK", "INSOMNIAC" };

    List<string> middleCards = new List<string>();

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        /*roles.Add("WEREWOLF");
        roles.Add("ROBBER");
        roles.Add("SEER");
        roles.Add("MASON");
        roles.Add("MASON");
        roles.Add("WEREWOLF");*/

        roles.Add("ROBBER");
        roles.Add("DRUNK");
        roles.Add("WEREWOLF");
        roles.Add("SEER");
        roles.Add("MASON");
        roles.Add("ROBBER");

        assignroles = new List<string>(roles);

        //Initialise XMLUnityServer
        serverReference = GetComponent<XmlUnityServer>();

        // Events subscription
        serverReference.Server.ClientManager.ClientConnected += ClientConnected;
        serverReference.Server.ClientManager.ClientDisconnected += ClientDisconnected;
    }

    #endregion

    #region Subscriptions

    /// <summary>
    /// When a client connects to the DarkRift server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClientConnected(object sender, ClientConnectedEventArgs e)
    {
        //Make user subscribe to these methods
        e.Client.MessageReceived += MessageReceived;

        Player newPlayer = new Player(e.Client.ID);


        //First player to join gets to be party leader
        if (players.Count == 0) {
            LobbyLeader = newPlayer;
            using (DarkRiftWriter lobbyLeaderWriter = DarkRiftWriter.Create())
            {
                //Filler
                lobbyLeaderWriter.Write(e.Client.ID);

                using (Message playerMessage = Message.Create(Tags.UserLeaderTag, lobbyLeaderWriter))
                    e.Client.SendMessage(playerMessage, SendMode.Reliable);
            }
        }

        //Add to Dictionary
        players.Add(e.Client, newPlayer);



        //Tell all other users that a new user has joined
        using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create())
        {
            newPlayerWriter.Write(newPlayer.ID);
            newPlayerWriter.Write(newPlayer.name);

            using (Message newPlayerMessage = Message.Create(Tags.newUserConnectedTag, newPlayerWriter))
            {
                foreach (IClient client in serverReference.Server.ClientManager.GetAllClients())
                {
                    if(client.ID != e.Client.ID)
                    {
                        Debug.Log("New client join and sending this to all users");
                        client.SendMessage(newPlayerMessage, SendMode.Reliable);
                    }
                }
            }
        }

        //Return all users already on server to client that just connected
        using (DarkRiftWriter playerWriter = DarkRiftWriter.Create())
        {
            //First write back the number of players
            playerWriter.Write(numOfPlayers);

            //Then write back their id so they know who they are
            playerWriter.Write(e.Client.ID);

            //Then send back all player id and names
            foreach (Player player in players.Values)
            {
                playerWriter.Write(player.ID);
                playerWriter.Write(player.name);
            }

            using (Message playerMessage = Message.Create(Tags.UserConnectedTag, playerWriter))
                e.Client.SendMessage(playerMessage, SendMode.Reliable);
        }


    }

    /// <summary>
    /// When a client disconnects to the DarkRift server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {

        players.Remove(e.Client);

        //Tell all other users a user has left
        using (DarkRiftWriter removePlayerWriter = DarkRiftWriter.Create())
        {
            removePlayerWriter.Write(e.Client.ID);

            using (Message removePlayerMessage = Message.Create(Tags.UserLeftTag, removePlayerWriter))
            {
                foreach (IClient client in serverReference.Server.ClientManager.GetAllClients())
                {
                    if (client.ID != e.Client.ID)
                    {
                        Debug.Log("Client has left: " + e.Client.ID);
                        client.SendMessage(removePlayerMessage, SendMode.Reliable);
                    }
                }
            }
        }
    }

    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            if (message.Tag == Tags.NameUpdateTag)
            {
                NameInfoMessage(sender, e);
            }
            else if (message.Tag == Tags.BoardHasChangedTag)
            {
                //UpdateEverybodysBoard(sender, e);
            }
            else if (message.Tag == Tags.StartGameTag && e.Client.ID == LobbyLeader.ID)
            {
                GameStart(sender, e);
            }
        }
    }

    private void UpdateEverybodysBoard(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Time to update everybodys board");
        //Update own playerList
        using (Message message = e.GetMessage() as Message)
        {
            using (DarkRiftReader reader = message.GetReader())
            {
                while (reader.Position < reader.Length)
                {
                    ushort id = reader.ReadUInt16();
                    string role = reader.ReadString();
                    players[serverReference.Server.ClientManager.GetClient(id)].role = role;
                }
            }
        }

        //Tell all (except the one who updated the board) to update their boards
        using (DarkRiftWriter updateBoardWriter = DarkRiftWriter.Create())
        {
            foreach(Player player in players.Values)
            {
                updateBoardWriter.Write(player.ID);
                updateBoardWriter.Write(player.role);
            }

            using (Message updateBoardMessage = Message.Create(Tags.BoardHasChangedTag, updateBoardWriter))
            {
                foreach (IClient client in serverReference.Server.ClientManager.GetAllClients())
                {
                    if (client.ID != e.Client.ID)
                    {
                        Debug.Log("Sending board update to user: " + e.Client.ID);
                        client.SendMessage(updateBoardMessage, SendMode.Reliable);
                    }
                }
            }
        }
    }

    private void NameInfoMessage(object sender, MessageReceivedEventArgs e)
    {
        //Read new name and edit it locally
        using (Message message = e.GetMessage() as Message)
        {
            using (DarkRiftReader reader = message.GetReader())
            {
                string name = reader.ReadString();

                players[e.Client].name = name;
            }
        }

        //Tell all other users a user has changed their name
        using (DarkRiftWriter changeNameWriter = DarkRiftWriter.Create())
        {
            changeNameWriter.Write(e.Client.ID);
            changeNameWriter.Write(players[e.Client].name);

            using (Message changeNameMessage = Message.Create(Tags.NameUpdateTag, changeNameWriter))
            {
                foreach (IClient client in serverReference.Server.ClientManager.GetAllClients())
                {
                    if (client.ID != e.Client.ID)
                    {
                        Debug.Log("Client has changed their name: " + e.Client.ID);
                        client.SendMessage(changeNameMessage, SendMode.Reliable);
                    }
                }
            }
        }
    }

    private void GameStart(object sender, MessageReceivedEventArgs e)
    {
        //Game should on start:
        /*
         * - Send a message to all users with their roles
         * - Send a message with the 3 middle cards and their roles
         * - Disallow anymore people joining
         * - Disallow people leaving (Stopping the session)
         * */
        

        //Tell all users the roles
        using (DarkRiftWriter updateBoardWriter = DarkRiftWriter.Create())
        {
            foreach (Player player in players.Values)
            {
                updateBoardWriter.Write(player.ID);
                player.role = AssignRandomRole();
                updateBoardWriter.Write(player.role);
            }

            using (Message updateBoardMessage = Message.Create(Tags.GetRoleTag, updateBoardWriter))
            {
                foreach (IClient client in serverReference.Server.ClientManager.GetAllClients())
                {
                    Debug.Log("Sending roles to user: " + client.ID);
                    client.SendMessage(updateBoardMessage, SendMode.Reliable);
                }
            }
        }

        //Tell all users the middle cards roles
        using (DarkRiftWriter updateBoardWriter = DarkRiftWriter.Create())
        {
            middleCards.Add(AssignRandomRole());
            middleCards.Add(AssignRandomRole());
            middleCards.Add(AssignRandomRole());

            foreach(string s in middleCards)
                updateBoardWriter.Write(s);

            using (Message updateBoardMessage = Message.Create(Tags.middleCardRolesTag, updateBoardWriter))
            {
                foreach (IClient client in serverReference.Server.ClientManager.GetAllClients())
                {
                    Debug.Log("Sending middle cards to user: " + client.ID);
                    client.SendMessage(updateBoardMessage, SendMode.Reliable);
                }
            }
        }

        StartCoroutine(NightPhase());
    }

    private IEnumerator NightPhase()
    {
        yield return new WaitForSeconds(3);
        //Tell each user in order to do their role
        foreach (string role in order)
        {
            List<IClient> clients = new List<IClient>();
            foreach (KeyValuePair<IClient, Player> entry in players)
            {
                if (entry.Value.role == role)
                {
                    clients.Add(entry.Key);
                }
            }
            if (clients.Count != 0)
            {
                Debug.Log("DO NIGHT EVENT");
                DoNightEvent(clients);
                yield return new WaitForSeconds(11);
            }
        }
    }
    
    private void DoNightEvent(List<IClient> clients)
    {
        using (DarkRiftWriter updateBoardWriter = DarkRiftWriter.Create())
        {
            using (Message updateBoardMessage = Message.Create(Tags.DoNightEventTag, updateBoardWriter))
            {
                foreach (IClient client in clients)
                {
                    Debug.Log("It's Client turn: " + client.ID);
                    client.SendMessage(updateBoardMessage, SendMode.Reliable);
                }
            }
        }
    }

    private string AssignRandomRole()
    {
        int rand = Random.Range(0, assignroles.Count);
        string role = assignroles[rand];
        Debug.Log(role);
        assignroles.RemoveAt(rand);

        return role;
    }


    

    #endregion


}
