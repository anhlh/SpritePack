using OriTool.SpritePackCollection;
using UnityEngine;
using UnityEngine.UI;

namespace OriTool.Example
{
    public class ChangeChara : MonoBehaviour
    {
        [SerializeField] private Image charaImage;
        [SerializeField] private SpritePack charaPack;

        public void OnClickRandomCharacter()
        {
            var len = charaPack.Sprites.Length;
            var rand = Random.Range(0, len);
            charaImage.sprite = charaPack.Sprites[rand];
        }
    }
}