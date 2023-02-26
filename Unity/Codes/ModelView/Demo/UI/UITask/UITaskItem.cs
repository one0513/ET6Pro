using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UITaskItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public UITextmesh lblTaskName;
		public UITextmesh lblDesc;
		public UITextmesh lblProgress;
		public UITextmesh lblReward;
		public UIButton btnGetReward;
		public UITextmesh lblTaskStatue;
		public Scene scene;
		public bool canGetReward = false;
		public int configId;

	}
}
