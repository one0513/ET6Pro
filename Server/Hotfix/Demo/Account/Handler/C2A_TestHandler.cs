using System;

namespace ET
{
    public class C2A_TestHandler : AMRpcHandler<C2A_TestSendMsg, A2C_TestSendMsg>
    {
        protected override async ETTask Run(Session session, C2A_TestSendMsg request, A2C_TestSendMsg response, Action reply)
        {
            response.testMsg = "服务端测试信息1";
            reply();
            await ETTask.CompletedTask;
        }
    }
}