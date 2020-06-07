using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllManager : MonoBehaviour
{
    public GameManager GM;
    public DataManager DM;
    public ShopManager SM;
    public BookManager BM;
    public UIManager UM;
    public ScrollManager ScrollM;
    public ScrollRect SR;
    public MapManager MM;
    public ScrollManager ScrollMMap;
    public ScrollRect SRMap;

    public static AllManager AM;

    private void Awake() => AM = this;

}
