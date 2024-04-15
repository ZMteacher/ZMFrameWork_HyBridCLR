
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
        Debug.Log("初始化资源框架...");
        //初始化游戏热更框架
        ZMAsset.Instance.InitFrameWork();
        Debug.Log("资源框架初始化成功 准备热更大厅资源...");
        Debug.Log(Application.persistentDataPath);
        //热更大厅资源
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, StartGame);
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="bundleModule">热更模块</param>
    public static void StartGame()
    {
        UIModule.Instance.Initialize();
        Debug.Log("资源热更完成...进入大厅世界");
        WorldManager.CreateWorld<HallWorld>();
    }
}
