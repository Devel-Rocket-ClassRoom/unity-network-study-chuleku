using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class CoinCollector : NetworkBehaviour
{
    [SerializeField]
    private float m_PickupSearchRadius = 2f;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            transform.position = new Vector3(Random.Range(-2f,2f),0.5f, Random.Range(-2f,2f));
        }
    }

    private void Update()
    {
        if(!IsOwner)return;
        Keyboard keyboard = Keyboard.current;
        if(keyboard ==null)return;

        if(keyboard.spaceKey.wasPressedThisFrame)
        {
            ArenaCoin coin = FindNearestCoin();
            if(coin==null)return;

            coin.RequestPickupRpc();
        }
    }
    public LayerMask coinLayer;

    private ArenaCoin FindNearestCoin()
    {

        var coins = Physics.OverlapSphere(transform.position,m_PickupSearchRadius,coinLayer);
        if(coins.Length ==0)return null;
        var minDistance = Vector3.Distance(transform.position,coins[0].transform.position);
        var minIndex = 0;
        for(int i = 0; i<coins.Length; i++)
        {
            var distance = Vector3.Distance(transform.position,coins[i].transform.position);
            if(minDistance >distance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }
        return coins[minIndex].GetComponent<ArenaCoin>();
    }
}
