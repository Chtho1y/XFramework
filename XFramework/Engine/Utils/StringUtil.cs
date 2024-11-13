using System;
using System.Text.RegularExpressions;


namespace XEngine.Engine
{
    public static class StringUtil
    {
        /// <summary>
        /// 判断一个字符是否为中文
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsChinese(char c)
        {
            return c >= 0x4E00 && c <= 0x9FA5;
        }

        /// <summary>
        /// 判断一个字符串是否包含中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChineseString(string str)
        {
            char[] ch = str.ToCharArray();
            if (str != null)
            {
                for (int i = 0; i < ch.Length; i++)
                {
                    if (IsChinese(ch[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 是否匹配正则表达式
        /// </summary>
        /// <param name="rule">正则表达式规则</param>
        /// <param name="str">要匹配的字符串</param>
        /// <returns></returns>
        public static bool IsMatch(string rule, string str)
        {
            return Regex.IsMatch(string.Format(@"{0}", rule), str);
        }

        /// <summary>
        /// 获取字符串长度，可选项：是否忽略空格、换行符
        /// </summary>
        /// <param name="str">要获取长度的字符串</param>
        /// <param name="removeSpace">是否忽略空格、换行符</param>
        /// <returns></returns>
        public static int GetStringLength(string str, bool removeSpace = true)
        {
            if (string.IsNullOrEmpty(str)) return 0;

            // If removeSpace is true, remove all whitespace characters, else return the original string's length.
            return removeSpace ? Regex.Replace(str, @"[\r\n\s]", "").Length : str.Length;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="startIndex">开始截取的位置索引</param>
        /// <param name="length">要截取的长度，默认到字符串末尾</param>
        /// <param name="removeSpace">是否忽略空格、换行符</param>
        /// <returns></returns>
        public static string DoSubString(string str, int startIndex, int length = -1, bool removeSpace = true)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;

            string tempStr = removeSpace ? Regex.Replace(str, @"[\r\n\s]", "") : str;

            startIndex = Math.Min(startIndex, tempStr.Length);
            startIndex = Math.Max(startIndex, 0);

            if (length < 0 || length > tempStr.Length - startIndex)
            {
                length = tempStr.Length - startIndex;
            }

            return tempStr.Substring(startIndex, length);
        }

    }
}
