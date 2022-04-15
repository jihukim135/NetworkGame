using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private float fallSpeedMin = 2f;
    private float fallSpeedMax = 5f;
    private float _fallSpeed;

    [SerializeField] private EItem itemType;

    private void OnEnable()
    {
        _fallSpeed = Random.Range(fallSpeedMin, fallSpeedMax + 1);
    }

    void Update()
    {
        transform.Translate(0f, -_fallSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name);
        gameObject.SetActive(false);
        GameManager.Instance.UpdateScoreAndPlaySound(itemType);
    }
}