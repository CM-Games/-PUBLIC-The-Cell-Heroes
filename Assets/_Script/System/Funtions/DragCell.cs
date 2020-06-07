using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragCell : MonoBehaviour
{
    // public GameManager gameManager;

    // 선택한 세포 최 상단 위치
    SpriteRenderer SROrderLayer;

    // 현재 클릭된 오브젝트 저장
    GameObject targetObj;

    // 기존 세포 좌표 저장
    Vector2 MainCellPos;

    // 머지 가능 여부
    bool merge;
       
    private void Awake()
    {
        merge = false;
        SROrderLayer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 마우스 누르면 눌린 오브젝트를 가져오고 소팅오더를 최상단으로 위치 
        if (Input.GetMouseButtonDown(0) && !SystemDB.OnUI)
        {
            TouchObj();

            if (User.ClickObj == gameObject)
            {
                merge = false;
                MainCellPos = transform.position;
                SROrderLayer.sortingOrder = 0;
            }
        }

        // 마우스 올리면 머지 시작
        if (Input.GetMouseButtonUp(0) && User.ClickObj == gameObject)
        {
            SROrderLayer.sortingOrder = -1;
            merge = true;
            targetObj = null;           
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (merge && (gameObject == User.ClickObj))
        {
           
            merge = false;
            string MainCellLevel, SubCellLevel, MainCellColor, SubCellColor, MainCellType, SubCellType;
            int cellNameLength, cellTypeLength;
            // 세포 색,종족 이름 따오기
            string cellName, cellType;

            MainCellLevel = name.Substring(2, 1);
            MainCellColor = name.Substring(4, 1);
            cellNameLength = name.IndexOf("|") -1;
            MainCellType = name.Substring(cellNameLength, 1);

           SubCellLevel = other.name.Substring(2, 1);
            SubCellColor = other.name.Substring(4, 1);
            cellNameLength = other.name.IndexOf("|") - 1;
            SubCellType = other.name.Substring(cellNameLength, 1);

            cellName = name.Substring(0, cellNameLength + 1);
            cellName = cellName.Substring(4);

            // 레벨이 같고, 색이 같고, 종족이 같아야 머지 가능
            if (MainCellLevel == SubCellLevel && MainCellColor == SubCellColor && MainCellType == SubCellType)
            {
                other.gameObject.SetActive(false);
                gameObject.SetActive(false);

                User.UserCellDeck.Remove(User.ClickObj);
                User.UserCellDeck.Remove(other.gameObject);
                User.ClickObj = null;

                switch (MainCellLevel)
                {
                    case "0":
                        PoolingManager.PM.MakeObj("Lv1", MainCellColor);
                        break;
                    case "1":                       
                        PoolingManager.PM.MakeObj("Lv2", MainCellColor,cellName);
                        break;
                    case "2":
                        PoolingManager.PM.MakeObj("Lv3", MainCellColor);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                transform.DOMove(MainCellPos, 0.4f).SetEase(Ease.Unset);
                Debug.Log("합칠수 없음!");
            }
        }
    }

    void TouchObj()
    {
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInformation = Physics2D.Raycast(touchPos, Camera.main.transform.forward);
        if (hitInformation.collider != null)
        {
            GameObject touchedObject = hitInformation.transform.gameObject;
            targetObj = touchedObject;
            User.ClickObj = targetObj;
        }
    }

    private void OnMouseDrag()
    {
        Vector2 mousePositon = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePositon);
        transform.position = objPosition;
    }

}
