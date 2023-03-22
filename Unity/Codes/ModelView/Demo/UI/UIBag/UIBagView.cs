using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIBagView : Entity, IAwake, ILoad, IOnCreate, IOnEnable, IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UIBag/Prefabs/UIBagView.prefab";
		public UIButton btnClose;
		public UIEmptyGameobject ItemList;
		public UIButton btnAll;
		public UIButton btnItem;
		public UIButton btnEquipment;

		public Scene scene;
		public List<Item> CurShowItems = new List<Item>(){new Item(){Id = 1},new Item(){Id = 2},new Item(){Id = 3},new Item(){Id = 4},new Item(){Id = 5}};
		public List<UIBagItem> BagItems = new List<UIBagItem>();

		public int curSelectType = 0;
	}
}
