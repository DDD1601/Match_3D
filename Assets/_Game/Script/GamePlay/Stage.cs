using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Stage : MonoBehaviour
{
    List<Item> items = new List<Item>();
    [SerializeField] Transform point1, point2;

    public void AddItem(Item item)
    {
        if (items.Count == 0)
        {
            //neu nhan 1 item thi di chuyen den vi tri dau tien
            items.Add(item);
            item.OnMove(point1.position, Quaternion.identity, 0.2f);
            item.SetKinematic(true);
        }
        else if (items.Count == 1) 
        {
            //neu nhan item thu 2 thi di chuyen den vi tri thu 2

            if (item.Type == items[0].Type)
            {
                //check neu la cung loai thi collect
                items.Add(item);
                item.OnMove(point2.position, Quaternion.identity, 0.2f);
                item.SetKinematic(true);

                Collect();
            }
            else
            {
                //khac loai thi nem item di
                item.Force(Vector3.up * 200 + Vector3.forward * 200);
            }
        }
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        item.SetKinematic(false);
    }

    //private void Collect()
    //{
    //    // Gọi hàm Collect trên cả 2 item
    //    items[0].Collect();
    //    items[1].Collect();
    //    items.Clear();

    //    // Gọi OnItemCollected() từ LevelManager để cập nhật trạng thái
    //    if (LevelManager.instance != null)
    //    {
    //        LevelManager.instance.OnItemCollected();
    //        Debug.Log("Cặp bóng đã được ghép thành công! Gọi OnItemCollected().");
    //    }
    //    else
    //    {
    //        Debug.LogError("LevelManager instance không tồn tại!");
    //    }
    //}

    private IEnumerator AnimateBothItems(Vector3 centerPoint, float duration)
    {
        Vector3 startPos1 = items[0].transform.position;
        Vector3 startPos2 = items[1].transform.position;

        Vector3 targetPos1 = centerPoint + new Vector3(-0.1f, -1, 0); // Điểm rơi bóng 1 (sang trái một chút)
        Vector3 targetPos2 = centerPoint + new Vector3(0.1f, -1, 0);  // Điểm rơi bóng 2 (sang phải một chút)

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Di chuyển bóng 1
            items[0].transform.position = Vector3.Lerp(startPos1, targetPos1, t);

            // Di chuyển bóng 2
            items[1].transform.position = Vector3.Lerp(startPos2, targetPos2, t);

            yield return null;
        }

        // Đảm bảo vị trí cuối cùng của cả hai bóng
        items[0].transform.position = targetPos1;
        items[1].transform.position = targetPos2;

        // Chờ một chút trước khi biến mất
        yield return new WaitForSeconds(0.2f);
    }

    private void Collect()
    {
        // Start animation trước khi thực hiện logic Collect
        StartCoroutine(AnimateAndCollect());
    }

    private IEnumerator AnimateAndCollect()
    {
        // Tạo vị trí trung tâm giữa hai quả bóng
        Vector3 centerPoint = (items[0].transform.position + items[1].transform.position) / 2;

        // Bước 1: Rơi xuống và di chuyển đến vị trí trung tâm
        float dropDuration = 0.5f; // Thời gian rơi xuống
        yield return StartCoroutine(AnimateBothItems(centerPoint, dropDuration));

        // Bước 2: Biến mất
        float disappearDuration = 0.3f; // Thời gian biến mất
        yield return StartCoroutine(items[0].Disappear(disappearDuration));
        yield return StartCoroutine(items[1].Disappear(disappearDuration));

        // Thực hiện logic sau khi biến mất
        foreach (var item in items)
        {
            item.Collect(); // Thực hiện các lệnh Collect
        }

        items.Clear();

        // Gọi OnItemCollected() từ LevelManager để cập nhật trạng thái
        if (LevelManager.instance != null)
        {
            LevelManager.instance.OnItemCollected();
            Debug.Log("Cặp bóng đã được ghép thành công! Gọi OnItemCollected().");
        }
        else
        {
            Debug.LogError("LevelManager instance không tồn tại!");
        }
    }
}
