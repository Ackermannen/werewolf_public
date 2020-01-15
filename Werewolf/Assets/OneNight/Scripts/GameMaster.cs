using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    public static GameMaster instance = null;

    private string IPAdress;

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

    void Start()
    {
        IPAdress = "127.0.0.1";
        
    }

    #region Getters & Setters

    public void SetIPAdress(string IP)
    {
        IPAdress = IP;
    }

    public string GetIPAdress()
    {
        return IPAdress;
    }

    #endregion

}
