using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [Serializable]
    public class CharacterSaveData{
        public string characterName;
        public int characterLevel;

        [Header("Equipment")]
        public int currentRightHandWeaponID;
        public int currentLeftHandWeaponID;
        public int currentHeadGearItemID;
        public int currentChestGearItemID;
        public int currentHandGearItemID;
        public int currentLegGearItemID;

        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Items Looted From World")]
        // ����κ��� ���� ������ �����۵��� ����ΰ��� ���� ����κ��� ���� �� �������� ����ΰ��� �ָ�
        // int �� ������ ���忡 ������ �������� �ĺ� ��ȣ
        // bool ������ �÷��̾ �������� ȸ���ߴ��� ����
        public SerializableDictionary<int, bool> itemsInWorld;

        public CharacterSaveData() {
            itemsInWorld = new SerializableDictionary<int, bool>();
        }
    }
}
