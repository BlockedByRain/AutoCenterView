using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AutoCenterItem : MonoBehaviour
{
    /// <summary>
    /// 按钮组件
    /// </summary>
    public Button itemBtn;
    /// <summary>
    /// 图片组件
    /// </summary>
    public Image itemImg;
    /// <summary>
    /// 索引
    /// </summary>
    public int itemIndex;

    private void Start()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitAutoCenterItem()
    {
        itemImg = GetComponent<Image>();
        itemBtn = GetComponent<Button>();

    }

}
