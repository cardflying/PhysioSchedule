using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PriceUI : MonoBehaviour
{
    [SerializeField]
    private Button clickButton;
    [SerializeField]
    private TMP_Text displayNameText;
    [SerializeField]
    private TMP_Text displayInfoText;

    public void Init(string info, int price, Action eventCallback)
    {
        displayNameText.text = info;
        displayInfoText.text = "RM " + price;
        clickButton.onClick.AddListener(() => eventCallback());

        gameObject.SetActive(true);
    }

    public void Disable()
    {
        clickButton.onClick.RemoveAllListeners();
        displayNameText.text = string.Empty;
        displayInfoText.text = string.Empty;
        gameObject.SetActive(false);
    }
}
