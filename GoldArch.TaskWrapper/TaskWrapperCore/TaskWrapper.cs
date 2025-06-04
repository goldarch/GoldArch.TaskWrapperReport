// File: TaskWrapper.cs
// Namespace: GoldArch.ControlBase.BackgroundWorkerUI

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// For TaskProgressInfo, ReportLevel

namespace GoldArch.TaskWrapperReport.TaskWrapperCore
{
    /// <summary>
    /// Defines the various execution states of a task managed by <see cref="TaskWrapper"/>.
    /// </summary>
    public enum TaskExecutionState
    {
        /// <summary>The task is idle and not running.</summary>
        Idle,
        /// <summary>The task is in the process of starting.</summary>
        Starting,
        /// <summary>The task is currently running.</summary>
        Running,
        /// <summary>A cancellation request has been made, and the task is in the process of cancelling.</summary>
        Cancelling,
        /// <summary>The task completed successfully.</summary>
        Completed,
        /// <summary>The task terminated due to an unhandled exception.</summary>
        Faulted,
        /// <summary>The task was cancelled by request.</summary>
        Cancelled
    }

    /// <summary>
    /// Provides data for the <see cref="TaskWrapper.StateChanged"/> event.
    /// </summary>
    public class TaskStateChangedEventArgs : EventArgs
    {
        /// <summary>Gets the previous state of the task.</summary>
        public TaskExecutionState PreviousState { get; }
        /// <summary>Gets the new (current) state of the task.</summary>
        public TaskExecutionState NewState { get; }
        /// <summary>Gets the exception that caused the task to fault, if applicable.</summary>
        public Exception Exception { get; }
        /// <summary>Gets an optional message associated with the state change.</summary>
        public string Message { get; }

        /// <summary>Initializes a new instance of the <see cref="TaskStateChangedEventArgs"/> class.</summary>
        public TaskStateChangedEventArgs(TaskExecutionState previousState, TaskExecutionState newState, Exception exception = null, string message = null)
        {
            PreviousState = previousState;
            NewState = newState;
            Exception = exception;
            Message = message;
        }
    }

    /// <summary>
    /// Wraps a long-running task, providing mechanisms for progress reporting, cancellation, and state management.
    /// </summary>
    public class TaskWrapper : IDisposable
    {
        private readonly Stopwatch _stopwatchTotal = new Stopwatch();
        private TaskExecutionState _currentState = TaskExecutionState.Idle;
        private readonly object _stateLock = new object();
        private Exception _lastNotifiedException; // To help manage re-notification for Faulted state
        private bool _disposedValue;

        /// <summary>Gets or sets the asynchronous function to be executed.</summary>
        public Func<CancellationToken, IProgress<TaskProgressInfo>, Task<string>> DoWorkFuncAsync { get; set; }
        /// <summary>Gets or sets an action to execute before the task starts.</summary>
        public Action BeforeDoWorkAction { get; set; }
        /// <summary>Gets or sets an action to execute after the task completes (regardless of outcome).</summary>
        public Action AfterCompletionAction { get; set; }

        /// <summary>Gets or sets a value indicating whether to show stack traces in error reports.</summary>
        public bool ShowErrorStackTrace { get; set; } = true;
        /// <summary>Gets the progress reporter for this task.</summary>
        public Progress<TaskProgressInfo> ProgressReporter { get; } = new Progress<TaskProgressInfo>();
        /// <summary>Gets or sets the minimum value for the progress bar.</summary>
        public int TextProgressBarMinimum { get; set; } = 0;
        /// <summary>Gets or sets the maximum value for the progress bar.</summary>
        public int TextProgressBarMaximum { get; set; } = 100;
        /// <summary>Gets the CancellationTokenSource for the current task.</summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>Gets the current execution state of the task.</summary>
        public TaskExecutionState CurrentState
        {
            get { lock (_stateLock) return _currentState; }
            // Note: There is no public setter. State is managed internally via SetStateAndNotify.
        }

        /// <summary>Occurs when the task's execution state changes.</summary>
        public event EventHandler<TaskStateChangedEventArgs> StateChanged;

        /// <summary>Initializes a new instance of the <see cref="TaskWrapper"/> class.</summary>
        public TaskWrapper()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>Raises the <see cref="StateChanged"/> event.</summary>
        protected virtual void OnStateChanged(TaskStateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Sets the internal state and notifies subscribers. This is the sole method for changing _currentState.
        /// </summary>
        private void SetStateAndNotify(TaskExecutionState newState, Exception ex = null)
        {
            TaskExecutionState previousState;
            bool shouldNotify = false;

            lock (_stateLock)
            {
                if (_currentState != newState)
                {
                    shouldNotify = true;
                }
                else if (newState == TaskExecutionState.Faulted && ex != _lastNotifiedException)
                {
                    // If state is still Faulted, but the exception instance (or its absence) has changed, notify.
                    shouldNotify = true;
                }
                // Add other conditions for re-notification if necessary.

                if (!shouldNotify) return;

                previousState = _currentState;
                _currentState = newState;
                _lastNotifiedException = (newState == TaskExecutionState.Faulted) ? ex : null;
            }
            OnStateChanged(new TaskStateChangedEventArgs(previousState, newState, ex));
        }

        /// <summary>Requests cancellation of the currently running task.</summary>
        public void RequestCancel()
        {
            TaskExecutionState localCurrentState;
            lock (_stateLock) { localCurrentState = _currentState; } // Read current state under lock

            if (localCurrentState == TaskExecutionState.Running || localCurrentState == TaskExecutionState.Starting)
            {
                SetStateAndNotify(TaskExecutionState.Cancelling);
                try
                {
                    CancellationTokenSource?.Cancel();
                }
                catch (ObjectDisposedException) { /* Ignore if already disposed */ }
                catch (Exception cancelEx)
                {
                    ((IProgress<TaskProgressInfo>)ProgressReporter).Report(new TaskProgressInfo($"Error during cancellation request: {cancelEx.Message}", ReportLevel.Error));
                }
            }
        }

        /// <summary>Starts the task asynchronously.</summary>
        public async void StartTaskAsync(bool reportWrapperStatusMessages = true)
        {
            TaskExecutionState localCurrentState;
            lock (_stateLock) { localCurrentState = _currentState; }

            if (localCurrentState == TaskExecutionState.Starting || localCurrentState == TaskExecutionState.Running || localCurrentState == TaskExecutionState.Cancelling)
            {
                ((IProgress<TaskProgressInfo>)ProgressReporter).Report(new TaskProgressInfo("A task is already in progress or attempting to cancel.", ReportLevel.Warning));
                return;
            }

            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.Token;

            SetStateAndNotify(TaskExecutionState.Starting);
            _stopwatchTotal.Restart();

            try
            {
                if (reportWrapperStatusMessages)
                {
                    ((IProgress<TaskProgressInfo>)ProgressReporter).Report(new TaskProgressInfo(
                        content: $@"{TaskReportUtil.Now年月日时分秒()} Task Starting...",
                        progressText: "Starting...",
                        progressValue: TextProgressBarMinimum,
                        level: ReportLevel.ProcessStart));
                }

                BeforeDoWorkAction?.Invoke();
                token.ThrowIfCancellationRequested();

                if (DoWorkFuncAsync == null)
                {
                    throw new InvalidOperationException($"{nameof(DoWorkFuncAsync)} delegate is not set.");
                }

                SetStateAndNotify(TaskExecutionState.Running);
                string taskResultMessage = await DoWorkFuncAsync(token, ProgressReporter).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();

                if (!string.IsNullOrWhiteSpace(taskResultMessage))
                {
                    throw new Exception(taskResultMessage);
                }

                SetStateAndNotify(TaskExecutionState.Completed);
                if (reportWrapperStatusMessages)
                {
                    ((IProgress<TaskProgressInfo>)ProgressReporter).Report(new TaskProgressInfo(
                        content: $@"----------Task Completed----------{Environment.NewLine}{TaskReportUtil.Now年月日时分秒()} Total time: {TaskReportUtil.StopAndgetSecondF2(_stopwatchTotal)} {Environment.NewLine}",
                        progressText: "Completed",
                        progressValue: TextProgressBarMaximum,
                        level: ReportLevel.ProcessEnd));
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                SetStateAndNotify(TaskExecutionState.Cancelled);
                if (reportWrapperStatusMessages)
                {
                    ((IProgress<TaskProgressInfo>)ProgressReporter).Report(new TaskProgressInfo(
                        content: $@"----------Task Cancelled----------{Environment.NewLine}{TaskReportUtil.Now年月日时分秒()} Total time: {TaskReportUtil.StopAndgetSecondF2(_stopwatchTotal)} {Environment.NewLine}",
                        progressText: "Cancelled by user",
                        progressValue: null,
                        level: ReportLevel.ProcessCancelled));
                }
            }
            catch (Exception ex)
            {
                SetStateAndNotify(TaskExecutionState.Faulted, ex); // Pass the exception here
                if (reportWrapperStatusMessages)
                {
                    var stackTraceContent = ShowErrorStackTrace ? (Environment.NewLine + "StackTrace: " + ex.StackTrace) : "";
                    ((IProgress<TaskProgressInfo>)ProgressReporter).Report(new TaskProgressInfo(
                        content: $@"----------Task Error----------{Environment.NewLine}{TaskReportUtil.Now年月日时分秒()} Total time: {TaskReportUtil.StopAndgetSecondF2(_stopwatchTotal)} {Environment.NewLine}Error: {ex.Message}{stackTraceContent}{Environment.NewLine}",
                        progressText: "Error occurred",
                        progressValue: null,
                        level: ReportLevel.Error));
                }
            }
            finally
            {
                TaskExecutionState finalStateCheck;
                lock (_stateLock) { finalStateCheck = _currentState; }

                if (finalStateCheck != TaskExecutionState.Completed &&
                    finalStateCheck != TaskExecutionState.Faulted &&
                    finalStateCheck != TaskExecutionState.Cancelled)
                {
                    // If not in a terminal state, implies an issue or premature exit from try block.
                    // Default to Idle, or Faulted if an unhandled error is suspected.
                    SetStateAndNotify(TaskExecutionState.Idle);
                }
                AfterCompletionAction?.Invoke();
            }
        }

        /// <summary>Releases the managed resources used by the <see cref="TaskWrapper"/>.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases resources.</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CancellationTokenSource?.Cancel(); // Attempt to cancel any ongoing task
                    CancellationTokenSource?.Dispose();
                }
                _disposedValue = true;
            }
        }
    }
}
