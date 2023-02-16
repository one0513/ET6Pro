using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ComTool {
    class LookAB
    {


        private static byte[] load(String fileName)
        {
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                // 首先判断，文件是否已经存在
                if (!File.Exists(fileName))
                {
                    // 如果文件不存在，那么提示无法读取！
                    Debug.Log("二进制文件{0}不存在！" + fileName);
                    return null;
                }


                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                return br.ReadBytes((int)br.BaseStream.Length);
            }
            catch (Exception ex)
            {
                Debug.Log("在读取文件的过程中，发生了异常！");
                Debug.Log(ex.Message);
                Debug.Log(ex.StackTrace);
            }
            finally
            {
                if (br != null)
                {
                    try
                    {
                        br.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }

                if (fs != null)
                {
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
            }

            return null;
        }
    }


    
}
