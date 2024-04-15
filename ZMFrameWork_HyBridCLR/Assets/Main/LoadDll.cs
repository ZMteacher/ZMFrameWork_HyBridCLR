using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public class LoadDll : MonoBehaviour
{
    /// <summary>
    /// 热更Dll下载地址
    /// </summary>
    private string m_HotDllDownLoadUrl = "http://192.168.2.74/HotUpdateDlls/";
    /// <summary>
    /// 补元数据DLL下载地址
    /// </summary>
    private string m_AOTMetaDllDownLoadUrl = "http://192.168.2.74/AssembliesPostIl2CppStrip/";
    /// <summary>
    /// 热更程序集
    /// </summary>
    private Assembly m_HotUpdateAssembly;
    void Start()
    {
         StartCoroutine(DownLoad_DLL(this.StartGame));
    }
    #region Dll 热更
    /// <summary>
    /// 获取DLL下载地址
    /// </summary>
    private string GetCurPlatformDownLoadDllURL(bool isAOTMate)
    {
        string donwLoadUrl = isAOTMate ? m_AOTMetaDllDownLoadUrl : m_HotDllDownLoadUrl;
#if UNITY_EDITOR
        return donwLoadUrl + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
#else
        if (Application.platform == RuntimePlatform.Android)
        {
            return donwLoadUrl + "Android/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return donwLoadUrl + "iOS/";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return donwLoadUrl + "StandaloneWindows64/"; 
        }
        else
	    {
              return donwLoadUrl;
	    }
#endif
    }


    private static Dictionary<string, byte[]> m_DLLBytesDic = new Dictionary<string, byte[]>();

    public static byte[] ReadBytesFromStreamingAssets(string dllName)
    {
        return m_DLLBytesDic[dllName];
    }


    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "mscorlib.dll.bytes",
        "System.dll.bytes",
        "System.Core.dll.bytes",
    };

    IEnumerator DownLoad_DLL(Action onDownloadComplete)
    {
        var dllNameList = new List<string>
        {
            "HotUpdate.dll.bytes",
        }.Concat(AOTMetaAssemblyFiles);

        foreach (string dllName in dllNameList)
        {
            string dllDownLoadPath = GetCurPlatformDownLoadDllURL(!dllName.Contains("HotUpdate"))+ dllName;
            Debug.Log($"start download dll:{dllDownLoadPath}");
            UnityWebRequest www = UnityWebRequest.Get(dllDownLoadPath);
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{dllName}  size:{assetData.Length}");
                //m_DLLBytesDic[asset] = assetData;
                m_DLLBytesDic.Add(dllName, assetData);
            }
        }
        onDownloadComplete();
    }
#endregion

    private void StartGame()
    {
        LoadMetadataForAOTAssemblies();
        byte[] bytes = ReadBytesFromStreamingAssets("HotUpdate.dll.bytes");
        m_HotUpdateAssembly = Assembly.Load(bytes);
        Type entryType = m_HotUpdateAssembly.GetType("HotUpdateMain");
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
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

}
