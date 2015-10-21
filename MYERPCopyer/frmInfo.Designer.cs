namespace MYERPCopyer
{
    partial class frmInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInfo));
            this.pbProcess = new System.Windows.Forms.ProgressBar();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lbList = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbProcess
            // 
            this.pbProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbProcess.ForeColor = System.Drawing.Color.Navy;
            this.pbProcess.Location = new System.Drawing.Point(3, 139);
            this.pbProcess.Name = "pbProcess";
            this.pbProcess.Size = new System.Drawing.Size(369, 19);
            this.pbProcess.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProcess.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(369, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "正在更新ERP系统";
            // 
            // lbList
            // 
            this.lbList.BackColor = System.Drawing.Color.LightYellow;
            this.lbList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbList.FormattingEnabled = true;
            this.lbList.ItemHeight = 12;
            this.lbList.Location = new System.Drawing.Point(3, 53);
            this.lbList.Name = "lbList";
            this.lbList.Size = new System.Drawing.Size(369, 80);
            this.lbList.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lbList, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pbProcess, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblDescription, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(375, 161);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // lblDescription
            // 
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(3, 30);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(369, 20);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "label1";
            // 
            // frmInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightYellow;
            this.ClientSize = new System.Drawing.Size(375, 161);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "明扬ERP系统更新 2.0";
            this.Load += new System.EventHandler(this.frmInfo_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbProcess;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListBox lbList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblDescription;
    }
}