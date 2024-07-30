using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class CharacterNetworkManager : NetworkBehaviour {

        CharacterManager character;
        // 연결되어 있는 오브젝트의 월드 좌표와 회전값을 구함
        [Header("Network Position")]
        // 네트워크 상의 좌표는 접속해 있는 모든 사람이 읽을 수 있고, 주인만이 수정가능
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkVelocity;
        public float networkPositionSmoothTime = 0.1f; // 얼마나 빠르게 실제 위치가 네트워크 상으로 반영될지, 값이 커질수록 느리게 반영되어 마치 순간이동 하는것 처럼 이동
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public float networkRotationSmoothTime = 0.1f;

        // 게임오브젝트의 주인이라면 내 오브젝트의 값들을 네트워크 변수들에 할당
        // 주인이 아니라면 네트워크 값들에서 가져와 할당한다.
        // 누군가의 게임에 합류했다면, 누군가의 게임내에서 나의 오브젝트는 네트워크로부터 값들을 가져와 대입받음
        // 네트워크에 존재하는 값들은 내 게임에서 나의 오브젝트로부터 값을 할당받은 것
        [Header("Network Movement Animation")]
        public NetworkVariable<float> verticalNetworkMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> horizontalNetworkMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmountNetworkMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // sprintFlag 와 lockOnFlag 대체 (내 경우 사다리를 오르내릴때의 블렌드 트리가 따로 있어서 추가해줘야 할듯)
        // 기본적으로는 게임내의 어떤 다른 사람이 내 게임의 특정 스크립트에서 특정 변수가 변경되는것을 알아낼 방법이 없음
        // 하지만 네트워크 변수는 한쪽에서 변경될 때마다 모든 클라이언트들에 대해 변경되기 때문에 sprintFlag 와 lockOnFlag를 네트워크 변수로 만들어 사용
        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Equipment")]
        public NetworkVariable<int> currentRightWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentHeadEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentTorsoEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentGuntletEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLegEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
        }
        
        // RPC(Remote Procedure Call) : 원격 프로시저 호출
        // 원격제어를 위한 코딩없이 다른 주소 공간에서 함수나 프로시저를 실행할 수 있게 하는 프로세스 간 통신 기술

        // 서버RPC : 어떤 클라이언트인지 상관없이 클라이언트로 부터 서버로 전달되는 메시지
        [ServerRpc]
        public void NotifyServerOfAnimationServerRpc(ulong clientID, string animationID, bool isInteracting) {
            // 서버만이 클라이언트RPC 수행 가능
            if (IsServer) { // 현재 내가 서버라면(호스트라면?)
                // 클라이언트가 연결된 다른 모든 클라이언트들에서 애니메이션을 수행하도록 한다.
                ProcessAllClientsAnimationClientRpc(clientID, animationID, isInteracting);
            }
        }

        // 클라이언트RPC : 서버로부터 하나의 클라이언트 혹은 모든 클라이언트 들에게 전달되는 메시지
        [ClientRpc]
        // 만약 내 오브젝트가 애니메이션을 재생중이라면 연결된 다른 모든 클라이언트들에게 이 오브젝트가 애니메이션을 재생하고 있다는 사실을 알려줌
        private void ProcessAllClientsAnimationClientRpc(ulong clientID, string animationID, bool isInteracting) {
            // 내 단말에서 내가 애니메이션을 수행했을 때, 다른 클라이언트의 내 오브젝트가 애니메이션을 수행 한 후
            // 클라이언트RPC에 의해 내 단말에서 또다시 애니메이션을 수행하지 않도록 클라이언트 id가 다른 경우에만
            if (clientID != NetworkManager.Singleton.LocalClientId) {
                // 애니메이션 수행
                PerformAnimationFromServer(animationID, isInteracting);
            }
        }

        private void PerformAnimationFromServer(string animationID, bool isInteracting) {
            //anim.applyRootMotion = isInteracting;
            character.animator.SetBool("isInteracting", isInteracting);
            character.animator.CrossFade(animationID, 0.2f);
        }

        // 무기 변경
        public void OnRightWeaponChange(int oldWeaponID, int newWeaponID) {
            if (character.IsOwner) return;
            WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponItemByID(newWeaponID);

            if (newWeapon != null) {
                character.characterInventoryManager.rightWeapon = newWeapon;
                character.characterWeaponSlotManager.LoadWeaponOnSlot(newWeapon, false);
            }
        }
        public void OnLeftWeaponChange(int oldWeaponID, int newWeaponID) {
            if (character.IsOwner) return;
            WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponItemByID(newWeaponID);

            if (newWeapon != null) {
                character.characterInventoryManager.leftWeapon = newWeapon;
                character.characterWeaponSlotManager.LoadWeaponOnSlot(newWeapon, true);
            }
        }

        // Effects
        // 어떤 클라이언트가 다른 클라이언트의 오브젝트에서 서버 rpc를 호출할 수 있도록 해줌
        // 다른 클라이언트의 오브젝트에 데미지를 줬다면, 해당 오브젝트에서 서버 rpc를 호출해서 모든 클라이언트에게 이 오브젝트가 데미지를 받았다는 사실을 알릴수 있도록
        [ServerRpc(RequireOwnership = false)]
        public virtual void NotifyServerOfCharacterDamageServerRpc(ulong damagedCharacterID, 
            float physicalDamage, 
            float fireDamage, 
            float poiseDamage,
            float contactPointX,
            float contactPointY,
            float contactPointZ,
            ulong characterCausingDamageID) {

            // 내가 요청을 수행하는 서버라면
            if (IsServer) {
                ProcessDamageForCharacterAcrossAllClientRpc(damagedCharacterID, physicalDamage, fireDamage, poiseDamage, contactPointX, contactPointY, contactPointZ, characterCausingDamageID);
            }
        
        }

        [ClientRpc]
        private void ProcessDamageForCharacterAcrossAllClientRpc(ulong damagedCharacterID,
            float physicalDamage,
            float fireDamage,
            float poiseDamage,
            float contactPointX,
            float contactPointY,
            float contactPointZ,
            ulong characterCausingDamageID) {

            ProcessCharacterDamage(damagedCharacterID, physicalDamage, fireDamage, poiseDamage, contactPointX, contactPointY, contactPointZ, characterCausingDamageID);
        }

        private void ProcessCharacterDamage(ulong damagedCharacterID,
            float physicalDamage,
            float fireDamage,
            float poiseDamage,
            float contactPointX,
            float contactPointY,
            float contactPointZ,
            ulong characterCausingDamageID) {

            TakeDamageEffect takeDamageEffect = Instantiate(WorldEffectsManager.instance.takeDamageEffect);
            takeDamageEffect.physicalDamage = physicalDamage;
            takeDamageEffect.fireDamage = fireDamage;
            takeDamageEffect.poiseDamage = poiseDamage;
            takeDamageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
            
            //공격한 오브젝트와 공격당한 오브젝트를 구분할때 클라이언트ID 를 사용하지 않는 이유는 AI 캐릭터는 클라이언트 ID를 갖지 않기 때문
            takeDamageEffect.characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
            CharacterManager damageTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();

            if (damageTarget.isInvulnerable) return;

            damageTarget.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
        }
    }
}
