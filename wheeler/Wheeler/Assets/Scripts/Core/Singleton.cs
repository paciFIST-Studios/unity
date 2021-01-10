using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
//{
//    // https://wiki.unity3d.com/index.php/Singleton
//
//    private static bool _shuttingDown = false;
//    private static object _lock = new object();
//    private static T _instance;
//
//    public static T Instance
//    {
//        get
//        {
//            if(_shuttingDown)
//            {
//                Debug.LogWarning(string.Format($"[Singleton] Instance {0} is already destroyed. Returning Null.", typeof(T)));
//                return null;
//            }
//
//            lock(_lock)
//            {
//                if(_instance == null)
//                {
//                    _instance = (T)FindObjectOfType(typeof(T));
//
//                    if(_instance == null)
//                    {
//                        var singletonObject = new GameObject();
//                        _instance = singletonObject.AddComponent<T>();
//                        singletonObject.name = string.Format($"Singleton<{0}>", typeof(T).ToString());
//
//                        DontDestroyOnLoad(singletonObject);
//                    }
//                }
//            }
//
//            return _instance;
//        }
//    }
//
//
//    private void OnApplicationQuit()
//    {
//        _shuttingDown = true;
//    }
//
//    private void OnDestroy()
//    {
//        _shuttingDown = true;
//    }
//
//}
