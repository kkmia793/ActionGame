using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text distanceText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateScore(float score)
    {
        scoreText.text = $"Score: {score:F1}";
    }

    public void UpdateDistance(float distance)
    {
        distanceText.text = $"Distance: {distance:F1}m";
    }

    public void DisplayGameOverScreen()
    {
        if(gameOverScreen == null) return;
        gameOverScreen.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameOverScreen.SetActive(false);
    }
}