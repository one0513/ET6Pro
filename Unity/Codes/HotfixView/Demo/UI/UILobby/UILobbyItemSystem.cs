using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UILobbyItem))]
	public class UILobbyItemOnCreateSystem : OnCreateSystem<UILobbyItem>
	{

		public override void OnCreate(UILobbyItem self)
		{
			self.lblServerName = self.AddUIComponent<UITextmesh>("lblServerName");
			self.buttonClick = self.AddUIComponent<UIButton>();
			self.buttonClick.SetOnClick(()=>{self.OnClickbuttonClick();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UILobbyItem))]
	public class UILobbyItemLoadSystem : LoadSystem<UILobbyItem>
	{

		public override void Load(UILobbyItem self)
		{
		}

	}
	[FriendClass(typeof(UILobbyItem))]
	[FriendClass(typeof(UITextmesh))]
	[FriendClass(typeof(ServerInfo))]
	[FriendClass(typeof(ServerInfosComponent))]
	public static class UILobbyItemSystem
	{

		public static void OnClickbuttonClick(this UILobbyItem self)
		{
			if (self.itemData.status != (int)ServerStatus.Normal)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "当前服务器正在维护" }).Coroutine();
				return;
			}
			self.scene.GetComponent<ServerInfosComponent>().curServerId = (int)self.itemData.Id;
			Game.EventSystem.Publish(new EventType.ChangServer(){serverInfo = self.itemData});
			
		}
		public static void SetData(this UILobbyItem self, ServerInfo date)
		{
			self.itemData = date;
			self.lblServerName.SetText(date.serverName);
		}
	}

}
