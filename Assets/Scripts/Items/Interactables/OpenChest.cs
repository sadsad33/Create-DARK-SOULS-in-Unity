using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class OpenChest : Interactable {
        Animator animator;
        [SerializeField]OpenChest openChest;
        public Transform playerStandingPosition;
        public GameObject itemSpawner;
        //public WeaponItem itemInChest;
        public Item itemInChest;
        private void Awake() {
            animator = GetComponent<Animator>();
            openChest = GetComponent<OpenChest>();
        }

        // 상자 열기
        public override void Interact(PlayerManager playerManager) {
            // 플레이어가 상자를 바라보도록 회전을 강제한다.
            Vector3 rotationDirection = -transform.forward;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            // 플레이어가 상자를 열때 적절한 위치에 오도록 좌표를 강제한다.
            playerManager.InteractionAtPosition("Open Chest", playerStandingPosition);
            
            // 상자 여는 애니메이션 실행
            animator.Play("ChestOpen");
            
            // 상자에 아이템을 스폰시켜 플레이어가 루팅할 수 있도록 한다.
            StartCoroutine(SpawnItemInChest());
            ItemPickUp itemPickUp = itemSpawner.GetComponent<ItemPickUp>();
            if (itemPickUp != null) {
                //weaponPickUp.weapon = itemInChest;
                itemPickUp.item = itemInChest;
            }
        }

        private IEnumerator SpawnItemInChest() {
            yield return new WaitForSeconds(3f);
            Instantiate(itemSpawner, transform);
            // 상자와 상호작용을 끝낸 후에는 더이상 상호작용이 되지 않도록
            Destroy(openChest);
        }
    }
}