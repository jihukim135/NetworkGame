using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject carObject;
    [SerializeField] private GameObject flagObject;
    [SerializeField] private Text scoreText;

    private float _distanceLeft;
    private bool _isGameOver = false;
    public bool IsGameOver => _isGameOver;
    private int _score = 0;
    private float _timeLimit = 60f;
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

    void Update()
    {
        if (_isGameOver)
        {
            return;
        }

        CheckTimeLeft();
    }

    private void CheckTimeLeft()
    {
        if (_timer > _timeLimit)
        {
            _isGameOver = true;
            return;
        }

        _timer += Time.deltaTime;
    }

    private void EndGame()
    {
        scoreText.gameObject.SetActive(false);
        
    }
    
    public void UpdateScoreAndSetText(EItem itemType)
    {
        switch (itemType)
        {
            case EItem.GoldCoin:
                _score += 1;
                break;
            case EItem.RedCoin:
                _score -= 1;
                break;
            case EItem.Chest:
                _score += 5;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        scoreText.text = $"SCORE: {_score}";
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