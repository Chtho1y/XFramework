using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Native.Audio
{
    public class AudioData : IReference
    {
        public bool Loop;
        public float Volume;
        public float MinDistance;
        public float MaxDistance;
        public AudioRolloffMode RolloffMode;
        public AnimationCurve Curve;
        public bool Audio2D;

        public static AudioData Create2D(
            bool loop
            , float volume)
        {
            var data = ReferencePool.Acquire<AudioData>();
            data.Loop = loop;
            data.Volume = volume;
            data.Audio2D = true;
            return data;
        }

        public static AudioData Create3D(
            bool loop
            , float volume)
        {
            var data = ReferencePool.Acquire<AudioData>();
            data.Loop = loop;
            data.Volume = volume;
            data.Audio2D = false;
            data.RolloffMode = AudioRolloffMode.Linear;
            data.MinDistance = 0;
            data.MaxDistance = 500;
            return data;
        }

        public static AudioData Create3D(
            bool loop
            , float volume
            , AnimationCurve curve)
        {
            var data = ReferencePool.Acquire<AudioData>();
            data.Loop = loop;
            data.Volume = volume;
            data.Audio2D = false;
            data.RolloffMode = AudioRolloffMode.Custom;
            data.MinDistance = 0;
            data.MaxDistance = 500;
            data.Curve = curve;
            return data;
        }

        public void Clear()
        {
            Curve = null;
        }
    }

    public interface IAudioManager
    {
        /// <summary>
        /// ��Ч�����������
        /// </summary>
        public Transform ComponentTs { get; }
        /// <summary>
        /// ÿ���������Ч��������
        /// </summary>
        public int MaxAudioChanel { get; }
        /// <summary>
        /// ������Ч
        /// </summary>
        /// <param name="path">������ַ</param>
        /// <param name="group">��Ч������</param>
        /// <param name="data">������Ч������</param>
        /// <returns></returns>
        public int PlayAudio(string path, int group, AudioData data);

        /// <summary>
        /// ֹͣ��Ч����
        /// </summary>
        /// <param name="audioId">��ЧID</param>
        public void StopAudio(int audioId);
    }
}
