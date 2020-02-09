using UnityEngine;
using UnityEditor;

namespace OriTool.SpritePackCollection
{
    [CustomEditor(typeof(SPImage))]
    public class SPImageEditor : Editor
    {
        private SPImage _image;

        private void OnEnable() => _image = (SPImage) target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Select Images"))
                SpritePackSpriteSelector.Show(_image.SpritePack, OnSelectSprite, true, _image.SpriteName);

            if (GUILayout.Button("Make Pixel Perfect")) _image.MakePixelPerfect();
        }

        private void OnSelectSprite(string sprite)
        {
            if (_image == null) return;
            _image.SpriteName = sprite;
            EditorUtility.SetDirty(_image);
        }
    }
}