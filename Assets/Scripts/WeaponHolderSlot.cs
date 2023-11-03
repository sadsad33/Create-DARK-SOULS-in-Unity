using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponHolderSlot : MonoBehaviour {
        public Transform parentOverride; // 무기 prefab을 하나만 쓰기위해 필요
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;

        public void UnloadWeapon() {
            if (currentWeaponModel != null) // 현재 무기가 null이 아니라면
                currentWeaponModel.SetActive(false); // 현재 무기를 비활성화 한다.
        }

        public void UnloadWeaponAndDestroy() {
            if (currentWeaponModel != null)
                Destroy(currentWeaponModel);
        }

        // 무기 가져오기
        public void LoadWeaponModel(WeaponItem weaponItem) {
            if (weaponItem == null) { // 불러올 weaponItem이 null이면
                UnloadWeapon();
                return;
            }
            GameObject model = Instantiate(weaponItem.modelPrefab); 
            if (model != null) {
                UnloadWeaponAndDestroy(); // 이전 무기는 파괴, 비활성화
                if (parentOverride != null)
                    model.transform.parent = parentOverride; // 무기 위치를 조정하기 위한 override 항목이 부모에 존재한다면 해당 위치에 맞춰준다.
                else
                    model.transform.parent = transform; // override 항목이 없으면 자신의 위치에 맞춘다.

                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }
            currentWeaponModel = model;
        }
    }
}
