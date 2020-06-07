using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DotweenAni : MonoBehaviour
{
    public static DotweenAni instance;

    Sequence mySequence;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (CompareTag("UIPopUp"))
        {
            mySequence = DOTween.Sequence()
           .SetAutoKill(false) //추가
           .OnStart(() =>
           {
               transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
               GetComponent<Image>().color = new Color32(255, 255, 255, 0);
           })
           .Append(transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce))
           .Join(GetComponent<Image>().DOFade(1, 0.5f));
        }

        if (CompareTag("UICellPopUp"))
        {
            mySequence = DOTween.Sequence()
          .SetAutoKill(false) //추가
          .OnStart(() =>
          {
              transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
          })
          .Append(transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce));
        }


        if (CompareTag("UILoading"))
        {
            transform.DORotate(new Vector3(0, 0, -360), 13f, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear)
                     .SetLoops(-1);
        }
    }

    void OnEnable()
    {
        if (CompareTag("UIPopUp") || CompareTag("UICellPopUp"))
            mySequence.Restart();
    }

    public void killme()
    {
        DOTween.Clear();
    }
}
