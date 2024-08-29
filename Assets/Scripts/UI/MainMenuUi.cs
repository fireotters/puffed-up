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
        private StudioEventEmitter _menuSong;
        [SerializeField] GameObject mainMenu, levelSelectMenu, settingsPanel, clickBlockerDuringButtonPops;
        private float uiBubblePopDuration = 0.2f;

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
            _menuSong = GetComponent<StudioEventEmitter>();
            base.ConfigureVersionText();
            SignalBus<SignalUiMainMenuStartGame>.Subscribe(StartGame).AddTo(_disposables);
        }

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
        public void WaitThenOpenSettings()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(OpenSettings), uiBubblePopDuration);
        }
        public void WaitThenOpenHelp()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(OpenHelp), uiBubblePopDuration);
        }
        public void WaitThenExit()
        {
            clickBlockerDuringButtonPops.SetActive(true);
            Invoke(nameof(QuitGame), uiBubblePopDuration);
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

        public void StartGame(SignalUiMainMenuStartGame signal)
        {
            _menuSong.Stop();
            SceneManager.LoadScene($"Scenes/LevelScenes/{signal.levelToLoad}");
        }
        public void OpenSettings()
        {
            clickBlockerDuringButtonPops.SetActive(false);
            settingsPanel.SetActive(true);
        }
        public void OpenHelp()
        {
            SceneManager.LoadScene("Scenes/HelpMenu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
