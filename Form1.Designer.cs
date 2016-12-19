namespace SQLParserDB
{
    partial class MainForm
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
            this.EditorPanel = new Telerik.WinControls.UI.RadPanel();
            this.EditorTextBox = new System.Windows.Forms.TextBox();
            this.ClearButton = new Telerik.WinControls.UI.RadButton();
            this.SubmitButton = new Telerik.WinControls.UI.RadButton();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.TableGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.EditorPanel)).BeginInit();
            this.EditorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClearButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubmitButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TableGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // EditorPanel
            // 
            this.EditorPanel.Controls.Add(this.EditorTextBox);
            this.EditorPanel.Controls.Add(this.ClearButton);
            this.EditorPanel.Controls.Add(this.SubmitButton);
            this.EditorPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.EditorPanel.Location = new System.Drawing.Point(0, 0);
            this.EditorPanel.Name = "EditorPanel";
            this.EditorPanel.Size = new System.Drawing.Size(1186, 268);
            this.EditorPanel.TabIndex = 0;
            this.EditorPanel.Text = "radPanel1";
            // 
            // EditorTextBox
            // 
            this.EditorTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.EditorTextBox.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditorTextBox.Location = new System.Drawing.Point(0, 0);
            this.EditorTextBox.Multiline = true;
            this.EditorTextBox.Name = "EditorTextBox";
            this.EditorTextBox.Size = new System.Drawing.Size(1186, 232);
            this.EditorTextBox.TabIndex = 3;
            this.EditorTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ClearButton
            // 
            this.ClearButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearButton.Location = new System.Drawing.Point(128, 238);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(110, 24);
            this.ClearButton.TabIndex = 2;
            this.ClearButton.Text = "Clear";
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // SubmitButton
            // 
            this.SubmitButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SubmitButton.Location = new System.Drawing.Point(12, 238);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(110, 24);
            this.SubmitButton.TabIndex = 1;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.TableGridView);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 268);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(1186, 237);
            this.radPanel1.TabIndex = 1;
            this.radPanel1.Text = "radPanel1";
            // 
            // TableGridView
            // 
            this.TableGridView.AllowUserToAddRows = false;
            this.TableGridView.AllowUserToDeleteRows = false;
            this.TableGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TableGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableGridView.Location = new System.Drawing.Point(0, 0);
            this.TableGridView.Name = "TableGridView";
            this.TableGridView.ReadOnly = true;
            this.TableGridView.Size = new System.Drawing.Size(1186, 237);
            this.TableGridView.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 505);
            this.Controls.Add(this.radPanel1);
            this.Controls.Add(this.EditorPanel);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.EditorPanel)).EndInit();
            this.EditorPanel.ResumeLayout(false);
            this.EditorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClearButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubmitButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TableGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel EditorPanel;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadButton ClearButton;
        private Telerik.WinControls.UI.RadButton SubmitButton;
        private System.Windows.Forms.TextBox EditorTextBox;
        private System.Windows.Forms.DataGridView TableGridView;
    }
}

