using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace XEngine.Engine
{
	public enum UpdateStatus
	{
		NoNeedUpdate,
		FirstTime, // 没有旧版本号, 第一次下载
		NeedDownloadNewClient, // 第一位大版本不一样，需要下载新的客户端
		Large, // 第二位不一样
		Medium, // 第三位不一样
		Small // 第四位不一样
	}

	public class VersionInfo
	{
		public string Version;

		public string BundleInfos;

		public Dictionary<string, BundleInfo> DecodeBundleInfo()
		{
			Dictionary<string, BundleInfo> bundlename2BundleInfo = new();
			string[] md5s = BundleInfos.Split(';');
			foreach (var md5 in md5s)
			{
				var bundleInfo = BundleInfo.Decode(md5);
				bundlename2BundleInfo.Add(bundleInfo.name, bundleInfo);
			}
			return bundlename2BundleInfo;
		}

		public static VersionInfo BuildVersionInfo(string version, Dictionary<string, BundleInfo> bundlename2BundleInfo)
		{
			var infos = new List<string>();
			foreach (var bundleInfo in bundlename2BundleInfo.Values)
			{
				infos.Add(BundleInfo.Encode(bundleInfo));
			}
			var versionInfo = new VersionInfo()
			{
				Version = version,
				BundleInfos = string.Join(";", infos)
			};
			return versionInfo;
		}
	}

	public static class VersionUtil
	{
		public const int VersionSplitNumber = 4;
		private static readonly UpdateStatus[] versionCompareMeaning = new UpdateStatus[4]
		{
			UpdateStatus.NeedDownloadNewClient,
			UpdateStatus.Large,
			UpdateStatus.Medium,
			UpdateStatus.Small
		};
		public static UpdateStatus CompareVersionForUpdate(VersionInfo oldVersion, VersionInfo newVersion)
		{
			if (oldVersion == null)
			{
				return UpdateStatus.FirstTime;
			}
			string[] oldArr = oldVersion.Version.Split('.');
			string[] newArr = newVersion.Version.Split('.');

			for (var i = 0; i < VersionSplitNumber; i++)
			{
				if (Convert.ToInt32(oldArr[i]) < Convert.ToInt32(newArr[i]))
				{
					return versionCompareMeaning[i];
				}
			}
			return UpdateStatus.NoNeedUpdate;
		}

		public static VersionInfo LoadVersionInfoFromFile(string versionFilePath)
		{
			if (!File.Exists(versionFilePath))
			{
				return null;
			}
			return JsonUtility.FromJson<VersionInfo>(File.ReadAllText(versionFilePath));
		}

		public static string NextVersion(string old)
		{
			string[] verArray = old.Split('.');
			if (verArray.Length != VersionSplitNumber)
			{
				throw new Exception("Version format error:" + old);
			}
			// version的最后一个数字+1
			verArray[^1] = (Convert.ToInt32(verArray[^1]) + 1).ToString();
			return string.Join(".", verArray);
		}

	}
}
