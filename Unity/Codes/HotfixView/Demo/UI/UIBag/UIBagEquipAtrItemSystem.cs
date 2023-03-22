using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIBagEquipAtrItem))]
	public class UIBagEquipAtrItemOnCreateSystem : OnCreateSystem<UIBagEquipAtrItem>
	{

		public override void OnCreate(UIBagEquipAtrItem self)
		{
			self.lblAttr = self.AddUIComponent<UITextmesh>("lblAttr");
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIBagEquipAtrItem))]
	public class UIBagEquipAtrItemLoadSystem : LoadSystem<UIBagEquipAtrItem>
	{

		public override void Load(UIBagEquipAtrItem self)
		{
		}

	}
	[FriendClass(typeof(UIBagEquipAtrItem))]
	[FriendClass(typeof(AttributeEntry))]
	public static class UIBagEquipAtrItemSystem
	{
		public static void SetData(this UIBagEquipAtrItem self,AttributeEntry entry)
		{
			string name = AttributeConfigCategory.Instance.Get(entry.Key).Name;
			self.lblAttr.SetText($"{name}:  {entry.Value}");
		}
	}

}
