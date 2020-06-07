
using UnityEngine;
using UnityEngine.UI;

class CellInfo
{
    public int slotId;
    public string name;
    public Sprite cellImg;
    public int starCount;
    public int cellPos;
}

public class slotManager : MonoBehaviour
{
    CellInfo cellinfo;
    public static Button useBtn;


    private void Start()
    {
        useBtn = transform.GetChild(3).GetComponent<Button>();
        useBtn.onClick.AddListener(() => useCell());
    }

    public void setCellInfo(int slotID, string name, Sprite cellImg, int starCount, int cellPos)
    {
        cellinfo = new CellInfo();

        cellinfo.slotId = slotID;
        cellinfo.name = name;
        cellinfo.cellImg = cellImg;
        cellinfo.starCount = starCount;
        cellinfo.cellPos = cellPos;
        setObj();
    }

    public void setObj()
    {
       var starObj = transform.GetChild(0).gameObject;


        for (int i = 0; i < starObj.transform.childCount; i++)
            starObj.transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i <= cellinfo.starCount; i++)
        {
            if (cellinfo.starCount == 0)
            {
                starObj.transform.GetChild(0).gameObject.SetActive(true);
                break;
            }

            i++;
            starObj.transform.GetChild(i).gameObject.SetActive(true);

        }

        transform.GetChild(1).GetComponent<Image>().sprite = cellinfo.cellImg;
        transform.GetChild(2).GetComponent<Text>().text = cellinfo.name;

        gameObject.SetActive(true);

    }

    public void useCell()
    {
        if (CellMakeManager.CMM.throwCell(cellinfo.cellPos, cellinfo.starCount))
            gameObject.SetActive(false);
    }
}
