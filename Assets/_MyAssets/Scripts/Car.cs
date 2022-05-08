using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Car : MonoBehaviour
{
    public int ID { get; set; }
    private float _speed = 0f;
    private const float SpeedDecreaseRate = 0.96f;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private AudioSource _audioSource;

    private GameManager _gameManager;
    private Camera _camera;

    [SerializeField] private bool autoMode;
    private Vector3 _autoPosition;

    void Start()
    {
        _camera = Camera.main;
        _audioSource = GetComponent<AudioSource>();
        _gameManager = GameManager.Instance;
        _autoPosition = new Vector3(0.01f, 0f, 0f);
    }

    void Update()
    {
        if (_gameManager.IsGameOver)
        {
            return;
        }

        if (autoMode)
        {
            MoveAuto();
        }
        else
        {
            CheckInputAndMove();
        }

        ClampPositionOutOfCamera();
    }

    private void MoveAuto()
    {
        if (transform.position.x < -7f || transform.position.x > 7f)
        {
            _autoPosition *= -1f;
        }

        transform.Translate(_autoPosition);
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