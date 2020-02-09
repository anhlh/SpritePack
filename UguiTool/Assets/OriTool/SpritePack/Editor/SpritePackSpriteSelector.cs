using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace OriTool.SpritePackCollection
{
    public class SpritePackSpriteSelector : ScriptableWizard
    {
        private static SpritePackSpriteSelector _instance;

        void OnEnable() => _instance = this;
        void OnDisable() => _instance = null;

        public delegate void Callback(string sprite);

        private Callback _callback;

        private string _selectedSprite;
        private Vector2 _pos = Vector2.zero;

        private SpritePack _spritePack;
        private List<Sprite> _sprites;

        private string _partialSprite = string.Empty;
        private bool _closeWhenSelected;

        void OnGUI()
        {
            if (_spritePack == null)
            {
                GUILayout.Label("Need SpritePack ref");
                return;
            }

            if (_spritePack.Sprites == null) return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(84f);

            var before = _partialSprite;
            var after = EditorGUILayout.TextField("", before, "SearchTextField");
            if (before != after)
            {
                _partialSprite = after;
                if (string.IsNullOrEmpty(_partialSprite))
                {
                    _sprites = new List<Sprite>(_spritePack.Sprites);
                }
                else
                {
                    _sprites = new List<Sprite>();
                    foreach (var spr in _spritePack.Sprites)
                    {
                        if (!spr.name.Contains(_partialSprite)) continue;
                        _sprites.Add(spr);
                    }
                }
            }

            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            {
                _partialSprite = string.Empty;
                GUIUtility.keyboardControl = 0;
            }

            if (string.IsNullOrEmpty(_partialSprite)) _sprites = _spritePack.Sprites.ToList();

            if (_sprites.Count == 0) return;

            GUILayout.Space(84f);
            GUILayout.EndHorizontal();

            var size = 80f;
            var padded = size + 10f;
            var screenWidth = (int) EditorGUIUtility.currentViewWidth;

            var columns = Mathf.FloorToInt(screenWidth / padded);
            if (columns < 1) columns = 1;

            var offset = 0;
            var rect = new Rect(10f, 0, size, size);

            GUILayout.Space(10f);
            _pos = GUILayout.BeginScrollView(_pos);
            var rows = 1;

            while (offset < _sprites.Count)
            {
                GUILayout.BeginHorizontal();
                {
                    var col = 0;
                    rect.x = 10f;

                    for (; offset < _sprites.Count; ++offset)
                    {
                        var sprite = _sprites[offset];
                        if (sprite == null) continue;

                        // Button comes first
                        if (GUI.Button(rect, ""))
                        {
                            if (Event.current.button == 0)
                            {
                                if (_selectedSprite != sprite.name)
                                {
                                    _selectedSprite = sprite.name;
                                    _callback?.Invoke(sprite.name);

                                    if (_closeWhenSelected)
                                    {
                                        Close();
                                    }
                                }
                            }
                        }

                        if (Event.current.type == EventType.Repaint)
                        {
                            // On top of the button we have a checkboard grid
                            DrawTiledTexture(rect, BackdropTexture);
                            var uv = sprite.rect;
                            var clipRect = rect;
                            if (System.Math.Abs(sprite.rect.width - sprite.rect.height) > 0.01f)
                            {
                                // Calculate the texture's scale that's needed to display the sprite in the clipped area
                                var scaleHeight = rect.height / uv.height;
                                var newWidth = scaleHeight * uv.width;
                                if (newWidth > rect.width)
                                {
                                    //need scale by width
                                    var scaleWidth = rect.width / uv.width;
                                    var newHeight = scaleWidth * uv.height;
                                    clipRect.height = newHeight;
                                    clipRect.width = rect.width;
                                    clipRect.y += (rect.height - newHeight) / 2;
                                }
                                else
                                {
                                    clipRect.height = rect.height;
                                    clipRect.width = newWidth;
                                    clipRect.x += (rect.width - newWidth) / 2;
                                }
                            }

                            GUI.DrawTexture(clipRect, sprite.texture);

                            // Draw the selection
                            if (_selectedSprite == sprite.name)
                            {
                                DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                            }
                        }

                        GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                        GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                        GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name,
                            "ProgressBarBack");
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = Color.white;

                        if (++col >= columns)
                        {
                            ++offset;
                            break;
                        }

                        rect.x += padded;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(padded);
                rect.y += padded + 26;
                ++rows;
            }

            GUILayout.Space(rows * 26);
            GUILayout.EndScrollView();
        }

        public static void Show(SpritePack spritePack, Callback callback, bool closeWhenSelected, string selectedSprite)
        {
            if (_instance != null)
            {
                _instance.Close();
                _instance = null;
            }

            var comp = ScriptableWizard.DisplayWizard<SpritePackSpriteSelector>("Select a Sprite");
            comp._spritePack = spritePack;
            comp._callback = callback;
            comp._selectedSprite = selectedSprite;
            comp._closeWhenSelected = closeWhenSelected;
            comp._partialSprite = string.Empty;
        }

        public static void DrawTiledTexture(Rect rect, Texture tex)
        {
            GUI.BeginGroup(rect);
            {
                var width = Mathf.RoundToInt(rect.width);
                var height = Mathf.RoundToInt(rect.height);

                for (var y = 0; y < height; y += tex.height)
                {
                    for (var x = 0; x < width; x += tex.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                    }
                }
            }
            GUI.EndGroup();
        }

        private static Texture2D _backdropTex;

        public static Texture2D BackdropTexture => _backdropTex ?? (_backdropTex = CreateCheckerTex(
                                                       new Color(0.1f, 0.1f, 0.1f, 0.5f),
                                                       new Color(0.2f, 0.2f, 0.2f, 0.5f)));

        private static Texture2D CreateCheckerTex(Color c0, Color c1)
        {
            var tex = new Texture2D(16, 16)
            {
                name = "[Generated] Checker Texture",
                hideFlags = HideFlags.DontSave
            };

            for (var y = 0; y < 8; ++y)
            for (var x = 0; x < 8; ++x)
                tex.SetPixel(x, y, c1);
            for (var y = 8; y < 16; ++y)
            for (var x = 0; x < 8; ++x)
                tex.SetPixel(x, y, c0);
            for (var y = 0; y < 8; ++y)
            for (var x = 8; x < 16; ++x)
                tex.SetPixel(x, y, c0);
            for (var y = 8; y < 16; ++y)
            for (var x = 8; x < 16; ++x)
                tex.SetPixel(x, y, c1);

            tex.Apply();
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        public static void DrawOutline(Rect rect, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;
            var tex = BlankTexture;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
            GUI.color = Color.white;
        }

        public static Texture2D BlankTexture => EditorGUIUtility.whiteTexture;
    }
}