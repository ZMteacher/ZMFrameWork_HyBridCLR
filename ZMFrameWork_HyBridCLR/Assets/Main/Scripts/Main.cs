
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;
//using ZMGC.Hall;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    //Assets/Hall/Prefab/LoginWindow.prefab
    private void Awake()
    {
       
       
       
    }

    void Start()  
    {
       
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public static void StartGameGo()
    {
        Debug.Log("热更大厅资源22");
        //初始化游戏热更框架
        ZMAsset.Instance.InitFrameWork();
        Debug.Log("热更大厅资源23");
        Debug.Log(Application.persistentDataPath);
        //热更大厅资源
        ZMAsset.HotAssets(BundleModuleEnum.Hall, null, (BundleModuleEnum) => {
           
           // UIModule.Instance.Initialize();
            Debug.Log("热更大厅资源22");
           // WorldManager.CreateWorld<HallWorld>();

        }, null);
       // HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, this);
        
    }
    public  void StartGame()
    {
        ZMAsset.HotAssets(BundleModuleEnum.Hall, null, (BundleModuleEnum) => {

          //  UIModule.Instance.Initialize();
            Debug.Log("热更大厅资源22");
           // WorldManager.CreateWorld<HallWorld>();

        }, null);
    }

}
