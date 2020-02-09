using UnityEngine;
using UnityEditor;

namespace OriTool.SpritePackCollection
{
    [CustomEditor(typeof(SPSpriteRender))]
    public class SPSpriteRenderEditor : Editor
    {
        private SPSpriteRender _image;
        private void OnEnable() => _image = (SPSpriteRender) target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Select Image"))
                SpritePackSpriteSelector.Show(_image.SpritePack, OnSelectSprite, true, _image.SpriteName);
        }

        void OnSelectSprite(string sprite)
        {
            if (_image == null) return;
            _image.SpriteName = sprite;
            EditorUtility.SetDirty(_image);
        }
    }
}