using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIBagItem))]
	public class UIBagItemOnCreateSystem : OnCreateSystem<UIBagItem>
	{

		public override void OnCreate(UIBagItem self)
		{
			self.spIcon = self.AddUIComponent<UIImage>("spIcon");
			self.lblNum = self.AddUIComponent<UITextmesh>("lblNum");
			UIEventListener.AddClick(self.GetGameObject(), (var)=>
			{
				self.ClickSelf().Coroutine();
			});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIBagItem))]
	public class UIBagItemLoadSystem : LoadSystem<UIBagItem>
	{

		public override void Load(UIBagItem self)
		{
		}

	}
	[FriendClass(typeof(UIBagItem))]
	[FriendClass(typeof(UIImage))]
	public static class UIBagItemSystem
	{
		public static void SetData(this UIBagItem self,Item item,Scene scene)
		{
			self.scnen = scene;
			self.Data = item;
			self.lblNum.SetActive(item.Config.Type == (int)ItemType.Item);
			self.lblNum.SetText(item.Count.ToString());
			self.spIcon.SetSpritePath(item.Config.Icon).Coroutine();
		}
		
		public static async  ETTask ClickSelf(this UIBagItem self)
		{
			if (self.Data.Config.Type == (int)ItemType.Item)
			{
				UIBagItemDescView view = await UIManagerComponent.Instance.OpenWindow<UIBagItemDescView>(UIBagItemDescView.PrefabPath);
				view.SetData(self.Data,self.scnen);
			}
			else if(self.Data.Config.Type == (int)ItemType.Equipment)
			{
				UIBagEquipDescView view = await UIManagerComponent.Instance.OpenWindow<UIBagEquipDescView>(UIBagEquipDescView.PrefabPath);
				view.SetData(self.Data,self.scnen).Coroutine();
			}
			
		}
		
		public static void SetNumLbl(this UIBagItem self,int num)
		{
			self.tempNum = num;
			self.lblNum.SetText(self.tempNum.ToString());
		}
		
		public static void SetNull(this UIBagItem self)
		{
			self.tempNum = 0;
			self.Data = null;
		}
		
	}

}
