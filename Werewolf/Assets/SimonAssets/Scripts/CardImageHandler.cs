using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardImageHandler : MonoBehaviour
{
    public static CardImageHandler instance = null;

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
   

    [System.Serializable]
    public struct NamedImage
    {
        public string name;
        public Sprite image;
    }

    [SerializeField] private List<NamedImage> cardImages;

    public Sprite GetSprite(string name)
    {
        foreach (NamedImage i in cardImages)
        {
            if (i.name == name)
                return i.image;
        }
        return null;
    }
}
