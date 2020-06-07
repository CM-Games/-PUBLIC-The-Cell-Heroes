using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CellMakeManager : MonoBehaviour
{
    public Transform[] cellCardPos;
    public Sprite[] cellImg;
    public slotManager[] CellSlot;
    public GameObject[] throwSlot;
    public Text starCost;

    public int currentStarCost;
    float generatorTime;

    public static CellMakeManager CMM;

    private void Awake()
    {
        CMM = this;
        generatorTime = 0;
        currentStarCost = 0;
        //SystemDB.DungeonCellDeck = new List<string>();

        //SystemDB.DungeonCellDeck.Add("lv1_red1");
        //SystemDB.DungeonCellDeck.Add("lv2_red2");
        //SystemDB.DungeonCellDeck.Add("lv1_white1");
        //SystemDB.DungeonCellDeck.Add("lv2_red1");
        User.starCost = 8;

    }

    private void Update()
    {
        generatorTime += Time.deltaTime;
        starCost.text = currentStarCost + "/" + User.starCost;

        if (generatorTime > 3)
        {
            setSlot(Random.Range(0, SystemDB.DungeonCellDeck.Count));
            generatorTime = 0;
        }
    }

    public void setSlot(int cell)
    {
        for (int i = 0; i < CellSlot.Length; i++)
        {
            if (!CellSlot[i].gameObject.activeSelf)
            {
                CellSlot[i].transform.position = cellCardPos[i].position;
                for (int j = 0; j < CellData.CellDataArray.GetLength(0); j++)
                {
                    if (CellData.CellDataArray[j, 4] == SystemDB.DungeonCellDeck[cell])
                    {
                        CellSlot[i].setCellInfo(i,
                            CellData.CellDataArray[j, 2],
                            cellImg[j],
                            int.Parse(CellData.CellDataArray[j, 1]), j);
                        break;
                    }
                }
                break;
            }
        }
    }

    public bool throwCell(int cellPos, int starCount)
    {
        for (int i = 0; i < throwSlot.Length; i++)
        {
            if (!throwSlot[i].activeSelf)
            {
                currentStarCost += starCount;

                if (currentStarCost > User.starCost)
                {
                    currentStarCost -= starCount;
                    return false;

                }

                throwSlot[i].GetComponent<Image>().sprite = cellImg[cellPos];
                throwSlot[i].transform.localPosition = new Vector3(-173, Random.Range(-44, 13), 0);
                throwSlot[i].GetComponent<MyCellManager>().setStarCost(starCount);
                throwSlot[i].SetActive(true);
                break;
            }
        }
        return true;
    }
}
