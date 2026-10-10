using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float _motionTime = 3f;
    [SerializeField] private float _debugScore = 1500f;
    private TextMeshProUGUI _scoreText;

    private void Start()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
        GetScore(_debugScore);//デバック用
    }

    //スコアの演出だけをする。スコアが決まったタイミングでイベントかなんかで呼んでもらう
    private void GetScore(float score)
    {
        int _score = Mathf.RoundToInt(score);
        LMotion.Create(0, _score, _motionTime)
            .BindToText(_scoreText);
    }
}
