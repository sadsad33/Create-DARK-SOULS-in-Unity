using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "World/WorldEvent/EventObject")]
    public class EventObject : ScriptableObject {

        public GameObject? modelPrefab; 
        public GameObject? instantiatedModel;

        public Vector3 eventObjectPosition;
        public Vector3 eventObjectRotation;
        public void ActivateEventObject() {
            //Debug.Log("오브젝트 활성화");
            instantiatedModel = Instantiate(modelPrefab);
            instantiatedModel.transform.position = eventObjectPosition;
            instantiatedModel.transform.rotation = Quaternion.Euler(eventObjectRotation);
        }

        public void DeactivateEventObject() {
            if (instantiatedModel != null) Destroy(instantiatedModel);
        }
    }
}
