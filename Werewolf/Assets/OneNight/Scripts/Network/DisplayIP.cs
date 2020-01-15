using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DisplayIP : MonoBehaviour
{

    [SerializeField] Text ipText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(IPManager.GetIP(ADDRESSFAM.IPv4));
        ipText.text = IPManager.GetIP(ADDRESSFAM.IPv4);
    }
}
