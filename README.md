# Auto Create Asset

指定したパスに自動でアセットを作成する属性を追加します

## 使い方

```cs
using Noranokyoju.AutoCreateAsset;
using UnityEngine;

// "Assets/TestScriptable.asset" に自動でアセットが作成されます
[AutoCreateAsset("Assets/TestScriptable.asset")]
public class TestScriptable : ScriptableObject
{
    public int a;
}
```

## アセンブリの設定

Assembly-CSharpアセンブリ内のクラスは自動で作成されます。
それ以外のアセンブリで使いたい場合は、"Edit/Preferences"の"Noranokyoju/AutoCreateAsset"からアセンブリを設定してください。