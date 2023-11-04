using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponPickUp : Interactable {
        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // �θ��� Interact�Լ��� ȣ���Ѵ�.
            PickUpItem(playerManager);
        }
        private void PickUpItem(PlayerManager playerManager) {
            Debug.Log("������ �ݱ�");
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            AnimatorHandler animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; // �÷��̾ �������� �ݴµ��� ����
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            playerInventory.weaponsInventory.Add(weapon);
            Destroy(gameObject);
        }
    }
}
