using System;
using System.IO;
using System.Text;
using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Text;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Regression tests for EncodingDetector fixes (#9: UTF-7 BOM no longer returned,
    /// ReadExactly for short reads), TextSource.SaveToFile fixes (#7 empty document
    /// guard, #12 failure-safe temp file handling) and Ruler.Dispose (#10).
    /// </summary>
    public class FileAndEncodingTests
    {
        #region EncodingDetector (#9)

        [Fact]
        public void DetectBOM_UTF8()
        {
            var enc = EncodingDetector.DetectBOMBytes([0xEF, 0xBB, 0xBF]);
            Assert.Equal(Encoding.UTF8, enc);
        }

        [Fact]
        public void DetectBOM_UTF16LE()
        {
            var enc = EncodingDetector.DetectBOMBytes([0xFF, 0xFE, 0x61, 0x00]);
            Assert.Equal(Encoding.Unicode, enc);
        }

        [Fact]
        public void DetectBOM_UTF16BE()
        {
            var enc = EncodingDetector.DetectBOMBytes([0xFE, 0xFF, 0x00, 0x61]);
            Assert.Equal(Encoding.BigEndianUnicode, enc);
        }

        [Fact]
        public void DetectBOM_UTF32LE()
        {
            var enc = EncodingDetector.DetectBOMBytes([0xFF, 0xFE, 0x00, 0x00]);
            Assert.Equal(Encoding.UTF32, enc);
        }

        [Fact]
        public void DetectBOM_UTF7BOM_FallsThroughToNull()
        {
            // #9: UTF-7 is obsolete/insecure (SYSLIB0001) and must not be returned;
            // files with an UTF-7 BOM fall through to heuristic detection.
            var utf7Bom = new byte[] { 0x2B, 0x2F, 0x76 }; // "+/v"
            Assert.Null(EncodingDetector.DetectBOMBytes(utf7Bom));
        }

        [Fact]
        public void DetectTextFileEncoding_UTF8WithBOM_RoundTrips()
        {
            var path = Path.Combine(Path.GetTempPath(), "fctb_test_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                File.WriteAllBytes(path, [0xEF, 0xBB, 0xBF, (byte)'a', (byte)'b']);
                var enc = EncodingDetector.DetectTextFileEncoding(path);
                Assert.Equal(Encoding.UTF8, enc);
            }
            finally { File.Delete(path); }
        }
        #endregion

        #region SaveToFile (#7, #12)

        [Fact]
        public void SaveToFile_EmptyDocument_DoesNotThrow()
        {
            StaRunner.Run(() =>
            {
                var path = Path.Combine(Path.GetTempPath(), "fctb_test_" + Guid.NewGuid().ToString("N") + ".txt");
                try
                {
                    using var tb = new FastColoredTextBox();
                    tb.Clear();
                    tb.SaveToFile(path, Encoding.UTF8); // #7: used to index lines[-1]

                    Assert.True(File.Exists(path), "file should exist");
                    // StreamWriter writes the UTF-8 BOM; the document itself is empty
                    Assert.Equal(3, new FileInfo(path).Length);
                }
                finally { File.Delete(path); }
            });
        }

        [Fact]
        public void SaveToFile_OpenFile_RoundTrip_PreservesContent()
        {
            StaRunner.Run(() =>
            {
                var path = Path.Combine(Path.GetTempPath(), "fctb_test_" + Guid.NewGuid().ToString("N") + ".txt");
                try
                {
                    File.WriteAllText(path, "line1\nline2\nline3", new UTF8Encoding(false));

                    using var tb = new FastColoredTextBox();
                    tb.OpenFile(path); // file-bound mode: SaveEOL = Environment.NewLine
                    Assert.Equal("line1\nline2\nline3", tb.Text.Replace("\r\n", "\n"));

                    var saved = Path.Combine(Path.GetTempPath(), "fctb_test_" + Guid.NewGuid().ToString("N") + ".txt");
                    try
                    {
                        tb.SaveToFile(saved, Encoding.UTF8);
                        Assert.Equal("line1\nline2\nline3", File.ReadAllText(saved).Replace("\r\n", "\n"));
                    }
                    finally { File.Delete(saved); }
                }
                finally { File.Delete(path); }
            });
        }

        [Fact]
        public void SaveToFile_FailureAtReplace_KeepsTargetFileIntact()
        {
            StaRunner.Run(() =>
            {
                // #12: if replacing the target fails mid-way, the original file must
                // not be lost or corrupted. Simulate by locking the destination path
                // as a directory (File.Move onto a directory fails).
                var dir = Path.Combine(Path.GetTempPath(), "fctb_test_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(dir);
                try
                {
                    using var tb = new FastColoredTextBox();
                    tb.Text = "hello\n";
                    // saving to a directory path must throw, not silently corrupt anything
                    Assert.ThrowsAny<Exception>(() => tb.SaveToFile(dir, Encoding.UTF8));
                    Assert.True(Directory.Exists(dir), "directory must remain");
                }
                finally { Directory.Delete(dir, true); }
            });
        }
        #endregion

        #region Ruler Dispose (#10)

        [Fact]
        public void Ruler_Dispose_UnsubscribesFromTargetEvents()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                var ruler = new Ruler { Target = tb };
                ruler.Dispose();

                // firing target events after ruler disposal must not throw or touch
                // a disposed control: handlers were removed in Dispose (#10)
                tb.OnSelectionChanged();
                tb.OnVisibleRangeChanged();
            });
        }
        #endregion
    }
}
