using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Unity.Collections;

namespace Assets.Scripts.Editor
{
    public class Leak
    {
        [MenuItem("Custom/Show Leak Detection Mode")]
        static void ShowLeakDetection()
        {
            EditorUtility.DisplayDialog("�ڴ�й©�������", string.Format("NativeLeakDetection.Mode��{0}", NativeLeakDetection.Mode.ToString()), "OK");
        }

        [MenuItem("Custom/Leak Detection Enabled")]
        static void LeakDetectionEnabled()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Enabled;
        }

        [MenuItem("Custom/Leak Detection Enabled", true)]  //�ڶ���������ʾ�������ǲ˵��Ƿ���õ���֤����
        static bool ValidateLeakDetectionEnabled()
        {
            return NativeLeakDetection.Mode != NativeLeakDetectionMode.Enabled;
        }

        [MenuItem("Custom/Leak Detection Enabled With StackTrace")]
        static void LeakDetectionEnabledWithStackTrace()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        }

        [MenuItem("Custom/Leak Detection Enabled With StackTrace", true)]  //�ڶ���������ʾ�������ǲ˵��Ƿ���õ���֤����
        static bool ValidateLeakDetectionEnabledWithStackTrace()
        {
            return NativeLeakDetection.Mode != NativeLeakDetectionMode.EnabledWithStackTrace;
        }

        [MenuItem("Custom/Leak Detection Disable")]
        static void LeakDetectionDisable()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
        }

        [MenuItem("Custom/Leak Detection Disable", true)]  //�ڶ���������ʾ�������ǲ˵��Ƿ���õ���֤����
        static bool ValidateLeakDetectionDisable()
        {
            return NativeLeakDetection.Mode != NativeLeakDetectionMode.Disabled;
        }
    }
}
