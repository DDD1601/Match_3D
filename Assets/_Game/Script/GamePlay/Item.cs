using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType type;
    public ItemType Type => type;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 2;

    public bool IsArrive(Vector3 target)
    {
        //kiem tra xem item co gan diem target hay khong
        return Vector3.Distance(rb.position, target) < 0.1f;
    }

    public void OnMove(Vector3 targetPoint)
    {
        //di chuyen den vi tri target
        rb.position = Vector3.MoveTowards(rb.position, targetPoint, Time.deltaTime * speed);
    }
    
    public void OnMove(Vector3 targetPoint, Quaternion targetRot, float time)
    {
        //di chuyen den vi tri target
        StartCoroutine(IEOnMove(targetPoint, targetRot, time));
    }

    private IEnumerator IEOnMove(Vector3 targetPoint, Quaternion targetRot, float time)
    {
        float timeCount = 0;
        Vector3 startPoint = rb.position;
        Quaternion startRot = rb.rotation;

        while (timeCount < time)
        {
            //loop theo thoi gian
            timeCount += Time.deltaTime;
            rb.position = Vector3.Lerp(startPoint, targetPoint, timeCount / time);
            rb.rotation = Quaternion.Lerp(startRot, targetRot, timeCount / time);
            yield return null;
        }

    }

    public void OnSelect()
    {
        //bat dau select
        rb.useGravity = false;
    }

    public void OnDrop()
    {
        rb.useGravity = true;
    }

    public void Force(Vector3 force)
    {
        //add them 1 luc cho item
        OnDrop();
        rb.velocity = Vector3.zero;
        rb.AddForce(force);
    }

    internal void SetKinematic(bool v)
    {
        //set co tinh vat ly hay khong
        rb.isKinematic = v;
    }

    internal void Collect()
    {
        //TODO: fix late
        gameObject.SetActive(false);
    }
}
