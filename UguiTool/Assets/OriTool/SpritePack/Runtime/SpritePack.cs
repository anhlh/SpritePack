using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OriTool.SpritePackCollection
{
    public class SpritePack : ScriptableObject
    {
#if UNITY_EDITOR
        public Object[] ObjectsOrFolderPath;
#endif
        [SerializeField, HideInInspector] private Sprite[] _sprites;

        public Sprite GetSprite(string spriteName) =>
            _sprites.FirstOrDefault(sprite => sprite != null && sprite.name == spriteName);

        public Sprite[] Sprites => _sprites;

#if UNITY_EDITOR

        public void SetSprites(IEnumerable<Sprite> sprites)
        {
            _sprites = sprites.ToArray();
        }

        public bool IsResourcesEqual(Object[] other)
        {
            if (other == null) return ObjectsOrFolderPath == null;
            if (other.Length != ObjectsOrFolderPath.Length) return false;
            return !ObjectsOrFolderPath.Where((t, i) => t.GetInstanceID() != other[i].GetInstanceID()).Any();
        }
#endif
    }
}