# GoldArch TaskWrapper & UI Controls

A .NET Windows Forms library for simplifying background task management with integrated UI controls for progress reporting and status display.

## Overview

The `GoldArch.TaskWrapper` library (found within the `GoldArch.TaskWrapperReport` namespace structure in your code) provides a robust `TaskWrapper` class to execute asynchronous operations while offering fine-grained control over progress reporting, cancellation, state management, and error handling. It is complemented by a suite of custom UI controls designed to visualize task progress and logs effectively.

The solution, as provided, includes:
* **GoldArch.TaskWrapper (Class Library Components):** Contains the core `TaskWrapper` logic and reusable UI controls. These are organized within namespaces like `GoldArch.TaskWrapperReport.TaskWrapperCore`, `GoldArch.TaskWrapperReport.TaskReportControl`, and `GoldArch.TaskWrapperReport.Common`.
* **GoldArch.TaskWrapper.Simple (WinForms Application):** A demonstration project showcasing the various features and UI controls of the library.

## Features

### Core TaskWrapper (`GoldArch.TaskWrapperReport.TaskWrapperCore.TaskWrapper`)
* **Asynchronous Task Execution:** Run long-running operations on background threads without freezing the UI.
* **Progress Reporting:** Tasks can report progress updates including messages, percentage values, and severity levels (`Information`, `Warning`, `Error`, `Success`, etc.) using `TaskProgressInfo` objects.
* **CancellationToken Support:** Graceful cancellation of tasks via `CancellationTokenSource`.
* **State Management:** Tasks transition through well-defined states (`Idle`, `Starting`, `Running`, `Cancelling`, `Completed`, `Faulted`, `Cancelled`) defined in `TaskExecutionState` enum.
* **Error Handling:** Captures and reports exceptions from tasks, with events providing `TaskStateChangedEventArgs` containing exception details.
* **Lifecycle Actions:** Define actions to be executed before a task starts (`BeforeDoWorkAction`) and after it completes (`AfterCompletionAction`).
* **Configurable Stack Trace Display:** Option to show or hide error stack traces in reports (`ShowErrorStackTrace` property).

### UI Controls (`GoldArch.TaskWrapperReport.TaskReportControl`)
* **`TextProgressBar`:** A custom `ProgressBar` that can display text directly on it, with configurable text/progress colors and a `CustomText` property.
* **`RichTextBoxEx`:** An enhanced `RichTextBox` with improved automatic scrolling behavior, especially when appending text while the user might have scrolled to a different position. It uses Win32 messages like `EM_GETSCROLLPOS` and `EM_SETSCROLLPOS` for scroll management.
* **`StatusProgressBar`:** A compact user control that uses a `TextProgressBar` to display all task status (idle, running, completed, error, etc.) and progress information as formatted text on the progress bar itself. Offers extensive color and text customization for different states (e.g., `TextFormatIdle`, `ProgressBarRunningColor`).
* **`StatusProgressDisplay`:** A user control featuring a `Label` for status text (`lblStatus`) and a `TextProgressBar` for visual progress. Provides clear separation of status messages and progress visualization, with customizable colors and texts for different states (e.g., `StatusTextIdle`, `ProgressBarIdleColor`).
* **`TaskReportRichTextBox`:** A comprehensive user control that integrates a `TextProgressBar`, `RichTextBoxEx` for detailed logging, and a cancel button. It internally manages a `TaskWrapperInstance` and provides a rich interface for task reporting with customizable text colors for different log levels (e.g., `InfoTextColor`, `ErrorTextColor`).

### Helper Classes
* **`TaskExecutionHelper`:** Provides static helper methods (`ExecuteAction`, `ExecuteAsyncFunc`) for quickly running tasks with a `TaskWrapper` using a more concise syntax.
* **`ControlInvokeHelper`:** Utilities for safely invoking actions on UI controls from non-UI threads, using `Control.Invoke` and `Control.BeginInvoke`.
* **`TaskReportUtil`:** Simple utilities, for instance, for formatting timestamps (e.g., `Now年月日时分秒`).

### Data Structures
* **`TaskProgressInfo`:** Encapsulates progress update details (content message, progress text, progress value, report level).
* **`ReportLevel` (enum):** Defines the nature of a progress message (e.g., `Information`, `Warning`, `Error`, `Success`, `Detail`).
* **`TaskExecutionState` (enum):** Represents the current state of a task (e.g., `Idle`, `Running`, `Completed`, `Faulted`).

## Project Structure (based on provided files)

* `GoldArch.TaskWrapper/` (Conceptual library root)
    * `Common/ControlInvokeHelper.cs`
    * `Properties/` (AssemblyInfo.cs, Resources.Designer.cs, Settings.Designer.cs)
    * `TaskReportControl/` (TextProgressBar.cs, RichTextBoxEx.cs, StatusProgressBar.cs, StatusProgressDisplay.cs, TaskReportRichTextBox.cs and their .designer.cs files)
    * `TaskWrapperCore/` (TaskWrapper.cs, TaskProgressInfo.cs, TaskReportUtil.cs, TaskExecutionHelper.cs)
* `GoldArch.TaskWrapper.Simple/` (Demo application)
    * `Properties/` (AssemblyInfo.cs, Resources.Designer.cs, Settings.Designer.cs)
    * `Program.cs`
    * `MainForm.cs`
    * `SimpleProgressForm.cs`
    * `SimpleProgressForm02.cs`
    * `SimpleProgressForm03.cs`
    * `App.config`

## How to Use

### 1. Using `TaskReportRichTextBox` (Recommended for detailed logs)

This control is self-contained and manages its own `TaskWrapperInstance`.

```csharp
// In your Form:
// Add TaskReportRichTextBox control (e.g., taskReportControl1) via designer or code.

// Configure in MainForm.cs or similar:
private void SetupTaskReportControl()
{
    taskReportControl1.ShowCancelButton = true;
    taskReportControl1.ShowErrorStackTraceInWrapper = true; 
    taskReportControl1.IsScrollToEndAfterReport = true;   

    taskReportControl1.TaskWrapperInstance.StateChanged += (s, e) => {
        // Custom logic based on task state
    };
}

private void BtnStartComplexTask_Click(object sender, EventArgs e)
{
    var wrapper = taskReportControl1.TaskWrapperInstance;
    // Check if task is already running
    if (wrapper.CurrentState == TaskExecutionState.Running || wrapper.CurrentState == TaskExecutionState.Starting)
    {
        MessageBox.Show("A task is already running.");
        return;
    }
    taskReportControl1.ResetReportControl(); 

    wrapper.DoWorkFuncAsync = async (token, progress) =>
    {
        for (int i = 0; i <= 10; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(500, token).ConfigureAwait(false);

            progress.Report(new TaskProgressInfo(
                content: $"Step {i + 1} logged. Details...\n",
                progressText: $"Processing item {i + 1} of 11",
                progressValue: (i + 1) * (100 / 11), // Example calculation
                level: i > 7 ? ReportLevel.Warning : ReportLevel.Information
            ));
        }
        progress.Report(new TaskProgressInfo("All steps processed successfully!", ReportLevel.Success));
        return null; // Success
    };
    
    wrapper.StartTaskAsync();
}

// To report messages not directly tied to the TaskWrapper's current task:
private void BtnReportDirectly_Click(object sender, EventArgs e)
{
    taskReportControl1.ReportDirectContent(
        $"This is a direct informational message.\n", ReportLevel.Information);
    taskReportControl1.ReportDirectProgress("Direct Update", 50, ReportLevel.StatusUpdate);
}

2. Using TaskWrapper with StatusProgressDisplay or StatusProgressBar
(Example adapted from SimpleProgressForm02.cs and SimpleProgressForm03.cs)

// In your Form (e.g., for StatusProgressDisplay):
// private StatusProgressDisplay statusProgressDisplayControl;
// private TaskWrapper _taskWrapper;

private void InitializeMyForm()
{
    _taskWrapper = new TaskWrapperReport.TaskWrapperCore.TaskWrapper();
    statusProgressDisplayControl.ResetDisplay(_taskWrapper.TextProgressBarMinimum, _taskWrapper.TextProgressBarMaximum);

    _taskWrapper.ProgressReporter.ProgressChanged += (s, progressInfo) => {
        statusProgressDisplayControl.ReportProgress(progressInfo, _taskWrapper.CurrentState);
    };
    _taskWrapper.StateChanged += (s, stateArgs) => {
        ControlInvokeHelper.ControlInvoke(this, () => { 
            statusProgressDisplayControl.SetVisualState(stateArgs.NewState, stateArgs.Exception);
            // Update other UI like button states
        });
    };
}

private void BtnStartTask_Click(object sender, EventArgs e)
{
    // Check if task is runnable and reset display
    statusProgressDisplayControl.ResetDisplay(_taskWrapper.TextProgressBarMinimum, _taskWrapper.TextProgressBarMaximum);

    _taskWrapper.DoWorkFuncAsync = async (token, progress) =>
    {
        // Your task logic here, reporting progress via progress.Report(...)
        return null; // On success
    };
    _taskWrapper.StartTaskAsync(reportWrapperStatusMessages: false); // Let the control handle status messages
}

// Remember to dispose _taskWrapper in FormClosing event

# Building and Running the Demo
Clone the repository https://github.com/goldarch/GoldArch.TaskWrapperReport.
Open the solution (likely GoldArch.TaskWrapperReport.sln or similar) in Visual Studio.
Set GoldArch.TaskWrapper.Simple as the startup project.
Build and run. The demo application will appear, allowing you to test various task reporting scenarios.
Dependencies
.NET Framework 4.5.2 (as specified in GoldArch.TaskWrapper.Simple/App.config and project files).
# License
MIT
