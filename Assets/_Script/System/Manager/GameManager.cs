using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    // 상단바 골드
    [Header("Cell")]
    public Toggle[] CellRadio;

    // 던전
    [Header("DunGeon")]
    public Transform CSVContent; // 던전 스크롤뷰
    public Button[] EnterDungeonBtn;

    // 로딩 관리
    [Header("Loading")]
    public Transform Loading;
    public Slider LoadingBar;

    [Header("Setting")]
    public Text userIdText;
    public Text userLvText;
    public Image userExpFill;

    // 테스트
    string id;
    string pw;

    private void Awake()
    {
        // 테스트
      //  User.FabId = null;
       // id = "g12422071366773018695@cell.com";
       // pw = "cjfals12";

       
        Init();
    }


    void Init()
    {
        if (!SystemDB.init)
        {
            #region 데이터 초기화
            User.money = 0;
            User.level = 1;
            User.exp = 0;
            User.ClickObj = null;
            User.skillLV = new int[4];
            User.starCost = 8;

            SystemDB.CurrentCellValue = 10;
            SystemDB.OnUI = false;
            SystemDB.startLoading = false;
            SystemDB.DungeonClear = new int[4];

            CellData.level1SetCount = 0;

            AllManager.AM.UM.setCellPayText(SystemDB.CurrentCellValue.ToString());
            #endregion

            #region 스킬 레벨별 가격
            SystemDB.skill1_price = new int[] { 10, 20, 30, 40, 50 };
            SystemDB.skill2_price = new int[] { 20, 40, 60, 80, 100 };
            SystemDB.skill3_price = new int[] { 100, 200, 300, 400, 500 };
            SystemDB.skill4_price = new int[] { 50, 100, 150, 200, 250 };
            #endregion

            #region 스킬 레벨별 이름
            SystemDB.skill1_Name = new string[] {
            "한계 돌파 LV.1",
            "한계 돌파 LV.2",
            "한계 돌파 LV.3",
            "한계 돌파 LV.4",
            "한계 돌파 LV.MAX"};

            SystemDB.skill2_Name = new string[] {
            "백신 개발 LV.1",
            "백신 개발 LV.2",
            "백신 개발 LV.3",
            "백신 개발 LV.4",
            "백신 개발 LV.MAX"};

            SystemDB.skill3_Name = new string[] {
            "예방 접종 LV.1",
            "예방 접종 LV.2",
            "예방 접종 LV.3",
            "예방 접종 LV.4",
            "예방 접종 LV.MAX"};

            SystemDB.skill4_Name = new string[] {
            "방역 LV.1",
            "방역 LV.2",
            "방역 LV.3",
            "방역 LV.4",
            "방역 LV.MAX"};
            #endregion

            #region 스킬 레벨별 설명
            SystemDB.skill1_Explain = new string[] {
            "던전에서 최대 스타코스트를 +1 한다.",
            "던전에서 최대 스타코스트를 +2 한다.",
            "던전에서 최대 스타코스트를 +3 한다.",
            "던전에서 최대 스타코스트를 +4 한다.",
            "던전에서 최대 스타코스트를 +5 한다."
        };
            SystemDB.skill2_Explain = new string[] {
            "적진의 1% 데미지를 줄수있는 백신 주사기를 떨어뜨린다.",
            "적진의 2% 데미지를 줄수있는 백신 주사기를 떨어뜨린다.",
            "적진의 3% 데미지를 줄수있는 백신 주사기를 떨어뜨린다.",
            "적진의 4% 데미지를 줄수있는 백신 주사기를 떨어뜨린다.",
            "적진의 5% 데미지를 줄수있는 백신 주사기를 떨어뜨린다."
        };
            SystemDB.skill3_Explain = new string[] {
            "본진의 1% 체력을 회복 할 수있는 백신 주사기를 떨어뜨린다.",
            "본진의 2% 체력을 회복 할 수있는 백신 주사기를 떨어뜨린다.",
            "본진의 3% 체력을 회복 할 수있는 백신 주사기를 떨어뜨린다.",
            "본진의 4% 체력을 회복 할 수있는 백신 주사기를 떨어뜨린다.",
            "본진의 5% 체력을 회복 할 수있는 백신 주사기를 떨어뜨린다."
        };
            SystemDB.skill4_Explain = new string[] {
            "던전 시작시 적진 바이러스라 1초 늦게 스폰하도록 한다.",
            "던전 시작시 적진 바이러스라 2초 늦게 스폰하도록 한다.",
            "던전 시작시 적진 바이러스라 3초 늦게 스폰하도록 한다.",
            "던전 시작시 적진 바이러스라 4초 늦게 스폰하도록 한다.",
            "던전 시작시 적진 바이러스라 5초 늦게 스폰하도록 한다."
        };
            #endregion

            #region 도감 세트 효과
            SystemDB.setEffect = new string[3] {
            "1세트 효과 : 공격력 + 20",
            "2세트 효과 : 공격력 + 40",
            "3세트 효과 : 공격력 + 60"
        };
            #endregion
        }

        User.UserCellDeck = new List<GameObject>();
        SystemDB.DungeonCellDeck = new List<string>();

        SystemDB.init = true;
        userIdText.text = User.FabId;

      //  testLogin();
    }

    public void testLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = id, Password = pw };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            User.FabId = result.PlayFabId;
            userIdText.text = "ID : " + User.FabId;
        }, (error) => testPlayFabRegister());
    }

    public void testPlayFabRegister()
    {
        var request = new RegisterPlayFabUserRequest { Email = id, Password = pw, Username = "ccm1441" };
        PlayFabClientAPI.RegisterPlayFabUser(request, (resule) => { Debug.Log("success Register"); testLogin(); }, (error) => Debug.Log("Fail Register"));
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        StartCoroutine("gameLoading");

        userLvText.text = User.level.ToString();
        userExpFill.fillAmount = User.exp;
    }

    // 게임 로딩
    IEnumerator gameLoading()
    {
        StartCoroutine(LoadingValueCheck());
        // 풀링 
        if (PoolingManager.PM.Generate()) LoadingBarFill();
        yield return new WaitForSeconds(0.3f);

        // 도감 초기화
        if (AllManager.AM.BM.BMInit()) LoadingBarFill();
        yield return new WaitForSeconds(0.3f);

        // 상점 초기화
        if (AllManager.AM.SM.ShopMInit()) LoadingBarFill();
        yield return new WaitForSeconds(0.3f);

        // UI 초기화
        if (AllManager.AM.UM.UMInit()) LoadingBarFill();
        openNextDungeon();
        yield return new WaitForSeconds(0.3f);

        // 유저 데이터 로드
        // ShopManager 에서 퍼센트 증가시킴
        AllManager.AM.DM.playFabLoad();
        yield return new WaitForSeconds(0.7f);
       
    }

    public IEnumerator LoadingValueCheck(bool dungeon = false)
    {
        // 모든 데이터 로드 완료
        while (true)
        {
            if (LoadingBar.value > 0.99)
            {
                if(dungeon) SceneManager.LoadSceneAsync("Dungeon");
                Loading.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    // 로딩 바 관리
    public void LoadingBarFill(int LoadingContentCount = 5)
    {
        float fill = 0.2f;
        float currentFill = LoadingBar.value;

        fill += currentFill;

        if (fill > 0.98) fill = 1;
        LoadingBar.DOValue(fill, 0.2f).SetEase(Ease.OutQuad);
    }

    public void resetLoadingBarFill()
    {
        LoadingBar.value = 0;       
    }

    /**
     *  #################################################
     *
     *                     세포 파트
     * 
     *  #################################################
     */
    #region 세포
    // 일반 증식= 0, 고급증식 = 1 버튼 누를시
    // 멕스 버튼 체크하면 재화로 뽑을수 있을때 까지 뽑음
    public void GenerateCell(int button)
    {
        int CellCount = 1;
        int WillUseMoney = 0; // 한번에 결제
        int maxCellStar;

        // 일반 증식인가 고급증식인가 구분
        if (button == 0) maxCellStar = 3;
        else maxCellStar = 4;

        // MAX 체크시
        if (CellRadio[0].isOn) CellCount = 999;

        // 생성
        for (int i = 0; i < CellCount; i++)
        {
            if (User.money  < SystemDB.CurrentCellValue)
            {
                AllManager.AM.UM.setStateText("세포를 증식할 재화가 부족합니다!");
                break;  
            }

            // 일반 뽑기 랜덤
            int ran;
            if (Random.Range(1, 101) > 5)
                ran = 1;
            else ran = 2;

            if (ran.Equals(1))
                PoolingManager.PM.MakeObj("Lv0", "");
            else if (ran.Equals(2))
                PoolingManager.PM.MakeObj("Lv1", "");
            //else if (ran.Equals(3))
            //    PoolingManager.PM.MakeObj("Lv1_3");

            WillUseMoney += SystemDB.CurrentCellValue;

            User.money -= SystemDB.CurrentCellValue;
            SystemDB.CurrentCellValue += 5;

            // # Cell data update
            AllManager.AM.UM.setCellPayText(SystemDB.CurrentCellValue.ToString());
        }

        if(WillUseMoney > 0)
        {
            AllManager.AM.DM.saveCellCount();
            AllManager.AM.SM.spandMoney(WillUseMoney);
        }
       
    }
    #endregion

    /**
      *  #################################################
      *
      *                     던전 파트
      * 
      *  #################################################
      */
    #region 던전
    public void JoinDunGeon()
    {
        foreach (var item in User.UserCellDeck)
        {
            Debug.Log(item);
        }
    }

    // 유저가 가지고 있는 세포를 셋팅
    public void setUserCellDunGeon()
    {
        // 초기화
        InitUserCellDunGeon();
        Debug.Log(User.UserCellDeck.Count);
       
        float temp = (User.UserCellDeck.Count - 9) / 3.0f;
        int row = temp == (int)temp ? (int)temp : (int)temp + 1;

        // 스크롤 사이즈 늘림
            AllManager.AM.UM.setSelectWidth(row);

        for (int i = 0; i < User.UserCellDeck.Count; i++)
        {
            // 세포 이미지 셋팅
            CSVContent.GetChild(i).GetComponent<Image>().sprite = User.UserCellDeck[i].GetComponent<SpriteRenderer>().sprite;
            CSVContent.GetChild(i).GetComponent<Image>().SetNativeSize();
            CSVContent.GetChild(i).GetChild(2).GetComponent<Text>().text = User.UserCellDeck[i].name;
            string star = User.UserCellDeck[i].name.Substring(2, 1);

            for (int j = 0; j < int.Parse(star) + 1; j++)
            {
                // 별 셋팅
                if (int.Parse(star) == 0)
                {
                    CSVContent.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    break;
                }
                else
                {
                    CSVContent.GetChild(i).GetChild(0).GetChild(j).gameObject.SetActive(true);
                    CSVContent.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }

            CSVContent.GetChild(i).gameObject.SetActive(true);
        }
    }

    void InitUserCellDunGeon()
    {
        for (int i = 0; i < 999; i++)
        {
            // 별 초기화
            for (int k = 0; k < 5; k++)
                CSVContent.GetChild(i).GetChild(0).GetChild(k).gameObject.SetActive(false);

            // 세포 초기화, 체크한것 초기화
            if (CSVContent.GetChild(i).gameObject.activeSelf)
            {
                CSVContent.GetChild(i).GetChild(1).GetComponent<Toggle>().isOn = false;
                CSVContent.GetChild(i).gameObject.SetActive(false);
            }
            else break;
        }
    }

    // 선택한 세포 저장
    public void JoinSelectCell()
    {
        for (int i = 0; i < User.UserCellDeck.Count; i++)
        {
            if (CSVContent.GetChild(i).GetChild(1).GetComponent<Toggle>().isOn)
            {
                int Cellcut = CSVContent.GetChild(i).GetChild(2).GetComponent<Text>().text.IndexOf("|");
                string CellName = CSVContent.GetChild(i).GetChild(2).GetComponent<Text>().text.Substring(0, Cellcut);
                SystemDB.DungeonCellDeck.Add(CellName);
            }
        }

        if(SystemDB.DungeonCellDeck.Count > 5)
        {
            SystemDB.DungeonCellDeck.Clear();
            AllManager.AM.UM.setStateText("5마리 이하로 선택하여 주세요!");
            return;
        }
        else if(SystemDB.DungeonCellDeck.Count == 0)
        {
            AllManager.AM.UM.setStateText("1마리 이상 선택하여 주세요!");
            return;
        }
        AllManager.AM.UM.LoadingUIOpen();
        AllManager.AM.DM.Save(true);
        //AllManager.AM.UM.OffSelectCell();
    }

    // 던전 클리어시 다음 던전 오픈
    void openNextDungeon()
    {
        for (int i = 0; i < EnterDungeonBtn.Length; i++)
        {
            if (SystemDB.DungeonClear[i] == 1)
                EnterDungeonBtn[i].interactable = true;            
        }
    }
    #endregion

    /**
  *  #################################################
  *
  *                     유저 파트
  * 
  *  #################################################
  */

   public void setUserInfo(string level, string exp)
    {
        User.level = int.Parse(level);
        User.exp = float.Parse(exp);
        userLvText.text = level;
        userExpFill.fillAmount = float.Parse(exp);
    }
}
