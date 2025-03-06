using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    [Header("설정 값")]
    public float scrollSpeed = 2f;       // 패널 이동 속도
    public float panelWidth = 20f;       // 각 패널의 너비
    public Transform[] panels;

    void Update()
    {
        // 모든 패널을 왼쪽으로 이동
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        if (panels[0].position.x <= -panelWidth)
        {
            Transform firstPanel = panels[0];
            Transform lastPanel = panels[panels.Length - 1];

            firstPanel.position = new Vector3(lastPanel.position.x + panelWidth,
                                              firstPanel.position.y,
                                              firstPanel.position.z);

            List<Transform> temp = new List<Transform>(panels);
            temp.RemoveAt(0);
            temp.Add(firstPanel);
            panels = temp.ToArray();
        }
    }
}
