using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[ChildOfAttribute(typeof(UIBagEquipDescView))]
	public class UIBagEquipAtrItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIBag/Prefabs/UIBagEquipAtrItem.prefab";
		public UITextmesh lblAttr;
		 

	}
}
