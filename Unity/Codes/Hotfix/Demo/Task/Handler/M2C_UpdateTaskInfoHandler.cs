namespace ET
{
    public class M2C_UpdateTaskInfoHandler : AMHandler<M2C_UpdateTaskInfo>
    {
        protected override void Run(Session session, M2C_UpdateTaskInfo message)
        {
            session.ZoneScene().GetComponent<TasksComponent>().AddOrUpdateTaskInfo(message.TaskInfoProto);
        }
    }
}