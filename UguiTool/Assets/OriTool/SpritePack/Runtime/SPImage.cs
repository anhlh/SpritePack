using UnityEngine;
using UnityEngine.UI;

namespace OriTool.SpritePackCollection
{
    [RequireComponent(typeof(Image))]
    public class SPImage : MonoBehaviour
    {
        [SerializeField] private SpritePack spritePack;
        public SpritePack SpritePack => spritePack;

        private Image _image;
        public Image Image => _image != null ? _image : _image = GetComponent<Image>();

        public string SpriteName
        {
            get => Image.sprite != null ? Image.sprite.name : string.Empty;
            set => Image.sprite = spritePack.GetSprite(value);
        }

        public void MakePixelPerfect() => Image.SetNativeSize();
    }
}