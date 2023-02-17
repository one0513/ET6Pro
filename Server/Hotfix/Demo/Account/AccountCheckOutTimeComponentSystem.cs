using System;

namespace ET
{
    
    [Timer(TimerType.LoginTimeOut)]
    public class LoginTimeOut: ATimer<AccountCheckOutTimeComponent>
    {
        public override void Run(AccountCheckOutTimeComponent self)
        {
            try
            {
                self.DeleteSession();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
    
    public class AccountCheckOutTimeComponentAwakeSystem : AwakeSystem<AccountCheckOutTimeComponent,long>
    {
        public override void Awake(AccountCheckOutTimeComponent self, long accountId)
        {
            self.accountId = accountId;

            TimerComponent.Instance.Remove(ref self.timer);
            self.timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 600000, TimerType.LoginTimeOut, self);
        }
    }
    
    public class AccountCheckOutTimeComponentDestroySystem : DestroySystem<AccountCheckOutTimeComponent>
    {
        public override void Destroy(AccountCheckOutTimeComponent self)
        {
            self.accountId = 0;
            TimerComponent.Instance.Remove(ref self.timer);
        }
    }

    [FriendClass(typeof(AccountCheckOutTimeComponent))]
    public static class AccountCheckOutTimeComponentSystem
    {
        public static void DeleteSession(this AccountCheckOutTimeComponent self)
        {
            Session session = self.GetParent<Session>();

            long sessionInstanceId = session.DomainScene().GetComponent<AccountSessionComponent>().Get(self.accountId);
            if (session.InstanceId == sessionInstanceId)
            {
                session.DomainScene().GetComponent<AccountSessionComponent>().Remove(self.accountId);
            }

            session?.Send(new A2C_Disconnect() { Error = 1 });
            session?.Disconnect().Coroutine();
        }
    }
}