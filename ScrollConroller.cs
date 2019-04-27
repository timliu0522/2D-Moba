using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollConroller : ScrollRect
{
    public bool isMoving = false;

    private float radius = 0f;
    private float[] messages = new float[2];
    private object endControl;

    protected override void Start()
    {
        base.Start();
        radius = (transform as RectTransform).sizeDelta.x * 0.5f;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        Vector2 contentPostion = this.content.anchoredPosition;
        if (contentPostion.magnitude > radius)
        {
            contentPostion = contentPostion.normalized * radius;
            SetContentAnchoredPosition(contentPostion);
            messages[0] = contentPostion.x / contentPostion.magnitude;
            messages[1] = contentPostion.y / contentPostion.magnitude;
            isMoving = true;
            GameObject.FindWithTag("MainCamera").SendMessage("GetValue", messages);//这里我控制相机，所以给相机发送消息
            // Debug.Log("X: "+ contentPostion.x / contentPostion.magnitude + "Y: "+ contentPostion.y / contentPostion.magnitude);
        }
    }
    //复写OnEndDrag方法
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        isMoving = false;
    }
}
