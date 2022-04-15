using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private float _speed = 0f;
    private const float SpeedDecreaseRate = 0.96f;
    private bool _isMoving = false;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private AudioSource _audioSource;

    private GameManager _gameManager;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (_gameManager.IsGameOver)
        {
            return;
        }

        CheckInputAndMove();

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
            _isMoving = true;
        }

        transform.Translate(_speed, 0, 0);
        _speed *= SpeedDecreaseRate;
    }
}