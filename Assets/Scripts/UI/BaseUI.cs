using TMPro;
using UI.UI_Elements;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BaseUI : MonoBehaviour
    {
        [Header("Base UI")]
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private bool showVersionText = false;


        protected void ConfigureVersionText()
        {
            versionText.gameObject.SetActive(Debug.isDebugBuild || showVersionText);
            SetVersionText();
        }

        private void SetVersionText()
        {
            if (versionText != null)
            {
                if (Debug.isDebugBuild)
                {
                    versionText.text = Application.isEditor
                        ? $"Version debug-{Application.version}-editor"
                        : $"Version debug-{Application.version}-{Application.buildGUID}";
                }
                else
                {
                    versionText.text = $"Version {Application.version}";
                }
            }
            else
            {
                Debug.LogWarning("No version text set!!!! please set one");
            }
        }
    }
}
