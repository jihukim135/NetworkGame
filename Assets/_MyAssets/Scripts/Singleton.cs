using System.Runtime.CompilerServices;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    private static void Init()
    {
        if (_instance != null)
        {
            return;
        }
        
        GameObject go = GameObject.FindWithTag(typeof(T).Name);
        if (go == null)
        {
            Debug.Log($"{typeof(T).Name} not found");
            return;
        }

        _instance = go.GetComponent<T>();
    }
}
