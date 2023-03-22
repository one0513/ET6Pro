using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UIForgeMarItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public UIEmptyGameobject TransfoBagItem;
		public UISlider SliderNum;
		public UITextmesh lblNum;
		public UIButton btnPut;

		public Scene scene;
		public Item Data;
		public UIBagItem BagItem = null;
		public int curSelectNum = 0;
		public int tempNum = 0;

	}
}
