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
    [SerializeField] Image inflateIndicator;
    [SerializeField] Sprite iconHealthFull, iconHealthEmpty;
    [SerializeField] GameStateSo gameStateSo;
    private float timeOfLastInflate, howMuchWaitLeft, waitTimeToInflate;
    private bool bodgePreventFirstResetInflate = true;

    private void Start()
    {
        gameStateSo.Shells.OnValueChanged += UpdateShellsText;
        gameStateSo.Pearls.OnValueChanged += UpdatePearlsText;
        gameStateSo.Health.OnValueChanged += UpdateHealthDisplay;
        gameStateSo.LastPlayerPuffTime.OnValueChanged += ResetInflateTimer;

        UpdateShellsText(gameStateSo.Shells.Value);
        UpdatePearlsText(gameStateSo.Pearls.Value);
        UpdateHealthDisplay(gameStateSo.Health.Value);
    }

    private void Update()
    {
        // Spend a third of the wait time DRAINING the bar, spend rest REFILLING the bar
        howMuchWaitLeft -= Time.deltaTime;
        if (Time.time < timeOfLastInflate + (waitTimeToInflate/3))
        {
            float drainValue = (howMuchWaitLeft / waitTimeToInflate * 3) - 2;
            //print("Imma draining: " + drainValue);
            UpdateInflateDisplay(drainValue);
        }
        else if (Time.time < timeOfLastInflate + waitTimeToInflate)
        {
            float fillValue = 1 - (howMuchWaitLeft / waitTimeToInflate * 1.5f);
            //print("Imma fillin: " + fillValue);
            UpdateInflateDisplay(fillValue);
        }
        else
        {
            UpdateInflateDisplay(1f);
        }
    }

    private void OnDisable()
    {
        gameStateSo.Shells.OnValueChanged -= UpdateShellsText;
        gameStateSo.Pearls.OnValueChanged -= UpdatePearlsText;
        gameStateSo.Health.OnValueChanged -= UpdateHealthDisplay;
        gameStateSo.LastPlayerPuffTime.OnValueChanged -= ResetInflateTimer;
    }

    private string SpriteAssetString(int input)
    {
        string output = string.Empty;
        foreach (char c in input.ToString())
        {
            output += "<sprite=" + c + ">";
        }
        return output;
    }

    void UpdateShellsText(int shells)
    {
        txtShells.text = SpriteAssetString(shells);
    }
    void UpdatePearlsText(int pearls)
    {
        txtPearls.text = SpriteAssetString(pearls);
    }
    void UpdateHealthDisplay(int currentHealth)
    {
        if (currentHealth > 3)
            currentHealth = 3;
        for (int i = 0; i < healthIndicators.Length; i++)
        {
            if (i < currentHealth)
                healthIndicators[i].sprite = iconHealthFull;
            else
                healthIndicators[i].sprite = iconHealthEmpty;
        }
    }
    void UpdateInflateDisplay(float waitTime)
    {
        inflateIndicator.fillAmount = waitTime;
    }
    void ResetInflateTimer(float lastPuffTime)
    {
        if (bodgePreventFirstResetInflate)
        {
            bodgePreventFirstResetInflate = false;
            return;
        }
        timeOfLastInflate = lastPuffTime;
        waitTimeToInflate = gameStateSo.PlayerPuffWaitTime;
        howMuchWaitLeft = gameStateSo.PlayerPuffWaitTime;
    }
}
