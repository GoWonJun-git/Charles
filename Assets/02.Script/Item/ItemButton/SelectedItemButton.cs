using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemButton : MonoBehaviour
{
    private SelectedItemType selectedItemType;
    public PlayerManager playerManager;
    public GameManager gameManager;
    public int[] ItemType;
    public Sprite[] ItemImage;

    // 선택한 Item 정보를 가져와서 저장.
    void Start()
    {
        selectedItemType = GameObject.Find("SelectedItemType").GetComponent<SelectedItemType>();

        for (int i = 0; i < selectedItemType.itemType.Length; i++)
        {
            ItemType[i] = selectedItemType.itemType[i];
            ItemImage[i] = selectedItemType.itemImage[i];
            gameObject.transform.GetChild(i).GetComponent<Image>().sprite = ItemImage[i];

            if (ItemImage[i] == null)
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 저장한 Item 사용.
    public void ItemClick(Button button)
    {
        if (gameManager.gameStart)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).transform == button.transform)
                {
                    playerManager.myPlayerObject.GetItem(ItemType[i]);
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

}
