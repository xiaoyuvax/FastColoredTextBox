namespace Tester
{
    partial class IMEsample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IMEsample));
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
            label1.Size = new System.Drawing.Size(486, 111);
            label1.TabIndex = 3;
            label1.Text = resources.GetString("label1.Text");
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
            fctb.AutoIndent = false;
            fctb.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            fctb.AutoScrollMinSize = new System.Drawing.Size(0, 24);
            fctb.BackBrush = null;
            fctb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            fctb.CharHeight = 24;
            fctb.CharWidth = 12;
            fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            fctb.DefaultMarkerSize = 8;
            fctb.DisabledColor = System.Drawing.Color.FromArgb(100, 180, 180, 180);
            fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            fctb.FindForm = null;
            fctb.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            fctb.GoToForm = null;
            fctb.Hotkeys = resources.GetString("fctb.Hotkeys");
            fctb.ImeMode = System.Windows.Forms.ImeMode.On;
            fctb.IsReplaceMode = false;
            fctb.LeftBracket = '(';
            fctb.Location = new System.Drawing.Point(0, 111);
            fctb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            fctb.Name = "fctb";
            fctb.Paddings = new System.Windows.Forms.Padding(0);
            fctb.ReplaceForm = null;
            fctb.RightBracket = ')';
            fctb.SelectionColor = System.Drawing.Color.FromArgb(60, 0, 0, 255);
            fctb.SelectionLength = 5;
            fctb.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("fctb.ServiceColors");
            fctb.ShowLineNumbers = false;
            fctb.Size = new System.Drawing.Size(486, 284);
            fctb.TabIndex = 2;
            fctb.Text = "ABC文字";
            fctb.WordWrap = true;
            fctb.Zoom = 100;
            // 
            // IMEsample
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(486, 395);
            Controls.Add(fctb);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "IMEsample";
            Text = "IMEsample";
            ((System.ComponentModel.ISupportInitialize)fctb).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fctb;
        private System.Windows.Forms.Label label1;
    }
}