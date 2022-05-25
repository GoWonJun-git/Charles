using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerObject myPlayer;
    bool isTouch;
    Vector3 offset;
    Vector3 targetVec;

    public Camera uiCamera;
    public RectTransform targetRectTr;
    int moveTouchCount;
    Vector2 moveDragPosition;

    float axis = 0;
    Vector3 lastDirection;
    float rotateTimer = 0.0f;
    Vector3 rotateVector = Vector3.zero;
    Vector3 saveRotationVector = Vector3.zero;

    void Start() => offset = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

    void Update()
    {
        if (isTouch)
            OnDrag();
    }

    // 터치 시작 시.
    public void OnTouchDown()
    {
        if (myPlayer == null)
            return;

        isTouch = true;
    }

    // 터치 종료 시.
    public void OnTouchUp()
    {
        if (myPlayer == null)
            return;

        isTouch = false;
    }

    // 터치 중.
    public void OnDrag()
    {
        // Vector2 value;
        // if (Application.platform.Equals(RuntimePlatform.Android))
        //     value = (Vector3)Input.GetTouch(0).position - offset;
        // else
        //     value = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Application.platform == RuntimePlatform.Android && Input.touchCount > 0)
        {
            moveDragPosition = uiCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        }   
        else
            moveDragPosition = uiCamera.ScreenToWorldPoint(Input.mousePosition);

        if (moveDragPosition.x < myPlayer.mapSize[0]) moveDragPosition.x = myPlayer.mapSize[0];
        if (moveDragPosition.x > myPlayer.mapSize[1]) moveDragPosition.x = myPlayer.mapSize[1];
        if (moveDragPosition.y < myPlayer.mapSize[2]) moveDragPosition.y = myPlayer.mapSize[2];
        if (moveDragPosition.y > myPlayer.mapSize[3]) moveDragPosition.y = myPlayer.mapSize[3];
        
        Vector3 myPos = myPlayer.transform.position;
        Vector3 targetPos = moveDragPosition;
        targetPos.z = myPos.z;
        Vector3 vectorToTarget = targetPos - myPos;
        Vector3 quaternionToTarget = Quaternion.Euler(0, 0, axis) * vectorToTarget;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: quaternionToTarget);
        myPlayer.transform.rotation = Quaternion.RotateTowards(myPlayer.transform.rotation, targetRotation, 1000 * Time.deltaTime);
    
        if (myPlayer.isSlow)
            myPlayer.transform.position = Vector3.Lerp(myPlayer.transform.position, moveDragPosition, 0.02f);
        else
            myPlayer.transform.position = Vector3.Lerp(myPlayer.transform.position, moveDragPosition, 0.3f);
    }

}