using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartJoin()
    {
        NetworkManager.Singleton.StartClient();
    }
}
