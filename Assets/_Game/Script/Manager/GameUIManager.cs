using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] private GameObject pauseMenuUI; // UI menu tạm dừng
    [SerializeField] private TextMeshProUGUI levelText; // Text hiển thị level hiện tại
    private bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        UpdateLevelText();

        // Ẩn UI tạm dừng (chỉ hiển thị khi cần)
        pauseMenuUI.SetActive(false);
    }

    public void UpdateLevelText()
    {
        if (LevelManager.instance != null)
        {
            levelText.text = $"Level: {LevelManager.instance.currentLevel}";
        }
    }

    public void OnPause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Hiển thị menu tạm dừng và dừng thời gian
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            // Tiếp tục game và ẩn menu tạm dừng
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OnRetry()
    {
        // Chơi lại màn chơi hiện tại
        Time.timeScale = 1;
        LevelManager.instance?.OnRetryLevel();
        pauseMenuUI.SetActive(false);

        UpdateLevelText();
    }

    public void OnExitGame()
    {
        // Thoát game
        Application.Quit();
    }

    public void OnNewGame()
    {
        // Bắt đầu chơi mới từ menu chính
        SceneManager.LoadScene("GameScene");
    }

    public void OnMainMenu()
    {
        // Quay về menu chính
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnContinue()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Ẩn menu tạm dừng
        }
        Time.timeScale = 1; // Tiếp tục game
        Debug.Log("Game tiếp tục!");
    }
}
