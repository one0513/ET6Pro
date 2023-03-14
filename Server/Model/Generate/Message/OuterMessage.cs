using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(M2C_TestResponse))]
	[Message(OuterOpcode.C2M_TestRequest)]
	[ProtoContract]
	public partial class C2M_TestRequest: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string request { get; set; }

	}

	[Message(OuterOpcode.M2C_TestResponse)]
	[ProtoContract]
	public partial class M2C_TestResponse: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string response { get; set; }

	}

	[ResponseType(nameof(Actor_TransferResponse))]
	[Message(OuterOpcode.Actor_TransferRequest)]
	[ProtoContract]
	public partial class Actor_TransferRequest: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int MapIndex { get; set; }

	}

	[Message(OuterOpcode.Actor_TransferResponse)]
	[ProtoContract]
	public partial class Actor_TransferResponse: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2C_EnterMap))]
	[Message(OuterOpcode.C2G_EnterMap)]
	[ProtoContract]
	public partial class C2G_EnterMap: Object, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.G2C_EnterMap)]
	[ProtoContract]
	public partial class G2C_EnterMap: Object, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

// 自己unitId
		[ProtoMember(4)]
		public long MyId { get; set; }

	}

	[Message(OuterOpcode.MoveInfo)]
	[ProtoContract]
	public partial class MoveInfo: Object
	{
		[ProtoMember(1)]
		public List<float> X = new List<float>();

		[ProtoMember(2)]
		public List<float> Y = new List<float>();

		[ProtoMember(3)]
		public List<float> Z = new List<float>();

		[ProtoMember(4)]
		public float A { get; set; }

		[ProtoMember(5)]
		public float B { get; set; }

		[ProtoMember(6)]
		public float C { get; set; }

		[ProtoMember(7)]
		public float W { get; set; }

		[ProtoMember(8)]
		public int TurnSpeed { get; set; }

	}

	[Message(OuterOpcode.UnitInfo)]
	[ProtoContract]
	public partial class UnitInfo: Object
	{
		[ProtoMember(1)]
		public long UnitId { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public int Type { get; set; }

		[ProtoMember(4)]
		public float X { get; set; }

		[ProtoMember(5)]
		public float Y { get; set; }

		[ProtoMember(6)]
		public float Z { get; set; }

		[ProtoMember(7)]
		public float ForwardX { get; set; }

		[ProtoMember(8)]
		public float ForwardY { get; set; }

		[ProtoMember(9)]
		public float ForwardZ { get; set; }

		[ProtoMember(10)]
		public List<int> Ks = new List<int>();

		[ProtoMember(11)]
		public List<long> Vs = new List<long>();

		[ProtoMember(12)]
		public MoveInfo MoveInfo { get; set; }

		[ProtoMember(13)]
		public List<int> SkillIds = new List<int>();

		[ProtoMember(14)]
		public List<int> BuffIds = new List<int>();

		[ProtoMember(15)]
		public List<long> BuffTimestamp = new List<long>();

		[ProtoMember(16)]
		public List<long> BuffSourceIds = new List<long>();

	}

	[Message(OuterOpcode.M2C_CreateUnits)]
	[ProtoContract]
	public partial class M2C_CreateUnits: Object, IActorMessage
	{
		[ProtoMember(2)]
		public List<UnitInfo> Units = new List<UnitInfo>();

	}

	[Message(OuterOpcode.M2C_CreateMyUnit)]
	[ProtoContract]
	public partial class M2C_CreateMyUnit: Object, IActorMessage
	{
		[ProtoMember(1)]
		public UnitInfo Unit { get; set; }

		[ProtoMember(2)]
		public List<int> GuidanceDone = new List<int>();

	}

	[Message(OuterOpcode.M2C_StartSceneChange)]
	[ProtoContract]
	public partial class M2C_StartSceneChange: Object, IActorMessage
	{
		[ProtoMember(1)]
		public long SceneInstanceId { get; set; }

		[ProtoMember(2)]
		public string SceneName { get; set; }

	}

	[Message(OuterOpcode.M2C_RemoveUnits)]
	[ProtoContract]
	public partial class M2C_RemoveUnits: Object, IActorMessage
	{
		[ProtoMember(2)]
		public List<long> Units = new List<long>();

	}

	[Message(OuterOpcode.C2M_PathfindingResult)]
	[ProtoContract]
	public partial class C2M_PathfindingResult: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }

		[ProtoMember(3)]
		public float Z { get; set; }

	}

	[Message(OuterOpcode.C2M_Stop)]
	[ProtoContract]
	public partial class C2M_Stop: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_PathfindingResult)]
	[ProtoContract]
	public partial class M2C_PathfindingResult: Object, IActorMessage
	{
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

	[Message(OuterOpcode.M2C_Stop)]
	[ProtoContract]
	public partial class M2C_Stop: Object, IActorMessage
	{
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

	[ResponseType(nameof(G2C_Ping))]
	[Message(OuterOpcode.C2G_Ping)]
	[ProtoContract]
	public partial class C2G_Ping: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.G2C_Ping)]
	[ProtoContract]
	public partial class G2C_Ping: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long Time { get; set; }

	}

	[Message(OuterOpcode.G2C_Test)]
	[ProtoContract]
	public partial class G2C_Test: Object, IMessage
	{
	}

	[ResponseType(nameof(M2C_Reload))]
	[Message(OuterOpcode.C2M_Reload)]
	[ProtoContract]
	public partial class C2M_Reload: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterOpcode.M2C_Reload)]
	[ProtoContract]
	public partial class M2C_Reload: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(R2C_Login))]
	[Message(OuterOpcode.C2R_Login)]
	[ProtoContract]
	public partial class C2R_Login: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		///<summary>帐号</summary>
		[ProtoMember(1)]
		public string Account { get; set; }

		///<summary>密码</summary>
		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterOpcode.R2C_Login)]
	[ProtoContract]
	public partial class R2C_Login: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string Address { get; set; }

		[ProtoMember(2)]
		public long Key { get; set; }

		[ProtoMember(3)]
		public long GateId { get; set; }

	}

	[ResponseType(nameof(G2C_LoginGate))]
	[Message(OuterOpcode.C2G_LoginGate)]
	[ProtoContract]
	public partial class C2G_LoginGate: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		///<summary>帐号</summary>
		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long GateId { get; set; }

	}

	[Message(OuterOpcode.G2C_LoginGate)]
	[ProtoContract]
	public partial class G2C_LoginGate: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long PlayerId { get; set; }

	}

	[Message(OuterOpcode.G2C_TestHotfixMessage)]
	[ProtoContract]
	public partial class G2C_TestHotfixMessage: Object, IMessage
	{
		[ProtoMember(1)]
		public string Info { get; set; }

	}

	[ResponseType(nameof(M2C_TestRobotCase))]
	[Message(OuterOpcode.C2M_TestRobotCase)]
	[ProtoContract]
	public partial class C2M_TestRobotCase: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int N { get; set; }

	}

	[Message(OuterOpcode.M2C_TestRobotCase)]
	[ProtoContract]
	public partial class M2C_TestRobotCase: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public int N { get; set; }

	}

	[ResponseType(nameof(M2C_TransferMap))]
	[Message(OuterOpcode.C2M_TransferMap)]
	[ProtoContract]
	public partial class C2M_TransferMap: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_TransferMap)]
	[ProtoContract]
	public partial class M2C_TransferMap: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterOpcode.C2M_UseSkill)]
	[ProtoContract]
	public partial class C2M_UseSkill: Object, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int SkillConfigId { get; set; }

		[ProtoMember(2)]
		public long Id { get; set; }

		[ProtoMember(3)]
		public float X { get; set; }

		[ProtoMember(4)]
		public float Y { get; set; }

		[ProtoMember(5)]
		public float Z { get; set; }

	}

	[Message(OuterOpcode.M2C_UseSkill)]
	[ProtoContract]
	public partial class M2C_UseSkill: Object, IActorMessage
	{
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

	[Message(OuterOpcode.M2C_AddBuff)]
	[ProtoContract]
	public partial class M2C_AddBuff: Object, IActorMessage
	{
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

	[Message(OuterOpcode.M2C_Damage)]
	[ProtoContract]
	public partial class M2C_Damage: Object, IActorMessage
	{
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

	[Message(OuterOpcode.M2C_ChangeSkillGroup)]
	[ProtoContract]
	public partial class M2C_ChangeSkillGroup: Object, IActorMessage
	{
		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public long UnitId { get; set; }

		[ProtoMember(3)]
		public int Result { get; set; }

		[ProtoMember(4)]
		public long Timestamp { get; set; }

	}

	[Message(OuterOpcode.M2C_RemoveBuff)]
	[ProtoContract]
	public partial class M2C_RemoveBuff: Object, IActorMessage
	{
		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public long Timestamp { get; set; }

		[ProtoMember(4)]
		public long UnitId { get; set; }

	}

	[Message(OuterOpcode.M2C_Interrupt)]
	[ProtoContract]
	public partial class M2C_Interrupt: Object, IActorMessage
	{
		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public long Timestamp { get; set; }

		[ProtoMember(4)]
		public long UnitId { get; set; }

	}

	[ResponseType(nameof(A2C_LoginAccount))]
	[Message(OuterOpcode.C2A_LoginAccount)]
	[ProtoContract]
	public partial class C2A_LoginAccount: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string AccountName { get; set; }

		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterOpcode.A2C_LoginAccount)]
	[ProtoContract]
	public partial class A2C_LoginAccount: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string Token { get; set; }

		[ProtoMember(2)]
		public long AccountId { get; set; }

	}

	[Message(OuterOpcode.A2C_Disconnect)]
	[ProtoContract]
	public partial class A2C_Disconnect: Object, IMessage
	{
		[ProtoMember(1)]
		public int Error { get; set; }

	}

	[ResponseType(nameof(A2C_TestSendMsg))]
	[Message(OuterOpcode.C2A_TestSendMsg)]
	[ProtoContract]
	public partial class C2A_TestSendMsg: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.A2C_TestSendMsg)]
	[ProtoContract]
	public partial class A2C_TestSendMsg: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string testMsg { get; set; }

	}

	[Message(OuterOpcode.ServerInfoProto)]
	[ProtoContract]
	public partial class ServerInfoProto: Object
	{
		[ProtoMember(1)]
		public int Id { get; set; }

		[ProtoMember(2)]
		public int Status { get; set; }

		[ProtoMember(3)]
		public string ServerName { get; set; }

	}

	[ResponseType(nameof(A2C_GetServerInfos))]
	[Message(OuterOpcode.C2A_GetServerInfos)]
	[ProtoContract]
	public partial class C2A_GetServerInfos: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Token { get; set; }

		[ProtoMember(2)]
		public long AccountId { get; set; }

	}

	[Message(OuterOpcode.A2C_GetServerInfos)]
	[ProtoContract]
	public partial class A2C_GetServerInfos: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<ServerInfoProto> ServerInfoList = new List<ServerInfoProto>();

	}

	[ResponseType(nameof(A2C_GetRealmKey))]
	[Message(OuterOpcode.C2A_GetRealmKey)]
	[ProtoContract]
	public partial class C2A_GetRealmKey: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Token { get; set; }

		[ProtoMember(2)]
		public int ServerId { get; set; }

		[ProtoMember(3)]
		public long AccountId { get; set; }

	}

	[Message(OuterOpcode.A2C_GetRealmKey)]
	[ProtoContract]
	public partial class A2C_GetRealmKey: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string RealmKey { get; set; }

		[ProtoMember(2)]
		public string RealmAddress { get; set; }

	}

	[ResponseType(nameof(R2C_LoginRealm))]
	[Message(OuterOpcode.C2R_LoginRealm)]
	[ProtoContract]
	public partial class C2R_LoginRealm: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long AccountId { get; set; }

		[ProtoMember(2)]
		public string RealmTokenKey { get; set; }

	}

	[Message(OuterOpcode.R2C_LoginRealm)]
	[ProtoContract]
	public partial class R2C_LoginRealm: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string GateSessionKey { get; set; }

		[ProtoMember(2)]
		public string GateAddress { get; set; }

	}

	[ResponseType(nameof(G2C_LoginGameGate))]
	[Message(OuterOpcode.C2G_LoginGameGate)]
	[ProtoContract]
	public partial class C2G_LoginGameGate: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Key { get; set; }

		[ProtoMember(2)]
		public long RoleId { get; set; }

		[ProtoMember(3)]
		public long Account { get; set; }

	}

	[Message(OuterOpcode.G2C_LoginGameGate)]
	[ProtoContract]
	public partial class G2C_LoginGameGate: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long PlayerId { get; set; }

	}

	[ResponseType(nameof(G2C_EnterGame))]
	[Message(OuterOpcode.C2G_EnterGame)]
	[ProtoContract]
	public partial class C2G_EnterGame: Object, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.G2C_EnterGame)]
	[ProtoContract]
	public partial class G2C_EnterGame: Object, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

// 自己unitId
		[ProtoMember(4)]
		public long MyId { get; set; }

	}

	[Message(OuterOpcode.RoleInfoProto)]
	[ProtoContract]
	public partial class RoleInfoProto: Object
	{
		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public string Name { get; set; }

		[ProtoMember(3)]
		public int State { get; set; }

		[ProtoMember(4)]
		public long AccountId { get; set; }

		[ProtoMember(5)]
		public long LastLoginTime { get; set; }

		[ProtoMember(6)]
		public long CreateTime { get; set; }

		[ProtoMember(7)]
		public int ServerId { get; set; }

	}

	[ResponseType(nameof(A2C_CreateRole))]
	[Message(OuterOpcode.C2A_CreateRole)]
	[ProtoContract]
	public partial class C2A_CreateRole: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Token { get; set; }

		[ProtoMember(2)]
		public long AccountId { get; set; }

		[ProtoMember(3)]
		public string Name { get; set; }

		[ProtoMember(4)]
		public int ServerId { get; set; }

	}

	[Message(OuterOpcode.A2C_CreateRole)]
	[ProtoContract]
	public partial class A2C_CreateRole: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public RoleInfoProto RoleInfo { get; set; }

	}

	[ResponseType(nameof(A2C_GetRoles))]
	[Message(OuterOpcode.C2A_GetRoles)]
	[ProtoContract]
	public partial class C2A_GetRoles: Object, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Token { get; set; }

		[ProtoMember(2)]
		public long AccountId { get; set; }

		[ProtoMember(3)]
		public int ServerId { get; set; }

	}

	[Message(OuterOpcode.A2C_GetRoles)]
	[ProtoContract]
	public partial class A2C_GetRoles: Object, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<RoleInfoProto> RoleInfo = new List<RoleInfoProto>();

	}

	[Message(OuterOpcode.M2C_NoticeUnitNumeric)]
	[ProtoContract]
	public partial class M2C_NoticeUnitNumeric: Object, IActorMessage
	{
		[ProtoMember(1)]
		public long UnitId { get; set; }

		[ProtoMember(2)]
		public int NumericType { get; set; }

		[ProtoMember(4)]
		public long NewValue { get; set; }

	}

	[ResponseType(nameof(M2C_TestUnitNumeric))]
	[Message(OuterOpcode.C2M_TestUnitNumeric)]
	[ProtoContract]
	public partial class C2M_TestUnitNumeric: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_TestUnitNumeric)]
	[ProtoContract]
	public partial class M2C_TestUnitNumeric: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_AddAttributePoint))]
	[Message(OuterOpcode.C2M_AddAttributePoint)]
	[ProtoContract]
	public partial class C2M_AddAttributePoint: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int NumericType { get; set; }

	}

	[Message(OuterOpcode.M2C_AddAttributePoint)]
	[ProtoContract]
	public partial class M2C_AddAttributePoint: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_UpRoleLevel))]
	[Message(OuterOpcode.C2M_UpRoleLevel)]
	[ProtoContract]
	public partial class C2M_UpRoleLevel: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_UpRoleLevel)]
	[ProtoContract]
	public partial class M2C_UpRoleLevel: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_TestLoaction))]
	[Message(OuterOpcode.C2M_TestLoaction)]
	[ProtoContract]
	public partial class C2M_TestLoaction: Object, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_TestLoaction)]
	[ProtoContract]
	public partial class M2C_TestLoaction: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_StartGameLevel))]
	[Message(OuterOpcode.C2M_StartGameLevel)]
	[ProtoContract]
	public partial class C2M_StartGameLevel: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int LevelId { get; set; }

	}

	[Message(OuterOpcode.M2C_StartGameLevel)]
	[ProtoContract]
	public partial class M2C_StartGameLevel: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_EndGameLevel))]
	[Message(OuterOpcode.C2M_EndGameLevel)]
	[ProtoContract]
	public partial class C2M_EndGameLevel: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Round { get; set; }

		[ProtoMember(3)]
		public int BattleResult { get; set; }

	}

	[Message(OuterOpcode.M2C_EndGameLevel)]
	[ProtoContract]
	public partial class M2C_EndGameLevel: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterOpcode.TaskInfoProto)]
	[ProtoContract]
	public partial class TaskInfoProto: Object
	{
		[ProtoMember(1)]
		public int ConfigId { get; set; }

		[ProtoMember(2)]
		public int TaskState { get; set; }

		[ProtoMember(3)]
		public int TaskPogress { get; set; }

	}

	[Message(OuterOpcode.M2C_UpdateTaskInfo)]
	[ProtoContract]
	public partial class M2C_UpdateTaskInfo: Object, IActorMessage
	{
		[ProtoMember(1)]
		public TaskInfoProto TaskInfoProto { get; set; }

	}

	[Message(OuterOpcode.M2C_AllTaskInfoList)]
	[ProtoContract]
	public partial class M2C_AllTaskInfoList: Object, IActorMessage
	{
		[ProtoMember(1)]
		public List<TaskInfoProto> TaskInfoProtoList = new List<TaskInfoProto>();

	}

	[ResponseType(nameof(M2C_ReceiveTaskReward))]
	[Message(OuterOpcode.C2M_ReceiveTaskReward)]
	[ProtoContract]
	public partial class C2M_ReceiveTaskReward: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int TaskConfigId { get; set; }

	}

	[Message(OuterOpcode.M2C_ReceiveTaskReward)]
	[ProtoContract]
	public partial class M2C_ReceiveTaskReward: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterOpcode.MonsterInfoProto)]
	[ProtoContract]
	public partial class MonsterInfoProto: Object
	{
		[ProtoMember(1)]
		public int ConfigId { get; set; }

		[ProtoMember(2)]
		public int CurHp { get; set; }

		[ProtoMember(3)]
		public int PosX { get; set; }

		[ProtoMember(4)]
		public int PosY { get; set; }

	}

	[Message(OuterOpcode.M2C_InitMonsterInfoList)]
	[ProtoContract]
	public partial class M2C_InitMonsterInfoList: Object, IActorMessage
	{
		[ProtoMember(1)]
		public List<UnitInfo> UnitList = new List<UnitInfo>();

	}

	[Message(OuterOpcode.BattleRoomInfoProto)]
	[ProtoContract]
	public partial class BattleRoomInfoProto: Object
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public string RoomName { get; set; }

		[ProtoMember(3)]
		public int RoomPlayerNum { get; set; }

		[ProtoMember(4)]
		public List<UnitInfo> UnitList = new List<UnitInfo>();

		[ProtoMember(5)]
		public List<long> PlayerList = new List<long>();

	}

	[Message(OuterOpcode.M2C_UpdateBattleRoomInfo)]
	[ProtoContract]
	public partial class M2C_UpdateBattleRoomInfo: Object, IActorMessage
	{
		[ProtoMember(1)]
		public BattleRoomInfoProto BattleRoomInfo { get; set; }

	}

	[ResponseType(nameof(M2C_CreateBattleRoom))]
	[Message(OuterOpcode.C2M_CreateBattleRoom)]
	[ProtoContract]
	public partial class C2M_CreateBattleRoom: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_CreateBattleRoom)]
	[ProtoContract]
	public partial class M2C_CreateBattleRoom: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_GetBattleRoomInfoList))]
	[Message(OuterOpcode.C2M_GetBattleRoomInfoList)]
	[ProtoContract]
	public partial class C2M_GetBattleRoomInfoList: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_GetBattleRoomInfoList)]
	[ProtoContract]
	public partial class M2C_GetBattleRoomInfoList: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<BattleRoomInfoProto> BattleRoomInfoList = new List<BattleRoomInfoProto>();

	}

	[ResponseType(nameof(M2C_JoinBattleRoom))]
	[Message(OuterOpcode.C2M_JoinBattleRoom)]
	[ProtoContract]
	public partial class C2M_JoinBattleRoom: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long RoomId { get; set; }

	}

	[Message(OuterOpcode.M2C_JoinBattleRoom)]
	[ProtoContract]
	public partial class M2C_JoinBattleRoom: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_OutBattleRoom))]
	[Message(OuterOpcode.C2M_OutBattleRoom)]
	[ProtoContract]
	public partial class C2M_OutBattleRoom: Object, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_OutBattleRoom)]
	[ProtoContract]
	public partial class M2C_OutBattleRoom: Object, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

}
