using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class WeaponBuffConsumableItem : MonoBehaviour {

        [Header("Effect")]
        [SerializeField] WeaponBuffEffect weaponBuffEffect;

        // 버프 사운드
        [Header("Buff SFX")]
        [SerializeField] AudioClip buffTriggerSound;
    }
}
