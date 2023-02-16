using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UILoginViewTest : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UILogin/Prefabs/UILoginViewTest.prefab";
		public UITextmesh lbl;
		public UIImage sp;
		public UIImage sp_blue;
		 

	}
}
