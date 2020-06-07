using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    [Header("UI")]
    public Transform UI;

    // 도감을 담을 상자
    GameObject[] bookTarget;

    // 도감 0성
    [Header("LV0")]
    public Transform star0;
    GameObject[] Lv0Book;

    // 도감 1성
    [Header("LV1")]
    public Transform star1;
    GameObject[] Lv1Book;

    // 도감 2성
    [Header("LV2")]
    public Transform star2;
    GameObject[] Lv2Book;

    // #. 도감 등록
    [Header("Book")]
    public Material setEffectMat;
    [SerializeField]
    Transform[] RegisterBookUI;
    Button RegisterBookExitBtn;
    Image CellImage;
    Transform CellStar;
    GameObject[] CellStarChild;
    Text CellName;
    Text CellExplain;

    // #. 도감에 표시할 셀 이미지
    [Header("Cell Image")]
    public Sprite[] CellSprite;

    public bool BMInit()
    {
        // #. 도감 등록시 판업 생성
        RegisterBookUI = new Transform[UI.GetChild(1).childCount];
        for (int i = 0; i < UI.GetChild(1).childCount; i++)
            RegisterBookUI[i] = UI.GetChild(1).GetChild(i);

        // #. 0성 도감 로딩
        Lv0Book = new GameObject[star0.childCount - 1];
        for (int i = 0; i < Lv0Book.Length; i++)
            Lv0Book[i] = star0.GetChild(i).gameObject;

        // #. 1성 도감 로딩
        Lv1Book = new GameObject[star1.childCount];
        for (int i = 0; i < Lv1Book.Length; i++)
            Lv1Book[i] = star1.GetChild(i).gameObject;

        // #. 2성 도감 로딩
        Lv2Book = new GameObject[star2.childCount];
        for (int i = 0; i < Lv2Book.Length; i++)
            Lv2Book[i] = star2.GetChild(i).gameObject;

        renewalBook(0, true);

        return true;
    }

    // 도감 등록
    public void BookValue(string name)
    {
        for (int i = 0; i < CellData.CellDataArray.GetLength(0); i++)
        {
            if (CellData.CellDataArray[i, 4] == name)
            {
                if (CellData.CellDataArray[i, 5] == "o")
                    break;
                else
                {
                    print(i);
                    setRegistserBookUI(i, true);
                    CellData.CellDataArray[i, 5] = "o";
                    renewalBook(i, false);
                    AllManager.AM.MM.MapRenewal(i);
                    BookSetEffect(i);
                    break;
                }
            }
        }
    }

    //도감 새로고침, 해당 세포 도감 활성화
    // start가 트루면 도감 데이터 셋팅함(이름 및 활성화 여부)
    public void renewalBook(int cellPos, bool start)
    {
        int cellLV;

        if (start)
        {
            // 세포 다른 별갯수 추가되면 여기 추가해야함~
            for (int i = 0; i < CellData.CellDataArray.GetLength(0); i++)
            {
                if (CellData.CellDataArray[i, 1] == "0")
                    Lv0Book[i].transform.GetChild(1).GetComponent<Text>().text = "???";

                if (CellData.CellDataArray[i, 1] == "1")
                    Lv1Book[i - 3].transform.GetChild(1).GetComponent<Text>().text = "???";

                if (CellData.CellDataArray[i, 1] == "2")
                    Lv2Book[i - 12].transform.GetChild(1).GetComponent<Text>().text = "???";

                if (CellData.CellDataArray[i, 5] == "o")
                {
                    cellLV = int.Parse(CellData.CellDataArray[i, 1]);

                    if (cellLV == 0) cellPos = int.Parse(CellData.CellDataArray[i, 0]) - 1;
                    else if (cellLV == 1) cellPos = int.Parse(CellData.CellDataArray[i, 0]) + 2;
                    else if (cellLV == 2) cellPos = int.Parse(CellData.CellDataArray[i, 0]) + 11;
                    setBookButtonOn(cellLV, cellPos);
                }
            }
            BookSetEffect();
            AllManager.AM.MM.MapRenewal();
            return;
        }

        cellLV = int.Parse(CellData.CellDataArray[cellPos, 1]);
        setBookButtonOn(cellLV, cellPos);
    }

    // 도감에 등록될시 해당 세포버튼 활성화
    void setBookButtonOn(int cellLV, int cellPos)
    {
        switch (cellLV)
        {
            case 0:
                bookTarget = Lv0Book;
                break;
            case 1:
                bookTarget = Lv1Book;
                break;
            case 2:
                bookTarget = Lv2Book;
                break;
            default:
                break;
        }

        cellPos = int.Parse(CellData.CellDataArray[cellPos, 0]) - 1;
        bookTarget[cellPos].transform.GetChild(2).gameObject.SetActive(false);

        if (bookTarget == Lv0Book) bookTarget[cellPos].transform.GetChild(1).GetComponent<Text>().text = CellData.CellDataArray[cellPos, 2];
        else if (bookTarget == Lv1Book) bookTarget[cellPos].transform.GetChild(1).GetComponent<Text>().text = CellData.CellDataArray[cellPos + 3, 2];
        else if (bookTarget == Lv2Book) bookTarget[cellPos].transform.GetChild(1).GetComponent<Text>().text = CellData.CellDataArray[cellPos + 12, 2];

        bookTarget[cellPos].GetComponent<Button>().interactable = true;
    }

    // #. 도감 판업 함수(이름, 새로 등록하는지 여부)
    public void setRegistserBookUI(int cellPos, bool newBook)
    {
        for (int i = 0; i < UI.GetChild(1).childCount; i++)
        {
            if (!RegisterBookUI[i].gameObject.activeSelf)
            {
                // 닫기 버튼 기능 부여
                RegisterBookExitBtn = RegisterBookUI[i].GetChild(6).GetComponent<Button>();

                RegisterBookExitBtn.onClick.AddListener(() =>
                {
                    RegisterBookUI[i].gameObject.SetActive(false);
                    SystemDB.OnUI = false;
                }
                );

                // 새로 등록된건지 도감에서 클릭한건지 구분
                if (newBook) RegisterBookUI[i].GetChild(1).GetChild(0).gameObject.SetActive(true);
                else RegisterBookUI[i].GetChild(1).GetChild(0).gameObject.SetActive(false);

                // 도감 데이터 처리
                CellImage = RegisterBookUI[i].GetChild(2).GetComponent<Image>();
                CellStar = RegisterBookUI[i].GetChild(3).GetComponent<Transform>();
                CellName = RegisterBookUI[i].GetChild(4).GetComponent<Text>();
                CellExplain = RegisterBookUI[i].GetChild(5).GetComponent<Text>();

                CellStarChild = new GameObject[4];
                bool check = false;

                for (int k = 0; k < CellStarChild.Length; k++)
                {
                    if (!check)
                    {
                        CellStarChild[k] = CellStar.GetChild(k).gameObject;
                        CellStarChild[k].SetActive(false);
                    }
                    else
                    {
                        CellStarChild[k] = CellStar.GetChild(k).gameObject;
                        CellStarChild[k].SetActive(true);
                    }

                    if (k == 3 && !check)
                    {
                        CellStarChild = new GameObject[int.Parse(CellData.CellDataArray[cellPos, 1])];
                        check = true;
                        k = -1;
                        continue;
                    }
                }

                CellImage.sprite = CellSprite[cellPos];
                CellImage.SetNativeSize();
                CellName.text = CellData.CellDataArray[cellPos, 2];
                CellExplain.text = CellData.CellDataArray[cellPos, 3];

                RegisterBookUI[i].gameObject.SetActive(true);
                SystemDB.OnUI = true;
                break;
            }
        }
    }

    // 도감 세트효과
    public void BookSetEffect(int cellPos)
    {
        // 등록 된 갯수를 체크해 100%면 세트효과 발휘
        // 또는 같은 종족이 다 모이면 세트효과 발휘
        int canSetEffect = 0;
        int breaksearch = 0; // 해당 종족검사 끝나면 종료
        string level = CellData.CellDataArray[cellPos, 1];

        if (level == "0")
        {
            for (int i = 0; i < Lv0Book.Length; i++)
            {
                if (CellData.CellDataArray[i, 5] == "o")
                    canSetEffect++;
            }

            // 세트효과 발동
            if (canSetEffect == 3)
            {
                for (int i = 0; i < Lv0Book.Length; i++)
                    Lv0Book[i].GetComponent<SpriteRenderer>().material = setEffectMat;

                star0.GetChild(3).GetComponent<Text>().text = SystemDB.setEffect[1];
            }
        }
        else if (level == "1")
        {
            // 메인 종족 분리
            int cut = CellData.CellDataArray[cellPos, 4].IndexOf("_");
            string group = CellData.CellDataArray[cellPos, 4].Substring(cut + 1, 1);

            for (int i = 3; i < Lv1Book.Length + 3; i++)
            {
                // 서브 종족 분리
                string subGroup = CellData.CellDataArray[i, 4].Substring(cut + 1, 1);

                if (group != subGroup && i < 8)
                {
                    i += 2;
                }
                else if(group == subGroup)
                {
                    if (CellData.CellDataArray[i, 5] == "o")
                        canSetEffect++;

                    if (canSetEffect == 3)
                    {
                        print(group + " 계열 세트 효과 발동, 현재 세트 효과 수 : " + CellData.level1SetCount);
                        if (group == "r") cut = 0;
                        else if (group == "y") cut = 3;
                        else if (group == "w") cut = 6;

                        for (int j = cut; j < cut + 3; j++)
                            Lv1Book[j].GetComponent<SpriteRenderer>().material = setEffectMat;

                        star1.parent.parent.GetChild(2).GetComponent<Text>().text = SystemDB.setEffect[CellData.level1SetCount];
                      
                        if (CellData.level1SetCount < 2)
                            CellData.level1SetCount++;

                        break;
                    }

                    breaksearch++;

                    if (breaksearch == 3)
                        break;
                }
            }
        }
    }

    // 도감 로드시 초기화용
    public void BookSetEffect()
    {
        // 등록 된 갯수를 체크해 100%면 세트효과 발휘
        // 또는 같은 종족이 다 모이면 세트효과 발휘
        int canSetEffect = 0;

        for (int i = 0; i < CellData.CellDataArray.GetLength(0); i++)
        {
            string level = CellData.CellDataArray[i, 1];

            if (level == "0")
            {
                if (CellData.CellDataArray[i, 5] == "o")
                    canSetEffect++;
                               
                // 세트효과 발동
                if (canSetEffect == 3)
                {
                    for (int j = 0; j < Lv0Book.Length; j++)
                        Lv0Book[j].GetComponent<SpriteRenderer>().material = setEffectMat;

                    star0.GetChild(3).GetComponent<Text>().text = SystemDB.setEffect[0];
                    canSetEffect = 0;
                }
            }
            else if (level == "1")
            {
                if (CellData.CellDataArray[i, 5] == "o" &&
                   CellData.CellDataArray[i + 1, 5] == "o" &&
                   CellData.CellDataArray[i + 2, 5] == "o")
                {
                    for (int k = i - 3; k < i; k++)
                    {
                        Lv1Book[k].GetComponent<SpriteRenderer>().material = setEffectMat;
                        star1.parent.parent.GetChild(2).GetComponent<Text>().text = SystemDB.setEffect[CellData.level1SetCount];
                    }
                    if (CellData.level1SetCount < 3)
                        CellData.level1SetCount++;
                }
                if (i < 9)
                    i += 2;
            }
        }
    }

    // #. 도감창에서 세포를 누를시
    public void setBookBtn(int cellDataPos) => setRegistserBookUI(cellDataPos, false);

}
