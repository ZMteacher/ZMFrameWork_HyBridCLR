﻿/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAssetFrameWork
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ZM.AssetFrameWork
{
    /// <summary>
    /// 热更资源模块
    /// </summary>
    public class HotAssetsModule
    {
        /// <summary>
        /// 热更资源下载储存路径
        /// </summary>
        public string HotAssetsSavePath { get { return Application.persistentDataPath + "/HotAssets/" + CurBundleModuleEnum + "/"; } }
        /// <summary>
        /// 所有热更的资源列表
        /// </summary>
        public List<HotFileInfo> mAllHotAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 需要下载的资源列表
        /// </summary>
        public List<HotFileInfo> mNeedDownLoadAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 服务端资源清单
        /// </summary>
        private HotAssetsManifest mServerHotAssetsManifest;
        /// <summary>
        /// 本地资源清单
        /// </summary>
        private HotAssetsManifest mLocalHotAssetsManifest;
        /// <summary>
        /// 服务端资源热更清单储存路径
        /// </summary>
        private string mServerHotAssetsManifestPath;
        /// <summary>
        /// 本地资源热更清单文件储存路径
        /// </summary>
        private string mLocalHotAssetManifestPath;
        /// <summary>
        /// 热更公告
        /// </summary>
        public string UpdateNoticeContent { get { return mServerHotAssetsManifest.updateNotice; } }
        /// <summary>
        /// 当前下载的资源模块类型
        /// </summary>
        public BundleModuleEnum CurBundleModuleEnum { get; set; }
        /// <summary>
        /// 最大下载资源大小
        /// </summary>
        public float AssetsMaxSizeM { get; set; }
        /// <summary>
        /// 资源已经下载的大小
        /// </summary>
        public float AssetsDownLoadSizeM;
        /// <summary>
        /// 资源下载器
        /// </summary>
        private AssetsDownLoader mAssetsDownLoader;
        /// <summary>
        /// AssetBundle配置文件下载完成监听
        /// </summary>
        public Action<string> OnDownLoadABConfigListener;
        /// <summary>
        /// 下载AssetBundle完成的回调
        /// </summary>
        public Action<string> OnDownLoadAssetBundleListener;
        /// <summary>
        /// 所有热更资源的一个长度
        /// </summary>
        public int HotAssetCount { get { return mAllHotAssetsList.Count; } }

        private MonoBehaviour mMono;
        /// <summary>
        /// 下载所有资源完成的回调
        /// </summary>
        public Action<BundleModuleEnum> OnDownLoadAllAssetsFinish;
        public HotAssetsModule(BundleModuleEnum bundleModule,MonoBehaviour mono)
        {
            mMono = mono;
            CurBundleModuleEnum = bundleModule;
        }
        /// <summary>
        /// 开始热更资源
        /// </summary>
        /// <param name="startDownLoadCallback">开始下载的回调</param>
        /// <param name="hotFinish">热更完成回调</param>
        /// <param name="isCheckAssetsVersion">是否检测资源版本</param>
        public void StartHotAssets(Action startDownLoadCallback,Action<BundleModuleEnum> hotFinish=null,bool isCheckAssetsVersion=true)
        {
            this.OnDownLoadAllAssetsFinish += hotFinish;
            if (isCheckAssetsVersion)
            {
                //检测资源版本是否需要热更
                CheckAssetsVersion((isHot,size)=> {
                    if (isHot)
                    {
                        Debug.Log("线程_-2");
                        StartDownLoadHotAssets(startDownLoadCallback);
                    }
                    else
                    {
                        OnDownLoadAllAssetsFinish?.Invoke(CurBundleModuleEnum);
                    }
                });
            }
        }
        /// <summary>
        /// 开始下载热更资源
        /// </summary>
        /// <param name="startDonwLoadCallBack"></param>
        public void StartDownLoadHotAssets(Action startDonwLoadCallBack)
        {
            //优先下载AssetBUndle配置文件，下载完成后呢，调用回调，让开发者及时加载配置文件
            //热更资源下载完成之后同样给与回调，供开发者动态加载刚下载完成的资源
            List<HotFileInfo> downLoadList = new List<HotFileInfo>();
            for (int i = 0; i < mNeedDownLoadAssetsList.Count; i++)
            {
                HotFileInfo hotFile = mNeedDownLoadAssetsList[i];
                //如果包含Config 说明是配置文件，需要优先下载
                if (hotFile.abName.Contains("config"))
                {
                    downLoadList.Insert(0, hotFile);
                }
                else
                {
                    downLoadList.Add(hotFile);
                }
            }
            //获取资源下载队列
            Queue<HotFileInfo> downLoadQueue = new Queue<HotFileInfo>();
            foreach (var item in downLoadList)
            {
                downLoadQueue.Enqueue(item);
            }
            //通过资源下载器，开始下载资源
            mAssetsDownLoader = new AssetsDownLoader(this,downLoadQueue,mServerHotAssetsManifest.downLoadURL,HotAssetsSavePath
                ,DownLoadAssetBundleSuccess,DownLoadAssetBundleFailed,DownLoadAllAssetBundleFinish);
            Debug.Log("线程_-1");
            startDonwLoadCallBack?.Invoke();
            //开始下载队列中的资源
            mAssetsDownLoader.StartThreadDownLoadQueue();

        }
        /// <summary>
        /// 检测资源版本
        /// </summary>
        /// <param name="checkCallBack"></param>
        public void CheckAssetsVersion(Action<bool,float> checkCallBack)
        {

            GeneratorHotAssetsManifest();
            mNeedDownLoadAssetsList.Clear();
            mMono.StartCoroutine(DownLoadHotAssetsManifest(()=> {
                //资源清单下载完成
                //1.检测当前版本是否需要热更
                if (CheckModuleAssetsIsHot())
                {
                    HotAssetsPatch serverHotPath = mServerHotAssetsManifest.hotAssetsPatchList[mServerHotAssetsManifest.hotAssetsPatchList.Count - 1];
                    bool isNeedHot= ComputeNeedHotAssetsList(serverHotPath);
                    if (isNeedHot)
                    {
                        Debug.Log("线程_-3");
                        checkCallBack?.Invoke(true,AssetsMaxSizeM);
                    }
                    else
                    {
                        checkCallBack?.Invoke(false, 0);
                    }
                }
                else
                {
                    checkCallBack?.Invoke(false, 0);
                }
                //2.如果需要热更，开始计算需要下载的文件 开始下载文件
                //3.如果不需要热更，说明文件是最新的，直接热更完成

            }));
        }
        /// <summary>
        /// 计算需要热更的文件列表
        /// </summary>
        /// <param name="serverAssetsPath"></param>
        /// <returns></returns>
        public bool ComputeNeedHotAssetsList(HotAssetsPatch serverAssetsPath)
        {
            if (!Directory.Exists(HotAssetsSavePath))
            {
                Directory.CreateDirectory(HotAssetsSavePath);
            }
            if(File.Exists(mLocalHotAssetManifestPath))
                mLocalHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(File.ReadAllText(mLocalHotAssetManifestPath));
            AssetsMaxSizeM = 0;
            foreach (var item in serverAssetsPath.hotAssetsList)
            {
                //获取本地AssetBundle文件路径
                string localFilePath = HotAssetsSavePath + item.abName;
                mAllHotAssetsList.Add(item);
                //如果本地文件不存在，或者本地文件与服务端不一致，就需要热更
                string localMd5= GetLocalFileMd5ByBundleName(item.abName);
                if (!File.Exists(localFilePath)||item.md5!= localMd5)
                {
                    mNeedDownLoadAssetsList.Add(item);
                    AssetsMaxSizeM += item.size / 1024f;
                }
            }
            
            return mNeedDownLoadAssetsList.Count > 0;
        }
        public string GetLocalFileMd5ByBundleName(string bundleName)
        {
            if (mLocalHotAssetsManifest!=null&& mLocalHotAssetsManifest.hotAssetsPatchList.Count>0)
            {
                HotAssetsPatch localPatch = mLocalHotAssetsManifest.hotAssetsPatchList[mLocalHotAssetsManifest.hotAssetsPatchList.Count-1];
                foreach (var item in localPatch.hotAssetsList)
                {
                    if (string.Equals(bundleName,item.abName))
                    {
                        return item.md5;
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// 检测模块资源是否需要热更
        /// </summary>
        /// <returns></returns>
        public bool CheckModuleAssetsIsHot()
        {
            //如果服务端资源清单不存，不需要热更
            if (mServerHotAssetsManifest==null)
            {
                return false;   
            }
            //如果本地资源清单文件不存在，说明我们需要热更
            if (!File.Exists(mLocalHotAssetManifestPath))
            {
                return true;
            }
            //判断本地资源清单补丁版本号是否与服务端资源清单补丁版本号一致，如果一致，不需要热更， 如果不一致，则需要热更
            HotAssetsManifest localHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(File.ReadAllText(mLocalHotAssetManifestPath));
            if (localHotAssetsManifest.hotAssetsPatchList.Count==0&&mServerHotAssetsManifest.hotAssetsPatchList.Count!=0)
            {
                return true;
            }
         
            //获取本地热更补丁的最后一个补丁
            HotAssetsPatch localHotPatch = localHotAssetsManifest.hotAssetsPatchList[localHotAssetsManifest.hotAssetsPatchList.Count - 1];
            //获取服务端热更补丁的最后一个补丁
            HotAssetsPatch serverHotPatch = mServerHotAssetsManifest.hotAssetsPatchList[mServerHotAssetsManifest.hotAssetsPatchList.Count - 1];

            if (localHotPatch!=null&& serverHotPatch!=null)
            {
                if (localHotPatch.patchVersion!=serverHotPatch.patchVersion)
                {
                    return true;
                }
                //else
                //{
                //    return false;
                //}
            }

            if (serverHotPatch!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 下载资源热更清单
        /// </summary>
        /// <returns></returns>
        private IEnumerator DownLoadHotAssetsManifest(Action downLoadFinish)
        {
            string url = BundleSettings.Instance.AssetBundleDownLoadUrl + "/HotAssets/" + CurBundleModuleEnum + "AssetsHotManifest.json";
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.timeout = 30;

            Debug.Log("*** Requset HotAssetsMainfest Url:"+ url);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("DownLoad Error:"+webRequest.error);
            }
            else
            {
                try
                {
                    Debug.Log("*** Request AssetBundle HotAssetsMainfest Url Finish Module:"+CurBundleModuleEnum 
                        +" txt:"+webRequest.downloadHandler.text);
                    //写入服务端资源热更清单到本地
                    Debug.Log("mLocalHotAssetManifestPath" + mLocalHotAssetManifestPath);
                    Debug.Log("mServerHotAssetsManifestPath" + mServerHotAssetsManifestPath);
                    FileHelper.WriteFile(mServerHotAssetsManifestPath, webRequest.downloadHandler.data);
                    mServerHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(webRequest.downloadHandler.text);
                    Debug.Log("写入服务端资源热更清单到本地-4" + mServerHotAssetsManifest);

                }
                catch (Exception e)
                {
                    Debug.Log("服务端资源清单配置下载异常，文件不存在或者配置有问题，更新出错，请检查_线程_-4");
                    Debug.LogError("服务端资源清单配置下载异常，文件不存在或者配置有问题，更新出错，请检查："+e.ToString());
                 }
            }
            downLoadFinish?.Invoke();
        }
        public void GeneratorHotAssetsManifest()
        {
            mServerHotAssetsManifestPath = Application.persistentDataPath + "/Server" + CurBundleModuleEnum + "AssetsHotManifest.json";
            mLocalHotAssetManifestPath = Application.persistentDataPath + "/Local" + CurBundleModuleEnum + "AssetsHotManifest.json";
        }

#region 资源下载回调
        public void DownLoadAssetBundleSuccess(HotFileInfo hotFile)
        {

            string abName = hotFile.abName.Contains(".")? hotFile.abName.Replace(BundleSettings.ABSUFFIX, "") : hotFile.abName;
            if (hotFile.abName.Contains("bundleconfig"))
            {
                OnDownLoadABConfigListener?.Invoke(abName);
                //如果下载成功需要及时去加载配置文件
                AssetBundleManager.Instance.LoadAssetBundleConfig(CurBundleModuleEnum);
            }
            else
            {
                OnDownLoadAssetBundleListener?.Invoke(abName);
            }
            HotAssetsManager.DownLoadBundleFinish?.Invoke(hotFile);
        }

        public void DownLoadAssetBundleFailed(HotFileInfo hotFile)
        {

        }

        public void DownLoadAllAssetBundleFinish (HotFileInfo hotFile)
        {
            if (File.Exists(mLocalHotAssetManifestPath))
            {
                File.Delete(mLocalHotAssetManifestPath);
            }
            //把服务端热更清单文件考本到本地
            File.Copy(mServerHotAssetsManifestPath, mLocalHotAssetManifestPath);
            OnDownLoadAllAssetsFinish?.Invoke(CurBundleModuleEnum);
        }
#endregion

        public void OnMainThreadUpdate()
        {
            mAssetsDownLoader?.OnMainThreadUpdate();
        }
        /// <summary>
        /// 设置下载线程个数
        /// </summary>
        /// <param name="threadCount"></param>
        public void SetDownLoadThreadCount(int threadCount)
        {
            Debug.Log("多线程负载均衡:"+threadCount+" ModuleType:"+CurBundleModuleEnum);
            if (mAssetsDownLoader!=null)
            {
                mAssetsDownLoader.MAX_THREAD_COUNT = threadCount;
            }
        }
        /// <summary>
        /// 判断热更文件是否存在
        /// </summary>
        public bool HotAssetsIsExists(string bundleName)
        {
            foreach (var item in mAllHotAssetsList)
            {
                if (string.Equals(bundleName,item.abName))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
