namespace Tester
{
    partial class MarkdownSample
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnTheme = new System.Windows.Forms.Button();
            this.fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(784, 35);
            this.label1.TabIndex = 1;
            this.label1.Text = "Markdown syntax highlighting sample. Uses built-in Language.Markdown highlighter.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnTheme
            // 
            this.btnTheme.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTheme.Location = new System.Drawing.Point(0, 0);
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Size = new System.Drawing.Size(784, 28);
            this.btnTheme.TabIndex = 2;
            this.btnTheme.Text = "Toggle dark theme (markdown colours are derived from BackColor)";
            this.btnTheme.UseVisualStyleBackColor = true;
            this.btnTheme.Click += new System.EventHandler(this.btnTheme_Click);
            // 
            // fctb
            // 
            this.fctb.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.fctb.BackBrush = null;
            this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctb.DescriptionFile = "";
            this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctb.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.fctb.Location = new System.Drawing.Point(0, 63);
            this.fctb.Name = "fctb";
            this.fctb.Paddings = new System.Windows.Forms.Padding(0);
            this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctb.Size = new System.Drawing.Size(784, 498);
            this.fctb.TabIndex = 0;
            // 
            // MarkdownSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.fctb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTheme);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MarkdownSample";
            this.Text = "Markdown Syntax Highlighting Sample";
            this.Load += new System.EventHandler(this.MarkdownSample_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTheme;
        private FastColoredTextBoxNS.FastColoredTextBox fctb;
    }
}
