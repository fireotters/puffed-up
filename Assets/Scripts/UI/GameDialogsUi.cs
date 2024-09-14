using Audio;
using FMODUnity;
using Signals;
using System.Collections;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameDialogsUi : MonoBehaviour
    {
        public string nextSceneToLoad;
        public StudioEventEmitter _sndMusicStage;

        [SerializeField] private GameUiDialogs _dialogs;
        [SerializeField] private GameUiSound _sound;

        private readonly CompositeDisposable _disposables = new();

        // --------------------------------------------------------------------------------------------------------------
        // Start & End
        // --------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            if (nextSceneToLoad == "")
                Debug.LogWarning("No 'CanvasGameUi.nextSceneToLoad' set! Selecting 'Next Level' will fail.");

            SignalBus<SignalGameEnded>.Subscribe(HandleEndGame).AddTo(_disposables);
            PlayLevelTransition(LevelTransitionIntent.OpenScene);
        }
        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        // --------------------------------------------------------------------------------------------------------------
        // Per-Frame Updates
        // --------------------------------------------------------------------------------------------------------------
        private void Update()
        {
            CheckKeyInputs();
        }

        private void CheckKeyInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                // Pause if pause panel isn't open, resume if it is open
                if (!_dialogs.options.activeInHierarchy)
                {
                    if (!IsPauseInterruptingPanelOpen())
                    {
                        GameIsPaused(!_dialogs.paused.activeInHierarchy);
                    }
                }
                else
                {
                    _dialogs.options.SetActive(!_dialogs.options.activeInHierarchy);
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------
        // Game Event Functions
        // --------------------------------------------------------------------------------------------------------------

        private void HandleEndGame(SignalGameEnded context)
        {
            _sndMusicStage.Stop();
            if (context.result == GameEndCondition.Loss)
            {
                _dialogs.gameLost.SetActive(true);
                return;
            }

            // Win Conditions trigger some similar behaviour
            _sound.musicStage.SetParameter("Win", 1);
            _dialogs.gameWon.SetActive(true);

            string levelName = SceneManager.GetActiveScene().name;
            GameEndCondition scoreType = context.result;
            int score = context.score;
            (bool wasThisNewHighscore, int highScore) = HighScoreManagement.TryAddScoreThenReturnHighscore(levelName, scoreType, score);
            _dialogs.SetupVictoryDialog(scoreType, score, highScore, wasThisNewHighscore);
        }

        // --------------------------------------------------------------------------------------------------------------
        // Pause Functions
        // --------------------------------------------------------------------------------------------------------------
        private void GameIsPaused(bool intent)
        {
            // Show or hide pause panel and set timescale
            _sound.musicStage.SetParameter("Menu", intent ? 1 : 0);
            _dialogs.paused.SetActive(intent);
            Time.timeScale = intent ? 0 : 1;
            _sound.fmodMixer.FindAllSfxAndPlayPause(isGamePaused: intent);
        }

        public void ResumeGame()
        {
            GameIsPaused(false);
        }
        public bool IsPauseInterruptingPanelOpen()
        {
            return _dialogs.gameLost.activeInHierarchy || _dialogs.gameWon.activeInHierarchy || _dialogs.levelTransitionOverlay.gameObject.activeInHierarchy;
        }

        public void ToggleOptionsPanel()
        {
            _dialogs.options.SetActive(!_dialogs.options.activeInHierarchy);
        }

        // --------------------------------------------------------------------------------------------------------------
        // Level Transitions
        // --------------------------------------------------------------------------------------------------------------
        public enum LevelTransitionIntent { OpenScene, ResetScene, NextScene, ExitToMainMenu }
        public void PlayLevelTransition(LevelTransitionIntent intent)
        {
            _dialogs.levelTransitionOverlay.gameObject.SetActive(true);
            // Level Start
            if (intent == LevelTransitionIntent.OpenScene)
            {
                _dialogs.levelTransitionOverlay.SetTrigger("transitionEndToStart");
                Invoke(nameof(OpenScene2), _dialogs.levelTransitionTime);
                return;
            }

            // Level End
            _sound.fmodMixer.KillEverySound();
            _dialogs.levelTransitionOverlay.SetTrigger("transitionStartToEnd");
            switch (intent)
            {
                case LevelTransitionIntent.ResetScene:
                    StartCoroutine(ResetCurrentLevel2()); break;
                case LevelTransitionIntent.ExitToMainMenu:
                    StartCoroutine(ExitGame2()); break;
                case LevelTransitionIntent.NextScene:
                    StartCoroutine(LoadNextScene2()); break;
            }
        }
        private void OpenScene2()
        {
            _dialogs.levelTransitionOverlay.gameObject.SetActive(false);
        }
        public void LoadNextScene()
        {
            PlayLevelTransition(LevelTransitionIntent.NextScene);
        }
        private IEnumerator LoadNextScene2()
        {
            yield return new WaitForSecondsRealtime(_dialogs.levelTransitionTime);
            Time.timeScale = 1;
            SceneManager.LoadScene(nextSceneToLoad); }
        public void ResetCurrentLevel()
        {
            PlayLevelTransition(LevelTransitionIntent.ResetScene);
        }
        private IEnumerator ResetCurrentLevel2()
        {
            yield return new WaitForSecondsRealtime(_dialogs.levelTransitionTime);
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void ExitGame()
        {
            PlayLevelTransition(LevelTransitionIntent.ExitToMainMenu);
        }
        private IEnumerator ExitGame2()
        {
            yield return new WaitForSecondsRealtime(_dialogs.levelTransitionTime);
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu"); }

    }

    // --------------------------------------------------------------------------------------------------------------
    // Classes for common GameUi Objects
    // --------------------------------------------------------------------------------------------------------------

    [System.Serializable]
    public class GameUiDialogs
    {
        // Support for victory screen, high scores, remembering the tutorials shown to the player
        public int tutorialIndex;
        public GameObject paused, options;
        public GameObject gameLost, gameWon;
        public Color clrVictoryScore1, clrVictoryScore1Best, clrVictoryScore2, clrVictoryScore2Best;
        public TextMeshProUGUI txtVictoryCurrent, txtVictoryBest;

        public Animator levelTransitionOverlay;
        public float levelTransitionTime = 1.0f;

        public void SetupVictoryDialog(GameEndCondition victoryType, int currentScore, int bestScore, bool wasThisNewHighscore)
        {
            if (victoryType == GameEndCondition.Win)
            {
                txtVictoryCurrent.color = clrVictoryScore1;
                txtVictoryBest.color = clrVictoryScore1Best;
            }

            txtVictoryCurrent.text = currentScore.ToString() + (currentScore > 1 ? " pts" : " pt");
            if (bestScore == -1)
            {
                txtVictoryBest.text = "";
                txtVictoryCurrent.verticalAlignment = VerticalAlignmentOptions.Middle;
            }
            else if (wasThisNewHighscore)
                txtVictoryBest.text = "New best score!";
            else
                txtVictoryBest.text = "Best: " + bestScore.ToString() + (bestScore > 1 ? " pts" : " pt");


            // Set tutorial as completed, so it won't appear next time
            if (tutorialIndex != 0)
            {
                if (PlayerPrefs.GetInt("tutorialUpTo", 0) < tutorialIndex)
                {
                    PlayerPrefs.SetInt("tutorialUpTo", tutorialIndex);
                    PlayerPrefs.Save();
                }
            }
        }
    }
    [System.Serializable]
    public class GameUiSound
    {
        public StudioEventEmitter musicStage;
        public FMODMixer fmodMixer;
    }
}