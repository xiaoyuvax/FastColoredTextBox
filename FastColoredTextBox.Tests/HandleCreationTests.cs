using System;
using System.Threading;
using System.Windows.Forms;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    public class HandleCreationTests
    {
        /// <summary>
        /// Control.OnHandleCreated registers the OLE drop target when AllowDrop is set, which needs
        /// an STA thread and otherwise throws ThreadStateException ("current thread must be set to
        /// single thread apartment"). A host that creates the control from a background call must
        /// get a working control instead of a crash.
        /// </summary>
        [Fact]
        public void HandleCreatedOnMtaThread_DoesNotThrow()
        {
            Exception failure = null;
            var thread = new Thread(() =>
            {
                try
                {
                    using var tb = new FastColoredTextBox();
                    _ = tb.Handle;
                    Assert.False(tb.AllowDrop);
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            thread.Join();

            if (failure != null)
                throw failure;
        }
    }
}
