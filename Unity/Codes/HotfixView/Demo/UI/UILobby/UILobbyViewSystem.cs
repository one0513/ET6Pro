using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UILobbyView))]
	public class UILobbyViewOnCreateSystem : OnCreateSystem<UILobbyView>
	{

		public override void OnCreate(UILobbyView self)
		{
			self.Panel = self.AddUIComponent<UIImage>("Panel");
			self.btnClienTest = self.AddUIComponent<UIButton>("Panel/btnClienTest");
			self.btnServerTest = self.AddUIComponent<UIButton>("Panel/btnServerTest");
			self.btnClienTest.SetOnClick(()=>{self.OnClickbtnClienTest();});
			self.btnServerTest.SetOnClick(()=>{self.OnClickbtnServerTest().Coroutine();});
		}

	}
	[UISystem]
	[FriendClass(typeof(UILobbyView))]
	public class UILobbyViewOnEnableSystem : OnEnableSystem<UILobbyView, Scene>
	{
		public override void OnEnable(UILobbyView self, Scene scene)
		{
			self.scene = scene;
		}
	}
	
	[ObjectSystem]
	[FriendClass(typeof(UILobbyView))]
	public class UILobbyViewLoadSystem : LoadSystem<UILobbyView>
	{

		public override void Load(UILobbyView self)
		{
			self.btnClienTest.SetOnClick(()=>{self.OnClickbtnClienTest();});
			self.btnServerTest.SetOnClick(()=>{self.OnClickbtnServerTest().Coroutine();});
		}

	}
	[FriendClass(typeof(UILobbyView))]
	public static class UILobbyViewSystem
	{
		public static void OnClickbtnClienTest(this UILobbyView self)
		{
			Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "客户端代码热更测试 更新啦。。。。" }).Coroutine();
		}
		public static async ETTask  OnClickbtnServerTest(this UILobbyView self)
		{
			Session session = self.scene.GetComponent<SessionComponent>().Session;
			A2C_TestSendMsg a2CTest = (A2C_TestSendMsg) await session.Call(new C2A_TestSendMsg(){});
			
			Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = $"服务端发来的信息：{a2CTest.testMsg}" }).Coroutine();
		}
	}

}
