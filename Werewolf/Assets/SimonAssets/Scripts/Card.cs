using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public ushort id;
    public string role;
    public string playerName;
    public bool playerActive = false;

    public void UpdateImage() {
        Image image = this.gameObject.GetComponent<Image>();


        CardImageHandler cardImageHandler = CardImageHandler.instance;
        Sprite newSprite = cardImageHandler.GetSprite(role);

        image.sprite = newSprite;
    }
}
