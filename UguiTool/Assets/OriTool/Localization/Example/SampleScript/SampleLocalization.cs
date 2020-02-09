using OriTool.Localization;
using TMPro;
using UnityEngine;


namespace OriTool.Example.Localization
{
    public class SampleLocalization : MonoBehaviour, IEventChangeLanguage
    {
        [SerializeField] private TextMeshProUGUI textMeshProUgui;

        private void Awake()
        {
            LocalizeManager.Register(this);
            LocalizeManager.LanguagePack = LanguagePack.en;
        }

        public void OnClickLanguage(int index) => LocalizeManager.LanguagePack = (LanguagePack) index;

        public void OnChangeLanguageSetting() => textMeshProUgui.SetLocalizeText("HelloWorld");

        private void OnDestroy() => LocalizeManager.Remove(this);
    }
}