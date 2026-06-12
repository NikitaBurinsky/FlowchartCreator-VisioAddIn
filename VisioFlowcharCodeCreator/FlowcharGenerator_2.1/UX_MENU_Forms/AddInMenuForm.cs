using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using FlowchartGenerator.UX_MENU_Forms;

namespace FlowchartGenerator.MENU
{
	public partial class AddInMenuForm : Form
	{	public enum EMenuResult : int { GenerateFromBuffer, GenerateFromFile, Exit};
		
		public EMenuResult MenuResult { get; private set; } = EMenuResult.Exit;

		//R
        string _filepath;
		string _commandsJsonPath;
        SettingsSystem _settings;
		private CheckBox chkWordWrap;
		private CheckBox chkAutoCloseBrackets;
		private ComboBox cmbTheme;
		private ComboBox cmbFont;
		private Button btnImportFile;
		private Label lblTheme;
		private Label lblFont;
		private Label lblTip;

        public AddInMenuForm(string filepath_, string commandspath_, SettingsSystem settings)
		{
			InitializeComponent();
			_commandsJsonPath = commandspath_;
			_filepath = filepath_;
			_settings = settings;
		}

        private void AddInMenuForm_Load(object sender, EventArgs e)
		{
			InitializeCustomSettingsControls();
			bool isOfficeDark = IsOfficeThemeDark();
			cmbTheme.SelectedIndex = isOfficeDark ? 1 : 0;
			ApplyTheme(isOfficeDark);
		}

		private bool IsOfficeThemeDark()
		{
			try
			{
				using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Office\16.0\Common"))
				{
					if (key != null)
					{
						object val = key.GetValue("Theme");
						if (val != null && int.TryParse(val.ToString(), out int themeVal))
						{
							// 3 = Dark Gray, 4 = Black
							if (themeVal == 3 || themeVal == 4)
								return true;
						}
					}
				}
			}
			catch
			{
				// Fallback
			}
			return false;
		}

		private void InitializeCustomSettingsControls()
		{
			// Word Wrap
			chkWordWrap = new CheckBox();
			chkWordWrap.Text = "Перенос строк";
			chkWordWrap.Size = new System.Drawing.Size(200, 24);
			chkWordWrap.Checked = textBox.WordWrap;
			chkWordWrap.CheckedChanged += (s, e) => { textBox.WordWrap = chkWordWrap.Checked; };
			this.Controls.Add(chkWordWrap);

			// Auto Close Brackets
			chkAutoCloseBrackets = new CheckBox();
			chkAutoCloseBrackets.Text = "Автозакрытие скобок";
			chkAutoCloseBrackets.Size = new System.Drawing.Size(200, 24);
			chkAutoCloseBrackets.Checked = textBox.EnableAutoCloseBrackets;
			chkAutoCloseBrackets.CheckedChanged += (s, e) => { textBox.EnableAutoCloseBrackets = chkAutoCloseBrackets.Checked; };
			this.Controls.Add(chkAutoCloseBrackets);

			// Theme label
			lblTheme = new Label();
			lblTheme.Text = "Тема окна:";
			lblTheme.Size = new System.Drawing.Size(120, 20);
			this.Controls.Add(lblTheme);

			// Theme combo
			cmbTheme = new ComboBox();
			cmbTheme.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbTheme.Items.AddRange(new object[] { "Светлая", "Темная" });
			cmbTheme.SelectedIndex = 0; // Light
			cmbTheme.Size = new System.Drawing.Size(120, 25);
			cmbTheme.SelectedIndexChanged += (s, e) => {
				ApplyTheme(cmbTheme.SelectedIndex == 1);
			};
			this.Controls.Add(cmbTheme);

			// Font label
			lblFont = new Label();
			lblFont.Text = "Шрифт кода:";
			lblFont.Size = new System.Drawing.Size(120, 20);
			this.Controls.Add(lblFont);

			// Font combo
			cmbFont = new ComboBox();
			cmbFont.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbFont.Items.AddRange(new object[] { "Consolas", "Courier New" });
			cmbFont.SelectedIndex = 0; // Consolas
			cmbFont.Size = new System.Drawing.Size(120, 25);
			cmbFont.SelectedIndexChanged += (s, e) => {
				textBox.EditorFontFamily = cmbFont.SelectedItem.ToString();
			};
			this.Controls.Add(cmbFont);

			// Rename Commands file open button
			Btn_CommandsFileOpen.Text = "Настройка команд...";

			// Import File Button
			btnImportFile = new Button();
			btnImportFile.Text = "Импортировать функции из файла...";
			btnImportFile.Location = new System.Drawing.Point(536, 8);
			btnImportFile.Size = new System.Drawing.Size(320, 28);
			btnImportFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
			btnImportFile.FlatStyle = FlatStyle.Flat;
			btnImportFile.FlatAppearance.BorderSize = 1;
			btnImportFile.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
			btnImportFile.BackColor = Color.FromArgb(0, 122, 204);
			btnImportFile.ForeColor = Color.White;
			btnImportFile.UseVisualStyleBackColor = false;
			btnImportFile.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			btnImportFile.Click += (s, e) =>
			{
				using (OpenFileDialog ofd = new OpenFileDialog())
				{
					ofd.Filter = "C/C++/Text Files (*.c;*.cpp;*.h;*.hpp;*.txt)|*.c;*.cpp;*.h;*.hpp;*.txt|All Files (*.*)|*.*";
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						textBox.LoadFile(ofd.FileName);
					}
				}
			};
			this.Controls.Add(btnImportFile);

			// Help/Tip Label
			lblTip = new Label();
			lblTip.Text = "💡 Подсказка: Если авто-импорт файла не определил нужные функции, вы можете просто скопировать и вставить их код вручную.";
			lblTip.AutoSize = true;
			lblTip.MaximumSize = new System.Drawing.Size(440, 0);
			lblTip.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
			lblTip.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
			this.Controls.Add(lblTip);

			// Dynamic vertical positioning to avoid any overlaps or DPI clipping
			int currentY = Btn_CommandsFileOpen.Bottom + 16;

			chkWordWrap.Location = new System.Drawing.Point(22, currentY);
			currentY = chkWordWrap.Bottom + 8;

			chkAutoCloseBrackets.Location = new System.Drawing.Point(22, currentY);
			currentY = chkAutoCloseBrackets.Bottom + 12;

			lblTheme.Location = new System.Drawing.Point(22, currentY + 3);
			cmbTheme.Location = new System.Drawing.Point(160, currentY);
			currentY = cmbTheme.Bottom + 12;

			lblFont.Location = new System.Drawing.Point(22, currentY + 3);
			cmbFont.Location = new System.Drawing.Point(160, currentY);
			currentY = cmbFont.Bottom + 16;

			lblTip.Location = new System.Drawing.Point(22, currentY);

			// Re-layout configuration controls at the bottom-left and anchor them
			int H = this.ClientSize.Height;

			numericUpDown2.Location = new System.Drawing.Point(22, H - 26 - 20);
			numericUpDown2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

			int label4Height = Math.Max(30, label4.GetPreferredSize(new System.Drawing.Size(442, 0)).Height);
			label4.Location = new System.Drawing.Point(20, numericUpDown2.Top - label4Height - 6);
			label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

			numericUpDown1.Location = new System.Drawing.Point(22, label4.Top - 26 - 15);
			numericUpDown1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

			int label3Height = Math.Max(18, label3.GetPreferredSize(new System.Drawing.Size(442, 0)).Height);
			label3.Location = new System.Drawing.Point(20, numericUpDown1.Top - label3Height - 6);
			label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

			// Position execution buttons programmatically and anchor them to bottom-right
			Generate_Btn.Location = new System.Drawing.Point(this.ClientSize.Width - Generate_Btn.Width - 20, this.ClientSize.Height - Generate_Btn.Height - 20);
			Generate_Btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			Btn_Cancel.Location = new System.Drawing.Point(Generate_Btn.Left - Btn_Cancel.Width - 15, this.ClientSize.Height - Btn_Cancel.Height - 20);
			Btn_Cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		}

		private void ApplyTheme(bool isDark)
		{
			Color bgColor = isDark ? Color.FromArgb(30, 30, 30) : Color.FromArgb(240, 240, 240);
			Color fgColor = isDark ? Color.FromArgb(220, 220, 220) : Color.Black;
			Color controlBgColor = isDark ? Color.FromArgb(45, 45, 48) : Color.White;
			Color controlFgColor = isDark ? Color.FromArgb(240, 240, 240) : Color.Black;

			this.BackColor = bgColor;
			this.ForeColor = fgColor;

			if (textBox != null)
			{
				textBox.Theme = isDark ? CodeEditor.ColorTheme.Dark : CodeEditor.ColorTheme.Light;
			}

			if (label2 != null)
			{
				SetupBeautifulHelpText(label2, isDark);
			}

			if (label3 != null) label3.ForeColor = fgColor;
			if (label4 != null) label4.ForeColor = fgColor;
			if (lblTheme != null) lblTheme.ForeColor = fgColor;
			if (lblFont != null) lblFont.ForeColor = fgColor;
			if (lblTip != null) lblTip.ForeColor = isDark ? Color.FromArgb(170, 170, 170) : Color.FromArgb(100, 100, 100);

			if (chkWordWrap != null) chkWordWrap.ForeColor = fgColor;
			if (chkAutoCloseBrackets != null) chkAutoCloseBrackets.ForeColor = fgColor;

			if (numericUpDown1 != null)
			{
				numericUpDown1.BackColor = controlBgColor;
				numericUpDown1.ForeColor = controlFgColor;
			}
			if (numericUpDown2 != null)
			{
				numericUpDown2.BackColor = controlBgColor;
				numericUpDown2.ForeColor = controlFgColor;
			}
			if (cmbTheme != null)
			{
				cmbTheme.BackColor = controlBgColor;
				cmbTheme.ForeColor = controlFgColor;
			}
			if (cmbFont != null)
			{
				cmbFont.BackColor = controlBgColor;
				cmbFont.ForeColor = controlFgColor;
			}

			ApplyButtonTheme(Generate_Btn, isDark, true);
			ApplyButtonTheme(Btn_Cancel, isDark, false);
			ApplyButtonTheme(Btn_CommandsFileOpen, isDark, false);
			ApplyButtonTheme(btnImportFile, isDark, true);
		}

		private void ApplyButtonTheme(Button btn, bool isDark, bool isPrimary)
		{
			if (btn == null) return;
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderSize = isDark ? 0 : 1;

			if (isPrimary)
			{
				btn.BackColor = Color.FromArgb(0, 122, 204);
				btn.ForeColor = Color.White;
				btn.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
			}
			else
			{
				btn.BackColor = isDark ? Color.FromArgb(50, 50, 52) : Color.FromArgb(225, 225, 225);
				btn.ForeColor = isDark ? Color.FromArgb(230, 230, 230) : Color.Black;
				btn.FlatAppearance.BorderColor = isDark ? Color.FromArgb(70, 70, 70) : Color.FromArgb(180, 180, 180);
			}
		}

		private void SetupBeautifulHelpText(RichTextBox rtb, bool isDark)
		{
			rtb.Clear();
			rtb.ReadOnly = true;
			rtb.BorderStyle = BorderStyle.None;
			rtb.BackColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(248, 248, 248);
			rtb.ForeColor = isDark ? Color.FromArgb(220, 220, 220) : Color.FromArgb(50, 50, 50);
			rtb.Font = new Font("Segoe UI", 9.5F);

			AppendStyledText(rtb, "Flowchart Generator Add-In\n", new Font("Segoe UI", 12F, FontStyle.Bold), isDark ? Color.FromArgb(0, 150, 255) : Color.FromArgb(0, 102, 204));
			AppendStyledText(rtb, "Creates a Visio flowchart from C/C++ source code.\n\n", new Font("Segoe UI", 9.5F, FontStyle.Italic), isDark ? Color.FromArgb(180, 180, 180) : Color.FromArgb(100, 100, 100));

			AppendStyledText(rtb, " ⚠  READ BEFORE USING! \n", new Font("Segoe UI", 10F, FontStyle.Bold), Color.FromArgb(220, 53, 69));
			
			AppendStyledText(rtb, "How to use:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
			AppendStyledText(rtb, "1. Insert the C/C++ function code in the editor on the right.\n", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "2. Click ", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "Generate", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
			AppendStyledText(rtb, " to build the flowchart in Visio.\n\n", rtb.Font, rtb.ForeColor);

			AppendStyledText(rtb, "Rules & Syntax Guidelines:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
			AppendStyledText(rtb, "• Each instruction/command must start on a new line.\n", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "• Compound block commands (like loops/conditionals) must use opening and closing braces { }.\n", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "  For example, use ", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "while(true) { }", new Font("Consolas", 9.5F), isDark ? Color.FromArgb(206, 145, 120) : Color.FromArgb(163, 21, 21));
			AppendStyledText(rtb, " instead of while(true);\n\n", rtb.Font, rtb.ForeColor);

			AppendStyledText(rtb, "Commands configuration:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
			AppendStyledText(rtb, "• Recognized commands are listed in the ", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "Commands.xlsx", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
			AppendStyledText(rtb, " file.\n", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "• Unrecognized commands are classified as standard Process or Subprocess blocks.\n", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "• To add custom keywords, click ", rtb.Font, rtb.ForeColor);
			AppendStyledText(rtb, "Настройка команд...", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
			AppendStyledText(rtb, " and define the type (OUT, STARTEND, IN).\n", rtb.Font, rtb.ForeColor);
		}

		private void AppendStyledText(RichTextBox rtb, string text, Font font, Color color)
		{
			int start = rtb.TextLength;
			rtb.AppendText(text);
			int end = rtb.TextLength;

			rtb.Select(start, end - start);
			rtb.SelectionFont = font;
			rtb.SelectionColor = color;
			rtb.SelectionLength = 0;
		}

        private void TextCodeBuffer_TextChanged(object sender, EventArgs e){}

		private void Btn_Generate_Click(object sender, EventArgs e)
		{
			// Simple brace validation check
			string braceWarning = StaticCodeValidator.ValidateBraces(textBox.Text);
			if (braceWarning != null)
			{
				DialogResult result = MessageBox.Show(
					braceWarning + "\n\nСхема может построиться некорректно или произойдет ошибка. Вы хотите продолжить генерацию?",
					"Предупреждение валидатора",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning
				);
				if (result == DialogResult.No)
				{
					return; // Abort generation
				}
			}

			MenuResult = EMenuResult.GenerateFromBuffer;
			try
			{
				File.WriteAllText(_filepath, textBox.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось записать буферный файл:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Close();
		}

		private void Btn_OpenCommandsFile_Click(object sender, EventArgs e)
		{
			using (var editor = new CommandsEditorForm(_commandsJsonPath, textBox.Theme == CodeEditor.ColorTheme.Dark))
			{
				editor.ShowDialog(this);
			}
		}

		private void Btn_Cancel_Click(object sender, EventArgs e)
		{
			MenuResult = EMenuResult.Exit;
			Close();
		}

		private void MaxNodesOneTypeNum_NUD_ValueChanged(object sender, EventArgs e)
		{
			_settings.MaxCombinedNodesOneType = System.Convert.ToInt32(numericUpDown1.Value);
			if(_settings.MaxCombinedNodesOneType < 1)
			{
				_settings.MaxCombinedNodesOneType = 1;
				numericUpDown1.Value = 1;
			}
		}

		private void MaxFigureTextNum_NUD_ValueChanged(object sender, EventArgs e)
		{
			_settings.FigureTextMaxSize = System.Convert.ToInt32(numericUpDown2.Value);
			if (_settings.FigureTextMaxSize < 1)
			{
				_settings.FigureTextMaxSize = 1;
				numericUpDown2.Value = 1;
			}
		}
	}
}	
