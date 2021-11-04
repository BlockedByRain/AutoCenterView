using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCenterController : MonoBehaviour
{
    /// <summary>
    /// 元素列表
    /// </summary>
    public List<AutoCenterItem> autoCenterItems = new List<AutoCenterItem>();

    /// <summary>
    /// 居中脚本
    /// </summary>
    public AutoCenterView autoCenterView;


    /// <summary>
    /// 点击触发事件
    /// </summary>
    /// <param name="index"></param>
    public void OnItemClick(int index)
    {
        autoCenterView.SetCenterChild(index);
    }

    public void InitController()
    {
        //获取脚本
        autoCenterView = GetComponent<AutoCenterView>();

        Transform container = autoCenterView.container;
        //获取所有容器内元素
        for (int i = 0; i < container.childCount; i++)
        {
            AutoCenterItem temp = container.GetChild(i).GetComponent<AutoCenterItem>();

            autoCenterItems.Add(temp);
            //元素初始化
            temp.InitAutoCenterItem();

            //给按钮绑定点击事件
            temp.itemBtn.onClick.AddListener(() =>
            {
                OnItemClick(temp.itemIndex);

            });
        }
    }
    

    private void Start()
    {
        InitController();
    }


}





