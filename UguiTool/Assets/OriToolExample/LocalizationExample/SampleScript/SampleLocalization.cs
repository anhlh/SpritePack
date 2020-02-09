using OriTool.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OriTool.Example.Localization
{
    public class SampleLocalization : MonoBehaviour, IEventChangeLanguage
    {
        [SerializeField] private TextMeshProUGUI textMeshProUgui;
        [SerializeField] private Button btnEn;
        [SerializeField] private Button btnJp;

        private void Awake()
        {
            LocalizeManager.Register(this);
            OnClickLanguage(0);
        }

        public void OnClickLanguage(int index)
        {
            LocalizeManager.LanguagePack = (LanguagePack) index;
            //Ui update
            btnJp.targetGraphic.color = index == 0 ? Color.green : Color.white;
            btnEn.targetGraphic.color = index == 1 ? Color.green : Color.white;
        }

        public void OnChangeLanguageSetting() => textMeshProUgui.SetLocalizeText("HelloWorld");

        private void OnDestroy() => LocalizeManager.Remove(this);
    }
}