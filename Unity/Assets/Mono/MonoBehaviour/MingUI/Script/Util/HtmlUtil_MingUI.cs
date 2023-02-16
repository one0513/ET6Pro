using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class HtmlUtil_MingUI
{
    public const string IMG = @"<img src=(.+?) scale=(\d*\.?\d+?) size=(\d+) click=(.+?) />";
    public const string RECT = @"<rct index=(\d+) width=(\d*\.?\d+?) height=(\d*\.?\d+?) inside=(\d+) />";
    public const string A = @"<a href=([^>]+) under=([^>]+)>(.*?)</a>";
    public const string EMO = @"<emo name=(.+?) scale=(\d*\.?\d+?) size=(\d+) />";
    public const string EMO_INSTANCE = @"(&\d+;)";
    public const string COL_TXT = @"<color[^>]*?>|</color>";
    public const string COLOR_EXTRACT = @"(?<=#)[a-fA-F0-9].*?(?=\>)";

    public const string SIZE_BRACKET = @"\[size=(\d+)\](.*?)\[size\]";
    public const string COLOR_BRACKET = @"\[color=([0-9a-fA-F]+)\](.*?)\[color\]";

    public const string BLANK = "　";
    public const string EMPTY_CLICK = " ";
    public const int DEFAULT_FONT_SIZE = 24;

    public static Regex allReg = new Regex(IMG + "|" + A + "|" + EMO + "|" + RECT, RegexOptions.Singleline); //图片正则
    public static Regex imgReg = new Regex(IMG, RegexOptions.Singleline); //图片正则
    public static Regex rectReg = new Regex(RECT, RegexOptions.Singleline); //Rect正则
    public static Regex aReg = new Regex(A, RegexOptions.Singleline); //超链接正则
    public static Regex emoReg = new Regex(EMO, RegexOptions.Singleline); //表情正则
    public static Regex sizeBracketReg = new Regex(SIZE_BRACKET, RegexOptions.Singleline);//中括号的size匹配
    public static Regex colReg = new Regex(COL_TXT, RegexOptions.Singleline);//html颜色正则

    /// <summary>
    /// 颜色
    /// </summary>
    /// <param name="str">文字</param>
    /// <param name="color">RRGGBB</param>
    /// <returns></returns>
    public static string Color(string str, string color)
    {
        return "<color=#" + color + "ff>" + str + "</color>";
    }

    /// <summary>
    /// 大小
    /// </summary>
    /// <param name="str"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string Size(string str, int size)
    {
        return "<size=" + size + ">" + str + "</size>";
    }

    public static string Size(string str, string size)
    {
        return "<size=" + size + ">" + str + "</size>";
    }
    /// <summary>
    /// 粗体
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Bold(string str)
    {
        return "<b>" + str + "</b>";
    }

    /// <summary>
    /// 斜体
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Italic(string str)
    {
        return "<i>" + str + "</i>";
    }

    /// <summary>
    /// 图像标签
    /// </summary>
    /// <param name="path">MingUI.aa</param>
    /// <param name="scale">相对于原图的缩放比率</param>
    /// <param name="size">占多大号子体的空间</param>
    /// <param name="click">点击事件字符串（默认无点击事件）</param>
    /// <returns></returns>
    public static string Img(string path, float scale , int size , string click )
    {
        return "<img src=" + path + " scale=" + scale + " size=" + size + " click=" + click + " />";
    }
    public static string Img(string path)
    {
        return Img(path, 1,DEFAULT_FONT_SIZE);
    }
    public static string Img(string path, float scale,int size)
    {
        return Img(path,scale, size,EMPTY_CLICK);
    }

    /// <summary>
    /// 超链接标签
    /// </summary>
    /// <param name="str"></param>
    /// <param name="linkStr"></param>
    /// <param name="color"></param>
    /// <param name="withBracket"></param>
    /// <returns></returns>
    public static string Link(string str, string linkStr, string color,bool withBracket)
    {
        if (withBracket) str = "[" + str + "]";
        return "<a href=" + linkStr + ">" + Color(str, color) + "</a>";
    }
    public static string Link(string str, string linkStr)
    {
        return Link(str, linkStr, "00ff00",true);
    }
    /// <summary>
    /// 表情标签
    /// </summary>
    /// <param name="name">&10</param>
    /// <param name="scale"></param>
    /// <param name="size">占多大号子体的空间</param>
    /// <returns></returns>
    public static string Emo(string name, float scale , int size)
    {
        return "<emo name=" + name + " scale=" + scale + " size=" + size + " />";
    }
    public static string Emo(string name)
    {
        return Emo(name,1, DEFAULT_FONT_SIZE);
    }
    /// <summary>
    /// 表情快速替换:
    /// </summary>
    /// <param name="str"></param>
    /// <param name="scale"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string ReplaceEmo(string str,float scale,int size)
    {
        return Regex.Replace(str, EMO_INSTANCE, Emo("$1",scale,size));
    }
    /// <summary>
    /// 中括号size替换尖括号size
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceSize(string str)
    {
        return Regex.Replace(str, SIZE_BRACKET, Size("$2", "$1"));
    }
    /// <summary>
    /// 中括号color替换尖括号color
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceColor(string str)
    {
        return Regex.Replace(str, COLOR_BRACKET, Color("$2", "$1"));
    }

    public static string ClearColor(string str) {
        return colReg.Replace(str, "");
    }

    public static string ExtractTextHtmlColor(string str) {
        if (colReg.IsMatch(str)) {
            return Regex.Match(str, COLOR_EXTRACT).Value;
        } else {
            return string.Empty;
        }
    }

    public static Color ConverHtmlClrToUnityClr(string htmlColor) {
        int r = Convert.ToInt32(htmlColor.Substring(0, 2), 16);
        int g = Convert.ToInt32(htmlColor.Substring(2, 2), 16);
        int b = Convert.ToInt32(htmlColor.Substring(4, 2), 16);
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }
}