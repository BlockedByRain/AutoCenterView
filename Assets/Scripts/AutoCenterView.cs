using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoCenterView : MonoBehaviour, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 中心容器
    /// </summary>
    public Transform container;
    /// <summary>
    /// 滑动组件
    /// </summary>
    public ScrollRect scrollRect;
    /// <summary>
    /// 布局组件
    /// </summary>
    public LayoutGroup layoutGroup = null;

    /// <summary>
    /// 当前中心元素索引
    /// </summary>
    public int curCenterChildIndex = 0;


    /// <summary>
    /// 当前中心元素
    /// </summary>
    public GameObject curCenterChildItem
    {
        get
        {
            GameObject centerChild = null;
            if (container != null && curCenterChildIndex >= 0 && curCenterChildIndex < container.childCount)
            {
                centerChild = container.GetChild(curCenterChildIndex).gameObject;
            }
            return centerChild;
        }
    }
    /// <summary>
    /// 存储元素位置
    /// </summary>
    [SerializeField]
    private List<float> childPos = new List<float>();
    /// <summary>
    /// 居中目标位置
    /// </summary>
    private float targetPos;
    /// <summary>
    /// 居中速度
    /// </summary>
    public float centerSpeed=20f;
    /// <summary>
    /// 是否居中
    /// </summary>
    public bool centering = true;
    /// <summary>
    /// 是否正在缩放
    /// </summary>
    public bool scaleing = true;
    /// <summary>
    /// 缩放比例
    /// </summary>
    public Vector3 centerChildScale = new Vector3(1.2f, 1.2f, 1.2f);



    public void InitView()
    {
        //获取滑动组件
        scrollRect = GetComponent<ScrollRect>();
        //获取容器
        container = scrollRect.content;
        //移动方式置为无限制
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        //获取布局组件
        layoutGroup = container.GetComponent<LayoutGroup>();
        //获取间隔
        float spacing = 0;

        if (layoutGroup is GridLayoutGroup)
        {
            GridLayoutGroup grid;
            grid = container.GetComponent<GridLayoutGroup>();
            //计算第一个元素的居中位置
            float childPosX = scrollRect.GetComponent<RectTransform>().rect.width * 0.5f - grid.cellSize.x * 0.5f;
            childPos.Add(childPosX);
            //存储所有子物体居中时的位置
            for (int i = 0; i < container.childCount - 1; i++)
            {
                childPosX -= grid.cellSize.x + grid.spacing.x;
                childPos.Add(childPosX);
                
            }
            //将当前容器的X坐标传入，获取当前居中的位置
            targetPos = FindClosestPos(container.localPosition.x);

        }
        else if (layoutGroup is HorizontalLayoutGroup)
        {
            scrollRect.horizontal = true;
            float childPosX = scrollRect.GetComponent<RectTransform>().rect.width * 0.5f - GetChildItemWidth(0) * 0.5f;
            spacing = (layoutGroup as HorizontalLayoutGroup).spacing;
            childPos.Add(childPosX);
            for (int i = 1; i < container.childCount; i++)
            {
                childPosX -= GetChildItemWidth(i) * 0.5f + GetChildItemWidth(i - 1) * 0.5f + spacing;
                childPos.Add(childPosX);
            }

            //将当前容器的X坐标传入，获取当前居中的位置
            targetPos = FindClosestPos(container.localPosition.x);

        }
        else if (layoutGroup is VerticalLayoutGroup)
        {
            scrollRect.vertical = true;
            float childPosY = -scrollRect.GetComponent<RectTransform>().rect.height * 0.5f + GetChildItemHeight(0) * 0.5f;
            spacing = (layoutGroup as VerticalLayoutGroup).spacing;
            childPos.Add(childPosY);
            for (int i = 1; i < container.childCount; i++)
            {
                childPosY += GetChildItemHeight(i) * 0.5f + GetChildItemHeight(i - 1) * 0.5f + spacing;
                childPos.Add(childPosY);
            }

            //将当前容器的Y坐标传入，获取当前居中的位置
            targetPos = FindClosestPos(container.localPosition.y);
        }

    }



    public void OnDrag(PointerEventData eventData)
    {
        centering = false;
        if (layoutGroup is GridLayoutGroup || layoutGroup is HorizontalLayoutGroup)
        {
            targetPos = FindClosestPos(container.localPosition.x);
        }
        else if (layoutGroup is VerticalLayoutGroup)
        {
            targetPos = FindClosestPos(container.localPosition.y);

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (layoutGroup is GridLayoutGroup || layoutGroup is HorizontalLayoutGroup)
        {
            targetPos = FindClosestPos(container.localPosition.x);
        }
        else if (layoutGroup is VerticalLayoutGroup)
        {
            targetPos = FindClosestPos(container.localPosition.y);

        }
        centering = true;
    }



    /// <summary>
    /// 寻找最近的中心元素
    /// </summary>
    /// <param name="currentPos">当前位置，可传入X或Y</param>
    /// <returns>返回相应的位置坐标</returns>
    private float FindClosestPos(float currentPos)
    {
        int childIndex = 0;
        float closest = 0;
        curCenterChildIndex = -1;
        float distance = Mathf.Infinity;

        //查找最近的子物体位置
        for (int i = 0; i < childPos.Count; i++)
        {
            float p = childPos[i];
            float d = Mathf.Abs(p - currentPos);
            if (d < distance)
            {
                distance = d;
                closest = p;
                childIndex = i;
            }
        }

        //设置当前中心元素索引
        curCenterChildIndex = childIndex;
        return closest;

    }

    /// <summary>
    /// 改变居中元素
    /// </summary>
    /// <param name="index">目标索引</param>
    public void SetCenterChild(int index)
    {
        curCenterChildIndex = index;
        targetPos = childPos[index];
    }


    private float GetChildItemWidth(int index)
    {
        return (container.GetChild(index) as RectTransform).sizeDelta.x;
    }

    private float GetChildItemHeight(int index)
    {
        return (container.GetChild(index) as RectTransform).sizeDelta.y;
    }




    private void Awake()
    {
        InitView();
    }

    private void Update()
    {
        if (centering)
        {
            if (layoutGroup is GridLayoutGroup || layoutGroup is HorizontalLayoutGroup)
            {
                Vector3 v = container.localPosition;
                v.x = Mathf.Lerp(container.localPosition.x, targetPos, centerSpeed * Time.deltaTime);
                container.localPosition = v;
            }

            else if (layoutGroup is VerticalLayoutGroup)
            {
                Vector3 v = container.localPosition;
                v.y = Mathf.Lerp(container.localPosition.y, targetPos, centerSpeed * Time.deltaTime);
                container.localPosition = v;
            }

        }


        //居中缩放
        if (scaleing)
        {
            //插值
            for (int i = 0; i < container.childCount; i++)
            {
                if (i == curCenterChildIndex)
                {
                    //放大
                    container.GetChild(i).transform.localScale =
                        new Vector3(Mathf.Lerp(curCenterChildItem.transform.localScale.x, centerChildScale.x, centerSpeed * Time.deltaTime),
                                    Mathf.Lerp(curCenterChildItem.transform.localScale.y, centerChildScale.y, centerSpeed * Time.deltaTime),
                                    Mathf.Lerp(curCenterChildItem.transform.localScale.z, centerChildScale.z, centerSpeed * Time.deltaTime)
                                    );
                }
                else
                {
                    //复原
                    container.GetChild(i).transform.localScale =
                        new Vector3(Mathf.Lerp(container.GetChild(i).transform.localScale.x, 1f, centerSpeed * Time.deltaTime),
                                    Mathf.Lerp(container.GetChild(i).transform.localScale.y, 1f, centerSpeed * Time.deltaTime),
                                    Mathf.Lerp(container.GetChild(i).transform.localScale.z, 1f, centerSpeed * Time.deltaTime)
                                    );
                }
            }
        }
    }


}
