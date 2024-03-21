using UnityEngine;

public class SingletoneComponent<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;

    public static bool IsRegistered()
    {
        return s_instance != null;
    }

    public static T Instance
    {
        get
        {
            // find on first call
            if (s_instance == null)
            {
                T[] founds = FindObjectsOfType<T>();

                if (founds.Length <= 0)
                {
                    Debug.LogError($"SingletoneComponent instance type of {typeof(T).Name} not found");
                    return null;
                }

                RegisterInstance(founds[0] );

                if (founds.Length >= 2 )    // multiple component found. destroy others
                {
                    foreach (T t in founds)
                    {
                        if (t == s_instance)
                            continue;

                        Debug.Log($"Multple instance of {typeof(T).Name} in {t.name} found. Destroyed. ");
                        DestroyImmediate(t);
                    }
                }
                
            }

            return s_instance;
        }
    }

    public static void RegisterInstance(T instance)
    {
        if (s_instance == null)
        {
            // downcasting? 
            s_instance = instance;
            DontDestroyOnLoad(s_instance.gameObject);
        }
        else if (s_instance != instance)
        {
            GameObject.Destroy(instance);
            Debug.LogError($"SingletonComponent.RegisterInstance() duplicated {typeof(T).Name} instance {instance.name} found. Destroying... ");
        }

    }

    private void Awake()
    {
        RegisterInstance(this as T);    // downcasting? 
    }
}
