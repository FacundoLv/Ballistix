using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreDisplays;

    private void Start()
    {
        GameManager.Instance.OnScoreUpdated += UpdateScore;
        GameManager.Instance.OnTracked += ShowReady;
        GameManager.Instance.OnLostMatch += ShowLost;
    }

    private void OnDestroy()
    {
        var gameManager = GameManager.Instance;
        if (!gameManager) return;
        gameManager.OnScoreUpdated -= UpdateScore;
        gameManager.OnTracked -= ShowReady;
        gameManager.OnLostMatch -= ShowLost;
    }

    private void UpdateScore(PlayerInfo info)
    {
        scoreDisplays[info.ID].text = $"{info.Score}";
    }

    private void ShowReady(int id)
    {
        scoreDisplays[id].text = "Ready!";
    }

    private void ShowLost(int id)
    {
        scoreDisplays[id].text = "Lost";
    }
}