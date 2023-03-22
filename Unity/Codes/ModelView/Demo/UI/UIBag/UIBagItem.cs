using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	[ChildOf()]
	public class UIBagItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIBag/Prefabs/UIBagItem.prefab";
		public UIImage spIcon;
		public UITextmesh lblNum;
		public Item Data;
		public Scene scnen;
		public int tempNum = 0;
	}
}
