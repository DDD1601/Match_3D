using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private int totalItems; // Tổng số bóng trong level
    private int collectedItems; // Số bóng đã ghép cặp

    [SerializeField] private ItemSpawner itemSpawner; // Tham chiếu đến ItemSpawner
    [SerializeField] private GameObject levelCompleteUI; // UI hoàn thành màn chơi
    [SerializeField] private GameObject gameOverUI; // UI khi thua
    [SerializeField] private TMPro.TextMeshProUGUI timerText; // Text hiển thị thời gian
    [SerializeField] public int currentLevelItemCount; // Số lượng bóng của màn hiện tại
    [SerializeField] public int currentLevel = 1; // Level hiện tại
    [SerializeField] private float levelTime = 60f; // Thời gian tối đa của màn chơi (giây)

    private float remainingTime; // Thời gian còn lại
    private bool isGameOver = false; // Kiểm tra xem game đã kết thúc chưa
    private List<Item> items = new List<Item>(); // Danh sách các item hiện tại

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("LevelManager instance đã được tạo.");
        }

        OnInit();
    }

    public void OnInit()
    {
        isGameOver = false;
        StartTimer(); // Bắt đầu đếm thời gian
        Debug.Log("LevelManager đã được khởi tạo.");
    }

    public void OnDespawn()
    {
        CleanupRemainingItems();
        StopAllCoroutines(); // Dừng tất cả Coroutine
        Debug.Log("LevelManager đã được dọn dẹp.");
    }

    public void OnPlay()
    {
        Time.timeScale = 1;
        Debug.Log("Game bắt đầu (Time.timeScale = 1).");
    }

    public void OnWin()
    {
        Debug.Log("Người chơi đã thắng!");
        Time.timeScale = 0;

        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            Debug.Log("Hiển thị UI hoàn thành màn chơi.");
        }
    }
    public void OnRetryLevel()
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

    public void OnLose()
    {
        isGameOver = true;
        Debug.Log("Game Over! Người chơi đã thua.");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Time.timeScale = 0; // Dừng game
    }
    private List<Item> collectedItemBuffer = new List<Item>(); // Lưu trạng thái các bóng đã thu thập

    public void CollectItem(Item item)
    {
        // Thêm bóng vừa thu thập vào danh sách tạm thời
        collectedItemBuffer.Add(item);

        // Kiểm tra xem có 2 bóng cùng loại trong danh sách không
        if (collectedItemBuffer.Count >= 2)
        {
            // Lấy 2 bóng cuối cùng trong danh sách để kiểm tra
            Item item1 = collectedItemBuffer[collectedItemBuffer.Count - 2];
            Item item2 = collectedItemBuffer[collectedItemBuffer.Count - 1];

            // So sánh loại (ItemType) của hai bóng
            if (item1.Type == item2.Type)
            {
                collectedItems++; // Tăng số cặp bóng đã ghép
                Debug.Log($"Số bóng đã ghép: {collectedItems}/{totalItems / 2}");

                // Xóa 2 bóng này khỏi danh sách tạm thời (vì đã ghép cặp thành công)
                collectedItemBuffer.Remove(item1);
                collectedItemBuffer.Remove(item2);

                // Kiểm tra nếu đã ghép đủ cặp bóng
                if (collectedItems >= totalItems / 2)
                {
                    Debug.Log("Đã ghép đủ cặp bóng. Chuẩn bị hoàn thành level...");
                    CleanupRemainingItems();
                    OnWin();
                }
            }
        }
    }

    public void OnNextLevel()
    {
        Debug.Log("Người chơi chọn chuyển sang level tiếp theo.");
        currentLevel++;

        OnDespawn();
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        spawner.OnLevelComplete();
        currentLevelItemCount = spawner.GetCurrentItemCount();
        SetTotalItems(currentLevelItemCount);

        ResetTimer();
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.UpdateLevelText();
        }
        levelCompleteUI.SetActive(false);
        OnPlay();
    }

    public void SetTotalItems(int count)
    {
        totalItems = count;
        collectedItems = 0;
        Debug.Log($"Tổng số bóng trong level: {totalItems}");
    }

    public void DespawnItem(Item item)
    {
        items.Remove(item);
    }

    private void CleanupRemainingItems()
    {
        foreach (Item item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    private void ResetTimer()
    {
        StopAllCoroutines();
        remainingTime = levelTime;

        if (timerText != null)
        {
            timerText.text = $"Thời gian: {Mathf.CeilToInt(remainingTime)}s";
        }

        StartTimer();
        Debug.Log("Đã reset thời gian cho màn chơi mới.");
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
            yield return new WaitForSeconds(1f);
            remainingTime--;

            if (timerText != null)
            {
                timerText.text = $"Thời gian: {Mathf.CeilToInt(remainingTime)}s";
            }

            if (remainingTime <= 0)
            {
                OnLose();
            }
        }
    }
}