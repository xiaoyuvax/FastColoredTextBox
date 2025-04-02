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
            label1 = new Label();
            fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)fctb).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(0, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(486, 111);
            label1.TabIndex = 3;
            label1.Text = resources.GetString("label1.Text");
            // 
            // fctb
            // 
            fctb.AccessibleDescription = "Textbox control";
            fctb.AccessibleName = "Fast Colored Text Box";
            fctb.AccessibleRole = AccessibleRole.Text;
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
            fctb.AutoScrollMinSize = new Size(0, 288);
            fctb.BackBrush = null;
            fctb.BorderStyle = BorderStyle.FixedSingle;
            fctb.CharCnWidth = 19;
            fctb.CharHeight = 24;
            fctb.CharWidth = 12;
            fctb.Cursor = Cursors.IBeam;
            fctb.DefaultMarkerSize = 8;
            fctb.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            fctb.Dock = DockStyle.Fill;
            fctb.FindForm = null;
            fctb.FoldingHighlightColor = Color.LightGray;
            fctb.FoldingHighlightEnabled = false;
            fctb.Font = new Font("Consolas", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            fctb.GoToForm = null;
            fctb.Hotkeys = resources.GetString("fctb.Hotkeys");
            fctb.ImeMode = ImeMode.On;
            fctb.IsReplaceMode = false;
            fctb.LeftBracket = '(';
            fctb.Location = new Point(0, 111);
            fctb.Margin = new Padding(4, 4, 4, 4);
            fctb.Name = "fctb";
            fctb.Paddings = new Padding(0);
            fctb.ReplaceForm = null;
            fctb.RightBracket = ')';
            fctb.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            fctb.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("fctb.ServiceColors");
            fctb.ShowLineNumbers = false;
            fctb.Size = new Size(486, 284);
            fctb.TabIndex = 2;
            fctb.Text = resources.GetString("fctb.Text");
            fctb.ToolTipDelay = 100;
            fctb.UseCJK = FastColoredTextBoxNS.Enums.CJKMode.CJK;
            fctb.WordWrap = true;
            fctb.Zoom = 100;
            // 
            // IMEsample
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(486, 395);
            Controls.Add(fctb);
            Controls.Add(label1);
            Margin = new Padding(4, 4, 4, 4);
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