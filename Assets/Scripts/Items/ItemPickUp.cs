using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ItemPickUp : Interactable {
        //public WeaponItem weapon;
        public Item item;
        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // �θ��� Interact�Լ��� ȣ���Ѵ�.
            PickUpItem(playerManager);
        }
        
        // ������ �ݱ�
        private void PickUpItem(PlayerManager playerManager) {
            PlayerInventoryManager playerInventory;
            PlayerLocomotionManager playerLocomotion;
            PlayerAnimatorManager animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventoryManager>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotionManager>();
            animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; // �÷��̾ �������� �ݴµ��� ����
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            //playerInventory.weaponsInventory.Add(weapon);
            if (item is WeaponItem) playerInventory.weaponsInventory.Add(item as WeaponItem);
            else if (item is ConsumableItem) playerInventory.consumablesInventory.Add(item as ConsumableItem);

            // ���� �������� ���� �Ҷ��� ���� �������� �̸��� ǥ�õǵ����Ѵ�.
            playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture; 
            playerManager.itemInteractableGameObject.SetActive(true);
            
            Destroy(gameObject);
        }
    }
}
