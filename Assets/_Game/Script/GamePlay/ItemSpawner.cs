using System.Collections;
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
        int pairCount = currentItemCount / 2;

        // Đảm bảo luôn spawn theo cặp
        for (int i = 0; i < pairCount; i++)
        {
            // Chọn một prefab item ngẫu nhiên
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            // Spawn cặp bóng cùng màu
            for (int j = 0; j < 2; j++) // 2 bóng mỗi cặp
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject newItem = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
                spawnedItems.Add(newItem);
            }
        }

        Debug.Log($"Level {currentLevel} bắt đầu với {currentItemCount} bóng (theo từng cặp)!");

    }

    // Gọi khi hoàn thành level
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
        return currentItemCount; // Trả về số lượng bóng hiện tại
    }


    // Dọn dẹp các bóng cũ
    private void CleanupSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }
        spawnedItems.Clear();
    }
}
