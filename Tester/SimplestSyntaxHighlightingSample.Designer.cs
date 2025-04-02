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
            label1 = new Label();
            fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            panel1 = new Panel();
            label2 = new Label();
            cboCJKMode = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)fctb).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(0, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(581, 35);
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
            fctb.AutoScrollMinSize = new Size(480, 120);
            fctb.BackBrush = null;
            fctb.CharCnWidth = 14;
            fctb.CharHeight = 15;
            fctb.CharWidth = 7;
            fctb.Cursor = Cursors.IBeam;
            fctb.DefaultMarkerSize = 8;
            fctb.DescriptionFile = "";
            fctb.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            fctb.Dock = DockStyle.Fill;
            fctb.FindForm = null;
            fctb.Font = new Font("Consolas", 9.75F);
            fctb.GoToForm = null;
            fctb.Hotkeys = resources.GetString("fctb.Hotkeys");
            fctb.IsReplaceMode = false;
            fctb.Location = new Point(0, 67);
            fctb.Margin = new Padding(4);
            fctb.Name = "fctb";
            fctb.Paddings = new Padding(0);
            fctb.ReplaceForm = null;
            fctb.SelectionColor = Color.FromArgb(50, 0, 0, 255);
            fctb.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("fctb.ServiceColors");
            fctb.Size = new Size(581, 274);
            fctb.TabIndex = 0;
            fctb.Text = resources.GetString("fctb.Text");
            fctb.UseCJK = FastColoredTextBoxNS.Enums.CJKMode.CJK;
            fctb.Zoom = 100;
            fctb.TextChanged += Fctb_TextChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(cboCJKMode);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(581, 67);
            panel1.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(371, 40);
            label2.Name = "label2";
            label2.Size = new Size(76, 17);
            label2.TabIndex = 3;
            label2.Text = "CJKMode：";
            // 
            // cboCJKMode
            // 
            cboCJKMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboCJKMode.FormattingEnabled = true;
            cboCJKMode.Location = new Point(448, 35);
            cboCJKMode.Name = "cboCJKMode";
            cboCJKMode.Size = new Size(121, 25);
            cboCJKMode.TabIndex = 2;
            cboCJKMode.SelectedIndexChanged += cboCJKMode_SelectedIndexChanged;
            // 
            // SimplestSyntaxHighlightingSample
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(581, 341);
            Controls.Add(fctb);
            Controls.Add(panel1);
            Margin = new Padding(4);
            Name = "SimplestSyntaxHighlightingSample";
            Text = "SimplestSyntaxHighlightingSample";
            ((System.ComponentModel.ISupportInitialize)fctb).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fctb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboCJKMode;
        private Label label2;
    }
}