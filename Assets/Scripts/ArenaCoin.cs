using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class ArenaCoin : NetworkBehaviour
{
    [SerializeField]
    private float m_PickupRadius = 1.5f;

    [SerializeField]
    private int m_ScoreValue = 1;

    private bool m_Collected;

    [Rpc(SendTo.Server)]
    public void RequestPickupRpc(RpcParams rpcParams = default)
    {
        if(m_Collected || !IsSpawned)return;
        ulong senderclientId = rpcParams.Receive.SenderClientId;
        NetworkObject playerObject = NetworkManager.SpawnManager.GetPlayerNetworkObject(senderclientId);
        if(playerObject == null)
        {
            Debug.LogWarning($"플레이어 오브젝트 탐색 실패");
            return;
        }
        float distance = Vector3.Distance(playerObject.transform.position,transform.position);
        if(distance>m_PickupRadius)
        {
            Debug.Log($"줍기 거리 부족 {distance}");
            return;
        }
        m_Collected = true;
        CoinArenaManager manager = FindAnyObjectByType<CoinArenaManager>();
        manager.ServerAwardPoint(senderclientId,m_ScoreValue);

        NetworkObject.Despawn();
    }
}
