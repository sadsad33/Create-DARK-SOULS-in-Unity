using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class StatSelectUI : MonoBehaviour {
        public Button[] downButtonSet, upButtonSet;
        public Text[] statTextSet;
        private void Awake() {
            Transform downBtnsParent = transform.GetChild(1);
            Transform upBtnsParent = transform.GetChild(3);
            Transform statTextsParent = transform.GetChild(2);

            //downButtonSet = new Button[downBtnsParent.childCount];
            //upButtonSet = new Button[upBtnsParent.childCount];
            //statTextSet = new Text[statTextsParent.childCount];

            downButtonSet = new Button[3];
            upButtonSet = new Button[3];
            statTextSet = new Text[3];

            for (int i = 0; i < downButtonSet.Length; i++) {
                downButtonSet[i] = downBtnsParent.GetChild(i).GetComponent<Button>();
                downButtonSet[i].GetComponent<LevelUpButton>().btnIndex = i;
                downButtonSet[i].GetComponent<LevelUpButton>().isUpperDirection = false;

                upButtonSet[i] = upBtnsParent.GetChild(i).GetComponent<Button>();
                upButtonSet[i].GetComponent<LevelUpButton>().btnIndex = i;
                upButtonSet[i].GetComponent<LevelUpButton>().isUpperDirection = true;

                statTextSet[i] = statTextsParent.GetChild(i).GetComponent<Text>();
                downButtonSet[i].GetComponent<LevelUpButton>().stat = statTextSet[i];
                upButtonSet[i].GetComponent<LevelUpButton>().stat = statTextSet[i];
            }
        }
    }
}