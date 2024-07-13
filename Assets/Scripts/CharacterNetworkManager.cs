using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// 연결되어 있는 플레이어(캐릭터?)의 월드 좌표와 회전값을 구함
namespace SoulsLike {
    public class CharacterNetworkManager : NetworkBehaviour {
        [Header("Network Position")]
        // 네트워크 상의 좌표는 접속해 있는 모든 사람이 읽을 수 있고, 주인만 수정가능
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkVelocity;
        public float networkPositionSmoothTime = 0.1f; // 얼마나 빠르게 실제 위치가 네트워크 상으로 반영될지, 값이 커질수록 느리게 반영되어 마치 순간이동 하는것 처럼 이동
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public float networkRotationSmoothTime = 0.1f;
    }
}
