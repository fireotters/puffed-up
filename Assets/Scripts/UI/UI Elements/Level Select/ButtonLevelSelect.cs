using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UI_Elements.Level_Select
{
    public class ButtonLevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string levelNum, levelName;
        [HideInInspector] public string attachedLevel;
        private Button _button;
        private string _highscore1 = "-", _highscore2 = "-";
        private TextMeshProUGUI _textLevelNum, _textScore1, _textScore2;

        private void Awake()
        {
            attachedLevel = levelNum.Length == 1 ? "Level0" + levelNum : "Level" + levelNum;
            _button = GetComponentInChildren<Button>();
            _button.onClick.AddListener(LoadLevel);
            _textLevelNum = transform.Find("TxtLvlNum").GetComponent<TextMeshProUGUI>();
            _textLevelNum.text = levelNum;
            _textScore1 = transform.Find("ScoreRecords").Find("TxtScore1").GetComponent<TextMeshProUGUI>();
            _textScore2 = transform.Find("ScoreRecords").Find("TxtScore2").GetComponent<TextMeshProUGUI>();
        }

        private void LoadLevel()
        {
            SignalBus<SignalUiMainMenuStartGame>.Fire(new SignalUiMainMenuStartGame { levelToLoad = attachedLevel });
        }

        public void UpdateLevelHighscores(int bingoScore, int pieceScore)
        {
            _highscore1 = bingoScore == -1 ? "-" : bingoScore.ToString();
            _highscore2 = pieceScore == -1 ? "-" : pieceScore.ToString();
            _textScore1.text = _highscore1;
            _textScore2.text = _highscore2;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Show the tooltip if the button is interactible
            if (_button.interactable)
            {
                var tempLevelName = levelNum + ". " + levelName;
                var tempHighscore1 = _highscore1 != "-" ? _highscore1 : "...";
                var tempHighscore2 = _highscore2 != "-" ? _highscore2 : "...";
                SignalBus<SignalUiMainMenuTooltipChange>.Fire(new SignalUiMainMenuTooltipChange
                {
                    Showing = true,
                    LevelName = tempLevelName,
                    ScoreType1 = tempHighscore1,
                    ScoreType2 = tempHighscore2
                });
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SignalBus<SignalUiMainMenuTooltipChange>.Fire(new SignalUiMainMenuTooltipChange
                { Showing = false, LevelName = "", ScoreType1 = "", ScoreType2 = "" });
        }
    }
}