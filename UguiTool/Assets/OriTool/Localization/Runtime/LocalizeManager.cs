using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OriTool.Localization
{
    public class LocalizeManager
    {
        //Key was saved in PlayerPref
        private const string KeySettingLanguage = "KeySettingLanguage";
        private static readonly object LockInstance = new object();
        private static LocalizeManager _instance;
        
        private readonly Dictionary<LanguagePack, LanguageData> _dict = new Dictionary<LanguagePack, LanguageData>();
        private LanguagePack _currentLanguagePack;
        private LanguageData _currentLanguageData;

        private static LocalizeManager Instance
        {
            get
            {
                lock (LockInstance)
                {
                    if (_instance != null) return _instance;
                    var savedLang = PlayerPrefs.GetInt(KeySettingLanguage, -1);
                    if (savedLang == -1) savedLang = Application.systemLanguage == SystemLanguage.Japanese ? 0 : 1;
                    var language = (LanguagePack) savedLang;
                    _instance = new LocalizeManager(language);
                    return _instance;
                }
            }
        }
        
        private LocalizeManager(LanguagePack pack)
        {
            _currentLanguagePack = pack;
            UpdateLanguageSetting();
        }

        public static LanguagePack LanguagePack
        {
            get => Instance._currentLanguagePack;
            set
            {
                if (Instance._currentLanguagePack == value) return;
                Instance._currentLanguagePack = value;
                Instance.UpdateLanguageSetting();
            }
        }

        #region Observer

        private readonly List<IEventChangeLanguage> _lstObserver = new List<IEventChangeLanguage>();
        private static readonly object LstObserverLock = new object();

        /// <summary>
        /// Receiver add
        /// </summary>
        /// <param name="observer"></param>
        public static void Register(IEventChangeLanguage observer)
        {
            lock (LstObserverLock)
            {
                Instance._lstObserver.Add(observer);
            }
        }

        /// <summary>
        /// Receiver remove
        /// </summary>
        /// <param name="observer"></param>
        public static void Remove(IEventChangeLanguage observer)
        {
            lock (LstObserverLock)
            {
                if (Instance._lstObserver.Contains(observer)) Instance._lstObserver.Remove(observer);
            }
        }

        #endregion

        private void UpdateLanguageSetting()
        {
            if (_dict.ContainsKey(_currentLanguagePack))
            {
                _currentLanguageData = _dict[_currentLanguagePack];
            }
            else
            {
                _currentLanguageData = new LanguageData(_currentLanguagePack);
                _dict[_currentLanguagePack] = _currentLanguageData;
            }

            NotifyLanguageSetting();
        }

        private void NotifyLanguageSetting()
        {
            lock (LstObserverLock)
            {
                foreach (var target in _lstObserver) target.OnChangeLanguageSetting();
            }
        }

        public static void SaveSetting() => PlayerPrefs.SetInt(KeySettingLanguage, (int) Instance._currentLanguagePack);

        public static string GetText(string key) =>
            Instance._currentLanguageData == null ? string.Empty : Instance._currentLanguageData.GetText(key);
    }

    public interface IEventChangeLanguage
    {
        void OnChangeLanguageSetting();
    }

    public static class LocalizeManagerExtension
    {
        /// <summary>
        /// Set text by Key
        /// </summary>
        /// <param name="uiText"></param>
        /// <param name="key"></param>
        public static void SetLocalizeText(this Text uiText, string key) => uiText.text = LocalizeManager.GetText(key);

        /// <summary>
        /// Set TextMeshProUGUI by Key
        /// </summary>
        /// <param name="tmpText"></param>
        /// <param name="key"></param>
        public static void SetLocalizeText(this TextMeshProUGUI tmpText, string key) =>
            tmpText.SetText(LocalizeManager.GetText(key));

        /// <summary>
        /// Get text by Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizeText(this string key) => LocalizeManager.GetText(key);
    }

    public class LanguageData
    {
        private readonly Dictionary<string, string> _languageDict = new Dictionary<string, string>();
        public string GetText(string key) => _languageDict.TryGetValue(key, out var val) ? val : string.Empty;

        public LanguageData(LanguagePack language)
        {
            var textFile = Resources.Load<TextAsset>($"LanguagePack/{language.ToString()}");
            var reader = new StringReader(textFile.text);

            while (reader.Peek() > -1)
            {
                var line = reader.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                var kv = line.Split('=');
                var val = System.Text.RegularExpressions.Regex.Unescape(kv[1]);
                _languageDict.Add(kv[0], val);
            }
        }
    }

    public enum LanguagePack
    {
        jp = 0, //localization file with same name
        en
    }
}