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
        // 월드로부터 루팅 가능한 아이템들의 목록인건지 내가 월드로부터 루팅 한 아이템의 목록인건지 애매
        // int 형 변수는 월드에 스폰된 아이템의 식별 번호
        // bool 변수는 플레이어가 아이템을 회수했는지 여부
        public SerializableDictionary<int, bool> itemsInWorld;

        public CharacterSaveData() {
            itemsInWorld = new SerializableDictionary<int, bool>();
        }
    }
}
