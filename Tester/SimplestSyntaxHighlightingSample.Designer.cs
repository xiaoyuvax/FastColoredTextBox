namespace Tester
{
    partial class SimplestSyntaxHighlightingSample
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimplestSyntaxHighlightingSample));
            label1 = new System.Windows.Forms.Label();
            fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)fctb).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Top;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(581, 59);
            label1.TabIndex = 1;
            label1.Text = "This example shows how to make simplest syntax highlighting.";
            // 
            // fctb
            // 
            fctb.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            fctb.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            fctb.AutoScrollMinSize = new System.Drawing.Size(480, 75);
            fctb.BackBrush = null;
            fctb.CharCnWidth = 15;
            fctb.CharHeight = 15;
            fctb.CharWidth = 7;
            fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            fctb.DefaultMarkerSize = 8;
            fctb.DescriptionFile = "";
            fctb.DisabledColor = System.Drawing.Color.FromArgb(100, 180, 180, 180);
            fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            fctb.FindForm = null;
            fctb.Font = new System.Drawing.Font("Consolas", 9.75F);
            fctb.GoToForm = null;
            fctb.Hotkeys = resources.GetString("fctb.Hotkeys");
            fctb.IsReplaceMode = false;
            fctb.Location = new System.Drawing.Point(0, 59);
            fctb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            fctb.Name = "fctb";
            fctb.Paddings = new System.Windows.Forms.Padding(0);
            fctb.ReplaceForm = null;
            fctb.SelectionColor = System.Drawing.Color.FromArgb(50, 0, 0, 255);
            fctb.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("fctb.ServiceColors");
            fctb.Size = new System.Drawing.Size(581, 282);
            fctb.TabIndex = 0;
            fctb.Text = "<li>Article\r\n<a href=\\\"#_comments\\\">Ask a Question about this article</a></li>\r\n<li class=\\\"heading\\\">Quick Answers</li>\r\n<li><a href=\\\"/Questions/ask.aspx\\\">Ask a Question</a></li>\r\n";
            fctb.UseCJK = FastColoredTextBoxNS.CJKMode.CJK;
            fctb.Zoom = 100;
            fctb.TextChanged += Fctb_TextChanged;
            // 
            // SimplestSyntaxHighlightingSample
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(581, 341);
            Controls.Add(fctb);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "SimplestSyntaxHighlightingSample";
            Text = "SimplestSyntaxHighlightingSample";
            ((System.ComponentModel.ISupportInitialize)fctb).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fctb;
        private System.Windows.Forms.Label label1;
    }
}