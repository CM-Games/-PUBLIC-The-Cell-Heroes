using UnityEngine;
using System.Collections.Generic;

// # Users Data Class 
public static class User
{
    // 유저가 가지고 있는 돈
    public static int money { get; set; }
    public static int level;
    public static float exp;

    // 유저가 들고있는 세포
    public static List<GameObject> UserCellDeck;
    
    // 유저가 어떤 오브젝트를 클릭하고 있는가
    public static GameObject ClickObj;

    // 유저의 서버 아이디
    public static string FabId;

    public static int[] skillLV;

    // 유저의 스타 코스트
    public static int starCost;
}

public static class SystemDB
{
    // 현재 세포 증식에 필요한 돈
    public static int CurrentCellValue { get; set; }

    // 초기화 여부
    public static bool init;

    // 던전에 데려갈 세포
    public static List<string> DungeonCellDeck;

    // 팝업창이 열려있는지 
    public static bool OnUI;

    // 데이터 로딩 체크
    public static bool startLoading;

    // 던전 클리어 현황
    public static int[] DungeonClear;

    // 세트 효과
    public static string[] setEffect;

    // 스킬 레벨별 가격
    public static int[] skill1_price;
    public static int[] skill2_price;
    public static int[] skill3_price;
    public static int[] skill4_price;

    // 스킬 레벨별 제목
    public static string[] skill1_Name;
    public static string[] skill2_Name;
    public static string[] skill3_Name;
    public static string[] skill4_Name;

    // 스킬 레벨별 설명
    public static string[] skill1_Explain;
    public static string[] skill2_Explain;
    public static string[] skill3_Explain;
    public static string[] skill4_Explain;

}

