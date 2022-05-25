using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    public GameManager gameManager;
    public Text timerText1;
    public Text timerText2;
    public Text timerText3;
    public float timer;

    // 게임 시작 시 플레이 시간을 화면에 출력.
    void Update()
    {
        if (!gameManager.gameStart || gameManager.startButton.activeSelf || gameManager.exitGame)
            return;

        timer += Time.deltaTime;
        timerText1.text = string.Format("{0:F1}", timer);
        timerText2.text = timerText3.text = Mathf.Round(timer).ToString() + "초간 생존하였습니다";
    }

}