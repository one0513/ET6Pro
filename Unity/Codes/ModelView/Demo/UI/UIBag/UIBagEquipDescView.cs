using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIBagEquipDescView : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIBag/Prefabs/UIBagEquipDescView.prefab";
		public UITextmesh lblName;
		public UIButton btnClose;
		public UITextmesh lblDesc;
		public UIEmptyGameobject AttrTrans;
		public UIButton btnWear;
		public UIButton btnUnload;
		public Scene scene;
		public Item Data;

		public List<UIBagEquipAtrItem> attrList = new List<UIBagEquipAtrItem>();
	}
}
