using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MapManager : MonoBehaviour
{
    #region Tab 0 + 0
    [TabGroup("0+0")]
    [Header("Red")]
    public Transform[] output00R;

    [TabGroup("0+0")]
    [Header("Yellow")]
    public Transform[] output00Y;

    [TabGroup("0+0")]
    [Header("White")]
    public Transform[] output00W;
    #endregion

    #region Tab 1 + 1
    [TabGroup("1+1")]
    [Header("Red")]
    public Transform[] input11R;
    [TabGroup("1+1")]
    public Transform[] output11R;

    [TabGroup("1+1")]
    [Header("Yellow")]
    public Transform[] input11Y;
    [TabGroup("1+1")]
    public Transform[] output11Y;

    [TabGroup("1+1")]
    [Header("White")]
    public Transform[] input11W;
    [TabGroup("1+1")]
    public Transform[] output11W;
    #endregion

    [Header("Resource")]
    public Sprite[] CellSprite;

    public void MapRenewal(int cellPos)
    {
        int level = int.Parse(CellData.CellDataArray[cellPos, 1]);

        if (level < 1)
            return;

        int cutLast = CellData.CellDataArray[cellPos, 4].Length;
        int arrayPos = int.Parse(CellData.CellDataArray[cellPos, 4].Substring(cutLast - 1, 1)) - 1;

        int cut = CellData.CellDataArray[cellPos, 4].IndexOf("_");
        string color = CellData.CellDataArray[cellPos, 4].Substring(cut + 1, 1);

        if (color == "r")
        {
            if (level == 1)
            {
                output00R[arrayPos].gameObject.SetActive(false);
                input11R[arrayPos].gameObject.SetActive(false);
                input11R[arrayPos + 3].gameObject.SetActive(false);
            }
            else if(level == 2)
            {
                output11R[arrayPos].gameObject.SetActive(false);
            }
        }
        else if (color == "y")
        {
            if (level == 1)
            {
                output00Y[arrayPos].gameObject.SetActive(false);
                input11Y[arrayPos].gameObject.SetActive(false);
                input11Y[arrayPos + 3].gameObject.SetActive(false);
            }
            else if (level == 2)
            {
                output11Y[arrayPos].gameObject.SetActive(false);
            }
        }
        else if (color == "w")
        {
            if (level == 1)
            {
                output00W[arrayPos].gameObject.SetActive(false);
                input11W[arrayPos].gameObject.SetActive(false);
                input11W[arrayPos + 3].gameObject.SetActive(false);
            }
            else if (level == 2)
            {
                output11W[arrayPos].gameObject.SetActive(false);
            }
        }

    }

    // 초기화용
    public void MapRenewal()
    {
        int level, cutLast, arrayPos, cut;
        string color;

        for (int i = 3; i < CellData.CellDataArray.GetLength(0); i++)
        {
            level = int.Parse(CellData.CellDataArray[i, 1]);

            cutLast = CellData.CellDataArray[i, 4].Length;
            arrayPos = int.Parse(CellData.CellDataArray[i, 4].Substring(cutLast - 1, 1)) - 1;

            cut = CellData.CellDataArray[i, 4].IndexOf("_");
            color = CellData.CellDataArray[i, 4].Substring(cut + 1, 1);

            if (CellData.CellDataArray[i, 5] == "o")
            {
                if (level == 1)
                {
                    if (color == "r")
                    {
                        output00R[arrayPos].gameObject.SetActive(false);
                        input11R[arrayPos].gameObject.SetActive(false);
                        input11R[arrayPos + 3].gameObject.SetActive(false);
                    }
                    else if (color == "y") 
                    {
                        output00Y[arrayPos].gameObject.SetActive(false);
                        input11Y[arrayPos].gameObject.SetActive(false);
                        input11Y[arrayPos + 3].gameObject.SetActive(false);
                    } 
                    else if (color == "w")
                    {
                        output00W[arrayPos].gameObject.SetActive(false);
                        input11W[arrayPos].gameObject.SetActive(false);
                        input11W[arrayPos + 3].gameObject.SetActive(false);
                    }                   
                }
                else if (level == 2) { }

            }
        }
    }
}
