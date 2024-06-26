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

    public GameManager gameManager;

    private void Update()
    {
        // �巡�� ���� �� ���õ� ��ü ����Ʈ �ʱ�ȭ
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            dragStartPos.y = Screen.height - dragStartPos.y;
            isDrag = true;
            gameManager.ClearSelectedApples();  
        }
        // �巡�� ���� �� ���� �� ��� ��ü�� ����
        else if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos); 
        }

        // �巡�� ���϶� ��ǥ ������Ʈ
        if (isDrag)
        {
            currentMousePos = Input.mousePosition;
            currentMousePos.y = Screen.height - currentMousePos.y;

            rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
            rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);
        }
    }

    // �巡�� ���� �׸���
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
