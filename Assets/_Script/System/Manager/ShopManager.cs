using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.EventSystems;
using Sirenix.Serialization;

public class ShopManager : MonoBehaviour
{
    public RectTransform Content;
    float contentHeight;

    public bool ShopMInit()
    {
        contentHeight = 0;
        Content.sizeDelta = new Vector2(1080, contentHeight);

        return true;
    }

    public void setShop(string[] SkillLevel)
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest() { CatalogVersion = "Shop" }, (result) =>
        {
            bool skill_1 = false;
            bool skill_2 = false;
            bool skill_3 = false;
            bool skill_4 = false;

            for (int i = 0; i < result.Catalog.Count; i++)
            {
                var Catalog = result.Catalog[i];

                // 레벨 가져옴
                int cut = Catalog.ItemId.IndexOf("_");
                int level = int.Parse(Catalog.ItemId.Substring(cut + 1));

                if (Catalog.ItemClass == "skill1")
                    if (int.Parse(SkillLevel[0]) + 1 != level || skill_1) continue;
                if (Catalog.ItemClass == "skill2")
                    if (int.Parse(SkillLevel[1]) + 1 != level || skill_2) continue;
                if (Catalog.ItemClass == "skill3")
                    if (int.Parse(SkillLevel[2]) + 1 != level || skill_3) continue;
                if (Catalog.ItemClass == "skill4")
                    if (int.Parse(SkillLevel[3]) + 1 != level || skill_4) continue;


                for (int j = 0; j < result.Catalog.Count; j++)
                {
                    if (!Content.transform.GetChild(j).gameObject.activeSelf)
                    {
                        Text nameText = Content.transform.GetChild(j).GetChild(1).GetComponent<Text>();
                        Text explainText = Content.transform.GetChild(j).GetChild(2).GetComponent<Text>();
                        Text priceText = Content.transform.GetChild(j).GetChild(3).GetComponent<Text>();

                        nameText.text = Catalog.DisplayName;
                        explainText.text = Catalog.Description;
                        priceText.text = Catalog.VirtualCurrencyPrices["MY"].ToString();

                        contentHeight += 220;
                        Content.sizeDelta = new Vector2(1080, contentHeight);

                        Content.transform.GetChild(j).gameObject.SetActive(true);

                        if (Catalog.ItemClass == "skill1") skill_1 = true;
                        if (Catalog.ItemClass == "skill2") skill_2 = true;
                        if (Catalog.ItemClass == "skill3") skill_3 = true;
                        if (Catalog.ItemClass == "skill4") skill_4 = true;

                        break;
                    }
                }
            }

            AllManager.AM.GM.LoadingBarFill();
        },
       (error) => Debug.Log("상점 불러오기 실패"));
    }

    // 상점 아이템 구입
    public void buyItem(string itemID)
    {
        // 가격 초기화
        int price = 0;
        // 부모 찾아옴
        Transform parent = EventSystem.current.currentSelectedGameObject.transform.parent;
        // 레벨 가져옴
        int cut = parent.GetChild(1).GetComponent<Text>().text.IndexOf(".");
        string level = parent.GetChild(1).GetComponent<Text>().text.Substring(cut + 1);

        // 버튼 임시 비활성
        AllManager.AM.UM.buttonOff(1);

        #region 다음 레벨 스킬 구매
        if (itemID == "skill1")
        {
            itemID = "skill1_" + level;
            price = SystemDB.skill1_price[int.Parse(level) - 1];
        }
        if (itemID == "skill2")
        {
            itemID = "skill2_" + level;
            price = SystemDB.skill2_price[int.Parse(level) - 1];
        }
        if (itemID == "skill3")
        {
            itemID = "skill3_" + level;
            price = SystemDB.skill3_price[int.Parse(level) - 1];
        }
        if (itemID == "skill4")
        {
            itemID = "skill4_" + level;
            price = SystemDB.skill4_price[int.Parse(level) - 1];
        }


        playfabBuy(parent, itemID, price, int.Parse(level));

        #endregion
    }

    // 서버 연동하여 아이템 구매
    // 부모, 아이템 아이디, 가격, 레벨
    void playfabBuy(Transform parent, string itemID, int price, int level)
    {
        var request = new PurchaseItemRequest() { CatalogVersion = "Shop", ItemId = itemID, VirtualCurrency = "MY", Price = price };
        PlayFabClientAPI.PurchaseItem(request, (result) =>
        {
            string[] eachSkillLevel = new string[4];

            for (int i = 0; i < User.skillLV.Length; i++)
                eachSkillLevel[i] = User.skillLV[i].ToString();

            string itemclass = itemID.Substring(0, 6);

            if (level <= 5)
            {
                if (itemclass == "skill1")
                {
                    parent.GetChild(1).GetComponent<Text>().text = SystemDB.skill1_Name[level];
                    parent.GetChild(2).GetComponent<Text>().text = SystemDB.skill1_Explain[level];
                    parent.GetChild(3).GetComponent<Text>().text = SystemDB.skill1_price[level].ToString();
                    eachSkillLevel[0] = level.ToString();
                    User.skillLV[0] = level;
                }
                else if (itemclass == "skill2")
                {
                    parent.GetChild(1).GetComponent<Text>().text = SystemDB.skill2_Name[level];
                    parent.GetChild(2).GetComponent<Text>().text = SystemDB.skill2_Explain[level];
                    parent.GetChild(3).GetComponent<Text>().text = SystemDB.skill2_price[level].ToString();
                    eachSkillLevel[1] = level.ToString();
                    User.skillLV[1] = level;
                }
                else if (itemclass == "skill3")
                {
                    parent.GetChild(1).GetComponent<Text>().text = SystemDB.skill3_Name[level];
                    parent.GetChild(2).GetComponent<Text>().text = SystemDB.skill3_Explain[level];
                    parent.GetChild(3).GetComponent<Text>().text = SystemDB.skill3_price[level].ToString();
                    eachSkillLevel[2] = level.ToString();
                    User.skillLV[2] = level;
                }
                else if (itemclass == "skill4")
                {
                    parent.GetChild(1).GetComponent<Text>().text = SystemDB.skill4_Name[level];
                    parent.GetChild(2).GetComponent<Text>().text = SystemDB.skill4_Explain[level];
                    parent.GetChild(3).GetComponent<Text>().text = SystemDB.skill4_price[level].ToString();
                    eachSkillLevel[3] = level.ToString();
                    User.skillLV[3] = level;
                }

                if (level - 1 == 0)
                    AllManager.AM.UM.SkillImgSetting(eachSkillLevel);
            }

            User.money -= price;
            AllManager.AM.UM.setUserMoney();
            AllManager.AM.UM.buttonOff(0);
            AllManager.AM.UM.skillButtonSetting();
        }, (error) => { Debug.Log("구매 실패"); AllManager.AM.UM.buttonOff(0); });
    }

    public void spandMoney(int amount)
    {
        AllManager.AM.UM.buttonOff(1);
        var spandrequest = new SubtractUserVirtualCurrencyRequest() { VirtualCurrency = "MY", Amount = amount };
        PlayFabClientAPI.SubtractUserVirtualCurrency(spandrequest, (result) =>
        {
            AllManager.AM.UM.buttonOff(0);
            AllManager.AM.UM.setUserMoney();
        }, (error) => Debug.Log(error));
    }



    // 스킬 래벨 표현
    public void explainSkill(int skillnum)
    {
        Debug.Log(User.skillLV[skillnum]);
    }
}
