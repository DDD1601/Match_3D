using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemPrefabs; // Danh sách các prefab của Item (bóng)
    [SerializeField] private Transform[] spawnPoints; // Các vị trí spawn bóng
    [SerializeField] private int initialItemCount = 6; // Số lượng bóng ban đầu
    private int currentItemCount; // Số lượng bóng hiện tại
    private int currentLevel = 1; // Level hiện tại

    private List<GameObject> spawnedItems = new List<GameObject>(); // Danh sách các bóng đã spawn

    private void Start()
    {
        currentItemCount = initialItemCount;
        LevelManager.instance?.SetTotalItems(currentItemCount); // Cập nhật giá trị cho LevelManager
        LevelManager.instance.currentLevelItemCount = currentItemCount; // Lưu số lượng bóng ban đầu

        SpawnItems();
    }

    public void SpawnItems(int? itemCount = null)
    {
        // Dọn dẹp danh sách bóng cũ
        CleanupSpawnedItems();

        // Nếu không truyền vào số lượng item cụ thể, sử dụng giá trị mặc định của currentItemCount
        int spawnItemCount = itemCount ?? currentItemCount;

        // Tính tổng số cặp bóng cần spawn
        int pairCount = spawnItemCount / 2;

        List<Vector3> usedPositions = new List<Vector3>(); // Danh sách vị trí đã sử dụng

        for (int i = 0; i < pairCount; i++)
        {
            // Chọn một prefab item ngẫu nhiên
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            // Spawn cặp bóng cùng màu
            for (int j = 0; j < 2; j++)
            {
                Vector3 spawnPoint;

                // Tìm một vị trí spawn hợp lệ
                int maxAttempts = 10; // Giới hạn số lần thử
                int attempts = 0;
                do
                {
                    spawnPoint = GetRandomSpawnPoint();
                    attempts++;
                } while (IsPositionTooClose(spawnPoint, usedPositions) && attempts < maxAttempts);

                // Thêm vào danh sách vị trí đã sử dụng
                usedPositions.Add(spawnPoint);

                // Tạo bóng tại vị trí spawn
                GameObject newItem = Instantiate(itemPrefab, spawnPoint, Quaternion.identity);
                spawnedItems.Add(newItem);
            }
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Vector3 randomPosition;
        int maxAttempts = 100; // Giới hạn số lần thử để tránh vòng lặp vô tận
        int attempts = 0;

        do
        {
            // Random vị trí trong một vùng giới hạn (giả sử vùng này nằm trong tường)
            float x = Random.Range(-3.6f, 3.6f); // Giới hạn tọa độ X (cập nhật theo level của bạn)
            float z = Random.Range(-5f, 5f); // Giới hạn tọa độ Z
            float y = 1f; // Chiều cao Y cố định (mặt đất)

            randomPosition = new Vector3(x, y, z);

            // Tăng số lần thử
            attempts++;

            // Nếu quá số lần thử, thoát vòng lặp và trả về vị trí mặc định
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Không tìm được vị trí spawn hợp lệ! Sử dụng vị trí mặc định.");
                return Vector3.zero;
            }

        } while (!IsValidSpawnPosition(randomPosition)); // Lặp cho đến khi tìm được vị trí hợp lệ

        return randomPosition;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        float checkRadius = 0.5f; // Bán kính kiểm tra (kích thước của bóng)
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);

        // Nếu có bất kỳ collider nào được tìm thấy (tường), vị trí không hợp lệ
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return false;
            }
        }

        // Nếu không có va chạm nào, vị trí hợp lệ
        return true;
    }

    private bool IsPositionTooClose(Vector3 position, List<Vector3> usedPositions)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < 1) // Khoảng cách tối thiểu giữa các bóng
            {
                return true;
            }
        }
        return false;
    }

    private void CleanupSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }
        spawnedItems.Clear();
    }

    public void OnLevelComplete()
    {
        Debug.Log($"Level {currentLevel} đã hoàn thành!");

        // Tăng level và số lượng bóng
        currentLevel++;
        currentItemCount += 2; // Tăng số lượng bóng mỗi level

        LevelManager.instance.SetTotalItems(currentItemCount);

        // Bắt đầu level mới
        SpawnItems();
    }

    public int GetCurrentItemCount()
    {
        return currentItemCount;
    }
}