namespace ET.Room
{
    [FriendClass(typeof(RoomInfo))]
    public static class RoomInfoSystem
    {
        public static void FromMessage(this RoomInfo self, BattleRoomInfoProto battleRoomInfoProto)
        {
            self.RoomId = battleRoomInfoProto.RoomId;
            self.RoomName = battleRoomInfoProto.RoomName;
            self.playerList = battleRoomInfoProto.PlayerList;
            self.RoomPlayerNum = battleRoomInfoProto.RoomPlayerNum;
        }

        public static BattleRoomInfoProto ToMessage(this RoomInfo self)
        {
            return new BattleRoomInfoProto()
            {
                RoomId = self.RoomId,
                RoomName = self.RoomName,
                PlayerList = self.playerList,
                RoomPlayerNum = self.RoomPlayerNum,
            };
        }
    }
}