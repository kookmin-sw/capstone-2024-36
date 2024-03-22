using UnityEngine;
using Unity.Netcode;

public class NetworkSingletoneComponent<T> : NetworkBehaviour where T : MonoBehaviour
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
            if (s_instance == null)
                Debug.LogError($"NetworkSingletoneComponent instance type of {typeof(T).Name} not found");

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
            Debug.LogError($"NetworkSingletoneComponent.RegisterInstance() duplicated {typeof(T).Name} instance {instance.name} found. Destroying... ");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RegisterInstance(this as T);
    }
}
