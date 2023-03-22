using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIBagView))]
	public class  UIBagViewOnEnableSystem : OnEnableSystem<UIBagView,Scene>
	{
		public override void OnEnable(UIBagView self, Scene scene)
		{
			self.scene = scene;
			self.OnEnable().Coroutine();
		}
	}
	
	
	[UISystem]
	[FriendClass(typeof(UIBagView))]
	public class UIBagViewOnCreateSystem : OnCreateSystem<UIBagView>
	{

		public override void OnCreate(UIBagView self)
		{
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.ItemList = self.AddUIComponent<UIEmptyGameobject>("spBg/sp/ItemList");
			self.btnAll = self.AddUIComponent<UIButton>("spBg/btnAll");
			self.btnItem = self.AddUIComponent<UIButton>("spBg/btnItem");
			self.btnEquipment = self.AddUIComponent<UIButton>("spBg/btnEquipment");
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnAll.SetOnClick(()=>{self.OnClickbtnAll();});
			self.btnItem.SetOnClick(()=>{self.OnClickbtnItem();});
			self.btnEquipment.SetOnClick(()=>{self.OnClickbtnEquipment();});
			
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIBagView))]
	public class UIBagViewLoadSystem : LoadSystem<UIBagView>
	{

		public override void Load(UIBagView self)
		{
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnAll.SetOnClick(()=>{self.OnClickbtnAll();});
			self.btnItem.SetOnClick(()=>{self.OnClickbtnItem();});
			self.btnEquipment.SetOnClick(()=>{self.OnClickbtnEquipment();});
		}

	}
	[FriendClass(typeof(UIBagView))]
	public static class UIBagViewSystem
	{
		public static async ETTask OnEnable(this UIBagView self)
		{
			await self.UpdateBagItem();
		}
		
		public static async ETTask UpdateBagItem(this UIBagView self) //0 全部 1道具 2装备
		{

			if (self.curSelectType == 0)
			{
				self.scene.GetComponent<BagComponent>().GetAllBagItem(ref self.CurShowItems);
			}
			if (self.curSelectType == 1)
			{
				self.scene.GetComponent<BagComponent>().GetBagTypeItem(ref self.CurShowItems,1);
			}
			if (self.curSelectType == 2)
			{
				self.scene.GetComponent<BagComponent>().GetBagTypeItem(ref self.CurShowItems,0);
			}

			for (int i = 0; i < self.BagItems.Count; i++)
			{
				self.BagItems[i].SetActive(false);
			}
			for (int i = 0; i < self.CurShowItems.Count; i++)
			{
				if (self.BagItems.Count-1 < i)
				{
					GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIBagItem.PrefabPath);
					UIBagItem ui = self.AddChild<UIBagItem>();
					var transform = gameObject.transform;
					ui.AddUIComponent<UITransform,Transform>("", transform);
					transform = gameObject.transform;
					transform.SetParent(self.ItemList.GetTransform());
					UIWatcherComponent.Instance.OnCreate(ui);
					UIWatcherComponent.Instance.OnEnable(ui);
					self.BagItems.Add(ui);
					ui.SetData(self.CurShowItems[i],self.scene);
					ui.SetActive(true);
				}
				else
				{
					self.BagItems[i].SetActive(true);
					self.BagItems[i].SetData(self.CurShowItems[i],self.scene);
				}
			}
			
			
		}
		
		public static void OnClickbtnClose(this UIBagView self)
		{
			self.CloseSelf().Coroutine();
		}
		public static void OnClickbtnAll(this UIBagView self)
		{
			self.curSelectType = 0;
			self.UpdateBagItem().Coroutine();

		}
		public static void OnClickbtnItem(this UIBagView self)
		{
			self.curSelectType = 1;
			self.UpdateBagItem().Coroutine();
		}
		public static void OnClickbtnEquipment(this UIBagView self)
		{
			self.curSelectType = 2;
			self.UpdateBagItem().Coroutine();
		}
	}

}
