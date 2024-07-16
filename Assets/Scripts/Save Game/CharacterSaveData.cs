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
    }
}
