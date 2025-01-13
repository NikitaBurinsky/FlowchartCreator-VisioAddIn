namespace FlowchartGenerator.MENU
{
	partial class AddInMenuForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddInMenuForm));
			this.textBox = new System.Windows.Forms.TextBox();
			this.Generate_Btn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.Btn_CommandsFileOpen = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.Btn_Cancel = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.AcceptsTab = true;
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.textBox.Location = new System.Drawing.Point(3, 3);
			this.textBox.MaxLength = 64000;
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(698, 735);
			this.textBox.TabIndex = 1;
			this.textBox.TabStop = false;
			this.textBox.Text = resources.GetString("textBox.Text");
			this.textBox.TextChanged += new System.EventHandler(this.TextCodeBuffer_TextChanged);
			// 
			// Generate_Btn
			// 
			this.Generate_Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.Generate_Btn.Location = new System.Drawing.Point(956, 818);
			this.Generate_Btn.Name = "Generate_Btn";
			this.Generate_Btn.Size = new System.Drawing.Size(284, 62);
			this.Generate_Btn.TabIndex = 1;
			this.Generate_Btn.Text = "Generate";
			this.Generate_Btn.UseVisualStyleBackColor = true;
			this.Generate_Btn.Click += new System.EventHandler(this.Btn_Generate_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.label1.Location = new System.Drawing.Point(14, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 20);
			this.label1.TabIndex = 2;
			// 
			// Btn_CommandsFileOpen
			// 
			this.Btn_CommandsFileOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.Btn_CommandsFileOpen.Location = new System.Drawing.Point(16, 472);
			this.Btn_CommandsFileOpen.Name = "Btn_CommandsFileOpen";
			this.Btn_CommandsFileOpen.Size = new System.Drawing.Size(177, 49);
			this.Btn_CommandsFileOpen.TabIndex = 3;
			this.Btn_CommandsFileOpen.Text = "Commands file";
			this.Btn_CommandsFileOpen.UseVisualStyleBackColor = true;
			this.Btn_CommandsFileOpen.Click += new System.EventHandler(this.Btn_OpenCommandsFile_Click);
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.Menu;
			this.label2.Location = new System.Drawing.Point(20, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(495, 412);
			this.label2.TabIndex = 4;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// Btn_Cancel
			// 
			this.Btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.Btn_Cancel.Location = new System.Drawing.Point(536, 823);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new System.Drawing.Size(177, 49);
			this.Btn_Cancel.TabIndex = 5;
			this.Btn_Cancel.Text = "Cancel";
			this.Btn_Cancel.UseVisualStyleBackColor = true;
			this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(22, 757);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(120, 26);
			this.numericUpDown1.TabIndex = 6;
			this.numericUpDown1.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.MaxNodesOneTypeNum_NUD_ValueChanged);
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(22, 838);
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(120, 26);
			this.numericUpDown2.TabIndex = 7;
			this.numericUpDown2.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numericUpDown2.ValueChanged += new System.EventHandler(this.MaxFigureTextNum_NUD_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(20, 734);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(442, 20);
			this.label3.TabIndex = 8;
			this.label3.Text = "Max number of cmds with similar types, combined in one node";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(20, 795);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(442, 51);
			this.label4.TabIndex = 9;
			this.label4.Text = "Max number of letters in one figure (other will be cut off). -1 means no limits";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.textBox, 0, 0);
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(536, 42);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(704, 741);
			this.tableLayoutPanel1.TabIndex = 10;
			// 
			// AddInMenuForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(1269, 891);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.Btn_Cancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Btn_CommandsFileOpen);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Generate_Btn);
			this.Name = "AddInMenuForm";
			this.Text = "Flowchart Generator Menu";
			this.Load += new System.EventHandler(this.AddInMenuForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Button Generate_Btn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button Btn_CommandsFileOpen;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button Btn_Cancel;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}