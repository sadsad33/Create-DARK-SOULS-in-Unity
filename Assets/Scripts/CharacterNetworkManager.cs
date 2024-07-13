using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// ����Ǿ� �ִ� �÷��̾�(ĳ����?)�� ���� ��ǥ�� ȸ������ ����
namespace SoulsLike {
    public class CharacterNetworkManager : NetworkBehaviour {
        [Header("Network Position")]
        // ��Ʈ��ũ ���� ��ǥ�� ������ �ִ� ��� ����� ���� �� �ְ�, ���θ� ��������
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkVelocity;
        public float networkPositionSmoothTime = 0.1f; // �󸶳� ������ ���� ��ġ�� ��Ʈ��ũ ������ �ݿ�����, ���� Ŀ������ ������ �ݿ��Ǿ� ��ġ �����̵� �ϴ°� ó�� �̵�
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public float networkRotationSmoothTime = 0.1f;
    }
}
