using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIBagItemDescView))]
	public class UIBagItemDescViewOnCreateSystem : OnCreateSystem<UIBagItemDescView>
	{

		public override void OnCreate(UIBagItemDescView self)
		{
			self.lblName = self.AddUIComponent<UITextmesh>("spBg/lblName");
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.lblDesc = self.AddUIComponent<UITextmesh>("spBg/lblDesc");
			self.CanUseTransform = self.AddUIComponent<UIEmptyGameobject>("spBg/CanUseTransform");
			self.lblUseNum = self.AddUIComponent<UITextmesh>("spBg/CanUseTransform/sp/lblUseNum");
			self.SlideChangeNum = self.AddUIComponent<UISlider>("spBg/CanUseTransform/SlideChangeNum");
			self.btnUse = self.AddUIComponent<UIButton>("spBg/CanUseTransform/btnUse");
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnUse.SetOnClick(()=>{self.OnClickbtnUse().Coroutine();});
			
			self.SlideChangeNum.SetOnValueChanged((num) =>
			{
				self.lblUseNum.SetText(num.ToString());
				self.selectNum = (int)num;
			});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIBagItemDescView))]
	public class UIBagItemDescViewLoadSystem : LoadSystem<UIBagItemDescView>
	{

		public override void Load(UIBagItemDescView self)
		{
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnUse.SetOnClick(()=>{self.OnClickbtnUse().Coroutine();});
		}

	}
	[FriendClass(typeof(UIBagItemDescView))]
	public static class UIBagItemDescViewSystem
	{

		public static async  ETTask OnClickbtnUse(this UIBagItemDescView self)
		{
			self.CloseSelf().Coroutine();
			M2C_UseItem m2CUseItem =(M2C_UseItem)await self.scene.GetComponent<SessionComponent>().Session.Call(new C2M_UseItem(){ItemUid = self.Data.Id,Num = self.selectNum});
			if (m2CUseItem.Error != ErrorCode.ERR_Success)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "道具使用失败" }).Coroutine();
			}
			else
			{
				UIManagerComponent.Instance.GetWindow<UIBagView>()?.UpdateBagItem();
			}
		}
		
		public static void OnClickbtnClose(this UIBagItemDescView self)
		{
			self.CloseSelf().Coroutine();
		}

		public static void SetData(this UIBagItemDescView self,Item item,Scene scene)
		{
			self.scene = scene;
			self.Data = item;
			self.lblName.SetText(item.Config.Name);
			self.lblDesc.SetText(item.Config.Desc);

			
			if (item.Config.CanUse == 0)
			{
				self.CanUseTransform.SetActive(true);
				self.SlideChangeNum.SetMinValue(1);
				self.SlideChangeNum.SetMaxValue(item.Count);
				self.SlideChangeNum.isWholeNumbers = true;
				self.SlideChangeNum.SetValue(1);
			}
			else
			{
				self.CanUseTransform.SetActive(false);
			}
		}

	}

}
