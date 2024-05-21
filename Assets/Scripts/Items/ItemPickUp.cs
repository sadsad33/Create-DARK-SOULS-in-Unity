using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ItemPickUp : Interactable {
        //public WeaponItem weapon;
        public Item item;
        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // 부모의 Interact함수를 호출한다.
            PickUpItem(playerManager);
        }
        
        // 아이템 줍기
        private void PickUpItem(PlayerManager playerManager) {
            PlayerInventoryManager playerInventory;
            PlayerLocomotionManager playerLocomotion;
            PlayerAnimatorManager animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventoryManager>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotionManager>();
            animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; // 플레이어가 아이템을 줍는동안 정지
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            //playerInventory.weaponsInventory.Add(weapon);
            if (item is WeaponItem) playerInventory.weaponsInventory.Add(item as WeaponItem);
            else if (item is ConsumableItem) playerInventory.consumablesInventory.Add(item as ConsumableItem);

            // 무기 아이템을 루팅 할때는 무기 아이템의 이름이 표시되도록한다.
            playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture; 
            playerManager.itemInteractableGameObject.SetActive(true);
            
            Destroy(gameObject);
        }
    }
}
