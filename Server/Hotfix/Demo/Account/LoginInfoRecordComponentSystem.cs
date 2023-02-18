namespace ET
{
    public class LoginInfoRecordComponentDestroySystem : DestroySystem<LoginInfoRecordComponent>
    {
        public override void Destroy(LoginInfoRecordComponent self)
        {
            self.accoutLoginInfoDict.Clear();
        }
    }
    [FriendClass(typeof(LoginInfoRecordComponent))]
    public static class LoginInfoRecordComponentSystem
    {
        public static void Add(this LoginInfoRecordComponent self,long key, int value)
        {
            if (self.accoutLoginInfoDict.ContainsKey(key))
            {
                self.accoutLoginInfoDict[key] = value;
                return;
            }
            self.accoutLoginInfoDict.Add(key,value);
        }
        
        
        public static void Remove(this LoginInfoRecordComponent self,long key)
        {
            if (self.accoutLoginInfoDict.ContainsKey(key))
            {
                self.accoutLoginInfoDict.Remove(key);
                
            }
        }
        
        public static int Get(this LoginInfoRecordComponent self,long key)
        {
            if (!self.accoutLoginInfoDict.TryGetValue(key,out int value))
            {
                return -1;
            }

            return value;
        }

        public static bool IsExits(this LoginInfoRecordComponent self, long key)
        {
            return self.accoutLoginInfoDict.ContainsKey(key);
        }
        

    }
}