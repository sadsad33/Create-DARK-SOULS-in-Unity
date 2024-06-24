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

        // ���� ����
        public override void Interact(PlayerManager playerManager) {
            // �÷��̾ ���ڸ� �ٶ󺸵��� ȸ���� �����Ѵ�.
            Vector3 rotationDirection = -transform.forward;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            // �÷��̾ ���ڸ� ���� ������ ��ġ�� ������ ��ǥ�� �����Ѵ�.
            playerManager.InteractionAtPosition("Open Chest", playerStandingPosition);
            
            // ���� ���� �ִϸ��̼� ����
            animator.Play("ChestOpen");
            
            // ���ڿ� �������� �������� �÷��̾ ������ �� �ֵ��� �Ѵ�.
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
            // ���ڿ� ��ȣ�ۿ��� ���� �Ŀ��� ���̻� ��ȣ�ۿ��� ���� �ʵ���
            Destroy(openChest);
        }
    }
}