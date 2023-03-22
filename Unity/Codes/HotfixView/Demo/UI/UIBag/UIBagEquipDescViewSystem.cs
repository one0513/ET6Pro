using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIBagEquipDescView))]
	public class UIBagEquipDescViewOnCreateSystem : OnCreateSystem<UIBagEquipDescView>
	{

		public override void OnCreate(UIBagEquipDescView self)
		{
			self.lblName = self.AddUIComponent<UITextmesh>("spBg/lblName");
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.btnWear = self.AddUIComponent<UIButton>("btnWear");
			self.btnUnload = self.AddUIComponent<UIButton>("btnUnload");
			
			self.lblDesc = self.AddUIComponent<UITextmesh>("spBg/lblDesc");
			self.AttrTrans = self.AddUIComponent<UIEmptyGameobject>("sp/AttrTrans");
			self.btnWear.SetOnClick(()=>{self.OnClickbtnWear().Coroutine();});
			self.btnUnload.SetOnClick(()=>{self.OnClickbtnUnload().Coroutine();});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIBagEquipDescView))]
	public class UIBagEquipDescViewLoadSystem : LoadSystem<UIBagEquipDescView>
	{

		public override void Load(UIBagEquipDescView self)
		{
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnWear.SetOnClick(()=>{self.OnClickbtnWear().Coroutine();});
			self.btnUnload.SetOnClick(()=>{self.OnClickbtnUnload().Coroutine();});
		}

	}
	[FriendClass(typeof(UIBagEquipDescView))]
	[FriendClass(typeof(EquipInfoComponent))]
	public static class UIBagEquipDescViewSystem
	{
		public static async ETTask OnClickbtnUnload(this UIBagEquipDescView self)
		{
			int error = await ItemApplyHelper.UnloadEquipItem(self.scene,self.Data.Id);
			if (error == ErrorCode.ERR_Success)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "卸载成功！！" }).Coroutine();
				UIManagerComponent.Instance.GetWindow<UIForgeView>()?.closeResult();
				UIManagerComponent.Instance.GetWindow<UIBagView>()?.UpdateBagItem();
				UIManagerComponent.Instance.GetWindow<UIRoleView>()?.UpdateEquipItems();
				self.CloseSelf().Coroutine();
			}
			else
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "卸载失败！！" }).Coroutine();
				self.CloseSelf().Coroutine();
			}
			
		}
		public static void OnClickbtnClose(this UIBagEquipDescView self)
		{
			self.CloseSelf().Coroutine();
		}
		public static async ETTask OnClickbtnWear(this UIBagEquipDescView self)
		{
			int error = await ItemApplyHelper.EquipItem(self.scene, self.Data.Id);
			if (error == ErrorCode.ERR_Success)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "装备成功！！" }).Coroutine();
				UIManagerComponent.Instance.GetWindow<UIForgeView>()?.closeResult();
				UIManagerComponent.Instance.GetWindow<UIBagView>()?.UpdateBagItem();
				UIManagerComponent.Instance.GetWindow<UIRoleView>()?.UpdateEquipItems();
				self.CloseSelf().Coroutine();
			}
			else
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "装备失败！！" }).Coroutine();
				self.CloseSelf().Coroutine();
			}

		}
		public static async ETTask SetData(this UIBagEquipDescView self,Item Data,Scene scene)
		{
			self.Data = Data;
			self.scene = scene;
			self.lblDesc.SetText(Data.Config.Desc);
			self.lblName.SetText(Data.Config.Name);
			var list = Data.GetComponent<EquipInfoComponent>().EntryList;
			for (int i = 0; i < self.attrList.Count; i++)
			{
				self.attrList[i].SetActive(false);
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (i+1 > self.attrList.Count)
				{
					await self.CreateAttrItem();
				}
				self.attrList[i].SetData(list[i]);
				self.attrList[i].SetActive(true);
			}

			bool isWear = self.scene.GetComponent<EquipmentsComponent>().IsEquipItemByItemId(self.Data.Id);
			self.btnUnload.SetActive(isWear);
			self.btnWear.SetActive(!isWear);
		}
		
		
		public static async ETTask CreateAttrItem(this UIBagEquipDescView self)
		{
			GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIBagEquipAtrItem.PrefabPath);
			UIBagEquipAtrItem ui = self.AddChild<UIBagEquipAtrItem>();
			var transform = gameObject.transform;
			ui.AddUIComponent<UITransform,Transform>("", transform);
			transform = gameObject.transform;
			transform.SetParent(self.AttrTrans.GetTransform());
			
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(1, 1, 1);
			ui.SetActive(false);
			UIWatcherComponent.Instance.OnCreate(ui);
			self.attrList.Add(ui);
			//UIWatcherComponent.Instance.OnEnable(ui);
		}
	}

}
