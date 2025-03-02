# ActionGame
強制横スクロールランゲーム。特に難易度管理、非同期シーン遷移、オブジェクトプールの実装において工夫をしました。

---

## 工夫点

### 1. 難易度管理システム (DifficultyManager)

- **動的な難易度調整:** ゲーム進行に応じて、敵のスポーン率、移動速度を動的に変更します。

```csharp
private void UpdateDifficulty()
{
    currentDifficulty = Mathf.Min(maxDifficulty, baseDifficulty + Time.timeSinceLevelLoad * difficultyIncreaseRate);
    enemySpawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, currentDifficulty / maxDifficulty);
}
```

### 2. 非同期シーン遷移 (SceneTransitionManager)

- **スムーズなシーン遷移:** `UniTask` を活用し、ローディング画面を表示しつつ非同期でシーンを読み込みます。
- **ユーザー体験向上:** 読み込み中もUIを操作可能にし、途切れない体験を実現。

```csharp
public async UniTask LoadSceneAsync(string sceneName)
{
    loadingScreen.SetActive(true);
    await SceneManager.LoadSceneAsync(sceneName);
    loadingScreen.SetActive(false);
}
```

### 3. オブジェクトプール (GameObjectPool)

- **メモリ効率の最適化:** 頻繁に生成・破棄されるゲームオブジェクトをプールすることで、メモリ負荷を軽減。
- **パフォーマンス向上:** ガベージコレクションの影響を抑え、特にモバイル環境でのフレームレートを安定させます。

```csharp
public GameObject GetObject()
{
    if (pool.Count > 0)
    {
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }
    return Instantiate(prefab);
}
```

### 4. シングルトンパターンの活用 (GameManager)

- **ゲーム状態の一元管理:** `GameManager` をシングルトンとして実装し、シーンを跨いでもデータを保持。
- **メモリリーク防止:** `DontDestroyOnLoad` を活用し、ゲーム全体の安定した状態管理を実現。

```csharp
private static GameManager instance;
public static GameManager Instance => instance ??= FindObjectOfType<GameManager>();

private void Awake()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}
```

### 5. 依存性注入 (DI) を使った柔軟な設計 (Zenject)

- **保守性・テスト性の向上:** `Zenject` を使用して、依存関係を外部から注入することで、各クラスが単一の責任を持つように設計。
- **柔軟なコンポーネント管理:** モジュール化された設計により、新しい機能追加や変更が容易です。

```csharp
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGameService>().To<GameService>().AsSingle();
        Container.Bind<SceneTransitionManager>().FromComponentInHierarchy().AsSingle();
    }
}
```

---

本プロジェクトでは、Unity開発における設計パターンや最適化手法を積極的に取り入れ、
ゲームのパフォーマンスやユーザー体験の向上を実現しました。

