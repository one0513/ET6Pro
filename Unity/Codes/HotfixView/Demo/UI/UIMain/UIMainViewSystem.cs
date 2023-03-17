using System.Collections;
using System.Collections.Generic;
using System;
using ET.Room;
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
			self.btnTask.AddUIComponent<UIRedDotComponent, string>("","UIMainTask");
			
			self.btnCreateRoom = self.AddUIComponent<UIButton>("btnCreateRoom");
			self.btnRoomList = self.AddUIComponent<UIButton>("btnRoomList");
			self.btnOutRoom = self.AddUIComponent<UIButton>("btnOutRoom");
			self.btnMyRoom= self.AddUIComponent<UIButton>("btnMyRoom");
			
			self.btnRole.SetOnClick(()=>{self.OnClickbtnRole().Coroutine();});
			self.btnAdventure.SetOnClick(()=>{self.OnClickbtnAdventure().Coroutine();});
			self.btnNextAdventure.SetOnClick(()=>{self.OnClickbtnNextAdventure().Coroutine();});
			self.btnTask.SetOnClick(()=>{self.OnClickbtnTask();});
			
			self.btnCreateRoom.SetOnClick(()=>{self.OnClickbtnCreateRoom().Coroutine();});
			self.btnRoomList.SetOnClick(()=>{self.OnClickbtnRoomList().Coroutine();});
			self.btnOutRoom.SetOnClick(()=>{self.OnClickbtnOutRoom().Coroutine();});
			self.btnMyRoom.SetOnClick(()=>{self.OnClickbtnMyRoom().Coroutine();});
			
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
			self.btnCreateRoom.SetOnClick(()=>{self.OnClickbtnCreateRoom().Coroutine();});
			self.btnRoomList.SetOnClick(()=>{self.OnClickbtnRoomList().Coroutine();});
			self.btnOutRoom.SetOnClick(()=>{self.OnClickbtnOutRoom().Coroutine();});
			self.btnMyRoom.SetOnClick(()=>{self.OnClickbtnMyRoom().Coroutine();});
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
			UIManagerComponent.Instance.OpenWindow<UITaskView, Scene>(UITaskView.PrefabPath, self.scene).Coroutine();
		}

		public static async ETTask OnClickbtnCreateRoom(this UIMainView self)
		{
			M2C_CreateBattleRoom m2CCreateBattleRoom = (M2C_CreateBattleRoom) await self.scene.GetComponent<SessionComponent>().Session.Call(new C2M_CreateBattleRoom() {});
		}
		public static async ETTask OnClickbtnRoomList(this UIMainView self)
		{
			
			M2C_GetBattleRoomInfoList m2CGetBattleRoomInfoList= (M2C_GetBattleRoomInfoList) await self.scene.GetComponent<SessionComponent>().Session.Call(new C2M_GetBattleRoomInfoList() {});
			if (m2CGetBattleRoomInfoList.Error == ErrorCode.ERR_Success)
			{
				await UIManagerComponent.Instance.OpenWindow<UIRoomListView, Scene>(UIRoomListView.PrefabPath, self.scene);
				List<RoomInfo> infos = new List<RoomInfo>();
				foreach (var battleRoomInfo in m2CGetBattleRoomInfoList.BattleRoomInfoList)
				{
					RoomInfo info = new RoomInfo();
					info.FromMessage(battleRoomInfo);
					infos.Add(info);
				}
				UIManagerComponent.Instance.GetWindow<UIRoomListView>()?.UpdateView(infos);
			}
			
		}
		
		public static async ETTask OnClickbtnOutRoom(this UIMainView self)
		{
			M2C_OutBattleRoom m2COutBattleRoom = (M2C_OutBattleRoom) await self.scene.GetComponent<SessionComponent>().Session.Call(new C2M_OutBattleRoom() {});
		}
		public static async ETTask OnClickbtnMyRoom(this UIMainView self)
		{
			M2C_GetBattleRoomPlayerInfo m2CGetBattleRoomPlayerInfo = (M2C_GetBattleRoomPlayerInfo) await self.scene.GetComponent<SessionComponent>().Session.Call(new C2M_GetBattleRoomPlayerInfo() {});
			if (m2CGetBattleRoomPlayerInfo.Error==ErrorCode.ERR_Success)
			{
				UIMyRoomView view = await UIManagerComponent.Instance.OpenWindow<UIMyRoomView>(UIMyRoomView.PrefabPath);
				view.UpdateView(m2CGetBattleRoomPlayerInfo.PlayerInfoList).Coroutine();
			}
			else
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "当前暂未加入小队" }).Coroutine();
			}
		}
		
		public static void UpdateView(this UIMainView self)
		{
			if (self.scene != null)
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

}
