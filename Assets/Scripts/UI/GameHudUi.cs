using GameLogic;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHudUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtShells, txtPearls;
    [SerializeField] Image[] healthIndicators;
    [SerializeField] Sprite iconHealthFull, iconHealthEmpty;
    [SerializeField] GameStateSo gameStateSo;

    private void Start()
    {
        gameStateSo.Shells.OnValueChanged += UpdateShellsText;
        gameStateSo.Pearls.OnValueChanged += UpdatePearlsText;
        gameStateSo.Health.OnValueChanged += UpdateHealthDisplay;

        UpdateShellsText(gameStateSo.Shells.Value);
        UpdatePearlsText(gameStateSo.Pearls.Value);
        UpdateHealthDisplay(gameStateSo.Health.Value);
    }

    private void OnDisable()
    {
        gameStateSo.Shells.OnValueChanged -= UpdateShellsText;
        gameStateSo.Pearls.OnValueChanged -= UpdatePearlsText;
        gameStateSo.Health.OnValueChanged -= UpdateHealthDisplay;
    }

    void UpdateShellsText(int shells)
    {
        txtShells.text = shells.ToString();
    }
    void UpdatePearlsText(int pearls)
    {
        txtPearls.text = pearls.ToString();
    }
    void UpdateHealthDisplay(int currentHealth)
    {
        for (int i = 0; i < healthIndicators.Length; i++)
        {
            if (i < currentHealth)
                healthIndicators[i].sprite = iconHealthFull;
            else
                healthIndicators[i].sprite = iconHealthEmpty;
        }
    }
}
