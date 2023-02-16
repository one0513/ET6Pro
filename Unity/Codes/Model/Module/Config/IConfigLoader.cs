using System.Collections.Generic;

namespace ET
{
    public interface IConfigLoader
    {
        Dictionary<string, byte[]> GetAllConfigBytes();
        byte[] GetOneConfigBytes(string configName);
    }
}