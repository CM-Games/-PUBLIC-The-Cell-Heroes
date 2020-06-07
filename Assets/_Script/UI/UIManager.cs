using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    // #. 유저 데이터
    [Header("User Data")]
    public Text goldText;
    public GameObject SelectCharactorUI;
    public Transform charactorPage;
    public Image charactorBG;
    public Transform PlayerObj;
    public Transform PlayerCharInfoObj;
    public Image[] currentPageImg;
    public Sprite[] charImage;
    Image playerCharInfoImg;
    Image PlayerImg;
    int charactorPageXpos;
    int currentPage;

    // #. 상태 표시 텍스트
    [Header("State")]
    public Text[] StateText;
    public Text cellPay;

    // #. 도감
    [Header("Book")]
    public GameObject[] bookObj;
    public GameObject starObj;
    public Scrollbar lv1SV;
    public Scrollbar lv2SV;
    int bookPage = 0;
    bool bookPageInit = false;

    // #. 던전
    [Header("Dungeon")]
    public GameObject CellSelectUI;
    public RectTransform cellSelectRT;
    public Scrollbar cellSelectScrollBar;
    int cellRTWidth;

    // #. 상점
    [Header("Shop")]
    public Transform[] SkillCell;

    // #. 모든 버튼
    [Header("Button")]
    public Button[] AllButton;

    // 셋팅
    [Header("Setting")]
    public GameObject creaditUI;

    [Header("Map")]
    public Transform UI;
    public Transform MainScroll;
    public Transform MapUI;
    public Transform CellObj;


    public bool UMInit()
    {
        for (int i = 0; i < StateText.Length; i++)
            StateText[i].gameObject.SetActive(false);

        charactorBG = SelectCharactorUI.transform.GetChild(0).GetComponent<Image>();
        charactorPageXpos = 1080;
        currentPage = 0;

        PlayerImg = PlayerObj.GetChild(3).GetComponent<Image>();
        playerCharInfoImg = PlayerCharInfoObj.GetChild(1).GetComponent<Image>();

       
        return true;
    }

    private void Update()
    {
        // # 도감 페이지 외 다른곳 다녀오면 도감 0성으로 초기화
        if (AllManager.AM.ScrollM.targetIndex == 2 && !bookPageInit)
        {
            bookPageInit = true;
            bookPage = 0;
            setBookPage(0);
        }
        else if (AllManager.AM.ScrollM.targetIndex != 2) bookPageInit = false;
    }

    // # 유저 데이터 업데이트
    public void setUserMoney() => goldText.text = User.money.ToString() + "K";

    // 상태를 나타내는 텍스트 설정
    public void setStateText(string value)
    {
        for (int i = 0; i < StateText.Length; i++)
        {
            if (!StateText[i].gameObject.activeSelf)
            {
                StateText[i].text = value;
                StateText[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    public void buttonOff(int num)
    {
        if (num == 1)
            for (int i = 0; i < AllButton.Length; i++)
                AllButton[i].interactable = false;
        else if (num == 0)
            for (int i = 0; i < AllButton.Length; i++)
                AllButton[i].interactable = true;
    }

    public void LoadingUIOpen()
    {
        AllManager.AM.GM.Loading.gameObject.SetActive(true);
        AllManager.AM.GM.resetLoadingBarFill();
    }

    /**
    *  #################################################
    *
    *                      세포
    * 
    *  #################################################
    */

      public void setCellPayText(string price)
    {
        cellPay.text = price;
    }

    /**
    *  #################################################
    *
    *               유저 캐릭터 선택 파트
    * 
    *  #################################################
    */
    #region "유저 캐릭터 선택"
    public void startSeletingChar() => SelectCharactorUI.SetActive(true);
    public void setUserCharMini(int currentPage) => PlayerImg.sprite = charImage[currentPage];
    // 저장 후 유저 캐릭터 창 셋팅 및 창 종료
    public void offSeletingChar()
    {
        PlayerImg.sprite = charImage[currentPage];
        onPlayerCharInfoUI(currentPage);
        SelectCharactorUI.SetActive(false);
    }

    public void SaveUserChar() => AllManager.AM.DM.SaveUserChar(currentPage);

    public void changeCharPage(int direction)
    {
        // 0 왼쪽 , 1 오른쪽
        if (direction == 0)
        {
            if (charactorPageXpos != 1080)
            {
                charactorPageXpos += 1080;
                charactorPage.DOLocalMoveX(charactorPageXpos, 1f).SetEase(Ease.OutQuart);
                currentPage--;
            }
        }
        else if (direction == 1)
        {
            if (charactorPageXpos != -1080)
            {
                charactorPageXpos -= 1080;
                charactorPage.DOLocalMoveX(charactorPageXpos, 1f).SetEase(Ease.OutQuart);
                currentPage++;
            }
        }

        changeCurrentPageImg(direction);
    }

    void changeCurrentPageImg(int direction)
    {
        float time = 0.5f;

        currentPageImg[currentPage].DOColor(Color.black, time);
        currentPageImg[currentPage].DOFade(0.6f, time);

        if (direction == 0)
        {
            currentPageImg[currentPage + 1].DOColor(Color.white, time);
            currentPageImg[currentPage + 1].DOFade(0.5f, time);
        }
        else if (direction == 1)
        {
            currentPageImg[currentPage - 1].DOColor(Color.white, time);
            currentPageImg[currentPage - 1].DOFade(0.5f, time);
        }

        if (currentPage == 0) charactorBG.DOColor(new Color(0.64f, 0.72f, 0.327f), time);
        else if (currentPage == 1) charactorBG.DOColor(new Color(1, 0.6f, 0.65f), time);
        else if (currentPage == 2) charactorBG.DOColor(new Color(0.69f, 0.66f, 0.90f), time);
    }
    #endregion

    #region 유저 캐릭터 정보 창
    public void onPlayerCharInfoUI()
    {
        SystemDB.OnUI = true;
        PlayerCharInfoObj.gameObject.SetActive(true);
    }

    public void offPlayterCharInfoUi()
    {
        SystemDB.OnUI = false;
        PlayerCharInfoObj.gameObject.SetActive(false);
    }
    public void onPlayerCharInfoUI(int charnum)
    {
        playerCharInfoImg.sprite = charImage[charnum + 3];
        playerCharInfoImg.SetNativeSize();
    }

    #endregion

    /**
    *  #################################################
    *
    *                    도감 파트
    * 
    *  #################################################
    */
    #region 도감
    // 도감 페이지 버튼 ( 0 = 왼쪽, 1 = 오른쪽)
    public void BookArrow(int arrow)
    {
        if (arrow == 0)
        {
            if (bookPage > 0) bookPage--;
        }
        else
        {
            if (bookPage < 4) bookPage++;
        }

        setBookPage(bookPage);
    }


    // 도감 페이지 설정
    void setBookPage(int page)
    {
        // 스크롤 있는 도감페이지를 맨위로 올림
        lv1SV.value = 1;
        lv2SV.value = 1;

        for (int i = 0; i < starObj.transform.childCount; i++)
        {
            // 별 갯수 표시
            if (i <= page)
            {
                if (page >= 1 && i == 1) starObj.transform.GetChild(i - 1).gameObject.SetActive(false);

                starObj.transform.GetChild(i).gameObject.SetActive(true);
            }

            // 도감 페이지 설정
            if (i == page)
            {
                bookObj[i].SetActive(true);
            }
            else
            {
                if (i > page) starObj.transform.GetChild(i).gameObject.SetActive(false);
                bookObj[i].SetActive(false);
            }
        }
    }
    #endregion // 도감

    /**
   *  #################################################
   *
   *                      던전 파트
   * 
   *  #################################################
   */
    #region 던전
    // 던전 버튼 누르면 세포 선택창 나옴
    public void OnSelectCell()
    {
        CellSelectUI.SetActive(true);
        cellSelectScrollBar.value = 1;
        SystemDB.OnUI = true;
        AllManager.AM.GM.setUserCellDunGeon();
    }
    public void OffSelectCell()
    {
        CellSelectUI.SetActive(false);
        SystemDB.OnUI = false;
    }

    public void setSelectWidth(int row)
    {
        cellRTWidth = 228;

        if(row > 0)
         cellRTWidth += 82 * row;

        cellSelectRT.sizeDelta = new Vector2(0, cellRTWidth);
    }
    #endregion


    /**
  *  #################################################
  *
  *                      상점 파트
  * 
  *  #################################################
  */
    #region 상점
    public void SkillImgSetting(string[] level)
    {
        for (int i = 0; i < level.Length; i++)
        {
            if (int.Parse(level[i]) > 0)
                SkillCell[i].GetChild(0).gameObject.SetActive(true);
        }
    }

    public void skillButtonSetting()
    {
        for (int i = 0; i < User.skillLV.Length; i++)
            if (User.skillLV[i] == 4) AllButton[i].interactable = false;

    }
    #endregion

    /**
    *  #################################################
    *
    *                      설정 파트
    * 
    *  #################################################
    */

    #region 설정

    public void onCreaditUI()
    {
        SystemDB.OnUI = true;
        creaditUI.SetActive(true);
    }
    public void offCreaditUI()
    {
        SystemDB.OnUI = false;
        creaditUI.SetActive(false);
    }

    #endregion


    /**
*  #################################################
*
*                    지도 파트
* 
*  #################################################
*/

    #region 도감 조합표

    // UI는 왼쪽으로 빠짐, 아래쪽 스크롤 뷰는 아래쪽으로 이동해서 빈 창을 만든d
    public void MoveMap()
    {
        UI.DOLocalMoveX(-1080, 0.3f).SetEase(Ease.OutQuad);
        MainScroll.DOLocalMoveY(-1500, 0.3f).SetEase(Ease.OutQuad);
        MapUI.DOLocalMove(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutQuad);
        CellObj.DOLocalMoveX(-6, 0.3f).SetEase(Ease.OutQuad);
    }

    public void MoveOriginal()
    {
        UI.DOLocalMoveX(0, 0.3f).SetEase(Ease.OutQuad);
        MainScroll.DOLocalMoveY(-657, 0.3f).SetEase(Ease.OutQuad);
        MapUI.DOLocalMove(new Vector3(1180, 0, 0), 0.3f).SetEase(Ease.OutQuad);
        CellObj.DOLocalMoveX(0, 0.3f).SetEase(Ease.OutQuad);
    }
    #endregion
}
