using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class CharacterNetworkManager : NetworkBehaviour {

        CharacterManager character;
        // ����Ǿ� �ִ� ������Ʈ�� ���� ��ǥ�� ȸ������ ����
        [Header("Network Position")]
        // ��Ʈ��ũ ���� ��ǥ�� ������ �ִ� ��� ����� ���� �� �ְ�, ���θ��� ��������
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkVelocity;
        public float networkPositionSmoothTime = 0.1f; // �󸶳� ������ ���� ��ġ�� ��Ʈ��ũ ������ �ݿ�����, ���� Ŀ������ ������ �ݿ��Ǿ� ��ġ �����̵� �ϴ°� ó�� �̵�
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public float networkRotationSmoothTime = 0.1f;

        // ���ӿ�����Ʈ�� �����̶�� �� ������Ʈ�� ������ ��Ʈ��ũ �����鿡 �Ҵ�
        // ������ �ƴ϶�� ��Ʈ��ũ ���鿡�� ������ �Ҵ��Ѵ�.
        // �������� ���ӿ� �շ��ߴٸ�, �������� ���ӳ����� ���� ������Ʈ�� ��Ʈ��ũ�κ��� ������ ������ ���Թ���
        // ��Ʈ��ũ�� �����ϴ� ������ �� ���ӿ��� ���� ������Ʈ�κ��� ���� �Ҵ���� ��
        [Header("Network Movement Animation")]
        public NetworkVariable<float> verticalNetworkMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> horizontalNetworkMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmountNetworkMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // sprintFlag �� lockOnFlag ��ü (�� ��� ��ٸ��� ������������ ���� Ʈ���� ���� �־ �߰������ �ҵ�)
        // �⺻�����δ� ���ӳ��� � �ٸ� ����� �� ������ Ư�� ��ũ��Ʈ���� Ư�� ������ ����Ǵ°��� �˾Ƴ� ����� ����
        // ������ ��Ʈ��ũ ������ ���ʿ��� ����� ������ ��� Ŭ���̾�Ʈ�鿡 ���� ����Ǳ� ������ sprintFlag �� lockOnFlag�� ��Ʈ��ũ ������ ����� ���
        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
        }
        
        // RPC(Remote Procedure Call) : ���� ���ν��� ȣ��
        // ������� ���� �ڵ����� �ٸ� �ּ� �������� �Լ��� ���ν����� ������ �� �ְ� �ϴ� ���μ��� �� ��� ���

        // ����RPC : � Ŭ���̾�Ʈ���� ������� Ŭ���̾�Ʈ�� ���� ������ ���޵Ǵ� �޽���
        [ServerRpc]
        public void NotifyServerOfAnimationServerRpc(ulong clientID, string animationID, bool isInteracting) {
            // �������� Ŭ���̾�ƮRPC ���� ����
            if (IsServer) { // ���� ���� �������(ȣ��Ʈ���?)
                // Ŭ���̾�Ʈ�� ����� �ٸ� ��� Ŭ���̾�Ʈ�鿡�� �ִϸ��̼��� �����ϵ��� �Ѵ�.
                ProcessAllClientsAnimationClientRpc(clientID, animationID, isInteracting);
            }
        }

        // Ŭ���̾�ƮRPC : �����κ��� �ϳ��� Ŭ���̾�Ʈ Ȥ�� ��� Ŭ���̾�Ʈ �鿡�� ���޵Ǵ� �޽���
        [ClientRpc]
        // ���� �� ������Ʈ�� �ִϸ��̼��� ������̶�� ����� �ٸ� ��� Ŭ���̾�Ʈ�鿡�� �� ������Ʈ�� �ִϸ��̼��� ����ϰ� �ִٴ� ����� �˷���
        private void ProcessAllClientsAnimationClientRpc(ulong clientID, string animationID, bool isInteracting) {
            // �� �ܸ����� ���� �ִϸ��̼��� �������� ��, �ٸ� Ŭ���̾�Ʈ�� �� ������Ʈ�� �ִϸ��̼��� ���� �� ��
            // Ŭ���̾�ƮRPC�� ���� �� �ܸ����� �Ǵٽ� �ִϸ��̼��� �������� �ʵ��� Ŭ���̾�Ʈ id�� �ٸ� ��쿡��
            if (clientID != NetworkManager.Singleton.LocalClientId) {
                // �ִϸ��̼� ����
                PerformAnimationFromServer(animationID, isInteracting);
            }
        }

        private void PerformAnimationFromServer(string animationID, bool isInteracting) {
            //anim.applyRootMotion = isInteracting;
            character.animator.SetBool("isInteracting", isInteracting);
            character.animator.CrossFade(animationID, 0.2f);
        }
    }
}
