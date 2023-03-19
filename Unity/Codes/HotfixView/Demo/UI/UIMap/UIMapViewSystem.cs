using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIMapView))]
	public class UIMapViewOnEnableSystem : OnEnableSystem<UIMapView,Scene>
	{
		public override void OnEnable(UIMapView self, Scene scene)
		{
			self.scene = scene;
			self.ScrollView.SetListItemCount(self.listdatas.Count);
			
		}
	}
	
	[UISystem]
	[FriendClass(typeof(UIMapView))]
	public class UIMapViewOnCreateSystem : OnCreateSystem<UIMapView>
	{

		public override void OnCreate(UIMapView self)
		{
			self.ScrollView = self.AddUIComponent<UILoopListView2>("spBg/ScrollView");
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIMapView))]
	public class UIMapViewLoadSystem : LoadSystem<UIMapView>
	{

		public override void Load(UIMapView self)
		{
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[FriendClass(typeof(UIMapView))]
	[FriendClass(typeof(UIMapItem))]
	public static class UIMapViewSystem
	{
		public static LoopListViewItem2 GetScrollViewItemByIndex(this UIMapView self, LoopListView2 listView, int index)
		{
			var data = self.listdatas[index];
			var itemView = listView.NewListViewItem("Item");
			if (!itemView.IsInitHandlerCalled)
			{
				itemView.IsInitHandlerCalled = true;
				self.ScrollView.AddItemViewComponent<UIMapItem>(itemView);
			}

			var item = self.ScrollView.GetUIItemView<UIMapItem>(itemView);
			item.SetData(data);
			item.scene = self.scene;
			return itemView;
		}
		public static void OnClickbtnClose(this UIMapView self)
		{
			self.CloseSelf().Coroutine();
		}
	}

}
