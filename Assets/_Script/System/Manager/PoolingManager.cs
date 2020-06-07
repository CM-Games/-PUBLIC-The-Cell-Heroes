using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolingManager : MonoBehaviour
{
    // 싱글톤
    public static PoolingManager PM = null;

    [Header("Cell")]
    const int maxCell = 50;
    public Transform cellPos;
    public Transform cellParent;
    public GameObject[] cellLv0;
    public GameObject[] cellLv1;
    public GameObject[] cellLv2;

    float x_max;
    float x_min;
    float y_max;
    float y_min;

    GameObject[,] cellLv0_Pool;
    GameObject[,] cellLv1_Pool;
    GameObject[,] cellLv2_Pool;

    GameObject[,] targetPool;

    // 초기화
    private void Awake()
    {
        // # 싱글톤
        PM = this;

        // # 세포 풀링 할 공간
        cellLv0_Pool = new GameObject[cellLv0.Length, maxCell];
        cellLv1_Pool = new GameObject[cellLv1.Length, maxCell];
        cellLv2_Pool = new GameObject[cellLv2.Length, maxCell];

        // # 세포 생성 좌표 설정
        x_max = cellPos.GetChild(0).transform.position.x;
        x_min = cellPos.GetChild(1).transform.position.x;
        y_max = cellPos.GetChild(2).transform.position.y;
        y_min = cellPos.GetChild(3).transform.position.y;

    }


    // 세포 생성 후 풀링
    public bool Generate()
    {
        for (int i = 0; i < cellLv0.Length; i++)
        {
            for (int j = 0; j < maxCell; j++)
            {
                cellLv0_Pool[i, j] = Instantiate(cellLv0[i], cellParent.transform.GetChild(0));
                cellLv0_Pool[i, j].name = cellLv0[i].name + "|" + j;
                cellLv0_Pool[i, j].SetActive(false);
            }
        }

        for (int i = 0; i < cellLv1.Length; i++)
        {
            for (int j = 0; j < maxCell; j++)
            {
                cellLv1_Pool[i, j] = Instantiate(cellLv1[i], cellParent.transform.GetChild(1));
                cellLv1_Pool[i, j].name = cellLv1[i].name + "|" + j;
                cellLv1_Pool[i, j].SetActive(false);
            }
        }

        for (int i = 0; i < cellLv2.Length; i++)
        {
            for (int j = 0; j < maxCell; j++)
            {
                cellLv2_Pool[i, j] = Instantiate(cellLv2[i], cellParent.transform.GetChild(2));
                cellLv2_Pool[i, j].name = cellLv2[i].name + "|" + j;
                cellLv2_Pool[i, j].SetActive(false);
            }
        }
        return true;
    }

    // 다른 스크립트에서 오브잭트생성 함수
    public void MakeObj(string type, string color, string cellType = "")
    {
        switch (type)
        {
            case "Lv0":
                RandomCell(cellLv0_Pool, cellLv0.Length, color, "0", cellType);
                break;
            case "Lv1":
                RandomCell(cellLv1_Pool, cellLv1.Length, color, "1", cellType);
                break;
            case "Lv2":
                RandomCell(cellLv2_Pool, cellLv2.Length, color, "2", cellType);
                break;
            default:
                break;
        }
    }

    // max 까지 종류에서 랜덤
    // 색, 생성 레벨
    void RandomCell(GameObject[,] Cell, int max, string color, string level, string cellType)
    {
        int random = 0;

        // 세포 데이터에서 받아온 레벨,색깔 조합해서 시작, 끝점 알아냄
        int colorStart = 0;
        int colorEnd = 0;
        bool pass = false;

        int j = 0;

        if (level == "1") j = 3;
        else if (level == "2") j = 12;

        if (color == "") random = Random.Range(0, max);
        else if(cellType != "")
        {
            print(cellType);
            for (; j < CellData.CellDataArray.GetLength(0); j++)
            {
                if (CellData.CellDataArray[j, 4].Substring(4) == cellType)
                    random = int.Parse(CellData.CellDataArray[j, 0])-1;
            }
            print(random);
        }
        else
        {
            for ( ;j < CellData.CellDataArray.GetLength(0) ; j++)
            {
                // 최초로 색깔과 레벨이 같음 - 시작구간
                if (CellData.CellDataArray[j, 4].Substring(4, 1) == color
                    && CellData.CellDataArray[j, 1] == level && !pass)
                {
                    colorStart = int.Parse(CellData.CellDataArray[j, 0]) - 1;
                    pass = true;
                }

                //// 시작구간 이후 색이 다르고 레벨이 다름 - 끝구간
                //if ((CellData.CellDataArray[j, 4].Substring(4, 1) != color
                //    || CellData.CellDataArray[j, 1] != level || j == CellData.CellDataArray.GetLength(0) -1) && pass)
                //{
                //    colorEnd = int.Parse(CellData.CellDataArray[j, 0]) - 1;
                //    break;
                //}
            }
            colorEnd = colorStart + 3;
            print(color);
            print(colorStart + "\\" + colorEnd);
            random = Random.Range(colorStart, colorEnd);
        }

        

        for (int i = 0; i < Cell.GetLength(1); i++)
        {
            if (!Cell[random, i].activeSelf)
            {
                // ( 시작지점을 찾아 (Clone)자르고 세포의 이름을 가져옴
                int textEnd = Cell[random, i].name.IndexOf("|");

                // 도감 등록
                AllManager.AM.BM.BookValue(Cell[random, i].name.Substring(0, textEnd));
                Cell[random, i].SetActive(true);

                // 세포 생성, 유저 정보UI 좌표에 나오면 다시 뽑음
                float x = Random.Range(x_min, x_max);
                float y = Random.Range(y_min, y_max);

                Cell[random, i].transform.position = new Vector2(x, y);
                User.UserCellDeck.Add(Cell[random, i]);
                break;
            }
        }
    }

    // 로드된 세포
    // 0성 = 0, 1성 = 3, 2성 = 12
    public void LoadCell(string[,] loadData)
    {
        // # 세포 정보 가져왔는지 판단
        bool pass = false;

        // # 세포 번호
        int cellNum = 0;

        // # 세포 정보 처리
        for (int j = 0; j < loadData.GetLength(0); j++)
        {
            for (int i = 0; i < CellData.CellDataArray.GetLength(0); i++)
            {
                // # 세포가 몇성인지 판단
                if (!pass)
                {
                    if (loadData[j, 3] == "0")
                    {
                        i = 0;
                        targetPool = cellLv0_Pool;
                    }
                    else if (loadData[j, 3] == "1")
                    {
                        i = 3;
                        targetPool = cellLv1_Pool;
                    }
                    else if (loadData[j, 3] == "2")
                    {
                        i = 12;
                        targetPool = cellLv2_Pool;
                    }

                    pass = true;
                }

                // # 세포 도감 등록 처리 하고 화면에 세포 출력
                if (loadData[j, 0] == CellData.CellDataArray[i, 4])
                {
                    CellData.CellDataArray[i, 5] = "o";
                    cellNum = int.Parse(CellData.CellDataArray[i, 0]) - 1;

                    for (int k = 0; k < targetPool.GetLength(1); k++)
                    {
                        if (!targetPool[cellNum, k].activeSelf)
                        {
                            targetPool[cellNum, k].transform.position = new Vector3(
                             float.Parse(loadData[j, 1]),
                             float.Parse(loadData[j, 2]),
                             0);
                            targetPool[cellNum, k].SetActive(true);
                            User.UserCellDeck.Add(targetPool[cellNum, k].gameObject);
                            break;
                        }
                    }
                    pass = false;
                    break;
                }
            }
        }

        // 도감 새로고침
        AllManager.AM.BM.renewalBook(0, true);
    }
}
