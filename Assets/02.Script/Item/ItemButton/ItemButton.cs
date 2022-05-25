using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public int count;
    public GameObject ItemBox; // 선택할 Item Image Box.
    public GameObject selectedItemBox;  // 선택한 Item Image Box.
    public SelectedItemType selectedItemType; // 선택한 Item Type.

    // Item 선택시 Selected ItemBox에 같은 Item을 저장.
    public void ItemClick(Button button)
    {
        switch (button.name)
        {
            case "Slow":
                selectedItemType.itemType[count] = 0;
                selectedItemType.itemImage[count] = button.gameObject.GetComponent<Image>().sprite;
                break;
            case "Shiled":
                selectedItemType.itemType[count] = 1;
                selectedItemType.itemImage[count] = button.gameObject.GetComponent<Image>().sprite;
                break;
            case "MachineGun":
                selectedItemType.itemType[count] = 2;
                selectedItemType.itemImage[count] = button.gameObject.GetComponent<Image>().sprite;
                break;
            case "Fire":
                selectedItemType.itemType[count] = 3;
                selectedItemType.itemImage[count] = button.gameObject.GetComponent<Image>().sprite;
                break;
        }
        selectedItemBox.transform.GetChild(count).GetComponent<Image>().sprite = button.gameObject.GetComponent<Image>().sprite;

        if (count >= 2)
            count = 0;
        else 
            count++;
    }

}
