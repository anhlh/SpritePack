using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using Object = UnityEngine.Object;

namespace OriTool.SpritePackCollection
{
    [CustomEditor(typeof(SpritePack))]
    public class SpritePackEditor : Editor
    {
        private SpritePack _sprAtlasPack;
        private Object[] _sourceDest;

        private void OnEnable()
        {
            _sprAtlasPack = (SpritePack) target;
            _sourceDest = _sprAtlasPack.ObjectsOrFolderPath.ToArray();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!_sprAtlasPack.IsResourcesEqual(_sourceDest)) UpdateSprites();

            if (GUILayout.Button("View Images"))
                SpritePackSpriteSelector.Show(_sprAtlasPack, OnSelectSprite, false, string.Empty);

            if (GUILayout.Button("Force Update Images")) UpdateSprites();
        }

        private void UpdateSprites()
        {
            _sourceDest = _sprAtlasPack.ObjectsOrFolderPath.ToArray();
            _sprAtlasPack.SetSprites(GetSprites(_sprAtlasPack.ObjectsOrFolderPath).Distinct());

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }


        private IEnumerable<Sprite> GetSprites(Object[] paths)
        {
            foreach (var o in paths)
            {
                //Folder
                if (o is DefaultAsset)
                {
                    var assetPath = AssetDatabase.GetAssetPath(o);
                    assetPath = assetPath.Replace("Assets/", string.Empty);
                    var spritePaths = Directory
                        .EnumerateFiles($"{Application.dataPath}/{assetPath}", "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));

                    foreach (var path in spritePaths)
                    {
                        var index = path.IndexOf("/Assets", StringComparison.Ordinal) + 1;
                        var spritePath = path.Substring(index);
                        var items = AssetDatabase.LoadAllAssetsAtPath(spritePath);
                        foreach (var item in items)
                        {
                            if (!(item is Sprite)) continue;
                            var spr = item as Sprite;
                            yield return spr;
                        }
                    }
                }
                else if (o is Texture2D)
                {
                    var path = AssetDatabase.GetAssetPath(o);
                    var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
                    foreach (var sprite in sprites)
                    {
                        yield return sprite;
                    }
                }
                else
                {
                    var spr = o as Sprite;
                    if (spr != null) yield return spr;
                }
            }
        }

        private void OnSelectSprite(string sprite)
        {
            //todo:
        }
    }
}