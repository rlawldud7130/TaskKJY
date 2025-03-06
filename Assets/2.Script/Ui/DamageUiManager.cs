using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUiManager : MonoBehaviour
{
    private GameObject[] damageUiList;
    private int rectIndex = 0;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        damageUiList = new GameObject[this.transform.childCount];
        for(int i = 0; i < this.transform.childCount; i++)
        {
            damageUiList[i] = this.transform.GetChild(i).gameObject;
        }
    }

    //데미지 입힐때마다 호출되기
    public void DamageUI(Vector2 objPosition, float damage)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(objPosition);
        damageUiList[rectIndex].GetComponent<RectTransform>().position = screenPos;
        damageUiList[rectIndex].GetComponent<DamageUI>().PlayAnimation();

        rectIndex++;
        if(rectIndex >= damageUiList.Length)
            rectIndex = 0;
    }
}
