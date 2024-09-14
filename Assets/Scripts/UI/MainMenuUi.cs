using FMODUnity;
using Signals;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUi : BaseUI
    {
        [Header("Version Text")]
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private bool showVersionText = false;

        [Header("Main Menu UI")]
        [SerializeField] private GameObject desktopButtons;
        [SerializeField] private GameObject webButtons;
        [SerializeField] private StudioEventEmitter _menuSong;
        [SerializeField] GameObject mainMenu, levelSelectMenu, settingsPanel;
        [SerializeField] GameObject clickBlockerDuringButtonPops; // Level transitions block clicks - this is used for the brief 'button pops' that happen within MainMenu.
        private float uiBubblePopDuration = 0.15f;
        private string levelToLoad;

        private readonly CompositeDisposable _disposables = new();

        // --------------------------------------------------------------------------------------------------------------
        // Start & End
        // --------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            // WebGL & Debug-Only Stuff
            #if UNITY_WEBGL
                        desktopButtons.SetActive(false);
                        webButtons.SetActive(true);
#else
                        desktopButtons.SetActive(true);
                        webButtons.SetActive(false);
#endif

            // Main Menu start tasks
            SetVersionText();
            SignalBus<SignalUiMainMenuStartGame>.Subscribe(WaitThenStartGame).AddTo(_disposables);
            base.OpeningTransition();
        }

        private void SetVersionText()
        {
            if (versionText != null)
            {
                versionText.gameObject.SetActive(Debug.isDebugBuild || showVersionText);
                if (Debug.isDebugBuild)
                    versionText.text = Application.isEditor ? $"Version debug-{Application.version}-editor" : $"Version debug-{Application.version}-{Application.buildGUID}";
                else
                    versionText.text = $"Version {Application.version}";
            }
            else
                Debug.LogWarning("No version text set! Please set one.");
        }
        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        // --------------------------------------------------------------------------------------------------------------
        // Open/Close Level Select
        // --------------------------------------------------------------------------------------------------------------
        public void WaitThenOpenLevelSelect()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(OpenLevelSelect), uiBubblePopDuration);
        }
        public void WaitThenCloseLevelSelect()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(OpenLevelSelect), uiBubblePopDuration);
        }
        public void OpenLevelSelect()
        {
            clickBlockerDuringButtonPops.SetActive(false);
            mainMenu.SetActive(false);
            levelSelectMenu.SetActive(true);
        }
        public void CloseLevelSelect()
        {
            clickBlockerDuringButtonPops.SetActive(false);
            mainMenu.SetActive(false);
            levelSelectMenu.SetActive(true);
        }

        // --------------------------------------------------------------------------------------------------------------
        // Start Game
        // --------------------------------------------------------------------------------------------------------------
        public void WaitThenStartGame(SignalUiMainMenuStartGame signal)
        {
            base.ClosingTransition();
            _menuSong.Stop();
            levelToLoad = signal.levelToLoad;
            Invoke(nameof(StartGame), levelTransitionTime);
        }
        public void StartGame()
        {
            SceneManager.LoadScene($"Scenes/LevelScenes/" + levelToLoad);
        }

        // --------------------------------------------------------------------------------------------------------------
        // Settings Panel
        // --------------------------------------------------------------------------------------------------------------
        public void WaitThenOpenSettings()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(OpenSettings), uiBubblePopDuration);
        }
        public void OpenSettings()
        {
            clickBlockerDuringButtonPops.SetActive(false);
            settingsPanel.SetActive(true);
        }

        // --------------------------------------------------------------------------------------------------------------
        // Help Menu & Exit Game
        // --------------------------------------------------------------------------------------------------------------
        public void WaitThenOpenHelp()
        {
            base.ClosingTransition();
            Invoke(nameof(OpenHelp), levelTransitionTime);
        }
        public void OpenHelp()
        {
            SceneManager.LoadScene("Scenes/HelpMenu");
        }
        public void WaitThenExit()
        {
            base.ClosingTransition();
            Invoke(nameof(QuitGame), uiBubblePopDuration);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
