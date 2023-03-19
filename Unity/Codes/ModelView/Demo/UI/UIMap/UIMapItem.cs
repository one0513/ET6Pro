using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UIMapItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public UITextmesh lblMapName;
		public UITextmesh lblMonsterNum;
		public UIButton btnJoin;
		public Scene scene;
		public BattleLevelConfig data;
	}
}
