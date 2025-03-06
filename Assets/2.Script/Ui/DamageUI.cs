using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    public float duration = 1.0f;
    public float horizontalDistance = 200f;
    public float peakHeight = 100f;

    private Text text;
    private RectTransform rectTransform;


    private void Start()
    {
        text = this.GetComponent<Text>();
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void PlayAnimation()
    {
        StartCoroutine(AnimateParabolic());
    }

    //UI 포물선으로 이동시키며 사라지게 하기
    private IEnumerator AnimateParabolic()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float time = 0.0f;
        SetAlpha(1.0f);

        while (time < duration)
        {
            float t = time / duration;

            float x = Mathf.Lerp(startPos.x, startPos.x + horizontalDistance, t);
            float y = startPos.y + 4 * peakHeight * t * (1 - t);
            rectTransform.anchoredPosition = new Vector2(x, y);

            SetAlpha(Mathf.Lerp(1f, 0f, t));

            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(startPos.x + horizontalDistance, startPos.y);
        SetAlpha(0.0f);
    }

    private void SetAlpha(float a)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, a);
    }
}
