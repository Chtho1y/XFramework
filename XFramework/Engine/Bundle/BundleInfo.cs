using System;


namespace XEngine.Engine
{
    public struct BundleInfo
    {
        public string name;

        public string md5;

        public long size;

        public BundleInfo(string name, string md5, long size)
        {
            this.name = name;
            this.md5 = md5;
            this.size = size;
        }

        public static string Encode(BundleInfo info)
        {
            return string.Join(',', info.name, info.md5, info.size);
        }

        public static BundleInfo Decode(string str)
        {
            string[] array = str.Split(',');
            return new BundleInfo(array[0], array[1], Convert.ToInt64(array[2]));
        }
    }
}