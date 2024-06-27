using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // �巡�� ���� - ������ ���õ� ��� ������ �ʱ�ȭ
        if (Input.GetMouseButtonDown(0))
        {
            gameManager.ClearSelectedApples();
            isDrag = true;
            dragStartPos = Input.mousePosition;
            dragStartPos.y = Screen.height - dragStartPos.y;
        }

        // �巡�� ���� - ���õ� ��� ������ ���
        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            gameManager.CalculateApples();
        }

        // �巡�� ���϶� ��ǥ ������Ʈ
        if (isDrag)
        {
            currentMousePos = Input.mousePosition;
            currentMousePos.y = Screen.height - currentMousePos.y;

            rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
            rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

            gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos);
            gameManager.ChangeSelectedApplesNumberColor(Color.blue);    // �Ķ������� ���� ���� ����
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
