using System;
using UnityEngine;

namespace Script.Misc
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static readonly object _lock = new object();


        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }


                    _instance = (T)FindObjectOfType(typeof(T));

                    // Try to create an instance from prefab if specified
                    if (_instance == null)
                    {
                        Type myType = typeof(T);
                        bool hasPrefab = Attribute.IsDefined(myType, typeof(SingletonPrefabAttribute));

                        if (hasPrefab)
                        {
                            SingletonPrefabAttribute attribute =
                                (SingletonPrefabAttribute)Attribute.GetCustomAttribute(myType, typeof(SingletonPrefabAttribute));

                            string prefabPath = attribute.Name;

                            if (string.IsNullOrEmpty(prefabPath))
                            {
                                prefabPath = myType.ToString();
                            }

                            try
                            {
                                GameObject prefab = Resources.Load<GameObject>(prefabPath);
                                GameObject newInstance = (GameObject)Instantiate(prefab);

                                _instance = newInstance.GetComponent<T>();
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                            }
                        }
                    }


                    return _instance;
                }
            }
        }

    }
}