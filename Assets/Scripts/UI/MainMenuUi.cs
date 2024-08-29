using FMODUnity;
using Signals;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUi : BaseUI
    {

        [Header("Main Menu UI")]
        [SerializeField] private GameObject desktopButtons;
        [SerializeField] private GameObject webButtons;
        [SerializeField] private StudioEventEmitter _menuSong;
        [SerializeField] GameObject mainMenu, levelSelectMenu, settingsPanel, clickBlockerDuringButtonPops;
        private float uiBubblePopDuration = 0.1f;
        private string levelToLoad;

        private readonly CompositeDisposable _disposables = new();

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
            base.ConfigureVersionText();
            SignalBus<SignalUiMainMenuStartGame>.Subscribe(WaitThenStartGame).AddTo(_disposables);
        }

        // Open/Close Level Select
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

        // Start Game
        public void WaitThenStartGame(SignalUiMainMenuStartGame signal)
        {
            clickBlockerDuringButtonPops.SetActive(true);
            _menuSong.Stop();
            levelToLoad = signal.levelToLoad;
            Invoke(nameof(StartGame), uiBubblePopDuration);
        }
        public void StartGame()
        {
            SceneManager.LoadScene($"Scenes/LevelScenes/" + levelToLoad);
        }

        // Open Settings
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

        // Open Help
        public void WaitThenOpenHelp()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(OpenHelp), uiBubblePopDuration);
        }
        public void OpenHelp()
        {
            SceneManager.LoadScene("Scenes/HelpMenu");
        }

        // Exit Game
        public void WaitThenExit()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(QuitGame), uiBubblePopDuration);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
