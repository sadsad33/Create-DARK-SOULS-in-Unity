using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class OpenChest : Interactable {
        Animator animator;
        [SerializeField]OpenChest openChest;
        public Transform playerStandingPosition;
        public GameObject itemSpawner;
        public WeaponItem itemInChest;

        private void Awake() {
            animator = GetComponent<Animator>();
            openChest = GetComponent<OpenChest>();
        }
        public override void Interact(PlayerManager playerManager) {
            // �÷��̾ ���ڸ� �ٶ󺸵��� ȸ���� �����Ѵ�.
            //Vector3 rotationDirection = transform.position - playerManager.transform.position;
            Vector3 rotationDirection = -transform.forward;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            // �÷��̾ ���ڸ� ���� ������ ��ġ�� ������ ��ǥ�� �����Ѵ�.
            playerManager.OpenChestInteraction(playerStandingPosition);
            
            // ���� ���� �ִϸ��̼� ����
            animator.Play("ChestOpen");
            
            // ���ڿ� �������� �������� �÷��̾ ������ �� �ֵ��� �Ѵ�.
            StartCoroutine(SpawnItemInChest());
            WeaponPickUp weaponPickUp = itemSpawner.GetComponent<WeaponPickUp>();
            if (weaponPickUp != null) {
                weaponPickUp.weapon = itemInChest;
            }
        }

        private IEnumerator SpawnItemInChest() {
            yield return new WaitForSeconds(3f);
            Instantiate(itemSpawner, transform);
            // ���ڿ� ��ȣ�ۿ��� ���� �Ŀ��� ������ �ı�
            Destroy(openChest);
        }
    }
}