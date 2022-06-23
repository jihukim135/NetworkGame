using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using System.Threading;

[RequireComponent(typeof(AudioSource))]
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool endGameForDebug = false;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeLeftText;
    [SerializeField] private GameObject timeOutText;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private InputField nameInputField;

    private static int _rankCount = 10;
    [SerializeField] private GameObject rankingObject;
    [SerializeField] private Text namesText;
    [SerializeField] private Text scoresText;
    private string[] _names = new string[_rankCount];
    private int[] _scores = new int[_rankCount];
    private string _userName;

    [SerializeField] private AudioClip badItemClip;
    [SerializeField] private AudioClip goodItemClip;
    private AudioSource _audioSource;

    private StringBuilder _timeLeftBuilder = new StringBuilder(64);
    private byte[] _receivedBytes = new byte[1024]; // 서버 통신용

    public bool IsGameOver { get; set; } = false;

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

    private void Awake()
    {
        scoreText.gameObject.SetActive(true);
        timeLeftText.gameObject.SetActive(true);
        timeOutText.SetActive(false);

        _timeLeftBuilder.Append($"TIME LEFT: {_timeLimit:f1}");
        timeLeftText.text = _timeLeftBuilder.ToString();

        _audioSource = GetComponent<AudioSource>();
        rankingObject.SetActive(false);
    }

    void Update()
    {
        if (!IsGameOver)
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
        IsGameOver = true;
        scoreText.gameObject.SetActive(false);
        timeLeftText.gameObject.SetActive(false);
        timeOutText.SetActive(true);

        finalScoreText.text = $"YOUR SCORE IS: <color=yellow>{Score}</color>";
    }

    public void UpdateScore(EItem itemType)
    {
        switch (itemType)
        {
            case EItem.GoldCoin:
                Score += 1;
                _audioSource.clip = goodItemClip;
                break;
            case EItem.RedCoin:
                Score -= 3;
                _audioSource.clip = badItemClip;
                break;
            case EItem.Chest:
                Score += 5;
                _audioSource.clip = goodItemClip;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        scoreText.text = $"SCORE: {Score}";
        _audioSource.Play();
    }

    public void OnClickSendScoreAndGetRanks()
    {
        if (nameInputField.text == string.Empty)
        {
            return;
        }

        _userName = nameInputField.text;
        nameInputField.text = string.Empty;

        timeOutText.SetActive(false);

        using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            byte[] buffer = Encoding.UTF8.GetBytes($"{_userName},{_score}");

            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 10200);
            clientSocket.SendTo(buffer, serverEndPoint);

            // 랭킹을 받아옴
            int numberReceived = clientSocket.ReceiveFrom(_receivedBytes, ref serverEndPoint);
            string text = Encoding.UTF8.GetString(_receivedBytes, 0, numberReceived);
            Debug.Log(text);

            // 파싱해서 저장
            char[] newLineDelimiters = {'\n', '\r'};
            string[] lines = text.Split(newLineDelimiters, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(',');
                _names[i] = temp[0];
                if (!int.TryParse(temp[1], out _scores[i]))
                {
                    Debug.LogError("Parse failed");
                }
            }
        }

        ShowRanks();
    }

    private void ShowRanks()
    {
        rankingObject.SetActive(true);
        StringBuilder rankBuilder = new StringBuilder(512);

        rankBuilder.AppendLine("<color=yellow>name</color>");
        for (int i = 0; i < _rankCount; i++)
        {
            if (string.IsNullOrEmpty(_names[i]))
            {
                break;
            }

            if (_names[i] == _userName)
            {
                rankBuilder.AppendLine($"<color=yellow>{_names[i]}</color>");
            }
            else
            {
                rankBuilder.AppendLine($"{_names[i]}");
            }
        }

        namesText.text = rankBuilder.ToString();

        rankBuilder.Clear();
        rankBuilder.AppendLine("<color=yellow>score</color>");
        for (int i = 0; i < _rankCount; i++)
        {
            if (_scores[i] == 0)
            {
                break;
            }

            rankBuilder.AppendLine($"{_scores[i]}");
        }

        scoresText.text = rankBuilder.ToString();
    }
}