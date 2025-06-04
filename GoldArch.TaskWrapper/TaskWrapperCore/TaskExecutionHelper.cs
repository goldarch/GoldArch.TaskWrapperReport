// File: TaskExecutionHelper.cs
// Namespace: GoldArch.ControlBase.BackgroundWorkerUI (or a new Utilities namespace)

using System;
using System.Threading;
using System.Threading.Tasks;

// For TaskProgressInfo, ReportLevel

namespace GoldArch.TaskWrapperReport.TaskWrapperCore
{
    /*
在您的窗体代码中，使用这个辅助类可以简化任务的启动：
```csharp
// 在您的 Form 类中:
// private TaskWrapper _myTaskWrapper = new TaskWrapper(); // 假设已初始化

// private void BtnRunSimpleAction_Click(object sender, EventArgs e)
// {
//     if (_myTaskWrapper.CurrentState != TaskExecutionState.Idle && 
//         _myTaskWrapper.CurrentState != TaskExecutionState.Completed &&
//         _myTaskWrapper.CurrentState != TaskExecutionState.Faulted &&
//         _myTaskWrapper.CurrentState != TaskExecutionState.Cancelled)
//     {
//         MessageBox.Show("Task is already running.");
//         return;
//     }
//     // 清理UI控件，例如：
//     // statusProgressBarControl.ResetDisplay(); 

//     TaskExecutionHelper.ExecuteAction(
//         _myTaskWrapper,
//         (token, progress) => {
//             // 这是您的具体工作逻辑
//             for (int i = 0; i < 5; i++)
//             {
//                 token.ThrowIfCancellationRequested();
//                 Thread.Sleep(500); // 模拟工作
//                 progress.Report(new TaskProgressInfo(null, $"Step {i+1}", (i+1)*20, ReportLevel.Information));
//             }
//         },
//         "My Simple Action Task"
//     );
// }
```
这个 `TaskExecutionHelper` 进一步封装了 `TaskWrapper` 的使用，针对特定模式（如执行一个简单的同步或异步动作）提供了更简洁的调用方式，从而更能体现 `TaskWrapper` 作为通用任务执行和监控框架的作用。
     */


    /// <summary>
    /// Provides helper methods to simplify the execution of common task patterns using TaskWrapper.
    /// </summary>
    public static class TaskExecutionHelper
    {
        /// <summary>
        /// Executes a simple fire-and-forget action asynchronously using a TaskWrapper,
        /// automatically reporting start, completion, and errors.
        /// </summary>
        /// <param name="taskWrapper">The TaskWrapper instance to use.</param>
        /// <param name="actionToExecute">The synchronous action to execute in the background.</param>
        /// <param name="taskName">A descriptive name for the task, used in progress reports.</param>
        /// <param name="reportProgressTo">Optional IProgress instance for direct reporting from the action (if needed, though typically TaskWrapper handles it).</param>
        public static void ExecuteAction(
            TaskWrapper taskWrapper,
            Action<CancellationToken, IProgress<TaskProgressInfo>> actionToExecute,
            string taskName = "Unnamed Task")
        {
            if (taskWrapper == null) throw new ArgumentNullException(nameof(taskWrapper));
            if (actionToExecute == null) throw new ArgumentNullException(nameof(actionToExecute));

            taskWrapper.DoWorkFuncAsync = (token, progress) =>
            {
                return Task.Run(() => // Ensure actionToExecute runs on a background thread
                {
                    try
                    {
                        progress.Report(new TaskProgressInfo(null, $"{taskName}: Processing...", 0, ReportLevel.Information));
                        actionToExecute(token, progress); // Pass token and progress
                        token.ThrowIfCancellationRequested();
                        progress.Report(new TaskProgressInfo(null, $"{taskName}: Successfully completed.", 100, ReportLevel.Success));
                        return null; // Success
                    }
                    catch (OperationCanceledException)
                    {
                        // TaskWrapper will handle the OperationCanceledException and set state to Cancelled.
                        // We can rethrow or return a specific message if needed, but TaskWrapper's default handling is usually sufficient.
                        throw;
                    }
                    catch (Exception ex)
                    {
                        // Return the error message to be handled by TaskWrapper's Faulted state.
                        return $"{taskName} failed: {ex.Message}";
                    }
                }, token);
            };

            // Optional: Define generic BeforeDoWorkAction or AfterCompletionAction if common for these types of tasks
            // taskWrapper.BeforeDoWorkAction = () => { /* Generic setup */ };
            // taskWrapper.AfterCompletionAction = () => { /* Generic cleanup */ };

            taskWrapper.StartTaskAsync(reportWrapperStatusMessages: true);
        }

        /// <summary>
        /// Executes a simple asynchronous function using a TaskWrapper,
        /// automatically reporting start, completion, and errors.
        /// </summary>
        /// <param name="taskWrapper">The TaskWrapper instance to use.</param>
        /// <param name="asyncFuncToExecute">The asynchronous function to execute. It should handle its own exceptions or let them propagate.</param>
        /// <param name="taskName">A descriptive name for the task.</param>
        public static void ExecuteAsyncFunc(
            TaskWrapper taskWrapper,
            Func<CancellationToken, IProgress<TaskProgressInfo>, Task> asyncFuncToExecute,
            string taskName = "Unnamed Async Task ")
        {
            if (taskWrapper == null) throw new ArgumentNullException(nameof(taskWrapper));
            if (asyncFuncToExecute == null) throw new ArgumentNullException(nameof(asyncFuncToExecute));

            taskWrapper.DoWorkFuncAsync = async (token, progress) =>
            {
                try
                {
                    progress.Report(new TaskProgressInfo(null, $"{taskName}: Processing...", 0, ReportLevel.Information));
                    await asyncFuncToExecute(token, progress).ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                    progress.Report(new TaskProgressInfo(null, $"{taskName}: Successfully completed.", 100, ReportLevel.Success));
                    return null; // Success
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    return $"{taskName} failed: {ex.Message}";
                }
            };
            taskWrapper.StartTaskAsync(reportWrapperStatusMessages: true);
        }
    }
}
