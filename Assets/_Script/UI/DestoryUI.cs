using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestoryUI : MonoBehaviour
{
    string obj;

    private void Awake()
    {
        obj = transform.tag;
    }

    void Update()
    {
        switch (obj)
        {
            case "UIState":
                if (gameObject.activeSelf)
                    if (GetComponent<Text>().color.a ==0)
                        gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
