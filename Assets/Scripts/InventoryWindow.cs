using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : MonoBehaviour
{
    public GameObject[] allInventoryWindows;
    public int currentIndex = 0;
    private void Awake() {
        PrintInventoryWindow();
    }

    public void PrintInventoryWindow() {
        for (int i = 0; i < allInventoryWindows.Length; i++) {
            allInventoryWindows[i].SetActive(false);
        }
        allInventoryWindows[currentIndex].SetActive(true);
    }

    public void NextPage() {
        if (currentIndex == allInventoryWindows.Length - 1) currentIndex = 0;
        else currentIndex += 1;

        PrintInventoryWindow();
    }

    public void PrevPage() {
        if (currentIndex == 0) currentIndex = allInventoryWindows.Length - 1;
        else currentIndex -= 1;

        PrintInventoryWindow();
    }
}
