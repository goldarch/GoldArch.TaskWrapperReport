using System;
using System.Drawing;
using System.Windows.Forms;

// For FormClosingEventHandler and MessageBoxIcon

// For TaskExecutionState

namespace GoldArch.TaskWrapperReport.Simple.MVP
{
    public interface ISimpleProgressView
    {
        // Properties for UI elements (getters for Presenter to read, setters for Presenter to update)
        string StatusText { get; set; }
        int ProgressBarValue { get; set; }
        int ProgressBarMinimum { get; set; }
        int ProgressBarMaximum { get; set; }
        string ProgressBarCustomText { get; set; }
        Color ProgressBarProgressColor { get; set; } // Renamed from ProgressBarColor to avoid conflict with Control.BackColor if any
        bool StartSimpleTaskEnabled { get; set; }
        bool StartErrorTaskEnabled { get; set; }
        bool CancelTaskEnabled { get; set; }

        // Events triggered by UI, handled by Presenter
        event EventHandler LoadView; // For presenter initialization
        event EventHandler StartSimpleTaskClicked;
        event EventHandler StartErrorTaskClicked;
        event EventHandler CancelTaskClicked;
        event FormClosingEventHandler ViewClosing;

        // Methods Presenter can call on the View
        void ShowMessage(string caption, string text, MessageBoxButtons buttons, MessageBoxIcon icon);
        void CloseView(); // If presenter needs to close the view
    }
}