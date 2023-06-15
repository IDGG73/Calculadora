using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampisConsoleMessage : MonoBehaviour
{
    [SerializeField] Image iconDisplay;
    [Space]
    public TextMeshProUGUI messageDisplay;
    public TextMeshProUGUI messageCountDisplay;

    public int Count { get; private set; }

    public void SetMessage(string message, int count, Sprite icon, Color iconColor)
    {
        Count = count;

        iconDisplay.sprite = icon;
        iconDisplay.color = iconColor;

        messageDisplay.text = message;
        messageCountDisplay.text = Count.ToString("N0");

        //messageCountDisplay.gameObject.SetActive(messageCountDisplay.text != "1");
    }
}
