using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ScrollScript : ScrollRect
{
    bool forParent;
    public bool isDrag;
    ScrollManager SM;
    ScrollRect parentScrollRect;

    protected override void Start()
    {
        SM = AllManager.AM.ScrollM;
        parentScrollRect = AllManager.AM.SR;

        if (name == "star2Scroll View")
        {
            SM = AllManager.AM.ScrollMMap;
            parentScrollRect = AllManager.AM.SRMap;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if (forParent)
        {
            SM.OnBeginDrag(eventData);
            parentScrollRect.OnBeginDrag(eventData);
        }
        else base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            SM.OnDrag(eventData);
            parentScrollRect.OnDrag(eventData);
        }
        else
        {
            isDrag = true;
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            SM.OnEndDrag(eventData);
            parentScrollRect.OnEndDrag(eventData);
        }
        else
        {
            isDrag = false;
            base.OnEndDrag(eventData);
        }
    }
}

