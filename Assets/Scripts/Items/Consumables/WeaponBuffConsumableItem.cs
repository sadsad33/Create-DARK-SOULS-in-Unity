using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Consumable/Weapon Buff")]
    public class WeaponBuffConsumableItem : ConsumableItem {

        [Header("Effect")]
        [SerializeField] WeaponBuffEffect weaponBuffEffect;

        // 버프 사운드
        [Header("Buff SFX")]
        [SerializeField] AudioClip buffTriggerSound;
        public AudioSource buffTriggerSource;
        public override void AttemptToConsumeItem(PlayerManager player) {
            if (!CanIUseThisItem(player)) return;
            if (currentItemAmount > 0) player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, isInteracting, true);
            else player.playerAnimatorManager.PlayTargetAnimation("Shrug", true);
        }

        // 성공적으로 아이템을 사용 했다면
        public override void SuccessfullyConsumedItem(PlayerManager player) {
            base.SuccessfullyConsumedItem(player);
            // player.charcterSoundFXManager.PlaySoundFX(buffTriggerSound); // 소리 재생
            WeaponBuffEffect weaponBuff = Instantiate(weaponBuffEffect); // Effect 불러오기
            weaponBuff.isRightHandedBuff = true; 
            player.playerEffectsManager.rightWeaponBuffEffect = weaponBuff;
            player.playerEffectsManager.ProcessWeaponBuffs();
        }

        public override bool CanIUseThisItem(PlayerManager player) {
            if (player.playerInventoryManager.currentConsumable.currentItemAmount <= 0) return false;
            MeleeWeaponItem meleeWeapon = player.playerInventoryManager.rightWeapon as MeleeWeaponItem;

            if (meleeWeapon != null && meleeWeapon.canBeBuffed) return true;
            else return false;
        }
    }
}
