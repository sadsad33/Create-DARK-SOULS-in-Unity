using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class MovingBlock : ActivatableObject {
        public Transform target;
        protected override void Update() {
            base.Update();
        }

        public override void Activation() {
            base.Activation();
            //Debug.Log("¿Ãµø");
            if (transform.position != target.position)
                transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime);
        }
    }
}