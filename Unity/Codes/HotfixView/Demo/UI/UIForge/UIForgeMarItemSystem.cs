using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIForgeMarItem))]
	public class UIForgeMarItemOnCreateSystem : OnCreateSystem<UIForgeMarItem>
	{

		public override void OnCreate(UIForgeMarItem self)
		{
			self.TransfoBagItem = self.AddUIComponent<UIEmptyGameobject>("TransfoBagItem");
			self.SliderNum = self.AddUIComponent<UISlider>("SliderNum");
			self.lblNum = self.AddUIComponent<UITextmesh>("sp/lblNum");
			self.btnPut = self.AddUIComponent<UIButton>("btnPut");
			self.btnPut.SetOnClick(()=>{self.OnClickbtnPut();});
			
			self.SliderNum.SetOnValueChanged((num) =>
			{
				self.lblNum.SetText(num.ToString());
				self.curSelectNum = (int)num;
			});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIForgeMarItem))]
	public class UIForgeMarItemLoadSystem : LoadSystem<UIForgeMarItem>
	{

		public override void Load(UIForgeMarItem self)
		{
			self.btnPut.SetOnClick(()=>{self.OnClickbtnPut();});
		}

	}
	[FriendClass(typeof(UIForgeMarItem))]
	public static class UIForgeMarItemSystem
	{
		public static void OnClickbtnPut(this UIForgeMarItem self)
		{
			self.SetPutInfo(self.curSelectNum);
		}
		
		public static async ETTask SetData(this UIForgeMarItem self,Item item)
		{
			self.Data = item;
			self.SliderNum.SetMinValue(1);
			self.SliderNum.SetMaxValue(item.Count);
			self.SliderNum.isWholeNumbers = true;
			self.SliderNum.SetValue(1);
			self.tempNum = item.Count;

			if (self.BagItem == null)
			{
				GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIBagItem.PrefabPath);
				UIBagItem ui = self.AddChild<UIBagItem>();
				var transform = gameObject.transform;
				ui.AddUIComponent<UITransform,Transform>("", transform);
				transform = gameObject.transform;
				transform.SetParent(self.TransfoBagItem.GetTransform());
				transform.localPosition = Vector3.zero;
				UIWatcherComponent.Instance.OnCreate(ui);
				UIWatcherComponent.Instance.OnEnable(ui);
				self.BagItem = ui;
			}
			self.BagItem.SetData(item,self.scene);
		}

		public static void SetPutInfo(this UIForgeMarItem self,int putNum)
		{
			self.SetSliderAndTip(putNum);

			((UIForgeView)self.Parent.Parent).SetMarInfo(self.Data,putNum).Coroutine();
		}
		
		public static void SetSliderAndTip(this UIForgeMarItem self,int putNum)
		{
			self.tempNum -= putNum;
			self.BagItem.SetNumLbl(self.tempNum);
			self.SliderNum.SetMinValue(self.tempNum >0?1:0);
			self.SliderNum.SetMaxValue(self.tempNum);
			self.SliderNum.isWholeNumbers = true;
			self.SliderNum.SetValue(1);
			self.curSelectNum = self.tempNum >0?1:0;
			
		}
		
	}

}
