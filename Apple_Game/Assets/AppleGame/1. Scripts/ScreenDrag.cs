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


    private void Update()
    {
        isDrag = Input.GetMouseButton(0);
        if (!isDrag) return;

        currentMousePos = Input.mousePosition;
        currentMousePos.y = Screen.height - currentMousePos.y;

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = currentMousePos;
        }

        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);
    }

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
