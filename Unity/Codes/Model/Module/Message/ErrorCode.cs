namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误
        
        // 110000以下的错误请看ErrorCore.cs
        
        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        public const int ERR_NetWorkError = 200002; //网络错误
        public const int ERR_LoginInfoError = 200003; //账号或密码错误
        public const int ERR_AccountMessaFormatError = 200004; //账号或密码格式错误
        public const int ERR_AccountInBlackListError = 200005; //账号在黑名单
        public const int ERR_RequestRepeatedly = 200006; //频繁请求
        public const int ERR_TokenError = 200007; //token验证错误
        public const int ERR_RequestSceneTypeError = 200008;//请求scene错误
        public const int ERR_ConnectGateKeyError = 200009;//gatekey验证失败
        public const int ERR_SessionPlayerError = 200010;
        public const int ERR_NonePlayerError = 200011;
        public const int ERR_PlayerSessionError = 200012;
        public const int ERR_SessionStateError = 200013;
        public const int ERR_ReEnterGameError = 200014;
        public const int ERR_ReEnterGameError2 = 200015;
        public const int ERR_EnterGameError = 200016;
        public const int ERR_HasCreatedRole = 200017;
        public const int ERR_NumericTypeNotExist = 200018;
        public const int ERR_NumericTypeNotAddPoint = 200019;
        public const int ERR_AddPointNotEnough = 200020;
        public const int ERR_ExpNotEnough = 200021;
        public const int ERR_ExpNumError = 200022;
        public const int ERR_OtherAccountLogin = 200023;


    }
}