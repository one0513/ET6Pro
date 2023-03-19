namespace ET
{
    public class M2C_MonsterDieHandler : AMHandler<M2C_MonsterDie>
    {
        protected override async void Run(Session session, M2C_MonsterDie message)
        {
            UnitComponent unitComponent = session.DomainScene().CurrentScene()?.GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                return;
            }
            await  TimerComponent.Instance.WaitAsync(2000);
            foreach (long unitId in message.UnitIds)
            {
                
                unitComponent.Remove(unitId);
            }
        }
    }
}