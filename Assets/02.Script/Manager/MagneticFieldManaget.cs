using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticFieldManaget : MonoBehaviour
{
    public PlayerManager playerManager;
    public GameObject[] magneticField;

    // 게임 시작 시 일정 시간마다 자기장(맵 제한)을 발생.
    public void MF_Start() => StartCoroutine(MagneticFieldSeting());
    IEnumerator MagneticFieldSeting()
    {
        for (int i = 0; i < magneticField.Length; i++)
        {
            yield return new WaitForSeconds(37f);
            GameObject t = Instantiate(Resources.Load<GameObject>("Text/MagneticFieldText"));
            Destroy(t, 2f);

            yield return new WaitForSeconds(3f);
            magneticField[i].SetActive(true);
            playerManager.myPlayerObject.MapDownSizing();
        }
    }

}
