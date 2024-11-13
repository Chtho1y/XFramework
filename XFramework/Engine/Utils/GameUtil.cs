using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace XEngine.Engine
{
	public static class GameUtil
	{
		public static byte[] String2Bytes(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		public static string Bytes2String(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		public static byte[] File2UTF8(string fileName)
		{
			Encoding encoding = Encoding.Default;
			string s = string.Empty;
			using (StreamReader streamReader = new(fileName, Encoding.Default))
			{
				s = streamReader.ReadToEnd();
				encoding = streamReader.CurrentEncoding;
				streamReader.Close();
			}
			byte[] bytes = encoding.GetBytes(s);
			if (encoding == Encoding.UTF8)
			{
				return bytes;
			}
			return Encoding.Convert(encoding, Encoding.UTF8, bytes);
		}

		public static void Write2Disk(string path, byte[] data)
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			fileStream.Write(data, 0, data.Length);
			fileStream.Flush();
			fileStream.Close();
			fileStream.Dispose();
		}

		public static T CreateInstance<T>(string typeName, string assembly = "Assembly-CSharp") where T : class
		{
			T result = null;
			Assembly assembly2 = Assembly.Load(assembly);
			if (assembly2 != null)
			{
				return assembly2.CreateInstance(typeName) as T;
			}
			return result;
		}

		public static string GetFileMD5(string fileName)
		{
			try
			{
				FileStream fileStream = new(fileName, FileMode.Open);
				MD5 mD = new MD5CryptoServiceProvider();
				byte[] array = mD.ComputeHash(fileStream);
				fileStream.Close();
				StringBuilder stringBuilder = new();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				return stringBuilder.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("GetMD5 Error: " + ex.Message);
			}
		}

		public static long GetFileSize(string path)
		{
			if (!File.Exists(path))
			{
				return 0L;
			}
			FileInfo fileInfo = new(path);
			return fileInfo.Length;
		}

		public static bool IsNull(object o)
		{
			return o == null;
		}

		public static void RegisterAnimationEvent(Animator ani, string name, float time, string data)
		{
			if (ani.GetComponent<AnimationEventHandler>() != null)
			{
				ani.gameObject.AddComponent<AnimationEventHandler>();
			}
			AnimationClip[] animationClips = ani.runtimeAnimatorController.animationClips;
			for (int i = 0; i < animationClips.Length; i++)
			{
				if (animationClips[i].name == name)
				{
					AnimationEvent val = new()
					{
						functionName = "OnAnimationEvent",
						time = time * animationClips[i].length,
						stringParameter = data
					};
					animationClips[i].AddEvent(val);
					break;
				}
			}
		}

		public static void ClearAnimationEvent(Animator ani)
		{
			AnimationClip[] animationClips = ani.runtimeAnimatorController.animationClips;
			for (int i = 0; i < animationClips.Length; i++)
			{
				animationClips[i].events = null;
			}
		}

		public static Component GetOrAddComponent(GameObject node, Type type)
		{
			Component val = node.GetComponent(type);
			if (val == null)
			{
				val = node.AddComponent(type);
			}
			return val;
		}

		public static T GetOrAddComponent<T>(GameObject node) where T : Component
		{
			Component orAddComponent = GetOrAddComponent(node, typeof(T));
			return (T)(object)((orAddComponent is T) ? orAddComponent : null);
		}

		public static bool InRange(GameObject self, GameObject other, float range)
		{
			if (range <= 0f)
			{
				return false;
			}
			float num = Vector3.Distance(self.transform.position, other.transform.position);
			return num <= range;
		}

		public static bool InAngle(GameObject self, GameObject other, float angle)
		{
			if (angle >= 360f)
			{
				return true;
			}
			Vector3 val = other.transform.position - self.transform.position;
			return Vector3.Angle(self.transform.forward, val) <= angle / 2f;
		}

		public static bool InRight(GameObject self, GameObject other)
		{
			Vector3 val = Vector3.Cross(self.transform.forward, other.transform.position);
			return val.y > 0f;
		}

		public static bool InFront(GameObject self, GameObject other)
		{
			Vector3 val = other.transform.position - self.transform.position;
			float num = Vector3.Dot(self.transform.forward, val);
			return num > 0f;
		}

		public static void SetLayer(GameObject node, string layer, bool child = true)
		{
			int layer2 = LayerMask.NameToLayer(layer);
			if (child)
			{
				Transform[] componentsInChildren = node.transform.GetComponentsInChildren<Transform>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.layer = layer2;
				}
			}
			else
			{
				node.layer = layer2;
			}
		}

		public static void ResetTransform(GameObject node)
		{
			node.transform.localPosition = Vector3.zero;
			node.transform.localEulerAngles = Vector3.zero;
			node.transform.localScale = Vector3.one;
		}

		public static GameObject FindChild(GameObject parent, string name)
		{
			if (parent == null)
			{
				return null;
			}
			Transform val = parent.transform.Find(name);
			if (val != null)
			{
				return val.gameObject;
			}
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				val = parent.transform.GetChild(i);
				GameObject val2 = FindChild(val.gameObject, name);
				if (val2 != null)
				{
					return val2;
				}
			}
			return null;
		}

		public static Component FindChild(GameObject parent, string name, Type type)
		{
			Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!(componentsInChildren[i].name != name))
				{
					Component component = componentsInChildren[i].GetComponent(type);
					if (component != null)
					{
						return component;
					}
				}
			}
			return null;
		}

		public static T FindChild<T>(GameObject parent, string name) where T : Component
		{
			Component obj = FindChild(parent, name, typeof(T));
			return (T)(object)((obj is T) ? obj : null);
		}

		public static GameObject Find(string path)
		{
			string[] array = path.Split('/');
			string text = array[0];
			GameObject val = GameObject.Find(text);
			if (val == null)
			{
				return null;
			}
			if (array.Length == 1)
			{
				return val;
			}
			string text2 = "";
			for (int i = 1; i < array.Length; i++)
			{
				text2 += array[i];
				if (i < array.Length - 1)
				{
					text2 += "/";
				}
			}
			Transform val2 = val.transform.Find(text2);
			if (val2 == null)
			{
				return null;
			}
			return val2.gameObject;
		}

		public static Component Find(string path, Type type)
		{
			GameObject val = Find(path);
			if (val != null)
			{
				return val.GetComponent(type);
			}
			return null;
		}

		public static T Find<T>(string path) where T : Component
		{
			GameObject val = Find(path);
			if (val != null)
			{
				return val.GetComponent<T>();
			}
			return default;
		}

		public static string[] GetAllDirectoriesFromDirectory(string dir)
		{
			return Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
		}

		public static string[] GetAllFilesFromDirectory(string dir)
		{
			return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
		}

		public static int QuickSearch(int[] arr, int target)
		{
			int num = 0;
			int num2 = arr.Length - 1;
			while (num <= num2)
			{
				int num3 = num + num2 >> 1;
				if (arr[num3] == target)
				{
					return num3;
				}
				if (arr[num3] < target)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			return -1;
		}

		public static void QuickSort(int[] arr, int left, int right)
		{
			if (left < right)
			{
				int num = Sort(arr, left, right);
				QuickSort(arr, left, num - 1);
				QuickSort(arr, num + 1, right);
			}
		}

		private static int Sort(int[] arr, int left, int right)
		{
			while (left < right)
			{
				int num = arr[left];
				if (num > arr[left + 1])
				{
					arr[left] = arr[left + 1];
					arr[left + 1] = num;
					left++;
				}
				else
				{
					(arr[right], arr[left + 1]) = (arr[left + 1], arr[right]);
					right--;
				}
			}
			return left;
		}
	}
}