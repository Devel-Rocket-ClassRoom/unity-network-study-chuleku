using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
public class RPCDemo : NetworkBehaviour
{
    
    public int m_ActionId = 1;
    private Keyboard keyboard;
    void Update()
    {
        if(!IsOwner)return;

        keyboard = Keyboard.current;
        if(keyboard ==null)return;

        if(keyboard.fKey.wasPressedThisFrame)
        {
            RequestActionRpc(m_ActionId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestActionRpc(int actionId,RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        if(actionId <= 0)
        {
            Debug.LogWarning($"[Server] 잘못된 actionId : {actionId} (clientId : {senderClientId})");
            return;
        }

        Debug.Log($"[Server] clientId = {senderClientId}, 액션 = {actionId} 수락");
        AnnounceActionRpc(senderClientId,actionId);
        AckRpc(actionId,RpcTarget.Single(senderClientId,RpcTargetUse.Temp));
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void AnnounceActionRpc(ulong actorClientId,int actionId)
    {
        bool isMine = actorClientId == NetworkManager.LocalClientId;
        Debug.Log($"[Client/Host] clientId = {actorClientId}, 액션 = {actionId}, Mind = {isMine}");
    }
    [Rpc(SendTo.SpecifiedInParams)]
    private void AckRpc(int actionId,RpcParams rpcParams = default)
    {
        Debug.Log($"[Client] 서버 응답 수신 : action{actionId} 처리완료");
    }

    [Rpc(SendTo.Server)]
    public void RequestScoreRpc(int score,RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        if(score <= 0)
        {
            Debug.LogWarning($"[Server] 잘못된 score : {score} (clientId : {senderClientId})");
            return;
        }

        Debug.Log($"[Server] Score 변동 수락 ");
        AnnounceScoreRpc(senderClientId,score);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AnnounceScoreRpc(ulong clientId,int score)
    {
        bool isMine = clientId == NetworkManager.LocalClientId;
        Debug.Log($"[Client/Host/Score] clientid = {clientId}, score = {score}, Mind = {isMine}");
    }
}
