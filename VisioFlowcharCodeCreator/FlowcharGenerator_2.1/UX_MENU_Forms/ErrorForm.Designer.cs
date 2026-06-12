namespace FlowchartGenerator.UX_MENU_Forms
{
	partial class ErrorForm
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

		private void InitializeComponent()
		{
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblMessage = new System.Windows.Forms.Label();
			this.lblShowDetails = new System.Windows.Forms.LinkLabel();
			this.txtTechDetails = new System.Windows.Forms.TextBox();
			this.btnCopyLog = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Location = new System.Drawing.Point(20, 20);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(48, 48);
			this.pictureBoxIcon.TabIndex = 0;
			this.pictureBoxIcon.TabStop = false;
			this.pictureBoxIcon.Image = System.Drawing.SystemIcons.Error.ToBitmap();
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(220, 53, 69);
			this.lblTitle.Location = new System.Drawing.Point(80, 20);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(320, 28);
			this.lblTitle.TabIndex = 1;
			this.lblTitle.Text = "Произошла ошибка генерации";
			// 
			// lblMessage
			// 
			this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
			this.lblMessage.Location = new System.Drawing.Point(80, 55);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(400, 75);
			this.lblMessage.TabIndex = 2;
			this.lblMessage.Text = "Описание ошибки...";
			// 
			// lblShowDetails
			// 
			this.lblShowDetails.AutoSize = true;
			this.lblShowDetails.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblShowDetails.LinkColor = System.Drawing.Color.FromArgb(0, 123, 255);
			this.lblShowDetails.Location = new System.Drawing.Point(20, 140);
			this.lblShowDetails.Name = "lblShowDetails";
			this.lblShowDetails.Size = new System.Drawing.Size(262, 20);
			this.lblShowDetails.TabIndex = 3;
			this.lblShowDetails.TabStop = true;
			this.lblShowDetails.Text = "Показать технические подробности >>";
			this.lblShowDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblShowDetails_LinkClicked);
			// 
			// txtTechDetails
			// 
			this.txtTechDetails.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
			this.txtTechDetails.Font = new System.Drawing.Font("Consolas", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTechDetails.ForeColor = System.Drawing.Color.FromArgb(108, 117, 125);
			this.txtTechDetails.Location = new System.Drawing.Point(20, 175);
			this.txtTechDetails.Multiline = true;
			this.txtTechDetails.Name = "txtTechDetails";
			this.txtTechDetails.ReadOnly = true;
			this.txtTechDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtTechDetails.Size = new System.Drawing.Size(460, 130);
			this.txtTechDetails.TabIndex = 4;
			this.txtTechDetails.Visible = false;
			// 
			// btnCopyLog
			// 
			this.btnCopyLog.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
			this.btnCopyLog.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(206, 212, 218);
			this.btnCopyLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCopyLog.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCopyLog.ForeColor = System.Drawing.Color.FromArgb(73, 80, 87);
			this.btnCopyLog.Location = new System.Drawing.Point(180, 135);
			this.btnCopyLog.Name = "btnCopyLog";
			this.btnCopyLog.Size = new System.Drawing.Size(160, 32);
			this.btnCopyLog.TabIndex = 5;
			this.btnCopyLog.Text = "Копировать лог";
			this.btnCopyLog.UseVisualStyleBackColor = false;
			this.btnCopyLog.Click += new System.EventHandler(this.BtnCopyLog_Click);
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
			this.btnClose.FlatAppearance.BorderSize = 0;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClose.ForeColor = System.Drawing.Color.White;
			this.btnClose.Location = new System.Drawing.Point(360, 135);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(120, 32);
			this.btnClose.TabIndex = 6;
			this.btnClose.Text = "Закрыть";
			this.btnClose.UseVisualStyleBackColor = false;
			this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
			// 
			// ErrorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(500, 190);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnCopyLog);
			this.Controls.Add(this.txtTechDetails);
			this.Controls.Add(this.lblShowDetails);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.pictureBoxIcon);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Внимание";
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.LinkLabel lblShowDetails;
		private System.Windows.Forms.TextBox txtTechDetails;
		private System.Windows.Forms.Button btnCopyLog;
		private System.Windows.Forms.Button btnClose;
	}
}
