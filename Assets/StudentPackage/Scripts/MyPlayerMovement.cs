using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

namespace NetworkStudy.Student
{
    public class MyPlayerMovement : NetworkBehaviour
    {
        [Tooltip("초당 이동 속도(월드 유닛).")]
        [SerializeField]
        private float m_MoveSpeed = 5f;
        [SerializeField]
        private float m_JumpForce = 3f;
        private float MoveSpeed =>m_MoveSpeed;

        [Tooltip("초당 회전 속도(도).")]
        [SerializeField]
        private float m_RotateSpeed = 120f;

        private bool isJump = false;
        private Coroutine cor;
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Debug.Log($"[MyPlayerMovement] 내 플레이어 스폰 {OwnerClientId}");
            }

        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            Keyboard keyboard = Keyboard.current;
            if(keyboard==null)return;
            float move = 0f;
            float turn = 0f;
            if(keyboard.wKey.isPressed)move +=1f;
            if(keyboard.aKey.isPressed)turn -=1f;
            if(keyboard.sKey.isPressed)move -=1f;
            if(keyboard.dKey.isPressed)turn +=1f;
            if (keyboard.shiftKey.isPressed)
            {
                m_MoveSpeed = 10f;
            }
            else
            {
                m_MoveSpeed = 5f;
            }
            if(keyboard.spaceKey.wasPressedThisFrame)
            {
                if(!isJump)
                StartCoroutine(Jump());
            }
            
            
            transform.Rotate(0f,turn*m_RotateSpeed*Time.deltaTime,0f);
            transform.Translate(0f,0f,move*MoveSpeed*Time.deltaTime);
        }

        private IEnumerator Jump()
        {
            isJump = true;
            float t = 0;
            float jumpduration = 0.5f;
            while(t<1f)
            {
                t += Time.deltaTime/jumpduration;
                float height = Mathf.Sin(t * Mathf.PI) * m_JumpForce;
                Vector3 CurrentPos = transform.position;
                transform.position = new Vector3(CurrentPos.x,height,CurrentPos.z);
                yield return null;
            }
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x,0f,pos.z);
            isJump = false;
        }
    }
}
