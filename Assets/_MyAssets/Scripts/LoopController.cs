using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

// [RequireComponent(typeof(BoxCollider2D))]
public class LoopController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float _spriteWidth;

    void Start()
    {
        _spriteWidth = transform.GetChild(0).GetComponent<SpriteRenderer>().size.x;
    }

    void Update()
    {
        MoveAndRepose();
    }

    private void MoveAndRepose()
    {
        if (transform.position.x < -_spriteWidth)
        {
            transform.position = Vector3.zero;
        }

        transform.Translate(-speed * Time.deltaTime, 0f, 0f);
    }
}