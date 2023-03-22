using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIForgeView))]
	public class UIForgeViewOnEnableSystem : OnEnableSystem<UIForgeView,Scene>
	{
		public override void OnEnable(UIForgeView self, Scene scene)
		{
			self.scene = scene;
			self.spSelect.GetTransform().localPosition = new Vector3(self.curSelectMar * 200 + 80, 200, 0);
			
			self.scene.GetComponent<BagComponent>().GetForgeItem(ref self.listData);
			self.ScrollView.SetListItemCount(self.listData.Count);
		}
	}
	[UISystem]
	[FriendClass(typeof(UIForgeView))]
	public class UIForgeViewOnCreateSystem : OnCreateSystem<UIForgeView>
	{

		public override void OnCreate(UIForgeView self)
		{
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.btnWeapon = self.AddUIComponent<UIButton>("spBg/btnWeapon");
			self.btnDeputy = self.AddUIComponent<UIButton>("spBg/btnDeputy");
			self.btnCasque = self.AddUIComponent<UIButton>("spBg/btnCasque");
			self.btnArmour = self.AddUIComponent<UIButton>("spBg/btnArmour");
			self.btnStart = self.AddUIComponent<UIButton>("spBg/btnStart");
			self.Mar1 = self.AddUIComponent<UIEmptyGameobject>("spBg/Mar1");
			self.Mar2 = self.AddUIComponent<UIEmptyGameobject>("spBg/Mar2");
			self.Mar3 = self.AddUIComponent<UIEmptyGameobject>("spBg/Mar3");
			self.Result = self.AddUIComponent<UIEmptyGameobject>("spBg/Result");
			self.lblResult = self.AddUIComponent<UITextmesh>("spBg/Result/lblResult");
			self.ScrollView = self.AddUIComponent<UILoopListView2>("spBg/MyMarList/ScrollView");
			self.spSelect = self.AddUIComponent<UIImage>("spBg/spSelect");
			
			UIEventListener.AddClick(self.Mar1.GetGameObject(), (var)=>
			{
				self.curSelectMar = 0;
				self.spSelect.GetTransform().localPosition = new Vector3(self.curSelectMar * 200 + 80, 200, 0);
				
			});
			UIEventListener.AddClick(self.Mar2.GetGameObject(), (var)=>
			{
				self.curSelectMar = 1;
				self.spSelect.GetTransform().localPosition = new Vector3(self.curSelectMar * 200 + 80, 200, 0);
			});
			UIEventListener.AddClick(self.Mar3.GetGameObject(), (var)=>
			{
				self.curSelectMar = 2;
				self.spSelect.GetTransform().localPosition = new Vector3(self.curSelectMar * 200 + 80, 200, 0);
			});

			self.InitMarDict().Coroutine();
			self.InitResult().Coroutine();
			
			
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnWeapon.SetOnClick(()=>{self.OnClickbtnWeapon();});
			self.btnDeputy.SetOnClick(()=>{self.OnClickbtnDeputy();});
			self.btnCasque.SetOnClick(()=>{self.OnClickbtnCasque();});
			self.btnArmour.SetOnClick(()=>{self.OnClickbtnArmour();});
			self.btnStart.SetOnClick(()=>{self.OnClickbtnStart().Coroutine();});
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIForgeView))]
	public class UIForgeViewLoadSystem : LoadSystem<UIForgeView>
	{

		public override void Load(UIForgeView self)
		{
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
			self.btnWeapon.SetOnClick(()=>{self.OnClickbtnWeapon();});
			self.btnDeputy.SetOnClick(()=>{self.OnClickbtnDeputy();});
			self.btnCasque.SetOnClick(()=>{self.OnClickbtnCasque();});
			self.btnArmour.SetOnClick(()=>{self.OnClickbtnArmour();});
			self.btnStart.SetOnClick(()=>{self.OnClickbtnStart().Coroutine();});
			self.ScrollView.InitListView(0,(a,b)=>{return self.GetScrollViewItemByIndex(a,b);});
		}

	}
	[FriendClass(typeof(UIForgeView))]
	[FriendClass(typeof(UIForgeMarItem))]
	[FriendClass(typeof(UIBagItem))]
	[FriendClass(typeof(Item))]
	public static class UIForgeViewSystem
	{
		public static void OnClickbtnClose(this UIForgeView self)
		{
			self.CloseSelf().Coroutine();
			self.UpdateView();
			self.ResultItem.SetActive(false);
		}
		public static void OnClickbtnWeapon(this UIForgeView self)
		{
			self.curSelectPos = (int)EquipPosition.Weapon;
			self.ResultItem.SetActive(false);
			self.lblResult.SetText("武器");
		}
		public static void OnClickbtnDeputy(this UIForgeView self)
		{
			self.curSelectPos = (int)EquipPosition.Deputy;
			self.ResultItem.SetActive(false);
			self.lblResult.SetText("副手");
		}
		public static void OnClickbtnCasque(this UIForgeView self)
		{
			self.curSelectPos = (int)EquipPosition.Casque;
			self.ResultItem.SetActive(false);
			self.lblResult.SetText("头盔");
		}
		public static void OnClickbtnArmour(this UIForgeView self)
		{
			self.curSelectPos = (int)EquipPosition.Armour;
			self.ResultItem.SetActive(false);
			self.lblResult.SetText("盔甲");
		}
		public static async ETTask OnClickbtnStart(this UIForgeView self)
		{
			for (int i = 0; i < 3; i++)
			{
				if (self.MarDit[i].Data == null)
				{
					Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "材料不足，请继续添加材料" }).Coroutine();
					return;
				}
			}

			C2M_StartForge c2MStartForge = new C2M_StartForge();
			for (int i = 0; i < 3; i++)
			{
				ForgeInfoProto infoProto = new ForgeInfoProto();
				infoProto.ItemUid = self.MarDit[i].Data.Id;
				infoProto.Num = self.MarDit[i].tempNum;
				c2MStartForge.Infos.Add(infoProto);
			}

			c2MStartForge.ForgePos = self.curSelectPos;
			M2C_StartForge m2CStartForge = (M2C_StartForge)await self.scene.GetComponent<SessionComponent>().Session.Call(c2MStartForge);
			if (m2CStartForge.Error == ErrorCode.ERR_Success)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "锻造成功！！" }).Coroutine();
				self.UpdateView();
				self.ResultItem.SetActive(true);
				self.ResultItem.SetData(self.scene.GetComponent<BagComponent>().GetItemById(m2CStartForge.ItemInfo.ItemUid),self.scene);
				
			}
			else
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "锻造失败！！" }).Coroutine();
				self.UpdateView();
			}
			
			
		}
		public static LoopListViewItem2 GetScrollViewItemByIndex(this UIForgeView self, LoopListView2 listView, int index)
		{
			var data = self.listData[index];
			var itemView = listView.NewListViewItem("Item");
			if (!itemView.IsInitHandlerCalled)
			{
				itemView.IsInitHandlerCalled = true;
				self.ScrollView.AddItemViewComponent<UIForgeMarItem>(itemView);
			}

			var item = self.ScrollView.GetUIItemView<UIForgeMarItem>(itemView);
			item.SetData(data).Coroutine();
			item.scene = self.scene;
			
			return itemView;
		}
		
		public static void UpdateView(this UIForgeView self)
		{
			self.scene.GetComponent<BagComponent>().GetForgeItem(ref self.listData);
			self.ScrollView.SetListItemCount(self.listData.Count);
			self.ScrollView.RefreshAllShownItem();

			for (int i = 0; i < 3; i++)
			{
				self.MarDit[i].SetActive(false);
				self.MarDit[i].SetNull();
			}
		}
		
		public static async ETTask InitMarDict(this UIForgeView self)
		{
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIBagItem.PrefabPath);
				UIBagItem ui = self.AddChild<UIBagItem>();
				var transform = gameObject.transform;
				ui.AddUIComponent<UITransform,Transform>("", transform);
				transform = gameObject.transform;
				if (i ==0 )
				{
					transform.SetParent(self.Mar1.GetTransform());
				}else if (i == 1)
				{
					transform.SetParent(self.Mar2.GetTransform());
				}
				else
				{
					transform.SetParent(self.Mar3.GetTransform());
				}
				transform.localPosition = Vector3.zero;
				transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				ui.SetActive(false);
				UIWatcherComponent.Instance.OnCreate(ui);
				//UIWatcherComponent.Instance.OnEnable(ui);
				self.MarDit.Add(i,ui);
			}
		}
		
		public static async ETTask InitResult(this UIForgeView self)
		{
			GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIBagItem.PrefabPath);
			UIBagItem ui = self.AddChild<UIBagItem>();
			var transform = gameObject.transform;
			ui.AddUIComponent<UITransform,Transform>("", transform);
			transform = gameObject.transform;
			transform.SetParent(self.Result.GetTransform());
			
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(1, 1, 1);
			ui.SetActive(false);
			UIWatcherComponent.Instance.OnCreate(ui);
			self.ResultItem = ui;
			//UIWatcherComponent.Instance.OnEnable(ui);
		}
		
		public static void closeResult(this UIForgeView self)
		{
			self.ResultItem.SetActive(false);
		}
		
		public static async ETTask SetMarInfo(this UIForgeView self,Item item,int num)
		{
			await ETTask.CompletedTask;
			if (self.MarDit[self.curSelectMar].Data == null)
			{
				self.MarDit[self.curSelectMar].SetActive(true);
				self.MarDit[self.curSelectMar].SetData(item,self.scene);
				self.MarDit[self.curSelectMar].SetNumLbl(num);
				return;
			}

			if (self.MarDit[self.curSelectMar].Data.ConfigId == item.ConfigId)
			{
				self.MarDit[self.curSelectMar].SetActive(true);
				self.MarDit[self.curSelectMar].SetData(item,self.scene);
				num += self.MarDit[self.curSelectMar].tempNum;
				self.MarDit[self.curSelectMar].SetNumLbl(num);
			}
			else
			{
				for (int i = 0; i < self.listData.Count; i++)
				{
					if (self.MarDit[self.curSelectMar].Data.ConfigId ==self.listData[i].ConfigId )
					{
						var aLoopListViewItem2 = self.ScrollView.loopListView.GetShownItemByIndex(i);
						var forgeMarItem = self.ScrollView.GetUIItemView<UIForgeMarItem>(aLoopListViewItem2);
						forgeMarItem.SetSliderAndTip(-self.MarDit[self.curSelectMar].tempNum);
					}
				}
				
				self.MarDit[self.curSelectMar].SetActive(true);
				self.MarDit[self.curSelectMar].SetData(item,self.scene);
				self.MarDit[self.curSelectMar].tempNum = 0;
				self.MarDit[self.curSelectMar].SetNumLbl(num);
				
			}

		}
	}

}
