using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    /// <summary>
    /// An extended RichTextBox control that attempts to provide more control over scrolling behavior
    /// when appending text, especially when the user has manually scrolled away from the end.
    /// Based on solutions found on StackOverflow, e.g., https://stackoverflow.com/questions/6547193/
    /// </summary>
    /// <remarks>
    /// The primary goal of this class is to prevent automatic scrolling to the end if the
    /// user has selected text or scrolled to a different part of the document.
    /// It also aims to handle colored text appending correctly under these conditions.
    /// </remarks>
    public class RichTextBoxEx : RichTextBox
    {
        // Windows messages
        private const int WM_USER = 0x0400;
        private const int WM_SETREDRAW = 0x000B; // Tells a window to stop or start redrawing itself
        // RichEdit messages
        private const int EM_GETEVENTMASK = WM_USER + 59;
        private const int EM_SETEVENTMASK = WM_USER + 69;
        private const int EM_GETSCROLLPOS = WM_USER + 221; // Gets the current scroll position
        private const int EM_SETSCROLLPOS = WM_USER + 222; // Sets the current scroll position

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref Point lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        // [DllImport("user32.dll")] // Not directly used in this version of the logic
        // private static extern int GetCaretPos(out Point p);

        private Point _savedScrollPoint;
        private bool _isPaintingSuspended = false;
        private IntPtr _savedEventMask;
        // private int _savedCaretIndex; // To save caret physical position if needed
        private int _savedSelectionStart;
        private int _savedSelectionLength;
        private bool _wasSelectionAtEnd;
        private Color _currentSelectionColor = SystemColors.WindowText; // Default to system text color

        /// <summary>
        /// Gets or sets the color to be used for newly appended text or the current selection.
        /// This overrides the base <see cref="RichTextBox.SelectionColor"/> to ensure color is applied correctly
        /// during suspended painting.
        /// </summary>
        public new Color SelectionColor
        {
            get { return _currentSelectionColor; }
            set { _currentSelectionColor = value; }
        }

        /// <summary>
        /// Appends text to the current text of a rich text box, applying the current <see cref="SelectionColor"/>.
        /// If the current selection is not at the end of the text, painting is suspended to preserve
        /// the user's scroll position and selection.
        /// </summary>
        /// <param name="text">The text to append.</param>
        public new void AppendText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            bool selectionIsAtEnd = (this.SelectionStart + this.SelectionLength) == this.TextLength;

            if (selectionIsAtEnd && !this.Focused) // If not focused but at end, standard append is fine.
            {
                // Or if it's focused and caret is at the very end.
                // The original logic `this.SelectionStart >= this.TextLength` is a bit simpler for "at end".
                // Let's refine this to better match user expectation: if caret is at end, auto-scroll.
                // If user has scrolled up, don't auto-scroll.
                // A common check is if (this.SelectionStart == this.TextLength && this.SelectionLength == 0)
                // For now, using a simplified check: if selection start is at or beyond current text length.
                // This implies new text is being added at the very end.
                if (this.SelectionStart >= this.TextLength)
                {
                    base.SelectionColor = _currentSelectionColor; // Apply the stored color
                    base.AppendText(text); // Use base AppendText for efficiency
                    return; // Auto-scroll will occur if caret was at end
                }
            }

            // If not at the end, or if complex selection exists, suspend painting to manage scroll and selection
            SuspendPaintingInternal();
            try
            {
                // Temporarily move to the end to append text with color
                base.Select(this.TextLength, 0);
                base.SelectionColor = _currentSelectionColor;
                base.AppendText(text); // This append happens at the actual end of the RichTextBox content

                // Restore original selection and scroll position
                // This is the tricky part: if the original selection was, e.g., in the middle,
                // appending text changes character indices.
                // The saved selection might need adjustment if text was inserted *before* it.
                // However, AppendText always adds to the very end, so original selection indices remain valid
                // relative to the *original* text length.
                // The key is to restore the *view*, not necessarily the exact character indices if the goal is to prevent scroll jump.
            }
            finally
            {
                ResumePaintingInternal();
            }
        }

        /// <summary>
        /// Suspends redrawing of the control.
        /// </summary>
        private void SuspendPaintingInternal()
        {
            if (!_isPaintingSuspended)
            {
                // Save current state
                _savedSelectionStart = this.SelectionStart;
                _savedSelectionLength = this.SelectionLength;
                _wasSelectionAtEnd = (_savedSelectionStart + _savedSelectionLength) >= this.TextLength;

                SendMessage(this.Handle, EM_GETSCROLLPOS, 0, ref _savedScrollPoint);
                _savedEventMask = SendMessage(this.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);

                // Stop redrawing
                SendMessage(this.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
                _isPaintingSuspended = true;
            }
        }

        /// <summary>
        /// Resumes redrawing of the control and restores its previous state.
        /// </summary>
        private void ResumePaintingInternal()
        {
            if (_isPaintingSuspended)
            {
                // Restore selection
                // If the original selection was at the end, and we want to keep it at the new end after appending,
                // then we might not need to restore the selection to _savedSelectionStart.
                // However, if the goal is to preserve the user's view, restoring scroll position is key.
                // And if they had text selected, restore that selection.

                // If the user's selection was NOT at the end, restore it.
                // If it WAS at the end, the caret will naturally be at the new end after AppendText.
                if (!_wasSelectionAtEnd)
                {
                    // Ensure selection values are valid for the current text length
                    int currentTextLength = this.TextLength;
                    int newSelStart = Math.Min(_savedSelectionStart, currentTextLength);
                    int newSelLength = _savedSelectionLength;
                    if (newSelStart + newSelLength > currentTextLength)
                    {
                        newSelLength = currentTextLength - newSelStart;
                    }
                    if (newSelLength < 0) newSelLength = 0;

                    this.Select(newSelStart, newSelLength);
                }
                else
                {
                    // If selection was at end, ensure caret is at the new end.
                    // This is usually handled by RichTextBox itself after AppendText.
                    // this.Select(this.TextLength, 0); // Could be redundant
                }


                // Restore scroll position
                SendMessage(this.Handle, EM_SETSCROLLPOS, 0, ref _savedScrollPoint);

                // Restore event mask and allow redrawing
                SendMessage(this.Handle, EM_SETEVENTMASK, 0, _savedEventMask);
                SendMessage(this.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
                _isPaintingSuspended = false;

                this.Invalidate(); // Force a repaint
            }
        }

        /// <summary>
        /// Handles the <see cref="Control.HandleCreated"/> event.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Ensure painting is not suspended if handle is recreated
            _isPaintingSuspended = false;
        }
    }
}