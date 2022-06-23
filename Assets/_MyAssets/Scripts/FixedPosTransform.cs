using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPosTransform : MonoBehaviour
{
    [SerializeField] public Vector2Int position;
    
    private void Update()
    {
        transform.position = ToWorldPos(position);
    }

    public static Vector2Int ToFixedPos(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x * 100), Mathf.FloorToInt(position.y * 100));
    }
    public static Vector2 ToWorldPos(Vector2Int position)
    {
        return new Vector2(position.x / 100f, position.y / 100f);
    }
    public static Vector2Int MousePosition()
    {
        return ToFixedPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}