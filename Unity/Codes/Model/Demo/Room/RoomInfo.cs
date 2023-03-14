using System.Collections.Generic;

namespace ET
{
    [ComponentOf]
    
#if SERVER
    public class RoomInfo : Entity,IAwake,ITransfer,IUnitCache
#else
    public class RoomInfo : Entity,IAwake
#endif
    {
        public string RoomName;
        
        public long RoomId;

        public int RoomPlayerNum = 0;

        public List<long> playerList = new List<long>();
        
    }

    
}