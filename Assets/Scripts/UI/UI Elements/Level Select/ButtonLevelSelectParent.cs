using Saving;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UI_Elements.Level_Select
{
    public class ButtonLevelSelectParent : MonoBehaviour
    {
        private ButtonLevelSelect[] _levelButtons;

        private void Start()
        {
            _levelButtons = GetComponentsInChildren<ButtonLevelSelect>();
            //var numOfUnlocks = 1;

            //if (PlayerPrefs.HasKey("LevelScores"))
            //{
            //    var jsonString = PlayerPrefs.GetString("LevelScores");
            //    var levelScores = JsonUtility.FromJson<Highscores>(jsonString);
            //    foreach (var entry in levelScores.highscoreEntryList)
            //    {
            //        foreach (var button in _levelButtons)
            //        {
            //            if (entry.levelName == button.attachedLevel)
            //            {
            //                button.UpdateLevelHighscores(entry.scoreType1, entry.scoreType2);
            //                numOfUnlocks++;
            //                break;
            //            }
            //        }
            //    }
            //}

            //// Only enable the buttons for completed levels, and the next uncomplete level
            //for (var i = 0; i < numOfUnlocks; i++)
            //{
            //    _levelButtons[i].GetComponentInChildren<Button>().interactable = true;
            //}
        }
    }
}