using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(ObjectQueryResponse))]
	[Message(InnerOpcode.ObjectQueryRequest)]
	[ProtoContract]
	public partial class ObjectQueryRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(A2M_Reload))]
	[Message(InnerOpcode.M2A_Reload)]
	[ProtoContract]
	public partial class M2A_Reload: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerOpcode.A2M_Reload)]
	[ProtoContract]
	public partial class A2M_Reload: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2G_LockResponse))]
	[Message(InnerOpcode.G2G_LockRequest)]
	[ProtoContract]
	public partial class G2G_LockRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[Message(InnerOpcode.G2G_LockResponse)]
	[ProtoContract]
	public partial class G2G_LockResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2G_LockReleaseResponse))]
	[Message(InnerOpcode.G2G_LockReleaseRequest)]
	[ProtoContract]
	public partial class G2G_LockReleaseRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[Message(InnerOpcode.G2G_LockReleaseResponse)]
	[ProtoContract]
	public partial class G2G_LockReleaseResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectAddResponse))]
	[Message(InnerOpcode.ObjectAddRequest)]
	[ProtoContract]
	public partial class ObjectAddRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectAddResponse)]
	[ProtoContract]
	public partial class ObjectAddResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectLockResponse))]
	[Message(InnerOpcode.ObjectLockRequest)]
	[ProtoContract]
	public partial class ObjectLockRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long InstanceId { get; set; }

		[ProtoMember(3)]
		public int Time { get; set; }

	}

	[Message(InnerOpcode.ObjectLockResponse)]
	[ProtoContract]
	public partial class ObjectLockResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectUnLockResponse))]
	[Message(InnerOpcode.ObjectUnLockRequest)]
	[ProtoContract]
	public partial class ObjectUnLockRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long OldInstanceId { get; set; }

		[ProtoMember(3)]
		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectUnLockResponse)]
	[ProtoContract]
	public partial class ObjectUnLockResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectRemoveResponse))]
	[Message(InnerOpcode.ObjectRemoveRequest)]
	[ProtoContract]
	public partial class ObjectRemoveRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectRemoveResponse)]
	[ProtoContract]
	public partial class ObjectRemoveResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectGetResponse))]
	[Message(InnerOpcode.ObjectGetRequest)]
	[ProtoContract]
	public partial class ObjectGetRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectGetResponse)]
	[ProtoContract]
	public partial class ObjectGetResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(G2R_GetLoginKey))]
	[Message(InnerOpcode.R2G_GetLoginKey)]
	[ProtoContract]
	public partial class R2G_GetLoginKey: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

	}

	[Message(InnerOpcode.G2R_GetLoginKey)]
	[ProtoContract]
	public partial class G2R_GetLoginKey: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long GateId { get; set; }

	}

	[Message(InnerOpcode.M2M_UnitTransferResponse)]
	[ProtoContract]
	public partial class M2M_UnitTransferResponse: Object, IActorResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public long NewInstanceId { get; set; }

	}

	[Message(InnerOpcode.M2M_UnitAreaTransferResponse)]
	[ProtoContract]
	public partial class M2M_UnitAreaTransferResponse: Object, IActorResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public long NewInstanceId { get; set; }

	}

	[Message(InnerOpcode.M2M_UnitAreaRemove)]
	[ProtoContract]
	public partial class M2M_UnitAreaRemove: Object, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long UnitId { get; set; }

	}

	[Message(InnerOpcode.G2M_SessionDisconnect)]
	[ProtoContract]
	public partial class G2M_SessionDisconnect: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerOpcode.M2M_PathfindingResult)]
	[ProtoContract]
	public partial class M2M_PathfindingResult: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public float X { get; set; }

		[ProtoMember(3)]
		public float Y { get; set; }

		[ProtoMember(4)]
		public float Z { get; set; }

		[ProtoMember(5)]
		public List<float> Xs = new List<float>();

		[ProtoMember(6)]
		public List<float> Ys = new List<float>();

		[ProtoMember(7)]
		public List<float> Zs = new List<float>();

	}

	[Message(InnerOpcode.M2M_Stop)]
	[ProtoContract]
	public partial class M2M_Stop: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public long Id { get; set; }

		[ProtoMember(3)]
		public float X { get; set; }

		[ProtoMember(4)]
		public float Y { get; set; }

		[ProtoMember(5)]
		public float Z { get; set; }

		[ProtoMember(6)]
		public float A { get; set; }

		[ProtoMember(7)]
		public float B { get; set; }

		[ProtoMember(8)]
		public float C { get; set; }

		[ProtoMember(9)]
		public float W { get; set; }

	}

	[Message(InnerOpcode.M2M_UseSkill)]
	[ProtoContract]
	public partial class M2M_UseSkill: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public int SkillConfigId { get; set; }

		[ProtoMember(3)]
		public long Sender { get; set; }

		[ProtoMember(4)]
		public long Reciver { get; set; }

		[ProtoMember(5)]
		public float X { get; set; }

		[ProtoMember(6)]
		public float Y { get; set; }

		[ProtoMember(7)]
		public float Z { get; set; }

	}

	[Message(InnerOpcode.M2M_AddBuff)]
	[ProtoContract]
	public partial class M2M_AddBuff: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public long Timestamp { get; set; }

		[ProtoMember(4)]
		public long UnitId { get; set; }

		[ProtoMember(5)]
		public long SourceId { get; set; }

	}

	[Message(InnerOpcode.M2M_Damage)]
	[ProtoContract]
	public partial class M2M_Damage: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public long FromId { get; set; }

		[ProtoMember(3)]
		public long ToId { get; set; }

		[ProtoMember(4)]
		public long Damage { get; set; }

		[ProtoMember(5)]
		public long NowBase { get; set; }

	}

	[Message(InnerOpcode.M2M_ChangeSkillGroup)]
	[ProtoContract]
	public partial class M2M_ChangeSkillGroup: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public long UnitId { get; set; }

		[ProtoMember(3)]
		public int Result { get; set; }

		[ProtoMember(4)]
		public long Timestamp { get; set; }

	}

	[Message(InnerOpcode.M2M_RemoveBuff)]
	[ProtoContract]
	public partial class M2M_RemoveBuff: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public long Timestamp { get; set; }

		[ProtoMember(4)]
		public long UnitId { get; set; }

	}

	[Message(InnerOpcode.M2M_Interrupt)]
	[ProtoContract]
	public partial class M2M_Interrupt: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public long Timestamp { get; set; }

		[ProtoMember(4)]
		public long UnitId { get; set; }

	}

	[ResponseType(nameof(L2A_LoginAccountResponse))]
	[Message(InnerOpcode.A2L_LoginAccountRequest)]
	[ProtoContract]
	public partial class A2L_LoginAccountRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

	}

	[Message(InnerOpcode.L2A_LoginAccountResponse)]
	[ProtoContract]
	public partial class L2A_LoginAccountResponse: Object, IActorResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2L_DisconnectGateUnit))]
	[Message(InnerOpcode.L2G_DisconnectGateUnit)]
	[ProtoContract]
	public partial class L2G_DisconnectGateUnit: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

	}

	[Message(InnerOpcode.G2L_DisconnectGateUnit)]
	[ProtoContract]
	public partial class G2L_DisconnectGateUnit: Object, IActorResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(R2A_GetRealmKey))]
	[Message(InnerOpcode.A2R_GetRealmKey)]
	[ProtoContract]
	public partial class A2R_GetRealmKey: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

	}

	[Message(InnerOpcode.R2A_GetRealmKey)]
	[ProtoContract]
	public partial class R2A_GetRealmKey: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string RealmKey { get; set; }

	}

	[ResponseType(nameof(G2R_GetLoginGateKey))]
	[Message(InnerOpcode.R2G_GetLoginGateKey)]
	[ProtoContract]
	public partial class R2G_GetLoginGateKey: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

	}

	[Message(InnerOpcode.G2R_GetLoginGateKey)]
	[ProtoContract]
	public partial class G2R_GetLoginGateKey: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string GateSessionKey { get; set; }

	}

	[ResponseType(nameof(L2G_AddLoginRecord))]
	[Message(InnerOpcode.G2L_AddLoginRecord)]
	[ProtoContract]
	public partial class G2L_AddLoginRecord: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

		[ProtoMember(2)]
		public int ServerId { get; set; }

	}

	[Message(InnerOpcode.L2G_AddLoginRecord)]
	[ProtoContract]
	public partial class L2G_AddLoginRecord: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2G_RequestExitGame))]
	[Message(InnerOpcode.G2M_RequestExitGame)]
	[ProtoContract]
	public partial class G2M_RequestExitGame: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerOpcode.M2G_RequestExitGame)]
	[ProtoContract]
	public partial class M2G_RequestExitGame: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(L2G_RemoveLoginRecord))]
	[Message(InnerOpcode.G2L_RemoveLoginRecord)]
	[ProtoContract]
	public partial class G2L_RemoveLoginRecord: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

		[ProtoMember(2)]
		public int ServerId { get; set; }

	}

	[Message(InnerOpcode.L2G_RemoveLoginRecord)]
	[ProtoContract]
	public partial class L2G_RemoveLoginRecord: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

//----------��һ������----------------
//���ӻ��߸���Unit����
	[ResponseType(nameof(UnitCache2Other_AddOrUpdateUnit))]
	[Message(InnerOpcode.Other2UnitCache_AddOrUpdateUnit)]
	[ProtoContract]
	public partial class Other2UnitCache_AddOrUpdateUnit: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		///<summary>��Ҫ�����UnitId</summary>
		[ProtoMember(1)]
		public long UnitId { get; set; }

		///<summary>ʵ������</summary>
		[ProtoMember(2)]
		public List<string> EntityTypes = new List<string>();

		///<summary>ʵ�����л����bytes</summary>
		[ProtoMember(3)]
		public List<byte[]> EntityBytes = new List<byte[]>();

	}

	[Message(InnerOpcode.UnitCache2Other_AddOrUpdateUnit)]
	[ProtoContract]
	public partial class UnitCache2Other_AddOrUpdateUnit: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

//��ȡUnit����
	[ResponseType(nameof(UnitCache2Other_GetUnit))]
	[Message(InnerOpcode.Other2UnitCache_GetUnit)]
	[ProtoContract]
	public partial class Other2UnitCache_GetUnit: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

		///<summary>��Ҫ��ȡ�������</summary>
		[ProtoMember(2)]
		public List<string> ComponentNameList = new List<string>();

	}

//ɾ��Unit����
	[ResponseType(nameof(UnitCache2Other_DeleteUnit))]
	[Message(InnerOpcode.Other2UnitCache_DeleteUnit)]
	[ProtoContract]
	public partial class Other2UnitCache_DeleteUnit: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

	}

	[Message(InnerOpcode.UnitCache2Other_DeleteUnit)]
	[ProtoContract]
	public partial class UnitCache2Other_DeleteUnit: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2G_RequestEnterGameState))]
	[Message(InnerOpcode.G2M_RequestEnterGameState)]
	[ProtoContract]
	public partial class G2M_RequestEnterGameState: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerOpcode.M2G_RequestEnterGameState)]
	[ProtoContract]
	public partial class M2G_RequestEnterGameState: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

}
