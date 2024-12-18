using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private int totalItems; // Tổng số bóng trong level
    private int collectedItems; // Số bóng đã ghép cặp

    [SerializeField] private ItemSpawner itemSpawner; // Tham chiếu đến ItemSpawner
    [SerializeField] private GameObject levelCompleteUI; // Tham chiếu đến UI hoàn thành màn chơi
    [SerializeField] public int currentLevelItemCount; // Lưu số lượng bóng của màn hiện tại
    [SerializeField] public int currentLevel = 1; // Level hiện tại
    [SerializeField] private float levelTime = 60f; // Thời gian tối đa của màn chơi (giây)
    private float remainingTime; // Thời gian còn lại
    [SerializeField] private GameObject gameOverUI; // Tham chiếu đến UI khi thua
    [SerializeField] private TMPro.TextMeshProUGUI timerText; // Text hiển thị thời gian
    private bool isGameOver = false; // Kiểm tra xem game đã kết thúc chưa

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("LevelManager instance đã được tạo.");
        }
        isGameOver = false;
        StartTimer(); // Bắt đầu đếm thời gian
    }

    public void SetTotalItems(int count)
    {
        totalItems = count;
        collectedItems = 0;
        Debug.Log($"Tổng số bóng trong level: {totalItems}");
    }

    public void OnItemCollected()
    {
        collectedItems++;
        Debug.Log($"Số bóng đã ghép: {collectedItems}/{totalItems / 2} (Mỗi cặp cần 2 bóng)");

        // Kiểm tra nếu tất cả bóng đã được ghép cặp
        if (collectedItems >= totalItems / 2) // Vì mỗi cặp bóng cần 2 bóng
        {
            Debug.Log("Đã ghép đủ cặp bóng. Chuẩn bị hoàn thành level...");

            CleanupRemainingItems(); // Dọn sạch các bóng còn lại

            Debug.Log("Đã dọn sạch các bóng còn lại. Gọi ItemSpawner.OnLevelComplete().");

            // Hiển thị UI hoàn thành màn chơi
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
                Debug.Log("Hiển thị UI hoàn thành màn chơi.");
            }

            // Tạm dừng game 
            Time.timeScale = 0;
            Debug.Log("Game đã tạm dừng (Time.timeScale = 0).");
        }
    }

    private void StartTimer()
    {
        remainingTime = levelTime;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0 && !isGameOver)
        {
            yield return new WaitForSeconds(1f); // Đợi 1 giây
            remainingTime--;

            // Cập nhật UI thời gian
            if (timerText != null)
            {
                timerText.text = $"Thời gian: {Mathf.CeilToInt(remainingTime)}s";
            }

            // Kiểm tra nếu hết thời gian
            if (remainingTime <= 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! Hết thời gian.");

        // Hiển thị UI thua
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Dừng game
        Time.timeScale = 0;
    }

    private void CleanupRemainingItems()
    {
        // Lấy tất cả các bóng còn lại trên sàn
        Item[] remainingItems = FindObjectsOfType<Item>();
        Debug.Log($"Số bóng còn lại cần dọn dẹp: {remainingItems.Length}");

        foreach (Item item in remainingItems)
        {
            Destroy(item.gameObject); // Xóa bóng
        }
    }

    public void ResetTimer()
    {
        StopAllCoroutines(); // Dừng bộ đếm thời gian
        remainingTime = levelTime; // Đặt lại thời gian còn lại
        if (timerText != null)
        {
            timerText.text = $"Thời gian: {Mathf.CeilToInt(remainingTime)}s"; // Cập nhật hiển thị
        }
        StartTimer(); // Bắt đầu đếm thời gian lại từ đầu
        Debug.Log("Đã reset thời gian cho màn chơi mới.");
    }

    public void RetryLevel()
    {
        Debug.Log("Người chơi chọn chơi lại level.");

        // Reset trạng thái game
        isGameOver = false;
        Time.timeScale = 1;
        // Dọn dẹp và chơi lại màn hiện tại
        CleanupRemainingItems();
        itemSpawner.SpawnItems(currentLevelItemCount); // Spawn lại các bóng với số lượng ban đầu
        SetTotalItems(currentLevelItemCount); // Đặt lại số lượng bóng ban đầu

        // Reset thời gian
        ResetTimer();
        // Ẩn UI và tiếp tục game
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        Time.timeScale = 1; // Tiếp tục game
        Debug.Log("Game tiếp tục (Time.timeScale = 1).");
    }

    public void NextLevel()
    {
        Debug.Log("Người chơi chọn chuyển sang level tiếp theo.");
        currentLevel++; // Tăng level

        // Chuyển sang màn tiếp theo
        CleanupRemainingItems();
        itemSpawner.OnLevelComplete(); // Tăng số lượng bóng
        currentLevelItemCount = itemSpawner.GetCurrentItemCount(); // Cập nhật số lượng bóng mới
        SetTotalItems(currentLevelItemCount); // Đặt số lượng bóng mới

        // Reset thời gian
        ResetTimer();
        // Cập nhật level text thông qua GameUIManager
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.UpdateLevelText();
        }
        // Ẩn UI và tiếp tục game
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        Time.timeScale = 1; // Tiếp tục game
        Debug.Log("Game tiếp tục (Time.timeScale = 1).");
    }
}
