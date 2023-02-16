using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Module;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmPatchDone : IStateNode
{
	void IStateNode.OnCreate(StateMachine machine)
	{
	}
	void IStateNode.OnEnter()
	{
		PatchEventDefine.PatchStatesChange.SendEventMessage("开始游戏！");
		CodeLoader.Instance.Start();

		var go = Transform.FindObjectOfType<PatchWindow>();
		go.gameObject.SetActive(false);
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}
}