using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponHolderSlot : MonoBehaviour {
        public Transform parentOverride; // ���� prefab�� �ϳ��� �������� �ʿ�
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;

        public void UnloadWeapon() {
            if (currentWeaponModel != null)
                currentWeaponModel.SetActive(false);
        }

        public void UnloadWeaponAndDestroy() { 
            if (currentWeaponModel != null)
                Destroy(currentWeaponModel);
        }

        // ���� ��������
        public void LoadWeaponModel(WeaponItem weaponItem) {
            if (weaponItem == null) {
                UnloadWeapon();
                return;
            }

            GameObject model = Instantiate(weaponItem.modelPrefab);
            if (model != null) {
                if (parentOverride != null)
                    model.transform.parent = parentOverride; // ���� ��ġ�� �����ϱ� ���� override �׸��� �θ� �����Ѵٸ� �ش� ��ġ�� �����ش�.
                else 
                    model.transform.parent = transform; // override �׸��� ������ �ڽ��� ��ġ�� �����.
                
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }
            currentWeaponModel = model;
        }
    }
}
