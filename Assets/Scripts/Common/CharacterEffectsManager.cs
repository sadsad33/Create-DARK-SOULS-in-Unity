using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterEffectsManager : MonoBehaviour {
        CharacterManager character;

        [Header("Static Effect")]
        [SerializeField] List<StaticCharacterEffect> staticCharacterEffects;

        [Header("Timed Effects")]
        public List<CharacterEffect> timedEffects;
        [SerializeField] float effectTickTimer = 0;

        [Header("Weapon FX")]
        public WeaponManager rightWeaponManager; 
        public WeaponManager leftWeaponManager;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        [Header("Right Weapon Buff")]
        public WeaponBuffEffect rightWeaponBuffEffect;

        [Header("Poison")]
        public GameObject defaultPoisonedParticleFX; // 플레이어가 기본적으로 가지고 있는 독 상태이상 이펙트
        public GameObject currentPoisonedParticleFX; // 독 상태이상에 걸렸다면 사용, 상태이상이 해제되면 파괴
        public Transform buildUpTransform; // 축적 이펙트가 생성될 위치

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
        }

        [Obsolete(" 직접적으로 플레이어에게 적용된 Static Effect 목록을 검사하는것이 아니라 PlayerInventoryManager 의 Start 메소드 에서 반지를 모두 장착하는 함수를 호출하여 Static Effect 들을 적용 시키는 상태. 만약 게임 시작시 Static Effect가 제대로 적용되지 않는다면 이곳 참조")]
        protected virtual void Start() {
            //foreach (var effect in staticCharacterEffects) {
            //    if (effect != null)
            //        effect.AddStaticEffect(character);
            //}
        }

        public virtual void ProcessAllTimedEffects() {
            effectTickTimer += Time.deltaTime;
            if (effectTickTimer >= 1) {
                effectTickTimer = 0;
                ProcessWeaponBuffs();

                for (int i = timedEffects.Count - 1; i > -1; i--) {
                    timedEffects[i].ProcessEffect(character);
                }
            }
        }


        public void ProcessWeaponBuffs() {
            // 현재 오른손 무기에 버프가 걸린 상태라면
            if (rightWeaponBuffEffect != null) {
                // 버프 적용
                rightWeaponBuffEffect.ProcessEffect(character);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect) {
            // StaticCharacterEffect 리스트를 체크하여 중복적용이 되지 않도록 함
            StaticCharacterEffect staticEffect;
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--) {
                if (staticCharacterEffects[i] != null) {
                    if (staticCharacterEffects[i].effectID == effect.effectID) {
                        staticEffect = staticCharacterEffects[i];
                        // 캐릭터에게서 해당 효과를 제거
                        staticEffect.RemoveStaticEffect(character);
                        // 캐릭터가 가진 정적 효과 목록에서도 제거
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }
            // 캐릭터가 가진 정적 효과 목록에 추가
            staticCharacterEffects.Add(effect);
            // 캐릭터에게 해당 효과를 추가
            effect.AddStaticEffect(character);

            for (int i = staticCharacterEffects.Count - 1; i > -1; i--) {
                if (staticCharacterEffects[i] == null)
                    staticCharacterEffects.RemoveAt(i);
            }
        }

        public void RemoveStaticEffect(int effectID) {
            StaticCharacterEffect staticEffect;

            for (int i = staticCharacterEffects.Count; i > -1; i--) {
                if (staticCharacterEffects[i] != null) {
                    if (staticCharacterEffects[i].effectID == effectID) {
                        staticEffect = staticCharacterEffects[i];
                        staticEffect.RemoveStaticEffect(character);
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            for (int i = staticCharacterEffects.Count - 1; i > -1; i--) {
                if (staticCharacterEffects[i] == null)
                    staticCharacterEffects.RemoveAt(i);
            }
        }

        public virtual void PlayWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponManager != null) {
                    rightWeaponManager.PlayWeaponTrailFX();
                }
            } else {
                if (leftWeaponManager != null) {
                    leftWeaponManager.PlayWeaponTrailFX();
                }
            }
        }

        public virtual void StopWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponManager != null) {
                    rightWeaponManager.StopWeaponTrailFX();
                }
            } else {
                if (leftWeaponManager != null) {
                    leftWeaponManager.StopWeaponTrailFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation) {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }
    }
}