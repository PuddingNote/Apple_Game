using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenDrag : MonoBehaviour
{
    private Vector2 currentMousePos;    // �ǽð�(���� ������) ���콺 ��ǥ
    private Vector2 dragStartPos;       // �巡�� ���� ���� ���콺 ��ǥ
    private Vector2 rectMinPos;         // Rect�� �ּ� ���� ��ǥ
    private Vector2 rectMaxPos;         // Rect�� �ִ� ���� ��ǥ
    private bool isDrag = false;        // �巡�� ����


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
