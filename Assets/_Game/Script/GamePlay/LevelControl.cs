using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private LayerMask itemLayer, stageLayer, groundLayer;
    [SerializeField] private Stage stage;
    private Item itemSelecting;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            itemSelecting = GetSelectItem();
            if (itemSelecting != null)
            {
                itemSelecting.OnSelect();
                stage.RemoveItem(itemSelecting);
            }
        }

        if (Input.GetMouseButton(0) && itemSelecting != null)
        {
            itemSelecting.OnMove(GetPoint());
        }

        if (Input.GetMouseButtonUp(0) && itemSelecting != null)
        {
            Stage targetStage = GetStage();
            if (targetStage != null && !targetStage.IsAnimating)
            {
                targetStage.AddItem(itemSelecting);
            }
            else
            {
                itemSelecting.OnDrop();
            }
        }
    }

    private Item GetSelectItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, itemLayer))
        {
            return hit.collider.GetComponent<Item>();
        }
        return null;
    }

    private Stage GetStage()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, stageLayer))
        {
            return hit.collider.GetComponent<Stage>();
        }
        return null;
    }

    private Vector3 GetPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100, groundLayer))
        {
            float x = (ray.origin.y - 2) * Vector3.Distance(ray.origin, hit.point) / ray.origin.y;
            return ray.origin + ray.direction * x;
        }
        return Vector3.zero;
    }
}