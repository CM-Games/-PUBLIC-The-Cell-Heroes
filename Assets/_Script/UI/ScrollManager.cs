using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    public Transform contentTr;

    // 탭 슬라이더
    [Header("Bottom Bar")]
    public Slider tabSlider;
    public RectTransform[] BtnRect, BtnTextRect, BtnImageTf;
    public Text[] RequestText;

    // 스크롤 할 컨탠츠 갯수
    const int SIZE = 5;
    // 현재 어디 스크롤인지 저장하기 위함
    float[] pos = new float[SIZE];
    // 현재 각 스크롤 컨탠츠 간의 간격
    float distance, targetPos, curPos;
    [HideInInspector]
    public int targetIndex;
    // 드래그 중인지 확인
    [HideInInspector]
    public bool isDrag;

    void Start()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) pos[i] = distance * i;
    }

    // 절반거리를 기준으로 가까운 위치를 반환하는 함수
    float SetPos()
    {
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        }
        return 0;
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData) => curPos = SetPos();

    // 드래그 중
    public void OnDrag(PointerEventData eventData) => isDrag = true;

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        targetPos = SetPos();

        // 스크롤 절반이 넘지 않아도 마우스 속도가 빠르다면 넘어가도록
        if (curPos == targetPos)
        {
            if (eventData.delta.x > 18 && curPos - distance >= 0)
            {
                --targetIndex;
                targetPos = curPos - distance;
            }
            else if (eventData.delta.x < -18 && curPos + distance <= 1.01f)
            {
                ++targetIndex;
                targetPos = curPos + distance;
            }
        }

        // 수직 스크롤 이용중 옆으로 다녀오면 다시 맨위로 올리는것
        for (int i = 0; i < SIZE; i++)
        {
            if (contentTr.GetChild(i).GetComponent<ScrollScript>() && curPos != pos[i] && targetPos == pos[i])
                contentTr.GetChild(i).GetChild(1).GetComponent<Scrollbar>().value = 1;
        }
    }


    // Update is called once per frame
    void Update()
    {
        tabSlider.value = scrollbar.value;

        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);

            // 슬라이더 탭 이동시 해당 탭 크기 변경
            for (int i = 0; i < SIZE; i++)
                BtnRect[i].sizeDelta = new Vector2(i == targetIndex ? 360 : 180, BtnRect[i].sizeDelta.y);
        }

        // 순간적으로 모이는 현상 제거
        if (Time.time < 0.1f) return;

        // 탭 이동시 해당 글씨 확대
        for (int i = 0; i < SIZE; i++)
        {
            Vector3 BtnTargetPos = BtnRect[i].anchoredPosition3D;

            BtnImageTf[i].anchoredPosition3D = Vector3.Lerp(BtnImageTf[i].anchoredPosition3D, BtnTargetPos, 0.15f);


            if (i == targetIndex)
            {
                // #.Text
                RequestText[i].fontSize = 67;
                BtnTargetPos.y = -43.69f;
                RequestText[i].color = Color.white;
                BtnTextRect[i].anchoredPosition3D = Vector3.Lerp(BtnTextRect[i].anchoredPosition3D, BtnTargetPos + new Vector3(80, 0, 0), 0.15f);

                // #. Img
                BtnImageTf[i].transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
                BtnImageTf[i].anchoredPosition = Vector3.Lerp(BtnImageTf[i].anchoredPosition, BtnTargetPos + new Vector3(-130f, 40, 0), 0.15f);

            }
            else
            {
                // #. Text
                RequestText[i].fontSize = 50;
                RequestText[i].color = Color.white;
                BtnTextRect[i].anchoredPosition = BtnTargetPos + new Vector3(0, -200, 0);

                // #. Img
                BtnImageTf[i].anchoredPosition = Vector3.Lerp(BtnImageTf[i].anchoredPosition, BtnTargetPos + new Vector3(0, 40, 0), 0.15f);
                BtnImageTf[i].transform.localScale = new Vector3(6.5f, 6.5f, 6.5f);
            }
        }
    }

    public void TabClick(int n)
    {
        targetIndex = n;
        targetPos = pos[n];
    }
}
