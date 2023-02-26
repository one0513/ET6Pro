using System;

namespace ET
{
    public static class AdventureHelper
    {
        public static async ETTask<int> RequestStartGameLevel(Scene zoneScene, int levelId)
        {
            M2C_StartGameLevel m2CStartGameLvel = null;
            try
            {
                m2CStartGameLvel  =  (M2C_StartGameLevel) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_StartGameLevel() {LevelId = levelId});
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (m2CStartGameLvel.Error != ErrorCode.ERR_Success)
            {
                Log.Error(m2CStartGameLvel.Error.ToString());
                return m2CStartGameLvel.Error;
            }
            return ErrorCode.ERR_Success;
        }
        
        public static async ETTask<int> RequestEndGameLevel(Scene zoneScene,BattleRoundResult battleRoundResult,int round)
        {
            M2C_EndGameLevel m2CEndGameLevel = null;
            try
            {
                m2CEndGameLevel  =  (M2C_EndGameLevel) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_EndGameLevel()
                {
                    BattleResult = (int)battleRoundResult,Round = round
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (m2CEndGameLevel.Error != ErrorCode.ERR_Success)
            {
                Log.Error(m2CEndGameLevel.Error.ToString());
                return m2CEndGameLevel.Error;
            }
            
            return ErrorCode.ERR_Success;
        }
        

    }
}