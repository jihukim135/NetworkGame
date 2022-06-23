using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Car : MonoBehaviour
{
    private float _totalSpeed;
    [SerializeField] private float speed;
    private const float SpeedDecreaseRate = 0.96f;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private AudioSource _audioSource;

    private GameManager _gameManager;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
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

        ClampPositionOutOfCamera();
    }


    private void CheckInputAndMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startPosition =  _camera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _endPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _totalSpeed = (_endPosition.x - _startPosition.x) * speed;

            _audioSource.Play();
        }

        transform.Translate(_totalSpeed, 0, 0);
        _totalSpeed *= SpeedDecreaseRate;
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