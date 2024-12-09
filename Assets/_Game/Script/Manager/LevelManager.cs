using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // Singleton để truy cập dễ dàng từ các script khác

    private int totalItems; // Tổng số bóng trong level
    private int collectedItems; // Số bóng đã ghép cặp

    [SerializeField] private ItemSpawner itemSpawner; // Tham chiếu đến ItemSpawner
    [SerializeField] private GameObject levelCompleteUI; // Tham chiếu đến UI hoàn thành màn chơi
    [SerializeField] public int currentLevelItemCount; // Lưu số lượng bóng của màn hiện tại

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("LevelManager instance đã được tạo.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Đã có một instance khác của LevelManager. GameObject này sẽ bị hủy.");
        }
    }

    public void SetTotalItems(int count)
    {
        totalItems = count;
        collectedItems = 0;
        Debug.LogError($"Tổng số bóng trong level: {totalItems}");
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
            /*itemSpawner.OnLevelComplete();*/ // Gọi để chuyển sang level mới

            //Debug.Log("Đặt lại tổng số bóng sau khi hoàn thành level.");
            //SetTotalItems(itemSpawner.GetCurrentItemCount()); // Cập nhật số lượng bóng mới cho level tiếp theo

            // Hiển thị UI hoàn thành màn chơi
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
                Debug.Log("Hiển thị UI hoàn thành màn chơi.");
            }

            // Tạm dừng game (nếu muốn)
            Time.timeScale = 0; // Nếu bạn không muốn dừng game, hãy bỏ dòng này
            Debug.Log("Game đã tạm dừng (Time.timeScale = 0).");
        }
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

    public void RetryLevel()
    {
        Debug.Log("Người chơi chọn chơi lại level.");

        // Dọn dẹp và chơi lại màn hiện tại
        CleanupRemainingItems();
        //itemSpawner.OnLevelComplete(); // Spawn lại các bóng
        //LevelManager.instance.SetTotalItems(itemSpawner.GetCurrentItemCount()); // Đặt lại số lượng bóng
        itemSpawner.SpawnItems(currentLevelItemCount); // Spawn lại các bóng với số lượng ban đầu
        SetTotalItems(currentLevelItemCount); // Đặt lại số lượng bóng ban đầu

        // Ẩn UI và tiếp tục game
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        Time.timeScale = 1; // Tiếp tục game
        Debug.Log("Game tiếp tục (Time.timeScale = 1).");
    }

    public void NextLevel()
    {
        Debug.Log("Người chơi chọn chuyển sang level tiếp theo.");

        // Chuyển sang màn tiếp theo
        CleanupRemainingItems();
        itemSpawner.OnLevelComplete(); // Tăng số lượng bóng
        //LevelManager.instance.SetTotalItems(itemSpawner.GetCurrentItemCount()); // Đặt số lượng bóng mới
        currentLevelItemCount = itemSpawner.GetCurrentItemCount(); // Cập nhật số lượng bóng mới
        SetTotalItems(currentLevelItemCount); // Đặt số lượng bóng mới

        // Ẩn UI và tiếp tục game
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        Time.timeScale = 1; // Tiếp tục game
        Debug.Log("Game tiếp tục (Time.timeScale = 1).");
    }
}
