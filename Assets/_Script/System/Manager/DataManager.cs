using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Text;
using UnityEngine.SceneManagement;

using DG.Tweening;
using Sirenix.OdinInspector;

[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
}

[System.Serializable]
public class Cell
{
    public Cell(string _Name, string _x_pos, string _y_pos)
    {
        Name = _Name;
        x_pos = _x_pos;
        y_pos = _y_pos;
    }

    public string Name, x_pos, y_pos;
}

public class DataManager : MonoBehaviour
{
    // 세포 데이터 로드
    string cellName;
    int textEnd;
    public List<Cell> CellList;
    string[,] loadData;

    // 유저 도감 등록 여부 조합
    StringBuilder sb = new StringBuilder();

    // ****************************************************
    // 테스트 계정 관리

    [InlineButton("Testmoney")]
    public int TestMoney;

    void Testmoney()
    {
        print("돈 지급");
        giveMoney(TestMoney);
        TestMoney = 0;
    }

    [InlineButton("resetCellPrice")]
    public int price;

    void resetCellPrice()
    {
        print("세포 가격 초기화");
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "cellCount", price.ToString() } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => {
            SystemDB.CurrentCellValue = price;
            AllManager.AM.UM.setCellPayText(SystemDB.CurrentCellValue.ToString());
            price = 0;
        }, (error) => Debug.Log("데이터 저장실패"));

    }




    // ****************************************************

    public void playFabLoad() => StartCoroutine("Load");

    IEnumerator Load()
    {
        int i = 0;
        string star = null;
        int cut = 0;

        #region 서버에 저장된 데이터를 꺼내옴
        var request = new GetUserDataRequest() { PlayFabId = User.FabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            loadData = new string[result.Data.Count, 4];

            #region 유저 도감, 재화, 세포 데이터 로드
            if(result.Data.Count > 0) // 유저 데이터가 있으면 로드 없으면 새로만들기
            {
                foreach (var item in result.Data)
                {
                    // 값이 비었으면 불러오지 않음
                    if (item.Value.Value != null)
                    {
                        // 도감 데이터 로드
                        if (item.Key == "book")
                        {
                            string[] book = item.Value.Value.Split('|');

                            for (int h = 0; h < book.Length; h++)
                                if (book[h] == "o")
                                    CellData.CellDataArray[h, 5] = book[h];

                            continue;
                        }

                        // 유저 자금
                        if (item.Key == "usermoney")
                        {
                            string[] init = item.Value.Value.Split('|');

                            if (init[0] == "true")
                            {
                                var moneyrequest1 = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "MY", Amount = 0 };
                                PlayFabClientAPI.AddUserVirtualCurrency(moneyrequest1, (resultmoney1) => { User.money = resultmoney1.Balance; AllManager.AM.UM.setUserMoney(); }, (errormoney1) => Debug.Log("돈 로드 실패"));
                            }
                            continue;
                        }

                        // 유저 캐릭터 정보                    
                        if (item.Key == "userchar")
                        {
                            string[] init = item.Value.Value.Split('|');

                            if (init[0] == "true")
                            {
                                AllManager.AM.UM.setUserCharMini(int.Parse(init[1]));
                                AllManager.AM.UM.onPlayerCharInfoUI(int.Parse(init[1]));
                            }
                            continue;
                        }

                        // 유저 정보
                        if (item.Key == "userInfo")
                        {
                            string[] init = item.Value.Value.Split('|');

                            AllManager.AM.GM.setUserInfo(init[0], init[1]);
                            continue;
                        }

                        // 세포 가격
                        if(item.Key == "cellCount")
                        {
                            SystemDB.CurrentCellValue = int.Parse(item.Value.Value);
                            AllManager.AM.UM.setCellPayText(item.Value.Value);
                            continue;
                        }

                        // 좌표와 몇성인지 분해
                        star = item.Key.Substring(2, 1);
                        cut = item.Value.Value.IndexOf("|");
                        // 이름 분해
                        textEnd = item.Key.IndexOf("|");
                        cellName = item.Key.Substring(0, textEnd);

                        loadData[i, 0] = cellName;
                        loadData[i, 1] = item.Value.Value.Substring(0, cut);
                        loadData[i, 2] = item.Value.Value.Substring(cut + 1);
                        loadData[i, 3] = star;
                        i++;
                    }
                }

                // 불러온 데이터 새로고침
                PoolingManager.PM.LoadCell(loadData);
            }else if(result.Data.Count == 0) // 완전 초기 실행
            {
                // 유저 돈 지급
                var moneyrequest = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "MY", Amount = 1000 };
                PlayFabClientAPI.AddUserVirtualCurrency(moneyrequest, (resultmoney) => { User.money = resultmoney.Balance; AllManager.AM.UM.setUserMoney(); }, (errormoney) => Debug.Log("돈 지급 실패"));

                var usermoneysave = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "usermoney", "true|" } } };
                PlayFabClientAPI.UpdateUserData(usermoneysave, (result3) => { }, (error2) => Debug.Log("데이터 저장실패"));

                // 캐릭터 생성
                AllManager.AM.UM.startSeletingChar();
            }
            #endregion

            #region 유저 인벤토리 로드
            getInventory();
            #endregion

        }, (error) => Debug.Log("세포 데이터 로드 실패"));
        #endregion

        yield return new WaitForSeconds(0.2f);
    }

    // 유저가 선택한 캐릭터 저장
    public void SaveUserChar(int selectChar)
    {
        var usercharsave = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "userchar", "true|" + selectChar.ToString() } } };
        PlayFabClientAPI.UpdateUserData(usercharsave, (result3) => { }, (error2) => Debug.Log("데이터 저장실패"));
        AllManager.AM.UM.offSeletingChar();
    }

    // 유저의 인벤토리를 가져옴
    public void getInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) =>
        {
            User.money = result.VirtualCurrency["MY"];
            AllManager.AM.UM.setUserMoney();
            string[] eachSkillLevel = new string[] { "0", "0", "0", "0" };

            for (int i = 0; i < result.Inventory.Count; i++)
            {
                var Inven = result.Inventory[i];

                int cut = Inven.ItemId.IndexOf("_");
                string cutLevel = Inven.ItemId.Substring(cut + 1);

                if (Inven.ItemClass == "skill1")
                    if (int.Parse(eachSkillLevel[0]) < int.Parse(cutLevel))
                        eachSkillLevel[0] = cutLevel;
                if (Inven.ItemClass == "skill2")
                    if (int.Parse(eachSkillLevel[1]) < int.Parse(cutLevel))
                        eachSkillLevel[1] = cutLevel;
                if (Inven.ItemClass == "skill3")
                    if (int.Parse(eachSkillLevel[2]) < int.Parse(cutLevel))
                        eachSkillLevel[2] = cutLevel;
                if (Inven.ItemClass == "skill4")
                    if (int.Parse(eachSkillLevel[3]) < int.Parse(cutLevel))
                        eachSkillLevel[3] = cutLevel;
            }

            // 레벨 저장
            for (int i = 0; i < eachSkillLevel.Length; i++)
                User.skillLV[i] = int.Parse(eachSkillLevel[i]);

            AllManager.AM.UM.skillButtonSetting();
            // 스킬이미지 셋팅
            AllManager.AM.UM.SkillImgSetting(eachSkillLevel);

            // 상점 로드
            AllManager.AM.SM.setShop(eachSkillLevel);
        },
        (error) => Debug.Log("인벤토리 로드 실패"));
    }

    string getLevel(string name)
    {
        int cut = name.IndexOf("_");
        string level = name.Substring(cut + 1);
        return level;
    }

    public void Save(bool moveScene = false)
    {
        for (int i = 0; i < CellData.CellDataArray.GetLength(0); i++)
        {
            sb.Append(CellData.CellDataArray[i, 5]);

            if (i != CellData.CellDataArray.GetLength(0) - 1)
                sb.Append("|");
        }

        StartCoroutine(RemoveAndSave(moveScene));
    }

    IEnumerator RemoveAndSave(bool moveScene)
    {
        #region 기존 데이터 삭제
        var request = new GetUserDataRequest() { PlayFabId = User.FabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var item in result.Data)
            {
                if (item.Key == "userchar" || item.Key == "usermoney")
                    continue;
                // 로드 후 데이터 값 삭제
                var request1 = new UpdateUserDataRequest() { KeysToRemove = new List<string>() { item.Key } };
                PlayFabClientAPI.UpdateUserData(request1, (result1) => Debug.Log("데이터 삭제 완료"), (error1) => Debug.Log("데이터 삭제 실패"));
            }
        }, (error) => Debug.Log("세포 데이터 로드 실패"));
        #endregion

        yield return new WaitForSeconds(1f);

        #region 도감 데이터 저장
        var request3 = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "book", sb.ToString() } } };
        PlayFabClientAPI.UpdateUserData(request3, (result3) => { Debug.Log("데이터 저장완료"); }, (error2) => Debug.Log("데이터 저장실패"));
        #endregion

        yield return new WaitForSeconds(0.2f);

        #region 세포 데이터 저장
        foreach (var item in User.UserCellDeck)
        {
            // 세포 저장
            var request2 = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { item.name, item.transform.position.x.ToString() + "|" + item.transform.position.y.ToString() } } };
            PlayFabClientAPI.UpdateUserData(request2, (result2) => { Debug.Log(item.name + "데이터 저장완료"); }, (error2) => Debug.Log(item.name + "데이터 저장실패"));
        }
        #endregion

        yield return new WaitForSeconds(1f);

        if (moveScene)
        {
            AllManager.AM.GM.LoadingBarFill(1);
            StartCoroutine(AllManager.AM.GM.LoadingValueCheck(true));
        }
        else AllManager.AM.UM.setStateText("저장 완료");


        sb.Clear();
    }


   public void saveUserInfo()
    {
        #region 유저 정보 저장
        var request4 = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "userInfo", User.level.ToString() + "|" + User.exp.ToString() } } };
        PlayFabClientAPI.UpdateUserData(request4, (result4) => SceneManager.LoadSceneAsync("Main"), (error4) => Debug.Log("데이터 저장실패"));

        #endregion
    }

    public void saveCellCount()
    {
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "cellCount", SystemDB.CurrentCellValue.ToString() } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Debug.Log("데이터 저장실패"));

    }

    // 유저 돈 지급
    public void giveMoney(int amout)
    {
        var moneyrequest = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "MY", Amount = amout };
        PlayFabClientAPI.AddUserVirtualCurrency(moneyrequest, (resultmoney) =>
        {
            User.money = resultmoney.Balance;
            AllManager.AM.UM.setUserMoney();
        }
        , (errormoney) => Debug.Log("돈 지급 실패"));



    }

}
