using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Native.Scheduler.Scheduler;

namespace Native.Scheduler
{
    public interface IScheduler : IReference
    {
        /// <summary>
        /// ��һ��
        /// </summary>
        public IScheduler Parent { get; }
        
        /// <summary>
        /// ��һ��
        /// </summary>
        public IScheduler Child { get; }

        /// <summary>
        /// ��ͼ�
        /// </summary>
        public IScheduler SchedulerLoweast { get; }

        /// <summary>
        /// Tick����
        /// </summary>
        public ITicker Ticker { get; }
        
        /// <summary>
        /// �̶ܿ�
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// ÿ���̶ȵĵ�λֵ
        /// </summary>
        public int SingleUnit { get; }

        /// <summary>
        /// ��������
        /// </summary>
        public IScheduler CreateParent(int parentMaxSize);

        /// <summary>
        /// ע�ᶨʱ��
        /// </summary>
        /// <param name="pack"></param>
        public void RegisterScheduler(FuncPack pack);

        /// <summary>
        /// �ƶ�����һ���̶�
        /// </summary>
        public void MoveOne();

        /// <summary>
        /// ����
        /// </summary>
        public void Invoke();

        public void GetAllFuncPackAndClear(List<FuncPack> tempList);

        public int GetOffset(int Interval);
    }

    public interface ITicker
    {
        /// <summary>
        /// ����Index����
        /// </summary>
        public int Tick(float elapseSeconds);
        public ITicker Clone();
    }
}
