using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // UI menu tạm dừng
    [SerializeField] private GameObject mainMenuUI;  // UI menu chính
    [SerializeField] private Button useTimeButton;   // Nút sử dụng thời gian
    [SerializeField] private Button pauseButton;     // Nút tạm dừng
    [SerializeField] private TextMeshProUGUI levelText;         // Text hiển thị level hiện tại
    [SerializeField] public int currentLevel = 1; // Level hiện tại

    private bool isPaused = false;

    private void Start()
    {
        // Gán sự kiện cho các nút
        useTimeButton.onClick.AddListener(OnUseTime);
        pauseButton.onClick.AddListener(OnPause);

        // Hiển thị level hiện tại
        UpdateLevelText();

        // Ẩn UI tạm dừng và menu chính (chỉ hiển thị khi cần)
        pauseMenuUI.SetActive(false);
        mainMenuUI.SetActive(false);
    }

    private void UpdateLevelText()
    {
        if (LevelManager.instance != null)
        {
            levelText.text = $"Level: {LevelManager.instance.currentLevelItemCount}";
        }
    }

    public void OnUseTime()
    {
        // Thêm thời gian vào level
        if (LevelManager.instance != null)
        {
            LevelManager.instance.ResetTimer(); // Reset lại thời gian
            Debug.Log("Thời gian đã được sử dụng!");
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
        LevelManager.instance?.RetryLevel();
        pauseMenuUI.SetActive(false);
    }

    public void OnMainMenu()
    {
        // Quay về menu chính
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu"); // Đảm bảo có scene tên "MainMenu"
    }

    public void OnNewGame()
    {
        // Bắt đầu chơi mới từ menu chính
        SceneManager.LoadScene("GameScene"); // Đảm bảo có scene tên "GameScene"
    }

    public void OnExitGame()
    {
        // Thoát game
        Application.Quit();
    }
}
