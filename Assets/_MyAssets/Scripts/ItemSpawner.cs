using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoinPrefab;
    [SerializeField] private GameObject redCoinPrefab;
    [SerializeField] private GameObject chestPrefab;

    private const int GoldCoinsCount = 10;
    private const int RedCoinsCount = 5;
    private const int ChestsCount = 3;

    private const int TotalPooledItemsCount = ChestsCount + RedCoinsCount + GoldCoinsCount;
    private GameObject[] _pooledItems = new GameObject[TotalPooledItemsCount];

    private Vector2 _initialPosition;
    [SerializeField] private float spawnInterval = 1f;

    private void Start()
    {
        for (int i = 0; i < GoldCoinsCount; i++)
        {
            _pooledItems[i] = Instantiate(goldCoinPrefab);
            _pooledItems[i].SetActive(false);
        }

        for (int i = GoldCoinsCount; i < GoldCoinsCount + RedCoinsCount; i++)
        {
            _pooledItems[i] = Instantiate(redCoinPrefab);
            _pooledItems[i].SetActive(false);
        }

        for (int i = GoldCoinsCount + RedCoinsCount; i < TotalPooledItemsCount; i++)
        {
            _pooledItems[i] = Instantiate(chestPrefab);
            _pooledItems[i].SetActive(false);
        }

        _initialPosition = Vector2.zero;

        StartCoroutine(StartSpawner());
    }

    private IEnumerator StartSpawner()
    {
        while (true)
        {
            if (GameManager.Instance.IsGameOver)
            {
                break;
            }

            yield return new WaitForSeconds(spawnInterval);

            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        int index = Random.Range(0, TotalPooledItemsCount);

        while (_pooledItems[index].activeSelf)
        {
            Debug.Log("RE-ROLL");
            index = Random.Range(0, TotalPooledItemsCount);
        }

        _pooledItems[index].SetActive(true);
        _initialPosition.x = Random.Range(-7, 8);
        _pooledItems[index].transform.position = _initialPosition;
    }
}