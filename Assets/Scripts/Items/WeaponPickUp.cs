using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class WeaponPickUp : Interactable {
        public WeaponItem weapon;

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
            playerInventory.weaponsInventory.Add(weapon);
            
            // ���� �������� ���� �Ҷ��� ���� �������� �̸��� ǥ�õǵ����Ѵ�.
            playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = weapon.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture; 
            playerManager.itemInteractableGameObject.SetActive(true);
            
            Destroy(gameObject);
        }
    }
}
