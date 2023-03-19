using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{

	[FriendClass(typeof(UIRoleView))]
	public class UIRoleViewOnEnableSystem : OnEnableSystem<UIRoleView,Scene>
	{
		public override void OnEnable(UIRoleView self, Scene scene)
		{
			self.scene = scene;
			self.UpdateView();
			
		}
	}
	
	
	[UISystem]
	[FriendClass(typeof(UIRoleView))]
	public class UIRoleViewOnCreateSystem : OnCreateSystem<UIRoleView>
	{

		public override void OnCreate(UIRoleView self)
		{
			self.lblCE = self.AddUIComponent<UITextmesh>("spBg/sp/lblCE");
			self.btnUpLevel = self.AddUIComponent<UIButton>("spBg/btnUpLevel");
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.SliderLevel = self.AddUIComponent<UISlider>("spBg/SliderLevel");
			self.lblLevel = self.AddUIComponent<UITextmesh>("spBg/SliderLevel/FrameIcon_s/lblLevel");
			self.lblExpProgress = self.AddUIComponent<UITextmesh>("spBg/SliderLevel/lblExpProgress");
			self.lblNowAddPoint = self.AddUIComponent<UITextmesh>("spBg/sp/lblNowAddPoint");
			self.lblAtk = self.AddUIComponent<UITextmesh>("spBg/sp/lblAtk");
			self.btnUpAtk = self.AddUIComponent<UIButton>("spBg/sp/lblAtk/btnUpAtk");
			self.lblDef = self.AddUIComponent<UITextmesh>("spBg/sp/lblDef");
			self.btnUpDef = self.AddUIComponent<UIButton>("spBg/sp/lblDef/btnUpDef");
			self.lblHp = self.AddUIComponent<UITextmesh>("spBg/sp/lblHp");
			self.btnUpHp = self.AddUIComponent<UIButton>("spBg/sp/lblHp/btnUpHp");
			self.lblDmg = self.AddUIComponent<UITextmesh>("spBg/sp/lblDmg");
			self.btnUpDmg = self.AddUIComponent<UIButton>("spBg/sp/lblDmg/btnUpDmg");
			self.btnUpTip = self.AddUIComponent<UIButton>("spBg/sp/btnUpTip");
			self.btnUpLevel.SetOnClick(()=>{self.OnClickbtnUpLevel();});
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose().Coroutine();});
			self.btnUpAtk.SetOnClick(()=>{self.OnClickbtnUpAtk();});
			self.btnUpDef.SetOnClick(()=>{self.OnClickbtnUpDef();});
			self.btnUpHp.SetOnClick(()=>{self.OnClickbtnUpHp();});
			self.btnUpDmg.SetOnClick(()=>{self.OnClickbtnUpDmg();});
			self.btnUpTip.SetOnClick(()=>{self.OnClickbtnUpTip();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIRoleView))]
	public class UIRoleViewLoadSystem : LoadSystem<UIRoleView>
	{

		public override void Load(UIRoleView self)
		{
			self.btnUpLevel.SetOnClick(()=>{self.OnClickbtnUpLevel();});
			self.btnUpAtk.SetOnClick(()=>{self.OnClickbtnUpAtk();});
			self.btnUpDef.SetOnClick(()=>{self.OnClickbtnUpDef();});
			self.btnUpHp.SetOnClick(()=>{self.OnClickbtnUpHp();});
			self.btnUpDmg.SetOnClick(()=>{self.OnClickbtnUpDmg();});
			self.btnUpTip.SetOnClick(()=>{self.OnClickbtnUpTip();});
		}

	}
	[FriendClass(typeof(UIRoleView))]
	[FriendClass(typeof(AdventureComponent))]
	public static class UIRoleViewSystem
	{
		public static void UpdateView(this UIRoleView self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.scene.CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
			
			self.lblLevel.SetText((numericComponent.GetAsInt((int)NumericType.Lv)).ToString());
			self.lblAtk.SetText($"攻击:{numericComponent.GetAsInt((int)NumericType.Atk).ToString()}");
			self.lblDef.SetText($"防御:{numericComponent.GetAsInt((int)NumericType.Def).ToString()}");
			self.lblHp.SetText($"生命:{numericComponent.GetAsInt((int)NumericType.MaxHp).ToString()}");
			self.lblDmg.SetText($"伤害:{numericComponent.GetAsInt((int)NumericType.Dmg).ToString()}");
			self.lblNowAddPoint.SetText($"当前可加点数:{numericComponent.GetAsInt((int)NumericType.AttributePoint).ToString()}");
			self.lblCE.SetText($"战斗力:{numericComponent.GetAsInt((int)NumericType.CE).ToString()}");
			
			int UnitLevel = numericComponent.GetAsInt(NumericType.Lv);
			var needExp = PlayerLevelConfigCategory.Instance.Get(UnitLevel+1).NeedExp;
			long nowExp = numericComponent.GetAsLong(NumericType.Exp);
			
			self.lblExpProgress.SetText($"{nowExp}/{needExp}");
			float slideValue = nowExp > needExp? 1 : (float)nowExp / (float)needExp;
			self.SliderLevel.SetValue(slideValue);

		}
		public static async ETTask OnClickbtnClose(this UIRoleView self)
		{
			await UIManagerComponent.Instance.CloseWindow<UIRoleView>();
		}
		public static void OnClickbtnUpLevel(this UIRoleView self)
		{
			NumericHelper.ReqeustUpRoleLevel(self.scene).Coroutine();
		}
		public static void OnClickbtnUpAtk(this UIRoleView self)
		{
			AddAtrPoint(self,NumericType.Atk);
		}
		public static void OnClickbtnUpDef(this UIRoleView self)
		{
			AddAtrPoint(self,NumericType.Def);
		}
		public static void OnClickbtnUpHp(this UIRoleView self)
		{
			AddAtrPoint(self,NumericType.MaxHp);
		}
		public static void OnClickbtnUpDmg(this UIRoleView self)
		{
			AddAtrPoint(self,NumericType.Dmg);
		}
		public static void OnClickbtnUpTip(this UIRoleView self)
		{
			Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "一次加点可选择提升5点攻击、5点防御、20点生命、1点伤害" }).Coroutine();
		}
		
		static void AddAtrPoint(this UIRoleView self,int type)
		{
			NumericHelper.ReqeustAddAttributePoint(self.scene, type).Coroutine();
		}
	}

}
