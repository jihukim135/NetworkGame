using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool endGameForDebug = false;

    [SerializeField] private GameObject carObject;
    [SerializeField] private GameObject flagObject;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeLeftText;
    [SerializeField] private GameObject timeOutText;
    [SerializeField] private Text finalScoreText;

    private StringBuilder _timeLeftBuilder = new StringBuilder(64);

    private bool _isGameOver = false;
    public bool IsGameOver => _isGameOver;

    private int _score = 0;

    private int Score
    {
        get => _score;
        set
        {
            if (value < 0)
            {
                _score = 0;
                return;
            }

            _score = value;
        }
    }

    private float _timeLimit = 40f;
    private float _timer = 0f;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.FindWithTag("GameManager");
            if (go == null)
            {
                Debug.Log("GameManager not found");
                return;
            }

            _instance = go.GetComponent<GameManager>();
        }
    }

    private void Awake()
    {
        scoreText.gameObject.SetActive(true);
        timeLeftText.gameObject.SetActive(true);
        timeOutText.SetActive(false);

        _timeLeftBuilder.Append($"TIME LEFT: {_timeLimit:f1}");
        timeLeftText.text = _timeLeftBuilder.ToString();
    }

    void Update()
    {
        if (!_isGameOver)
        {
            if (endGameForDebug)
            {
                EndGame();
            }

            CheckTimeLeftAndSetText();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void CheckTimeLeftAndSetText()
    {
        if (_timer > _timeLimit)
        {
            EndGame();
            return;
        }

        _timer += Time.deltaTime;
        _timeLeftBuilder.Clear();
        _timeLeftBuilder.Append($"TIME LEFT: {_timeLimit - _timer:f1}");
        timeLeftText.text = _timeLeftBuilder.ToString();
    }

    private void EndGame()
    {
        _isGameOver = true;
        scoreText.gameObject.SetActive(false);
        timeLeftText.gameObject.SetActive(false);
        timeOutText.SetActive(true);
        finalScoreText.text = $"YOUR SCORE IS: <color=yellow>{Score}</color>";
    }

    public void UpdateScoreAndSetText(EItem itemType)
    {
        switch (itemType)
        {
            case EItem.GoldCoin:
                Score += 1;
                break;
            case EItem.RedCoin:
                Score -= 3;
                break;
            case EItem.Chest:
                Score += 5;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        scoreText.text = $"SCORE: {Score}";
    }

    private float GetDistanceLeft()
    {
        return flagObject.transform.position.x - carObject.transform.position.x;
    }

    public void UpdateDistanceFromServer()
    {
        try
        {
            using Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] buffer = Encoding.UTF8.GetBytes($"{GetDistanceLeft()}");

            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 10200);
            clientSocket.SendTo(buffer, serverEndPoint);

            byte[] receivedBytes = new byte[1024];
            int numberReceived = clientSocket.ReceiveFrom(receivedBytes, ref serverEndPoint);
            string text = Encoding.UTF8.GetString(receivedBytes, 0, numberReceived);

            Debug.Log(text);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}