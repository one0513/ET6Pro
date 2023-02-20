using System.Collections;
using System.Collections.Generic;
using System;
using ET.EventType;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[FriendClass(typeof(ServerInfo))]
	[FriendClass(typeof(UITextmesh))]
	public class ChangServerHandler : AEvent<EventType.ChangServer>
	{
		protected override void Run(ChangServer a)
		{
			if (UILobbyView.Instance != null)
			{
				UILobbyView.Instance.lblServer.SetText(a.serverInfo.serverName);
				PlayerPrefs.SetInt(CacheKeys.ServerID, (int)a.serverInfo.Id);
			}
		}
	}

	[UISystem]
	[FriendClass(typeof(UILobbyView))]
	public class UILobbyViewOnCreateSystem : OnCreateSystem<UILobbyView>
	{

		public override void OnCreate(UILobbyView self)
		{
			self.btnStart = self.AddUIComponent<UIButton>("btnStart");
			self.ScrollView = self.AddUIComponent<UILoopListView2>("ScrollView");
			self.lblServer = self.AddUIComponent<UITextmesh>("sp/lblServer");
			self.btnStart.SetOnClick(()=>{self.OnClickbtnStart();});
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
		}

	}
	
	[UISystem]
	[FriendClass(typeof(UILobbyView))]
	[FriendClass(typeof(ServerInfo))]
	[FriendClass(typeof(UITextmesh))]
	public class UILobbyViewOnEnableSystem : OnEnableSystem<UILobbyView,List<ServerInfo>>
	{
		
		public override void OnEnable(UILobbyView self, List<ServerInfo> data)
		{
			UILobbyView.Instance = self;
			self.listData = data;
			self.ScrollView.SetListItemCount(data.Count);

			int localServerID = PlayerPrefs.GetInt(CacheKeys.ServerID) != 0? PlayerPrefs.GetInt(CacheKeys.ServerID) : self.listData.Count;
			string serverName = self.listData[localServerID - 1].serverName;
			self.lblServer.SetText(serverName);
		}
	}
	
	[ObjectSystem]
	[FriendClass(typeof(UILobbyView))]
	public class UILobbyViewLoadSystem : LoadSystem<UILobbyView>
	{

		public override void Load(UILobbyView self)
		{
			self.btnStart.SetOnClick(()=>{self.OnClickbtnStart();});
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
		}

	}
	[FriendClass(typeof(UILobbyView))]
	public static class UILobbyViewSystem
	{
		public static void OnClickbtnStart(this UILobbyView self)
		{
			//todo： 链接网关服务器  进入游戏
			

		}
		public static LoopListViewItem2 GetScrollViewItemByIndex(this UILobbyView self, LoopListView2 listView, int index)
		{
			var data = self.listData[index];
			var itemView = listView.NewListViewItem("Item");
			if (!itemView.IsInitHandlerCalled)
			{
				itemView.IsInitHandlerCalled = true;
				self.ScrollView.AddItemViewComponent<UILobbyItem>(itemView);
			}

			var item = self.ScrollView.GetUIItemView<UILobbyItem>(itemView);
			item.SetData(data);
			
			return itemView;
		}
	}

}
