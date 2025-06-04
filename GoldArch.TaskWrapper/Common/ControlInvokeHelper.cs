using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoldArch.TaskWrapperReport.Common
{
    /// <summary>
    /// Provides helper methods for invoking actions on UI controls from different threads.
    /// </summary>
    public static class ControlInvokeHelper
    {
        /// <summary>
        /// Executes the specified action on the thread that owns the control's underlying window handle.
        /// If the calling thread is different from the thread that created the control,
        /// this method switches to the control's thread and then executes the action.
        /// </summary>
        /// <param name="control">The control whose thread the action should be invoked on.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="ArgumentNullException">control or action is null.</exception>
        /// <remarks>
        /// This method checks if the control's handle has been created and if the control is not disposed.
        /// It uses <see cref="Control.Invoke(Delegate)"/> for synchronous execution.
        /// </remarks>
        public static void ControlInvoke(Control control, Action action)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (action == null) throw new ArgumentNullException(nameof(action));

            try
            {
                if (control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                    return;

                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (ObjectDisposedException) { /* Control might have been disposed between checks and Invoke */ }
            catch (InvalidOperationException) { /* Handle might have been destroyed */ }
            // Catch other specific exceptions if necessary, or let them propagate if they are unexpected.
        }

        /// <summary>
        /// Executes the specified function on the thread that owns the control's underlying window handle
        /// and returns the result of the function.
        /// </summary>
        /// <typeparam name="T">The type of the return value of the function.</typeparam>
        /// <param name="control">The control whose thread the function should be invoked on.</param>
        /// <param name="func">The function to execute.</param>
        /// <returns>The result of the function execution.</returns>
        /// <exception cref="ArgumentNullException">control or func is null.</exception>
        public static T ControlInvoke<T>(Control control, Func<T> func)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (func == null) throw new ArgumentNullException(nameof(func));

            try
            {
                if (control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                    return default(T); // Or throw, depending on desired behavior for disposed controls

                if (control.InvokeRequired)
                {
                    return (T)control.Invoke(func);
                }
                else
                {
                    return func();
                }
            }
            catch (ObjectDisposedException) { return default(T); }
            catch (InvalidOperationException) { return default(T); }
        }


        /// <summary>
        /// Asynchronously executes the specified action on the thread that owns the control's underlying window handle.
        /// The calling thread does not wait for the action to complete.
        /// </summary>
        /// <param name="control">The control whose thread the action should be invoked on.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="ArgumentNullException">control or action is null.</exception>
        /// <remarks>
        /// This method checks if the control's handle has been created and if the control is not disposed.
        /// It uses <see cref="Control.BeginInvoke(Delegate)"/>.
        /// </remarks>
        public static void ControlBeginInvoke(Control control, Action action)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (action == null) throw new ArgumentNullException(nameof(action));
            try
            {
                if (control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                    return;

                // No need to check InvokeRequired for BeginInvoke, it handles it internally.
                // However, checking IsHandleCreated is still good practice.
                control.BeginInvoke(action);
            }
            catch (ObjectDisposedException) { /* Control might have been disposed */ }
            catch (InvalidOperationException) { /* Handle might have been destroyed */ }
        }


        // The generic versions of ControlInvoke and ControlBeginInvoke with 'dynamic' or 'object[]'
        // are less type-safe and generally less recommended than using lambdas with the Action/Func overloads.
        // They are kept here if they serve a specific purpose in your existing codebase.
        // Consider refactoring their use cases to leverage strongly-typed delegates if possible.

        /// <summary>
        /// Dynamically invokes a function on the control's thread. (Consider refactoring for type safety).
        /// </summary>
        public static void ControlInvoke(Control control, Func<dynamic, bool> function, dynamic obj)
        {
            ControlInvoke(control, () => function(obj));
        }

        /// <summary>
        /// Dynamically invokes a function with parameters on the control's thread. (Consider refactoring for type safety).
        /// </summary>
        public static object ControlInvoke(Control control, Func<object[], object> function, object[] paras)
        {
            return ControlInvoke(control, () => function(paras));
        }

        /// <summary>
        /// Dynamically begins invocation of a function on the control's thread. (Consider refactoring for type safety).
        /// </summary>
        public static void ControlBeginInvoke(Control control, Func<dynamic, bool> function, dynamic obj)
        {
            ControlBeginInvoke(control, () => function(obj));
        }


        /// <summary>
        /// Wraps an action with <see cref="ControlInvoke(Control, Action)"/> for easy passing to methods expecting an Action.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action to wrap.</param>
        /// <returns>An action that, when invoked, will execute the original action on the control's UI thread.</returns>
        public static Action GetControlInvokeAction(this Control control, Action action)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (action == null) throw new ArgumentNullException(nameof(action));
            return () => { ControlInvoke(control, action); };
        }

        /// <summary>
        /// Wraps an action with <see cref="ControlBeginInvoke(Control, Action)"/> for easy passing to methods expecting an Action.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action to wrap.</param>
        /// <returns>An action that, when invoked, will begin executing the original action on the control's UI thread.</returns>
        public static Action GetControlBeginInvokeAction(this Control control, Action action)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (action == null) throw new ArgumentNullException(nameof(action));
            return () => { ControlBeginInvoke(control, action); };
        }

        /// <summary>
        /// Runs an action on a background thread and then invokes a UI update via <see cref="ControlInvoke(Control, Action)"/>.
        /// This is a simplified helper; for complex scenarios, <see cref="TaskWrapper"/> is recommended.
        /// Ensures the UI action is only performed if the control is not disposed.
        /// </summary>
        /// <param name="control">The control for UI updates.</param>
        /// <param name="uiAction">The action to perform on the UI thread.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>The action itself should not contain long-running operations if invoked directly on the UI thread.</remarks>
        public static Task ControlInvokeActionTaskRun(this Control control, Action uiAction)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (uiAction == null) throw new ArgumentNullException(nameof(uiAction));

            return Task.Factory.StartNew(() => {
                ControlInvoke(control, uiAction);
            });
        }

        /// <summary>
        /// Runs an action on a background thread and then begins invocation of a UI update via <see cref="ControlBeginInvoke(Control, Action)"/>.
        /// This is a simplified helper; for complex scenarios, <see cref="TaskWrapper"/> is recommended.
        /// Ensures the UI action is only performed if the control is not disposed.
        /// </summary>
        /// <param name="control">The control for UI updates.</param>
        /// <param name="uiAction">The action to perform on the UI thread.</param>
        /// <returns>A task representing the asynchronous operation of queuing the UI update.</returns>
        public static Task ControlBeginInvokeActionTaskRun(this Control control, Action uiAction)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (uiAction == null) throw new ArgumentNullException(nameof(uiAction));

            return Task.Factory.StartNew(() => {
                ControlBeginInvoke(control, uiAction);
            });
        }
    }
}