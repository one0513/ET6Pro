using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using YooAsset;
using System.Linq;

namespace ET
{
    [UISystem]
    [FriendClass(typeof(UIUpdateView))]
    public class UIUpdateViewOnCreateSystem : OnCreateSystem<UIUpdateView>
    {
        public override void OnCreate(UIUpdateView self)
        {
            self.Slider = self.AddUIComponent<UISlider>("Loadingscreen/Slider");
        }
    }
    [UISystem]
    [FriendClass(typeof(UIUpdateView))]
    public class UIUpdateViewOnEnableSystem : OnEnableSystem<UIUpdateView,Action>
    {
        public override void OnEnable(UIUpdateView self,Action func)
        {
            self.ForceUpdate = Define.ForceUpdate;
            self.OnOver = func;
            self.LastProgress = 0;
            self.Slider.SetValue(0);
            //如果这个界面依赖了其他没加载过的ab包，等会提示下载前会自动下载依赖包，所以这里需要提前预加载
            GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(UIMsgBoxWin.PrefabPath,1).Coroutine();
            
        }
    }
    [FriendClass(typeof(UIUpdateView))]
    public static class UIUpdateViewSystem
    {
        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        static void SetProgress(this UIUpdateView self, float value)
        {
            if(value> self.LastProgress)
                self.LastProgress = value;
            self.Slider.SetNormalizedValue(self.LastProgress);
        }
        /// <summary>
        /// 提示窗
        /// </summary>
        /// <param name="self"></param>
        /// <param name="content"></param>
        /// <param name="confirmBtnText"></param>
        /// <param name="cancelBtnText"></param>
        /// <returns></returns>
        async static ETTask<int> ShowMsgBoxView(this UIUpdateView self,string content, string confirmBtnText, string cancelBtnText)
        {
            ETTask<int> tcs = ETTask<int>.Create();
            Action confirmBtnFunc = () =>
             {
                 tcs.SetResult(self.BTN_CONFIRM);
             };

            Action cancelBtnFunc = () =>
            {
                tcs.SetResult(self.BTN_CANCEL);
            };
            I18NComponent.Instance.I18NTryGetText(content, out self.Para.Content);
            I18NComponent.Instance.I18NTryGetText(confirmBtnText, out self.Para.ConfirmText);
            I18NComponent.Instance.I18NTryGetText(cancelBtnText, out self.Para.CancelText);
            self.Para.ConfirmCallback = confirmBtnFunc;
            self.Para.CancelCallback = cancelBtnFunc;
            await UIManagerComponent.Instance.OpenWindow<UIMsgBoxWin, UIMsgBoxWin.MsgBoxPara>(UIMsgBoxWin.PrefabPath,
                self.Para,UILayerNames.TipLayer);
            var result = await tcs;
            await UIManagerComponent.Instance.CloseWindow<UIMsgBoxWin>();
            return result;
        }
        
        
        /// <summary>
        /// 白名单
        /// </summary>
        /// <param name="self"></param>
        async static ETTask CheckIsInWhiteList(this UIUpdateView self)
        {
            var url = ServerConfigComponent.Instance.GetWhiteListCdnUrl();
            if (string.IsNullOrEmpty(url))
            {
                Log.Info(" no white list cdn url");
                return;
            }
            var info = await HttpManager.Instance.HttpGetResult<List<WhiteConfig>>(url);
            if (info != null)
            {
                ServerConfigComponent.Instance.SetWhiteList(info);
                if (ServerConfigComponent.Instance.IsInWhiteList())
                {
                    var btnState = await self.ShowMsgBoxView("Update_White", "Global_Btn_Confirm", "Global_Btn_Cancel");
                    if (btnState == self.BTN_CONFIRM)
                    {
                        ServerConfigComponent.Instance.SetWhiteMode(true);
                    }
                }
                return;
            }
        }

        /// <summary>
        /// 版本号信息
        /// </summary>
        /// <param name="self"></param>
        async static ETTask CheckUpdateList(this UIUpdateView self)
        {
            var url = ServerConfigComponent.Instance.GetUpdateListCdnUrl();
            // UpdateConfig aa = new UpdateConfig
            // {
            //     app_list = new Dictionary<string, AppConfig>
            //     {
            //         {
            //             "googleplay",
            //             new AppConfig()
            //             {
            //                 app_url = "http://127.0.0.1",
            //                 app_ver = new Dictionary<int, Resver>()
            //                 {
            //                     {
            //                         1,
            //                         new Resver()
            //                         {
            //                             channel = new List<string>() { "all" },
            //                             update_tailnumber = new List<string>() { "all" },
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     },
            //     res_list = new Dictionary<string, Dictionary<int, Resver>>
            //     {
            //         {
            //             "googleplay",
            //             new Dictionary<int, Resver>
            //             {
            //                 {
            //                     1,
            //                     new Resver
            //                     {
            //                         channel = new List<string>() { "all" }, update_tailnumber = new List<string>() { "all" },
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // };
            var info = await HttpManager.Instance.HttpGetResult<UpdateConfig>(url);
            if (info == null)
            {
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", self.ForceUpdate?"Btn_Exit":"Update_Skip");
                if (btnState == self.BTN_CONFIRM)
                {
                    await self.CheckUpdateList();
                }
                else if(self.ForceUpdate)
                {
                    GameUtility.Quit();
                    return;
                }
            }
            else
            {
                ServerConfigComponent.Instance.SetUpdateList(info);
            }
        }

        /// <summary>
        /// 是否需要整包更新
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        async static ETTask<bool> CheckAppUpdate(this UIUpdateView self)
        {
            var appChannel = PlatformUtil.GetAppChannel();
            var channelAppUpdateList = ServerConfigComponent.Instance.GetAppUpdateListByChannel(appChannel);
            if (channelAppUpdateList == null || channelAppUpdateList.app_ver == null)
            {
                Log.Info("CheckAppUpdate channel_app_update_list or app_ver is nil, so return");
                return false;
            }
            var version = ServerConfigComponent.Instance.FindMaxUpdateAppVer(appChannel);
            Log.Info("FindMaxUpdateAppVer =" + version);
            if (version < 0)
            {
                Log.Info("CheckAppUpdate maxVer is nil");
                return false;
            }
            //x.x.xxx这种的话，这里就自己改一下
            int appVer = int.Parse(Application.version);
            var flag = appVer - version;
            Log.Info(string.Format("CoCheckAppUpdate AppVer:{0} maxVer:{1}", appVer, version));
            if (flag >= 0)
            {
                Log.Info("CheckAppUpdate AppVer is Most Max Version, so return; flag = " + flag);
                return false;
            }

            var appURL = channelAppUpdateList.app_url;
            var verInfo = channelAppUpdateList.app_ver[appVer];
            Log.Info("CheckAppUpdate app_url = " + appURL);

            self.ForceUpdate = Define.ForceUpdate; 
            if (Define.ForceUpdate)//默认强更
            {
                if (verInfo != null && verInfo.force_update == 0)
                    self.ForceUpdate = false;
            }
            else
            {
                if (verInfo != null && verInfo.force_update != 0)
                    self.ForceUpdate = true;
            }


            var cancelBtnText = self.ForceUpdate ? "Btn_Exit" : "Btn_Enter_Game";
            var content_updata = self.ForceUpdate ? "Update_ReDownload" : "Update_SuDownload";
            var btnState = await self.ShowMsgBoxView(content_updata, "Global_Btn_Confirm", cancelBtnText);

            if (btnState == self.BTN_CONFIRM)
            {
                GameUtility.OpenURL(appURL);
                //为了防止切换到网页后回来进入了游戏，所以这里需要继续进入该流程
                return await self.CheckAppUpdate();
            }
            else if(self.ForceUpdate)
            {
                Log.Info("CheckAppUpdate Need Force Update And User Choose Exit Game!");
                GameUtility.Quit();
                return true;
            }
            return false;
        }
        
      

    }
}
