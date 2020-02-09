using TMPro;
using UnityEngine;

namespace OriTool.Localization
{
    [ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class UITextMeshProLocalize : MonoBehaviour, IEventChangeLanguage
    {
        [SerializeField] private string key;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                UpdateText();
            }
        }

        private TextMeshProUGUI _text;
        private TextMeshProUGUI Text => _text != null ? _text : _text = GetComponent<TextMeshProUGUI>();

#if UNITY_EDITOR
        [SerializeField] private bool init;

        private void OnValidate()
        {
            if (!init) return;
            if (!string.IsNullOrEmpty(key)) Key = key;
        }
#endif

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(key)) Key = key;
            init = true;
#endif
        }

        private void Start()
        {
            if (Application.isPlaying) LocalizeManager.Register(this);
            UpdateText();
        }

        private void UpdateText() => Text.text = LocalizeManager.GetText(Key);

        void IEventChangeLanguage.OnChangeLanguageSetting() => UpdateText();

        private void OnDestroy()
        {
            if (Application.isPlaying) LocalizeManager.Remove(this);
        }
    }
}