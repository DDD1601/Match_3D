using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType type;
    public ItemType Type => type;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 2;

    public bool IsArrive(Vector3 target)
    {
        // Kiểm tra xem item có gần điểm target hay không
        return Vector3.Distance(rb.position, target) < 0.1f;
    }

    public void OnMove(Vector3 targetPoint)
    {
        // Di chuyển đến vị trí target
        rb.position = Vector3.MoveTowards(rb.position, targetPoint, Time.deltaTime * speed);
    }

    public void OnMove(Vector3 targetPoint, Quaternion targetRot, float time)
    {
        // Di chuyển đến vị trí target với animation
        StartCoroutine(IEOnMove(targetPoint, targetRot, time));
    }

    private IEnumerator IEOnMove(Vector3 targetPoint, Quaternion targetRot, float time)
    {
        float timeCount = 0;
        Vector3 startPoint = rb.position;
        Quaternion startRot = rb.rotation;

        while (timeCount < time)
        {
            // Loop theo thời gian
            timeCount += Time.deltaTime;
            rb.position = Vector3.Lerp(startPoint, targetPoint, timeCount / time);
            rb.rotation = Quaternion.Lerp(startRot, targetRot, timeCount / time);
            yield return null;
        }
    }

    public IEnumerator Disappear(float duration)
    {
        // Hiệu ứng scale nhỏ dần
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        // Đảm bảo đã scale về 0
        transform.localScale = targetScale;
    }

    public void OnSelect()
    {
        // Bắt đầu select
        rb.useGravity = false;
    }

    public void OnDrop()
    {
        rb.useGravity = true;
    }

    public void Force(Vector3 force)
    {
        // Add thêm một lực cho item
        OnDrop();
        rb.velocity = Vector3.zero;
        rb.AddForce(force * 0.5f);
    }

    internal void SetKinematic(bool v)
    {
        // Set có tính vật lý hay không
        rb.isKinematic = v;
    }

    internal void Collect()
    {
        Debug.Log($"{gameObject.name} đã được thu thập!");
        gameObject.SetActive(false);
        LevelManager.instance?.DespawnItem(this);
    }
}