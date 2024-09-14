using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class HelpMenuUi : BaseUI
    {
        private void Start()
        {
            base.OpeningTransition();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToMainMenu();
            }
        }

        public void VisitSite(string who)
        {
            switch (who)
            {
                case "bench":
                    Application.OpenURL("https://about.rubenbermejoromero.com/");
                    break;
                case "cross":
                    Application.OpenURL("https://crossfirecam.itch.io/");
                    break;
                case "rioni":
                    Application.OpenURL("https://github.com/Nrosa01");
                    break;
                case "darelt":
                    Application.OpenURL("https://darelt.itch.io/");
                    break;
                case "danirbu":
                    Application.OpenURL("https://www.youtube.com/@danirbumusic");
                    break;
                case "tesla":
                    Application.OpenURL("https://teslasp2.com/");
                    break;
                case "fireotters":
                    Application.OpenURL("https://fireotters.com");
                    break;
            }
        }

        public void BackToMainMenu()
        {
            base.ClosingTransition();
            Invoke(nameof(BackToMainMenu2), levelTransitionTime);
        }

        private void BackToMainMenu2()
        {
            SceneManager.LoadScene("Scenes/MainMenu");
        }
    }
}