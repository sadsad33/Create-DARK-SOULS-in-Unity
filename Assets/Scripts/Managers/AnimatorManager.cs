using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AnimatorManager : MonoBehaviour {
        public Animator anim;
        public bool canRotate;

        // �ش� �ִϸ��̼��� �����Ѵ�.
        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false) {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", canRotate);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.3f);
        }

        public virtual void TakeCriticalDamageAnimationEvent() {

        } 
    }
}