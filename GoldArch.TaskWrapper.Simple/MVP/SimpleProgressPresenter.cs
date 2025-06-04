using System;
using System.Drawing;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.TaskWrapperCore;
// For MessageBoxIcon

// For ControlInvokeHelper (though presenter won't use it directly if view handles invoke)

namespace GoldArch.TaskWrapperReport.Simple.MVP
{
    public class SimpleProgressPresenter : IDisposable
    {
        private readonly ISimpleProgressView _view;
        private readonly TaskWrapperReport.TaskWrapperCore.TaskWrapper _taskWrapper;

        // UI Color Scheme (could be moved to a config or theme class)
        private Color ProgressBarDefaultColor { get; } = Color.SkyBlue;
        private Color ProgressBarWarningColor { get; } = Color.Gold;
        private Color ProgressBarErrorColor { get; } = Color.Salmon;
        private Color ProgressBarSuccessColor { get; } = Color.MediumSeaGreen;
        private Color ProgressBarStartingColor { get; } = Color.LightSteelBlue;
        private Color ProgressBarCancellingColor { get; } = Color.OrangeRed;
        private Color ProgressBarCancelledColor { get; } = Color.Gray;

        public SimpleProgressPresenter(ISimpleProgressView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            _taskWrapper = new TaskWrapperReport.TaskWrapperCore.TaskWrapper
            {
                TextProgressBarMinimum = 0,
                TextProgressBarMaximum = 100
            };
            
            // Subscribe to view events
            _view.LoadView += OnViewLoad;
            _view.StartSimpleTaskClicked += OnStartSimpleTaskClicked;
            _view.StartErrorTaskClicked += OnStartErrorTaskClicked;
            _view.CancelTaskClicked += OnCancelTaskClicked;
            _view.ViewClosing += OnViewClosing;

            // Subscribe to TaskWrapper events
            _taskWrapper.ProgressReporter.ProgressChanged += TaskWrapper_ProgressChanged;
            _taskWrapper.StateChanged += TaskWrapper_StateChanged;
        }

        private void OnViewLoad(object sender, EventArgs e)
        {
            // Initialize view's progress bar range from TaskWrapper settings
            _view.ProgressBarMinimum = _taskWrapper.TextProgressBarMinimum;
            _view.ProgressBarMaximum = _taskWrapper.TextProgressBarMaximum;
            UpdateUIForState(_taskWrapper.CurrentState, null); // Set initial UI state
        }

        private void TaskWrapper_ProgressChanged(object sender, TaskProgressInfo e)
        {
            if (e.ProgressValue.HasValue)
            {
                _view.ProgressBarValue = Math.Max(_view.ProgressBarMinimum, Math.Min(e.ProgressValue.Value, _view.ProgressBarMaximum));
            }
            if (e.ProgressText != null)
            {
                _view.ProgressBarCustomText = e.ProgressText;
            }

            if (_taskWrapper.CurrentState == TaskExecutionState.Running)
            {
                switch (e.Level)
                {
                    case ReportLevel.Warning: _view.ProgressBarProgressColor = ProgressBarWarningColor; break;
                    case ReportLevel.Error: _view.ProgressBarProgressColor = ProgressBarErrorColor; break;
                    case ReportLevel.Success: _view.ProgressBarProgressColor = ProgressBarSuccessColor; break;
                    default:
                        if (_view.ProgressBarProgressColor != ProgressBarWarningColor && _view.ProgressBarProgressColor != ProgressBarErrorColor)
                        {
                            _view.ProgressBarProgressColor = ProgressBarDefaultColor;
                        }
                        break;
                }
            }
        }

        private void TaskWrapper_StateChanged(object sender, TaskStateChangedEventArgs e)
        {
            UpdateUIForState(e.NewState, e.Exception);
        }

        private void UpdateUIForState(TaskExecutionState state, Exception ex)
        {
            _view.StatusText = $"状态: {state}";
            _view.CancelTaskEnabled = (state == TaskExecutionState.Running || state == TaskExecutionState.Starting || state == TaskExecutionState.Cancelling);
            _view.StartSimpleTaskEnabled = (state == TaskExecutionState.Idle || state == TaskExecutionState.Completed || state == TaskExecutionState.Faulted || state == TaskExecutionState.Cancelled);
            _view.StartErrorTaskEnabled = _view.StartSimpleTaskEnabled;

            switch (state)
            {
                case TaskExecutionState.Idle:
                    _view.ProgressBarProgressColor = ProgressBarDefaultColor;
                    _view.ProgressBarCustomText = "准备就绪";
                    _view.ProgressBarValue = _taskWrapper.TextProgressBarMinimum;
                    break;
                case TaskExecutionState.Starting:
                    _view.ProgressBarProgressColor = ProgressBarStartingColor;
                    _view.ProgressBarCustomText = "正在启动...";
                    break;
                case TaskExecutionState.Running:
                    _view.ProgressBarProgressColor = ProgressBarDefaultColor;
                    // CustomText typically updated by ProgressChanged
                    break;
                case TaskExecutionState.Cancelling:
                    _view.ProgressBarProgressColor = ProgressBarCancellingColor;
                    _view.ProgressBarCustomText = "正在取消...";
                    break;
                case TaskExecutionState.Completed:
                    _view.ProgressBarProgressColor = ProgressBarSuccessColor;
                    _view.ProgressBarCustomText = "任务完成!";
                    _view.ProgressBarValue = _taskWrapper.TextProgressBarMaximum;
                    break;
                case TaskExecutionState.Faulted:
                    _view.ProgressBarProgressColor = ProgressBarErrorColor;
                    _view.ProgressBarCustomText = $"任务失败: {ex?.Message.Split('\n')[0]}";
                    if (ex != null) System.Diagnostics.Debug.WriteLine($"Task Faulted in Presenter: {ex}");
                    break;
                case TaskExecutionState.Cancelled:
                    _view.ProgressBarProgressColor = ProgressBarCancelledColor;
                    _view.ProgressBarCustomText = "任务已取消";
                    break;
                default:
                    _view.ProgressBarProgressColor = ProgressBarDefaultColor;
                    break;
            }
        }

        private bool IsTaskRunnable()
        {
            if (_taskWrapper.CurrentState == TaskExecutionState.Idle ||
                _taskWrapper.CurrentState == TaskExecutionState.Completed ||
                _taskWrapper.CurrentState == TaskExecutionState.Faulted ||
                _taskWrapper.CurrentState == TaskExecutionState.Cancelled)
            {
                return true;
            }
            _view.ShowMessage("提示", "任务正在运行或处理中。", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void OnStartSimpleTaskClicked(object sender, EventArgs e)
        {
            if (!IsTaskRunnable()) return;

            _taskWrapper.DoWorkFuncAsync = async (token, progress) =>
            {
                for (int i = 0; i <= 100; i += 10)
                {
                    token.ThrowIfCancellationRequested();
                    await System.Threading.Tasks.Task.Delay(300, token).ConfigureAwait(false);
                    ReportLevel level = (i > 70) ? ReportLevel.Warning : ReportLevel.Information;
                    progress.Report(new TaskProgressInfo(null, $"正常任务处理中... {i}%", i, level));
                }
                progress.Report(new TaskProgressInfo(null, "正常任务完成!", 100, ReportLevel.Success));
                return null;
            };
            _taskWrapper.StartTaskAsync();
        }

        private void OnStartErrorTaskClicked(object sender, EventArgs e)
        {
            if (!IsTaskRunnable()) return;

            _taskWrapper.DoWorkFuncAsync = async (token, progress) =>
            {
                progress.Report(new TaskProgressInfo(null, "错误任务启动...", 0, ReportLevel.Information));
                await System.Threading.Tasks.Task.Delay(500, token).ConfigureAwait(false);
                for (int i = 0; i <= 50; i += 10)
                {
                    token.ThrowIfCancellationRequested();
                    await System.Threading.Tasks.Task.Delay(200, token).ConfigureAwait(false);
                    progress.Report(new TaskProgressInfo(null, $"错误任务进展... {i}%", i, ReportLevel.Information));
                }
                progress.Report(new TaskProgressInfo(null, "准备抛出异常...", 50, ReportLevel.Warning));
                await System.Threading.Tasks.Task.Delay(500, token).ConfigureAwait(false);
                throw new InvalidOperationException("这是一个来自 Presenter 的故意测试异常!");
            };
            _taskWrapper.StartTaskAsync();
        }

        private void OnCancelTaskClicked(object sender, EventArgs e)
        {
            _taskWrapper.RequestCancel();
        }

        private void OnViewClosing(object sender, FormClosingEventArgs e)
        {
            // Request cancel if task is running
            if (_taskWrapper.CurrentState == TaskExecutionState.Running ||
                _taskWrapper.CurrentState == TaskExecutionState.Starting ||
                _taskWrapper.CurrentState == TaskExecutionState.Cancelling)
            {
                _taskWrapper.RequestCancel();
                // Potentially wait briefly or show message if critical, but usually RequestCancel is enough for graceful shutdown
            }
        }

        public void Dispose()
        {
            // Unsubscribe from TaskWrapper events
            if (_taskWrapper != null)
            {
                _taskWrapper.ProgressReporter.ProgressChanged -= TaskWrapper_ProgressChanged;
                _taskWrapper.StateChanged -= TaskWrapper_StateChanged;
                _taskWrapper.Dispose();
            }
            // Unsubscribe from view events (important if presenter can outlive view, less so if view owns presenter)
             if (_view != null)
             {
                _view.LoadView -= OnViewLoad;
                _view.StartSimpleTaskClicked -= OnStartSimpleTaskClicked;
                _view.StartErrorTaskClicked -= OnStartErrorTaskClicked;
                _view.CancelTaskClicked -= OnCancelTaskClicked;
                _view.ViewClosing -= OnViewClosing;
             }
        }
    }
}