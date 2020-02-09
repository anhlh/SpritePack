using UnityEngine;

namespace OriTool.SpritePackCollection
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SPSpriteRender : MonoBehaviour
    {
        [SerializeField] SpritePack spritePack;
        public SpritePack SpritePack => spritePack;

        private SpriteRenderer _sprite;
        public SpriteRenderer Sprite => _sprite != null ? _sprite : _sprite = GetComponent<SpriteRenderer>();

        public string SpriteName
        {
            get => Sprite.sprite != null ? Sprite.sprite.name : string.Empty;
            set => Sprite.sprite = spritePack.GetSprite(value);
        }
    }
}