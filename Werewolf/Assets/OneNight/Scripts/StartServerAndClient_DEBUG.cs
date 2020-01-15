using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartServerAndClient_DEBUG : MonoBehaviour
{

    [SerializeField] InputField inputField;

    public void StartServer()
    {
        // Load the server scene
        SceneManager.LoadScene("MasterServerScene", LoadSceneMode.Additive);
    }

    public void SetIPAdressToInputFieldValue() {
        GameMaster.instance.SetIPAdress(inputField.text.ToString());
        Debug.Log(inputField.text.ToString());
    }

}
