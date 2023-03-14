namespace ET.MonsterFactory
{
    public class M2C_InitMonsterInfoListHandler : AMHandler<M2C_InitMonsterInfoList>
    {
        protected override void Run(Session session, M2C_InitMonsterInfoList message)
        {
            session.DomainScene().GetComponent<ObjectWait>().Notify(new WaitType.Wait_InitMonsterInfoList() {Message = message});
        }
    }
}