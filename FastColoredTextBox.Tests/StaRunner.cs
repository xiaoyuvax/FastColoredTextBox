using System;
using System.Threading;
using Xunit;

// FastColoredTextBox keeps static state (TextSource.CurrentTB) shared by all
// instances, so FCTB instances must never live on concurrent threads.
// Disable xUnit collection parallelization to keep control-level tests safe.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Runs a test body on a dedicated STA thread and propagates failures.
    /// FastColoredTextBox is a WinForms control: all of its GDI/Win32 objects
    /// (handles, brushes, fonts) must be created on an STA thread, so every
    /// control-level test wraps its body in StaRunner.Run(...).
    /// </summary>
    public static class StaRunner
    {
        public static void Run(Action test)
        {
            Exception failure = null;
            var thread = new Thread(() =>
            {
                try { test(); }
                catch (Exception ex) { failure = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (failure != null)
                throw failure;
        }
    }
}
