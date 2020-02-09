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