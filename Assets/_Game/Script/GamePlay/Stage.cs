using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private List<Item> items = new List<Item>();
    [SerializeField] private Transform point1, point2;
    private bool isAnimating = false; // Đánh dấu trạng thái của Stage
    public bool IsAnimating => isAnimating;

    public void AddItem(Item item)
    {
        if (isAnimating)
        {
            Debug.LogWarning("Stage đang bận thực hiện animation! Không thể thêm bóng mới.");
            return;
        }

        if (items.Count == 0)
        {
            // Nếu nhận 1 item, di chuyển đến vị trí đầu tiên
            items.Add(item);
            item.OnMove(point1.position, Quaternion.identity, 0.2f);
            item.SetKinematic(true);
        }
        else if (items.Count == 1)
        {
            // Nếu nhận item thứ 2, di chuyển đến vị trí thứ 2
            if (item.Type == items[0].Type)
            {
                // Nếu cùng loại, ghép cặp
                items.Add(item);
                item.OnMove(point2.position, Quaternion.identity, 0.2f);
                item.SetKinematic(true);

                Collect();
            }
            else
            {
                // Nếu khác loại, ném item đi
                item.Force(Vector3.up * 200 + Vector3.forward * 200);
            }
        }
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        item.SetKinematic(false);
    }

    private void Collect()
    {
        isAnimating = true; // Khóa Stage trước khi bắt đầu animation
        StartCoroutine(AnimateAndCollect());
    }

    private IEnumerator AnimateAndCollect()
    {
        // Tạo vị trí trung tâm giữa hai quả bóng
        Vector3 centerPoint = (items[0].transform.position + items[1].transform.position) / 2;

        // Bước 1: Hai quả bóng rơi tự do xuống 2 điểm trên `Stage`
        yield return StartCoroutine(AnimateBothItemsToStage(0.5f));

        // Bước 2: Hai quả bóng hợp lại với nhau
        yield return StartCoroutine(AnimateMerge(centerPoint, 0.5f));

        // Bước 3: Hai quả bóng nhỏ dần rồi biến mất
        yield return StartCoroutine(items[0].Disappear(0.3f));
        yield return StartCoroutine(items[1].Disappear(0.3f));

        // Thực hiện logic sau khi biến mất
        foreach (var item in items)
        {
            item.Collect();
            LevelManager.instance?.CollectItem(item);
        }

        items.Clear();
        isAnimating = false;
    }

    private IEnumerator AnimateBothItemsToStage(float duration)
    {
        Vector3 targetPos1 = point1.position;
        Vector3 targetPos2 = point2.position;

        Vector3 startPos1 = items[0].transform.position;
        Vector3 startPos2 = items[1].transform.position;

        float time = 0;
        // Lấy rotation mục tiêu (cố định hoặc giống bóng đầu tiên)
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0); // Đặt rotation về cùng 0,0,0 
        items[1].transform.rotation = items[0].transform.rotation; // Đồng bộ rotation của bóng 2 với bóng 1

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            items[0].transform.position = Vector3.Lerp(startPos1, targetPos1, t);
            items[0].transform.rotation = Quaternion.Lerp(items[0].transform.rotation, targetRotation, t);

            items[1].transform.position = Vector3.Lerp(startPos2, targetPos2, t);
            items[1].transform.rotation = Quaternion.Lerp(items[1].transform.rotation, targetRotation, t);


            yield return null;
        }
        // Đảm bảo vị trí cuối cùng của các bóng
        items[0].transform.position = targetPos1;
        items[0].transform.rotation = targetRotation;

        items[1].transform.position = targetPos2;
        items[1].transform.rotation = targetRotation;

        yield return null;
    }

    private IEnumerator AnimateMerge(Vector3 centerPoint, float duration)
    {
        Vector3 startPos1 = items[0].transform.position;
        Vector3 startPos2 = items[1].transform.position;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            items[0].transform.position = Vector3.Lerp(startPos1, centerPoint, t);
            items[1].transform.position = Vector3.Lerp(startPos2, centerPoint, t);

            yield return null;
        }
        // Đảm bảo vị trí cuối cùng của các bóng là trung tâm
        items[0].transform.position = centerPoint;
        items[1].transform.position = centerPoint;

        yield return null;
    }
}