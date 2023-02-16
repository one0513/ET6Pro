public class StringUtil
{
    public static int GetStrLength(string str)
    {
        return str.Length;
    }

    public static string ChangeToLimitStr(string str, int limitLength, string replaceStr)
    {
        if (str.Length > limitLength)
        {
            return str.Substring(0, limitLength) + replaceStr;
        }
        return str;
    }

    public static string BR(string str)
    {
        return str.Replace("\\n", "\n");
    }
}