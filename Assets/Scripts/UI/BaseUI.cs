using TMPro;
using UI.UI_Elements;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BaseUI : MonoBehaviour
    {
        [Header("Level Transitions")]
        public Animator levelTransitionOverlay;
        internal float levelTransitionTime = 1.0f;

        internal void OpeningTransition()
        {
            levelTransitionOverlay.gameObject.SetActive(true);
            levelTransitionOverlay.SetTrigger("transitionEndToStart");
            Invoke(nameof(OpeningTransition2), levelTransitionTime);
        }
        private void OpeningTransition2()
        {
            levelTransitionOverlay.gameObject.SetActive(false);
        }
        internal void ClosingTransition()
        {
            levelTransitionOverlay.gameObject.SetActive(true);
            levelTransitionOverlay.SetTrigger("transitionStartToEnd");
            // Scene is supposed to end here. Other duties should be handled by the derived scripts.
        }
    }
}
