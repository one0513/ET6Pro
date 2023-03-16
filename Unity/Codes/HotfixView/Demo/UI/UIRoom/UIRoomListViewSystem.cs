using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIRoomListView))]
	public class UIRoomListViewOnEnableSystem : OnEnableSystem<UIRoomListView,Scene>
	{
		public override void OnEnable(UIRoomListView self, Scene scene)
		{
			self.scene = scene;
		}
	}
	
	
	[UISystem]
	[FriendClass(typeof(UIRoomListView))]
	public class UIRoomListViewOnCreateSystem : OnCreateSystem<UIRoomListView>
	{

		public override void OnCreate(UIRoomListView self)
		{
			self.ScrollView = self.AddUIComponent<UILoopListView2>("spBg/ScrollView");
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.btnRefresh = self.AddUIComponent<UIButton>("spBg/btnRefresh");
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnRefresh.SetOnClick(()=>{self.OnClickbtnRefresh().Coroutine();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIRoomListView))]
	public class UIRoomListViewLoadSystem : LoadSystem<UIRoomListView>
	{

		public override void Load(UIRoomListView self)
		{
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnRefresh.SetOnClick(()=>{self.OnClickbtnRefresh().Coroutine();});
		}

	}
	[FriendClass(typeof(UIRoomListView))]
	[FriendClass(typeof(UIRoomListItem))]
	public static class UIRoomListViewSystem
	{
		public static LoopListViewItem2 GetScrollViewItemByIndex(this UIRoomListView self, LoopListView2 listView, int index)
		{
			var data = self.listData[index];
			var itemView = listView.NewListViewItem("Item");
			if (!itemView.IsInitHandlerCalled)
			{
				itemView.IsInitHandlerCalled = true;
				self.ScrollView.AddItemViewComponent<UIRoomListItem>(itemView);
			}

			var item = self.ScrollView.GetUIItemView<UIRoomListItem>(itemView);
			item.SetData(data);
			item.scene = self.scene;
			return itemView;
		}
		public static void OnClickbtnClose(this UIRoomListView self)
		{
			UIManagerComponent.Instance.CloseWindow<UIRoomListView>().Coroutine();
		}
		public static async ETTask OnClickbtnRefresh(this UIRoomListView self)
		{
			await ETTask.CompletedTask;
			// GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIRoomListItem.PrefabPath);
			// UIRoomListItem ui = self.AddChild<UIRoomListItem>();
			// var transform = gameObject.transform;
			// ui.AddUIComponent<UITransform,Transform>("", transform);
			// transform = gameObject.transform;
			// transform.SetParent(self.btnRefresh.GetTransform());
			// transform.localPosition = Vector3.zero;
			// transform.localRotation = Quaternion.identity;
			// transform.localScale = new Vector3(1, 1, 1);
			// UIWatcherComponent.Instance.OnCreate(ui);
			// UIWatcherComponent.Instance.OnEnable(ui);
			// await TimerComponent.Instance.WaitAsync(seconds*1000);
			// ui.BeforeOnDestroy();
			// UIWatcherComponent.Instance.OnDestroy(ui);
			// GameObjectPoolComponent.Instance.RecycleGameObject(gameObject);
		}
		
		public static void UpdateView(this UIRoomListView self,List<RoomInfo> listData)
		{
			int count = listData.Count;
			self.listData = listData;
			self.ScrollView.SetListItemCount(count);
			self.ScrollView.RefreshAllShownItem();
		}
	}

}
