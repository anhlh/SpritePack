# Localization
Change language at runtime
Support TextMeshPro, Unity Text

```
LocalizeManager.LanguagePack = LanguagePack.en;
textMeshProUgui.SetLocalizeText("HelloWorld");
```

# PoolManager
GameObject Pool system
Pool any GameObject

```
//Init Pool
GOPoolManager.Init(bulletPrefab, 5);

//Get
var bullet = GOPoolManager.Get<Bullet>(bulletEmitParent);

//Put to cache
GOPoolManager.Put(bullet);
```

# SpritePack
A simple way to manage Sprites.

Create SpritePack in editor. Drag and drop sprite or folder to SpritePack
SpritePack can be loaded in Resources or AssetBundle, or set in Inspector
Retrive sprite at runtime by spriteName or Sprite

```
[SerializeField] private Image charaImage;
[SerializeField] private SpritePack charaPack;//Link in Inspector

var rand = Random.Range(0, charaPack.Sprites.Length);
charaImage.sprite = charaPack.Sprites[rand];
```
or
```
charaImage.sprite = charaPack.GetSprite("SPRITE_NAME");
```