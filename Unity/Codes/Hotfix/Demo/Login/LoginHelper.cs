using System;


namespace ET
{
    [FriendClass(typeof(RouterDataComponent))]
    [FriendClass(typeof(GetRouterComponent))]
    public static class LoginHelper
    {
        [Timer(TimerType.LoginTimeOut)]
        public class LoginTimeOut: ATimer<ETCancellationToken>
        {
            public override void Run(ETCancellationToken cancel)
            {
                try
                {
                    cancel.Cancel();
                    Log.Info("Login Time Out");
                }
                catch (Exception e)
                {
                    Log.Error($"move timer error: LoginTimeOut\n{e}");
                }
            }
        }

        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password, Action onError = null)
        {
            A2C_LoginAccount a2CLoginAccount = null;
            Session accountSession = null;
            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                a2CLoginAccount = (A2C_LoginAccount)await accountSession.Call(new C2A_LoginAccount() { AccountName = account, Password = password });
            }
            catch (Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
                onError?.Invoke();
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CLoginAccount.Error != ErrorCode.ERR_Success)
            {
                accountSession?.Dispose();
                onError?.Invoke();
                return a2CLoginAccount.Error;
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;

            zoneScene.GetComponent<AccountInfoComponent>().token = a2CLoginAccount.Token;
            zoneScene.GetComponent<AccountInfoComponent>().accountId = a2CLoginAccount.AccountId;

            return ErrorCode.ERR_Success;
        }
    }
}