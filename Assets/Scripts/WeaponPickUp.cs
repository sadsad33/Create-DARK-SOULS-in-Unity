using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class WeaponPickUp : Interactable {
        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // �θ��� Interact�Լ��� ȣ���Ѵ�.
            PickUpItem(playerManager);
        }
        private void PickUpItem(PlayerManager playerManager) {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            PlayerAnimatorManager animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
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
