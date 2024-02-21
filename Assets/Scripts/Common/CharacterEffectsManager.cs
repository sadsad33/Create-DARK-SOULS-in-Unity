using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterEffectsManager : MonoBehaviour {
        CharacterStatsManager characterStatsManager;
        [Header("Weapon FX")]
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        [Header("Poison")]
        public GameObject defaultPoisonedParticleFX; // �÷��̾ �⺻������ ������ �ִ� �� �����̻� ����Ʈ
        public GameObject currentPoisonedParticleFX; // �� �����̻� �ɷȴٸ� ���, �����̻��� �����Ǹ� �ı�
        public Transform buildUpTransform; // ���� ����Ʈ�� ������ ��ġ
        public bool isPoisoned;
        public float poisonBuildUp = 0; // �� ������ ���� ��ġ, �� ��ġ�� 100�� �Ǹ� �� �����̻� �ɸ���
        public float poisonAmount = 100; // �� ���¿��� �� �����̻��� �����Ǳ� ���� �ʿ��� ��ġ
        public float defaultPoisonAmount = 100; // �� �����̻� �ɷ��� ��� ������ �� ��ġ
        public float poisonDamage;
        public float poisonTimer = 2; // �� 2�ʸ��� �� ����
        float timer;

        protected virtual void Awake() {
            characterStatsManager = GetComponent<CharacterStatsManager>();
        }

        public virtual void PlayWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponFX != null) {
                    rightWeaponFX.PlayWeaponFX();
                }
            } else {
                if (leftWeaponFX != null) {
                    leftWeaponFX.PlayWeaponFX();
                }
            }
        }

        public virtual void StopWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponFX != null) {
                    rightWeaponFX.StopWeaponFX();
                }
            } else {
                if (leftWeaponFX != null) {
                    leftWeaponFX.StopWeaponFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation) {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }

        // �� ����
        protected virtual void HandlePoisonBuildUp() {
            if (isPoisoned) return; // �̹� �� ���¿� �ɷ��ִٸ� ���̻� �������� �ʴ´�
            if (poisonBuildUp > 0 && poisonBuildUp < 100) { // �� ���¿� �ɸ��� �ʾ����� �� ��ġ�� �����Ǿ� �ִٸ�
                poisonBuildUp -= Time.deltaTime;
            } else if (poisonBuildUp >= 100) { // �� ����ġ�� 100 �̻��̶��
                isPoisoned = true; // �� �����̻�
                poisonBuildUp = 0;
                
                if (buildUpTransform != null) {
                    currentPoisonedParticleFX = Instantiate(defaultPoisonedParticleFX, buildUpTransform.transform);
                } else {
                    currentPoisonedParticleFX = Instantiate(defaultPoisonedParticleFX, characterStatsManager.transform);
                }
            }
        }

        // �� �����̻�
        protected virtual void HandlePoisonedEffect() {
            if (isPoisoned) {
                if (poisonAmount > 0) {
                    timer += Time.deltaTime;

                    if (timer >= poisonTimer) {
                        characterStatsManager.TakePoisonDamage(poisonDamage);
                        timer = 0;
                    }
                    poisonAmount -= Time.deltaTime;
                } else {
                    isPoisoned = false;
                    poisonAmount = defaultPoisonAmount;
                    Destroy(currentPoisonedParticleFX);
                }
            }
        }

        // ��� ����/�����̻� ����
        public virtual void HandleAllBuildUpEffects() {
            if (characterStatsManager.isDead) return;
            HandlePoisonBuildUp();
            HandlePoisonedEffect();
        }
    }
}