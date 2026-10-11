# AudioManager

`AudioManager`は`TitleScene`に配置され、`DontDestroyOnLoad`でシーンを跨いで使用できます。

## 基本的な再生

```csharp
AudioManager audio = AudioManager.Instance;

// BGM
audio.PlayBGM("Title");
audio.PlayBGM("InGame");
audio.PlayBGM("Result");

// 通常のSE（呼び出すたびに再生）
audio.PlaySE("Fire");
audio.PlaySE("Walk");
```

利用できるキーはAudioManagerのInspectorに登録されている`bgmClips`と`seClips`で設定します。

## 条件中だけ鳴らすループSE

ループSEは、条件を満たしている間に`PlayLoopSE`を呼び続け、条件を外れたときに`StopLoopSE`を呼びます。同じキーの再生中に再度呼び出しても、再生位置はリセットされません。

```csharp
private void Update()
{
    if (isNearFire)
    {
        AudioManager.Instance?.PlayLoopSE("Fire");
    }
    else
    {
        AudioManager.Instance?.StopLoopSE("Fire");
    }
}
```

複数のループSEはキーごとに独立して再生できます。

```csharp
AudioManager.Instance?.PlayLoopSE("Fire");
AudioManager.Instance?.PlayLoopSE("Water");

AudioManager.Instance?.StopLoopSE("Fire");
AudioManager.Instance?.StopAllLoopSE();
```

Fireが1つ以上、水に触れて消火中の間は`Water4`が自動でループ再生されます。消火中のFireが0個になると自動停止するため、Fire側で個別に`Water4`を停止する必要はありません。

## 音量・ミュート

```csharp
AudioManager.Instance?.SetBgmVolume(0.5f);
AudioManager.Instance?.SetSeVolume(0.8f);
AudioManager.Instance?.SetMuted(true);
AudioManager.Instance?.ToggleMute();
```

音量は`PlayerPrefs`に保存されます。保存が不要な場合はInspectorの`saveVolumeSettings`を無効にしてください。
