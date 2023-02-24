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
	public class UIMainViewOnCreateSystem : OnCreateSystem<UIMainView>
	{

		public override void OnCreate(UIMainView self)
		{
			self.lblRoleName = self.AddUIComponent<UITextmesh>("sp/lblRoleName");
			self.lblLevel = self.AddUIComponent<UITextmesh>("sp/lblLevel");
			self.lblExp = self.AddUIComponent<UITextmesh>("sp/lblExp");
			self.btnRole = self.AddUIComponent<UIButton>("btnRole");
			self.btnAdventure = self.AddUIComponent<UIButton>("btnAdventure");
			self.btnTask = self.AddUIComponent<UIButton>("btnTask");
			self.btnRole.SetOnClick(()=>{self.OnClickbtnRole().Coroutine();});
			self.btnAdventure.SetOnClick(()=>{self.OnClickbtnAdventure().Coroutine();});
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
			self.btnTask.SetOnClick(()=>{self.OnClickbtnTask();});
		}

	}
	[FriendClass(typeof(UIMainView))]
	public static class UIMainViewSystem
	{
		public static async ETTask OnClickbtnRole(this UIMainView self)
		{
			var a = self.scene.CurrentScene();
			Session session = self.scene.GetComponent<SessionComponent>().Session;
			//M2C_TestLoaction m2CTestUnitNumeric = (M2C_TestLoaction) await session.Call(new C2M_TestLoaction() { });
			self.scene.GetComponent<UnitComponent>();
			
			A2C_TestSendMsg m2CTestUnitNumeric = (A2C_TestSendMsg) await session.Call(new C2A_TestSendMsg() { });

			
			// try
			// {
			// 	int error = await NumericHelper.TestUpdateNumeric(self.scene);
			// 	if (error != ErrorCode.ERR_Success)
			// 	{
			// 		return;
			// 	}
			// 	Log.Debug("测试更新属性成功");
			// }
			// catch (Exception e)
			// {
			// 	Log.Error(e.ToString());
			// }
		}
		public static async ETTask OnClickbtnAdventure(this UIMainView self)
		{
			Session session = self.scene.GetComponent<SessionComponent>().Session;
			M2C_TestUnitNumeric m2CTestUnitNumeric = (M2C_TestUnitNumeric) await session.Call(new C2M_TestUnitNumeric() { });
		}
		public static void OnClickbtnTask(this UIMainView self)
		{

		}

		public static void UpdateView(this UIMainView self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.scene.CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
			
			self.lblExp.SetText(numericComponent.GetAsInt((int)NumericType.Exp).ToString());
			self.lblLevel.SetText(numericComponent.GetAsInt((int)NumericType.Level).ToString());

		}
	}

}
