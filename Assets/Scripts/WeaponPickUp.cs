using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class WeaponPickUp : Interactable {
        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // 부모의 Interact함수를 호출한다.
            PickUpItem(playerManager);
        }
        private void PickUpItem(PlayerManager playerManager) {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            PlayerAnimatorManager animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; // 플레이어가 아이템을 줍는동안 정지
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            playerInventory.weaponsInventory.Add(weapon);
            
            // 무기 아이템을 루팅 할때는 무기 아이템의 이름이 표시되도록한다.
            playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = weapon.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture; 
            playerManager.itemInteractableGameObject.SetActive(true);
            
            Destroy(gameObject);
        }
    }
}
