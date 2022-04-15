using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Camera;
using Debug = System.Diagnostics.Debug;

public class Car : MonoBehaviour
{
    private float _speed = 0f;
    private const float SpeedDecreaseRate = 0.96f;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private AudioSource _audioSource;

    private GameManager _gameManager;
    private Camera _camera;

    void Start()
    {
        _camera = main;
        _audioSource = GetComponent<AudioSource>();
        _gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (!_gameManager.IsGameOver)
        {
            CheckInputAndMove();
            ClampPositionOutOfCamera();
        }

        // if (!_gameManager.IsGameOver && _isMoving && _speed < 0.05f)
        // {
        //     _gameManager.UpdateDistanceFromServer();
        //     _isMoving = false;
        // }
    }

    private void CheckInputAndMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _endPosition = Input.mousePosition;
            _speed = (_endPosition.x - _startPosition.x) / 5000f;

            _audioSource.Play();
        }

        transform.Translate(_speed, 0, 0);
        _speed *= SpeedDecreaseRate;
    }

    private void ClampPositionOutOfCamera()
    {
        Vector3 pos = _camera.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos = _camera.ViewportToWorldPoint(pos);
        pos.z = 0f;
        transform.position = pos;
    }
}