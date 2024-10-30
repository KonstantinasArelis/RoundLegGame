using TMPro;
using UnityEngine;

public class MainHudController : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private string startScoreText;

    void Awake()
    {
        scoreText = transform.Find("Panel/Score").GetComponent<TextMeshProUGUI>();
        startScoreText = scoreText.text;
        scoreText.text = startScoreText + "0";
    }

    public void AddScore(int score)
    {
        string currentScoreText = scoreText.text[startScoreText.Length..];
        int currentScore = int.Parse(currentScoreText);
        scoreText.text = startScoreText + (currentScore + score).ToString();
    }
}
