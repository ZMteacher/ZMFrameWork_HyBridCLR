
using dnlib.DotNet.Writer;
using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZM.AssetFrameWork;
//using ZMGC.Hall;

public class Main : MonoBehaviour
{
 
    private void Awake()
    {
        Debug.Log("初始化资源框架...");
        //初始化游戏热更框架
        ZMAsset.Instance.InitFrameWork();
        Debug.Log("资源框架初始化成功 准备热更大厅资源...");
        //热更大厅资源
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, StartGame);
    }
 
    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        //AOT
        "mscorlib.dll.bytes",
        "System.dll.bytes",
        "System.Core.dll.bytes",
    };
    public void StartGame()
    {
        Debug.Log("Hall Asset Hot Finish... StartScriptsEnv...");
        LoadMetadataForAOTAssemblies();
        byte[] dllBytes = ZMAsset.LoadTextAsset("Assets/GameData/Hall/DLL/HotUpdate.dll.bytes").bytes;

        Assembly hotUpdateAssembly = Assembly.Load(dllBytes);
        Type entryType = hotUpdateAssembly.GetType("HotUpdateMain");
        entryType.GetMethod("InitGameEnv").Invoke(null, null);
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            //byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            byte[] dllBytes = ZMAsset.LoadTextAsset("Assets/GameData/Hall/DLL/"+aotDllName).bytes;
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
}
