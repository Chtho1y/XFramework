using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace XEngine.Engine
{
    public class UniTaskUtil : Singleton<UniTaskUtil>
    {
        private ConcurrentDictionary<string, CancellationTokenSource> cancellationTokenSources = new();

        /// <summary>
        /// 启动具有唯一标识符的 UniTask。
        /// </summary>
        /// <param name="taskId">唯一任务标识符。</param>
        /// <param name="taskFunction">任务函数。</param>
        public UniTask StartUniTask(string taskId, Func<CancellationToken, UniTask> taskFunction)
        {
            if (cancellationTokenSources.TryRemove(taskId, out var existingCts))
            {
                existingCts.Cancel();
            }

            var cts = new CancellationTokenSource();
            if (cancellationTokenSources.TryAdd(taskId, cts))
            {
                return RunTask(taskId, cts, taskFunction);
            }
            return UniTask.CompletedTask;
        }

        private async UniTask RunTask(string taskId, CancellationTokenSource cts, Func<CancellationToken, UniTask> taskFunction)
        {
            try
            {
                await taskFunction(cts.Token);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Task {taskId} failed: {ex}");
            }
            finally
            {
                CleanUp(taskId);
            }
        }

        /// <summary>
        /// 停止具有唯一标识符的 UniTask。
        /// </summary>
        /// <param name="taskId">唯一任务标识符。</param>
        public void StopUniTask(string taskId)
        {
            if (cancellationTokenSources.TryRemove(taskId, out var cts))
            {
                cts.Cancel();
            }
        }

        /// <summary>
        /// 停止所有正在运行的 UniTasks。
        /// </summary>
        public void StopAllUniTasks()
        {
            foreach (var kvp in cancellationTokenSources)
            {
                kvp.Value.Cancel();
            }
            cancellationTokenSources.Clear();
        }

        /// <summary>
        /// 检查任务是否正在运行。
        /// </summary>
        /// <param name="taskId">唯一任务标识符。</param>
        /// <returns>如果任务正在运行则返回 true，否则返回 false。</returns>
        public bool IsTaskRunning(string taskId)
        {
            return cancellationTokenSources.ContainsKey(taskId);
        }

        /// <summary>
        /// 清理任务。
        /// </summary>
        /// <param name="taskId">唯一任务标识符。</param>
        private void CleanUp(string taskId)
        {
            if (cancellationTokenSources.TryRemove(taskId, out var cts))
            {
                cts.Dispose();
            }
        }
    }
}