using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerStats : NetworkBehaviour
{
    public int mScorePerPress = 1;
    public int m_StartHealth = 100;
    private readonly NetworkVariable<int> m_Score = new NetworkVariable<int>
    (
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private readonly NetworkVariable<int> m_Health = new NetworkVariable<int>
    (
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private readonly NetworkVariable<FixedString32Bytes> m_PlayerName = new NetworkVariable<FixedString32Bytes>
    (
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );  
    public int Rpc_Score =>m_Score.Value;
    private int rpcScore;

    void Update()
    {
        if(!IsOwner)return;

        Keyboard keyboard = Keyboard.current;
        if(keyboard==null)return;

        if(keyboard.rKey.wasPressedThisFrame)
        {
            m_Score.Value += mScorePerPress;
        }
        if(keyboard.hKey.wasPressedThisFrame)
        {
            m_Health.Value -=5;
        }

    }
    public override void OnNetworkSpawn()
    {
        m_Score.OnValueChanged += HandleScoreChanged;
        m_Health.OnValueChanged += HandleHpChanged;
        m_PlayerName.OnValueChanged += HandleNameChanged;
        ApplyScore(m_Score.Value);
        ApplyHp(m_Health.Value);
        ApplyName(m_PlayerName.Value);
        if(IsServer)
        {
            m_Health.Value = m_StartHealth;
        }
        if(IsOwner&&m_PlayerName.Value.Length == 0)
        {
            m_PlayerName.Value = new FixedString32Bytes($"Player {OwnerClientId}");
        }
    }
    public override void OnNetworkDespawn()
    {
        m_Score.OnValueChanged -= HandleScoreChanged;
        m_Health.OnValueChanged -= HandleHpChanged;
        m_PlayerName.OnValueChanged -= HandleNameChanged;
    }
    private void HandleScoreChanged(int prev,int current)
    {
        ApplyScore(current);
        Debug.Log($"[PlayerStats] 점수 변경 {OwnerClientId} : {prev} -> {current}");
    }
    private void HandleHpChanged(int prev,int current)
    {
        ApplyHp(current);
        Debug.Log($"[PlayerStats] 체력 변경 {OwnerClientId} : {prev} -> {current}");
    }
    private void HandleNameChanged(FixedString32Bytes prev,FixedString32Bytes current)
    {
        ApplyName(current);
        Debug.Log($"[PlayerStats] 이름 변경 {OwnerClientId} : {prev} -> {current}");
        
    }
    [Rpc(SendTo.Server)]
    private void ReqScoreRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        if(senderClientId != OwnerClientId)return;

        rpcScore +=mScorePerPress;
        BroadcastScoreRpc(rpcScore);
        RequestCurrentScoreRpc(RpcTarget.Single(senderClientId,RpcTargetUse.Temp));
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastScoreRpc(int newScore)
    {
        rpcScore = newScore;
        ApplyScore(rpcScore);
    }
    [Rpc(SendTo.Server)]
    private void RequestCurrentScoreRpc(RpcParams rpcParams)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        SendCurrentScoreRpc(rpcScore,RpcTarget.Single(senderClientId,RpcTargetUse.Temp));
    }
    [Rpc(SendTo.SpecifiedInParams)]
    private void SendCurrentScoreRpc(int score,RpcParams rpcParams)
    {
        rpcScore = score;
        ApplyScore(rpcScore);
    }

    private void ApplyScore(int value)
    {
        
    }
    private void ApplyHp(int value)
    {
    }
    private void ApplyName(FixedString32Bytes value)
    {
        
    }
}
