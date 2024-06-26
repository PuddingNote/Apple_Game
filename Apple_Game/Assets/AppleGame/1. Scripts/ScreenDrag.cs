using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenDrag : MonoBehaviour
{
    private Vector2 currentMousePos;    // 실시간(현재 프레임) 마우스 좌표
    private Vector2 dragStartPos;       // 드래그 시작 지점 마우스 좌표
    private Vector2 rectMinPos;         // Rect의 최소 지점 좌표
    private Vector2 rectMaxPos;         // Rect의 최대 지점 좌표
    private bool isDrag = false;        // 드래그 여부

    public GameManager gameManager;

    private void Update()
    {
        // 드래그 시작 시 선택된 객체 리스트 초기화
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            dragStartPos.y = Screen.height - dragStartPos.y;
            isDrag = true;
            gameManager.ClearSelectedApples();  
        }
        // 드래그 종료 시 범위 내 사과 객체들 선택
        else if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos); 
        }

        // 드래그 중일때 좌표 업데이트
        if (isDrag)
        {
            currentMousePos = Input.mousePosition;
            currentMousePos.y = Screen.height - currentMousePos.y;

            rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
            rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);
        }
    }

    // 드래그 영역 그리기
    private void OnGUI()
    {
        if (!isDrag) return;

        Rect selectionRect = new Rect
        {
            min = rectMinPos,
            max = rectMaxPos
        };

        GUI.Box(selectionRect, "");
    }

}
