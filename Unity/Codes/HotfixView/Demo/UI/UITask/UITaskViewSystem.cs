using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	
	[UISystem]
	[FriendClass(typeof(UITaskView))]
	[FriendClass(typeof(TasksComponent))]
	public class UITaskViewOnEnableSystem : OnEnableSystem<UITaskView,Scene>
	{
		public override void OnEnable(UITaskView self, Scene scene)
		{
			self.scene = scene;

			int count = self.scene.GetComponent<TasksComponent>().GetTaskInfoCount();
			self.listData = self.scene.GetComponent<TasksComponent>().TaskInfoList;
			self.ScrollView.SetListItemCount(count);

		}
	}
	
	[UISystem]
	[FriendClass(typeof(UITaskView))]
	public class UITaskViewOnCreateSystem : OnCreateSystem<UITaskView>
	{

		public override void OnCreate(UITaskView self)
		{
			self.ScrollView = self.AddUIComponent<UILoopListView2>("spBg/ScrollView");
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UITaskView))]
	public class UITaskViewLoadSystem : LoadSystem<UITaskView>
	{

		public override void Load(UITaskView self)
		{
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[FriendClass(typeof(UITaskView))]
	[FriendClass(typeof(UITaskItem))]
	[FriendClass(typeof(TasksComponent))]
	public static class UITaskViewSystem
	{
		public static LoopListViewItem2 GetScrollViewItemByIndex(this UITaskView self, LoopListView2 listView, int index)
		{
			var data = self.listData[index];
			var itemView = listView.NewListViewItem("Item");
			if (!itemView.IsInitHandlerCalled)
			{
				itemView.IsInitHandlerCalled = true;
				self.ScrollView.AddItemViewComponent<UITaskItem>(itemView);
			}

			var item = self.ScrollView.GetUIItemView<UITaskItem>(itemView);
			item.SetData(data);
			item.scene = self.scene;
			return itemView;
		}
		public static void OnClickbtnClose(this UITaskView self)
		{
			UIManagerComponent.Instance.CloseWindow<UITaskView>().Coroutine();
		}
		public static void UpdateView(this UITaskView self)
		{
			int count = self.scene.GetComponent<TasksComponent>().GetTaskInfoCount();
			self.listData = self.scene.GetComponent<TasksComponent>().TaskInfoList;
			self.ScrollView.SetListItemCount(count);
			self.ScrollView.RefreshAllShownItem();
		}
	}

}
