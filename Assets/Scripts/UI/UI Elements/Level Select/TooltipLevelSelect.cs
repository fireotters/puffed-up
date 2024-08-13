using Signals;
using TMPro;
using UnityEngine;

namespace UI.UI_Elements.Level_Select
{
    public class TooltipLevelSelect : MonoBehaviour
    {
        private GameObject _tooltipBg;
        private TextMeshProUGUI _textLevelName, _textScore1, _textScore2;
        private RectTransform _rectTransform, _canvasRectTransform;
        private readonly Vector3 _tooltipOffset = new Vector3(10, -10, 0);
        private readonly CompositeDisposable _disposables = new();
        private const string Ellipsis = "...";
        private const string One = "1";
        private const string OnePoint = "1 pt";
        
        // Cursor changing
        public Texture2D normalCursor, tooltipCursor;
        private CursorMode _cursorMode = CursorMode.Auto;
        private Vector2 _cursorHotspot = Vector2.zero;

        // --------------------------------------------------------------------------------------------------------------
        // Start & End
        // --------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
            _tooltipBg = transform.Find("TooltipBg").gameObject;
            _textLevelName = _tooltipBg.transform.Find("TextLevelName").GetComponent<TextMeshProUGUI>();
            _textScore1 = _tooltipBg.transform.Find("Text1").GetComponent<TextMeshProUGUI>();
            _textScore2 = _tooltipBg.transform.Find("Text2").GetComponent<TextMeshProUGUI>();
            SignalBus<SignalUiMainMenuTooltipChange>.Subscribe(ChangeState).AddTo(_disposables);
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
            var desiredPosition = Input.mousePosition + _tooltipOffset;
            var anchoredPosition = desiredPosition / _canvasRectTransform.localScale.x;
            if (anchoredPosition.x + _rectTransform.rect.width > _canvasRectTransform.rect.width)
                anchoredPosition.x = _canvasRectTransform.rect.width - _rectTransform.rect.width;
            if (anchoredPosition.y > _canvasRectTransform.rect.height)
                anchoredPosition.y = _canvasRectTransform.rect.height - _rectTransform.rect.height;
            _rectTransform.anchoredPosition = anchoredPosition;
        }

        // --------------------------------------------------------------------------------------------------------------
        // Signal
        // --------------------------------------------------------------------------------------------------------------
        private void ChangeState(SignalUiMainMenuTooltipChange signal)
        {
            _textLevelName.text = signal.LevelName;
            _textScore1.text = signal.ScoreType1 switch
            {
                Ellipsis => Ellipsis,
                One => OnePoint,
                _ => $"{signal.ScoreType1} pts",
            };
            _textScore2.text = signal.ScoreType2 switch
            {
                Ellipsis => Ellipsis,
                One => OnePoint,
                _ => $"{signal.ScoreType2} pts",
            };
            _tooltipBg.SetActive(signal.Showing);
            Cursor.SetCursor(signal.Showing ? tooltipCursor : normalCursor, _cursorHotspot, _cursorMode);
        }
    }
}