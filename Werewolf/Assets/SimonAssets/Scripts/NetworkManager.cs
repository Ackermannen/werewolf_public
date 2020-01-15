using Assets.SimonAssets.Scripts.RoleCreator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using DarkRift;
using Assets.OneNight.Scripts.Network;

public class NetworkManager : MonoBehaviour
{

    [SerializeField] Text timerText;

    [SerializeField] MasterClientManager ClientManager;

    private AbstractRole myRole;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        ClientManager.attemptConnection("127.0.0.1");
    }

    public void SetRole(string roleName)
    {
        AbstractRoleCreator role;

        switch (roleName)
        {
            case "ROBBER":
                role = new RobberCreator();
                break;
            case "SEER":
                role = new SeerCreator();
                break;
            case "TROUBLE":
                role = new TroubleCreator();
                break;
            case "WEREWOLF":
                role = new WerewolfCreator();
                break;
            case "DRUNK":
                role = new DrunkCreator();
                break;
            case "INSOMNIAC":
                role = new InsomniacCreator();
                break;
            default:
                role = new OtherRoleCreator();
                break;
        }

        myRole = role.CreateRole();
    }

    public AbstractRole getMyRole() {
        return myRole;
    }

    public IEnumerator waitInSecondsToEndTurn(int seconds)
    {
        int counter = seconds;
        timerText.text = counter.ToString();
        while(counter > 0){
            timerText.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
        timerText.text = counter.ToString();
        
        Debug.Log("Ending turn");
        myRole.EndTurn();
    }
}
