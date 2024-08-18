using GameLogic;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameStateSo gameStateSo;

    private void Start()
    {
        gameStateSo.Coins.OnValueChanged += UpdateText;
        UpdateText(gameStateSo.Coins.Value);
    }

    private void OnDisable()
    {
        gameStateSo.Coins.OnValueChanged -= UpdateText;
    }

    void UpdateText(int coins)
    {
        text.text = coins.ToString();
    }
}
