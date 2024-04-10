using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnInfo
{
    public SpawnInfo()
    {
        NetTransform = null;
        SceneSequenceNumber = 0;
        CurrentSceneIndex = -1;
        RegisterId = -1;
    }

    public SpawnInfo(MyNetworkTransform netTr)
    {
        Update(netTr);
    }

    public SpawnInfo(ushort sceneSequenceNumber, int currentSceneIndex, int registerId)
    {
        NetTransform = null;
        SceneSequenceNumber = sceneSequenceNumber;
        CurrentSceneIndex = currentSceneIndex;
        RegisterId = registerId;
    }

    public void Update(MyNetworkTransform netTr)
    {
        NetTransform = netTr;
        SceneSequenceNumber = netTr.SceneSequenceNumber;
        CurrentSceneIndex = netTr.CurrentSceneIndex;
        RegisterId = netTr.RegisterId;
    }

    public MyNetworkTransform NetTransform;
    public ushort SceneSequenceNumber;
    public int CurrentSceneIndex;
    public int RegisterId;
}

public class NetworkSpawnInfoMap : Dictionary<ulong, SpawnInfo>
{
    public IEnumerator<MyNetworkTransform> GetSpawnedIterator()
    {
        List<MyNetworkTransform> netTrList = new List<MyNetworkTransform>();
        foreach(SpawnInfo info in Values)
        {
            if (info.NetTransform == null || !info.NetTransform.IsSpawned)
                continue;
                
            netTrList.Add(info.NetTransform);
        }

        return netTrList.GetEnumerator();
    }
}
