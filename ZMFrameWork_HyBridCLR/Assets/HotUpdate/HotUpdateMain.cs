
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;
using ZMGC.Hall;

public class HotUpdateMain : MonoBehaviour
{
    /// <summary>
    /// 初始化游戏环境
    /// </summary>
    public  static void InitGameEnv()
    {
        UIModule.Instance.Initialize();
        Debug.Log("资源热更完成...进入大厅世界");
        WorldManager.CreateWorld<HallWorld>();
    }
   
}
