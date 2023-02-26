using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	
	[FriendClass(typeof(RoleInfosComponent))]
	[FriendClass(typeof(RoleInfo))]
	[FriendClass(typeof(UIMainView))]
	public class UIMainViewOnEnableSystem : OnEnableSystem<UIMainView,Scene>
	{
		public override void OnEnable(UIMainView self, Scene scene)
		{
			self.scene = scene.ZoneScene();
			self.lblRoleName.SetText(scene.ZoneScene().GetComponent<RoleInfosComponent>().RoleInfos[0].Name);
			self.UpdateView();
		}
	}
	
	
	[UISystem]
	[FriendClass(typeof(UIMainView))]
	[FriendClass(typeof(AdventureComponent))]
	public class UIMainViewOnCreateSystem : OnCreateSystem<UIMainView>
	{

		public override void OnCreate(UIMainView self)
		{
			self.lblRoleName = self.AddUIComponent<UITextmesh>("sp/lblRoleName");
			self.lblLevel = self.AddUIComponent<UITextmesh>("sp/lblLevel");
			self.lblExp = self.AddUIComponent<UITextmesh>("sp/lblExp");
			self.lblCoin = self.AddUIComponent<UITextmesh>("sp/lblCoin");
			self.lblCurLevel = self.AddUIComponent<UITextmesh>("lblCurLevel");
			self.btnRole = self.AddUIComponent<UIButton>("btnRole");
			self.btnAdventure = self.AddUIComponent<UIButton>("btnAdventure");
			self.btnNextAdventure = self.AddUIComponent<UIButton>("btnNextAdventure");
			self.btnTask = self.AddUIComponent<UIButton>("btnTask");
			self.btnRole.SetOnClick(()=>{self.OnClickbtnRole().Coroutine();});
			self.btnAdventure.SetOnClick(()=>{self.OnClickbtnAdventure().Coroutine();});
			self.btnNextAdventure.SetOnClick(()=>{self.OnClickbtnNextAdventure().Coroutine();});
			self.btnTask.SetOnClick(()=>{self.OnClickbtnTask();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIMainView))]
	public class UIMainViewLoadSystem : LoadSystem<UIMainView>
	{

		public override void Load(UIMainView self)
		{
			self.btnRole.SetOnClick(()=>{self.OnClickbtnRole().Coroutine();});
			self.btnAdventure.SetOnClick(()=>{self.OnClickbtnAdventure().Coroutine();});
			self.btnNextAdventure.SetOnClick(()=>{self.OnClickbtnNextAdventure().Coroutine();});
			self.btnTask.SetOnClick(()=>{self.OnClickbtnTask();});
		}

	}
	[FriendClass(typeof(UIMainView))]
	[FriendClass(typeof(AdventureComponent))]
	public static class UIMainViewSystem
	{
		public static async ETTask OnClickbtnRole(this UIMainView self)
		{
			await UIManagerComponent.Instance.OpenWindow<UIRoleView, Scene>(UIRoleView.PrefabPath, self.scene);
		}
		public static async ETTask OnClickbtnAdventure(this UIMainView self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.scene.CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
			if (self.scene.CurrentScene().GetComponent<AdventureComponent>().isFighting)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "当前正在战斗中！！" }).Coroutine();
			}
			else
			{
				self.scene.CurrentScene().GetComponent<AdventureComponent>().SetFightStatue(true);
				await AdventureHelper.RequestStartGameLevel(self.scene, numericComponent.GetAsInt((int)NumericType.CurLevel));
				await self.scene.CurrentScene().GetComponent<AdventureComponent>().StartAdventure();
			}
		}
		
		public static async ETTask OnClickbtnNextAdventure(this UIMainView self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.scene.CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
			if (self.scene.CurrentScene().GetComponent<AdventureComponent>().isFighting)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "当前正在战斗中！！" }).Coroutine();
			}
			else
			{
				self.scene.CurrentScene().GetComponent<AdventureComponent>().SetFightStatue(true);
				await AdventureHelper.RequestStartGameLevel(self.scene, numericComponent.GetAsInt((int)NumericType.CurLevel)+1);
				await self.scene.CurrentScene().GetComponent<AdventureComponent>().StartAdventure();
			}
		}
		public static void OnClickbtnTask(this UIMainView self)
		{

		}

		public static void UpdateView(this UIMainView self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.scene.CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
			
			self.lblExp.SetText(numericComponent.GetAsInt((int)NumericType.Exp).ToString());
			//self.lblCoin.SetText(numericComponent.GetAsInt((int)NumericType.Gold).ToString());
			self.lblLevel.SetText($"Lv:{numericComponent.GetAsInt((int)NumericType.Lv).ToString()}");
			self.lblCurLevel.SetText($"当前关卡:{numericComponent.GetAsInt((int)NumericType.CurLevel).ToString()}关");
		}
	}

}
