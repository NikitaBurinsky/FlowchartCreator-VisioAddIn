using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;
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
		private ComboBox cmbLanguage;
		private ComboBox cmbHistory;
		private Button btnImportFile;
		private Label lblTheme;
		private Label lblFont;
		private Label lblLanguage;
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
			
			// Detect theme automatically
			bool isOfficeDark = IsOfficeThemeDark();
			cmbTheme.SelectedIndex = isOfficeDark ? 1 : 0;
			ApplyTheme(isOfficeDark);

			// Determine initial language
			string initialLang = _settings.Language;
			if (string.IsNullOrEmpty(initialLang) || initialLang == "Auto")
			{
				bool isSystemRu = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower() == "ru";
				initialLang = isSystemRu ? "ru" : "en";
				_settings.Language = initialLang;
			}
			cmbLanguage.SelectedIndex = initialLang == "ru" ? 0 : 1; // triggers SelectedIndexChanged -> ApplyLanguage
			LoadHistoryList();
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
							if (themeVal == 0 || themeVal == 5)
								return false; // Colorful or White
							// 6 = Use System Setting (since newer Office versions)
						}
					}
				}
			}
			catch { }

			// Fallback: check Windows App theme
			try
			{
				using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
				{
					if (key != null)
					{
						object val = key.GetValue("AppsUseLightTheme");
						if (val != null && int.TryParse(val.ToString(), out int lightThemeVal))
						{
							return lightThemeVal == 0; // 0 means dark theme
						}
					}
				}
			}
			catch { }

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

			// Language label
			lblLanguage = new Label();
			lblLanguage.Text = "Язык интерфейса:";
			lblLanguage.Size = new System.Drawing.Size(120, 20);
			this.Controls.Add(lblLanguage);

			// Language combo
			cmbLanguage = new ComboBox();
			cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbLanguage.Items.AddRange(new object[] { "Русский", "English" });
			cmbLanguage.SelectedIndex = 0; // Russian
			cmbLanguage.Size = new System.Drawing.Size(120, 25);
			cmbLanguage.SelectedIndexChanged += (s, e) => {
				bool isRu = cmbLanguage.SelectedIndex == 0;
				_settings.Language = isRu ? "ru" : "en";
				ApplyLanguage(isRu);
			};
			this.Controls.Add(cmbLanguage);

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

			// Code History ComboBox
			cmbHistory = new ComboBox();
			cmbHistory.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbHistory.Location = new System.Drawing.Point(870, 8);
			cmbHistory.Size = new System.Drawing.Size(300, 25);
			cmbHistory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
			cmbHistory.SelectedIndexChanged += CmbHistory_SelectedIndexChanged;
			this.Controls.Add(cmbHistory);

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
			currentY = cmbFont.Bottom + 12;

			lblLanguage.Location = new System.Drawing.Point(22, currentY + 3);
			cmbLanguage.Location = new System.Drawing.Point(160, currentY);
			currentY = cmbLanguage.Bottom + 16;

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
				bool isRu = (cmbLanguage != null) ? cmbLanguage.SelectedIndex == 0 : true;
				SetupBeautifulHelpText(label2, isDark, isRu);
			}

			if (label3 != null) label3.ForeColor = fgColor;
			if (label4 != null) label4.ForeColor = fgColor;
			if (lblTheme != null) lblTheme.ForeColor = fgColor;
			if (lblFont != null) lblFont.ForeColor = fgColor;
			if (lblLanguage != null) lblLanguage.ForeColor = fgColor;
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
			if (cmbLanguage != null)
			{
				cmbLanguage.BackColor = controlBgColor;
				cmbLanguage.ForeColor = controlFgColor;
			}
			if (cmbHistory != null)
			{
				cmbHistory.BackColor = controlBgColor;
				cmbHistory.ForeColor = controlFgColor;
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

		private void ApplyLanguage(bool isRu)
		{
			if (textBox != null)
			{
				textBox.IsRussianLanguage = isRu;
			}

			if (isRu)
			{
				this.Text = "Конструктор Блок-Схем Visio";
				if (Generate_Btn != null) Generate_Btn.Text = "Сгенерировать";
				if (Btn_Cancel != null) Btn_Cancel.Text = "Отмена";
				if (Btn_CommandsFileOpen != null) Btn_CommandsFileOpen.Text = "Настройка команд...";
				if (chkWordWrap != null) chkWordWrap.Text = "Перенос строк";
				if (chkAutoCloseBrackets != null) chkAutoCloseBrackets.Text = "Автозакрытие скобок";
				if (lblTheme != null) lblTheme.Text = "Тема окна:";
				if (lblFont != null) lblFont.Text = "Шрифт кода:";
				if (lblLanguage != null) lblLanguage.Text = "Язык интерфейса:";
				if (btnImportFile != null) btnImportFile.Text = "Импортировать функции из файла...";
				if (lblTip != null) lblTip.Text = "💡 Подсказка: Если авто-импорт файла не определил нужные функции, вы можете просто скопировать и вставить их код вручную.";
				if (label3 != null) label3.Text = "Макс. кол-во однотипных блоков, объединяемых в один";
				if (label4 != null) label4.Text = "Макс. длина текста в блоке (остальное будет обрезано). -1 — без ограничений";
			}
			else
			{
				this.Text = "Visio Flowchart Creator";
				if (Generate_Btn != null) Generate_Btn.Text = "Generate";
				if (Btn_Cancel != null) Btn_Cancel.Text = "Cancel";
				if (Btn_CommandsFileOpen != null) Btn_CommandsFileOpen.Text = "Configure Commands...";
				if (chkWordWrap != null) chkWordWrap.Text = "Word Wrap";
				if (chkAutoCloseBrackets != null) chkAutoCloseBrackets.Text = "Auto-close Brackets";
				if (lblTheme != null) lblTheme.Text = "Window Theme:";
				if (lblFont != null) lblFont.Text = "Code Font:";
				if (lblLanguage != null) lblLanguage.Text = "Interface Language:";
				if (btnImportFile != null) btnImportFile.Text = "Import functions from file...";
				if (lblTip != null) lblTip.Text = "💡 Tip: If auto-import didn't detect the functions, you can simply copy and paste your code manually.";
				if (label3 != null) label3.Text = "Max number of similar type nodes to combine into one";
				if (label4 != null) label4.Text = "Max number of characters per block (the rest will be cut off). -1 for unlimited";
			}

			if (cmbTheme != null)
			{
				int prevThemeIdx = cmbTheme.SelectedIndex;
				cmbTheme.Items.Clear();
				if (isRu)
				{
					cmbTheme.Items.AddRange(new object[] { "Светлая", "Темная" });
				}
				else
				{
					cmbTheme.Items.AddRange(new object[] { "Light", "Dark" });
				}
				if (prevThemeIdx >= 0 && prevThemeIdx < cmbTheme.Items.Count)
				{
					cmbTheme.SelectedIndex = prevThemeIdx;
				}
				else
				{
					cmbTheme.SelectedIndex = 0;
				}
			}

			if (label3 != null && label4 != null && numericUpDown1 != null && numericUpDown2 != null)
			{
				int label4Height = Math.Max(30, label4.GetPreferredSize(new System.Drawing.Size(442, 0)).Height);
				label4.Location = new System.Drawing.Point(20, numericUpDown2.Top - label4Height - 6);

				int label3Height = Math.Max(18, label3.GetPreferredSize(new System.Drawing.Size(442, 0)).Height);
				label3.Location = new System.Drawing.Point(20, numericUpDown1.Top - label3Height - 6);
			}

			if (label2 != null && cmbTheme != null)
			{
				SetupBeautifulHelpText(label2, cmbTheme.SelectedIndex == 1, isRu);
			}

			if (cmbHistory != null && cmbHistory.Items.Count > 0)
			{
				_isLoadingHistory = true;
				try
				{
					string placeholderText = isRu ? "— История кода —" : "— Code History —";
					cmbHistory.Items[0] = placeholderText;
					if (cmbHistory.SelectedIndex == 0)
					{
						cmbHistory.Text = placeholderText;
					}
				}
				finally
				{
					_isLoadingHistory = false;
				}
			}
		}

		private void SetupBeautifulHelpText(RichTextBox rtb, bool isDark, bool isRu)
		{
			rtb.Clear();
			rtb.ReadOnly = true;
			rtb.BorderStyle = BorderStyle.None;
			rtb.BackColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(248, 248, 248);
			rtb.ForeColor = isDark ? Color.FromArgb(220, 220, 220) : Color.FromArgb(50, 50, 50);
			rtb.Font = new Font("Segoe UI", 9.5F);

			if (isRu)
			{
				AppendStyledText(rtb, "Конструктор Блок-Схем Visio\n", new Font("Segoe UI", 12F, FontStyle.Bold), isDark ? Color.FromArgb(0, 150, 255) : Color.FromArgb(0, 102, 204));
				AppendStyledText(rtb, "Создает блок-схему в Visio на основе исходного кода C/C++.\n\n", new Font("Segoe UI", 9.5F, FontStyle.Italic), isDark ? Color.FromArgb(180, 180, 180) : Color.FromArgb(100, 100, 100));

				AppendStyledText(rtb, " ⚠  ПРОЧТИТЕ ПЕРЕД ИСПОЛЬЗОВАНИЕМ! \n", new Font("Segoe UI", 10F, FontStyle.Bold), Color.FromArgb(220, 53, 69));

				AppendStyledText(rtb, "Как использовать:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
				AppendStyledText(rtb, "1. Вставьте код C/C++ функции в редактор справа.\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "2. Нажмите ", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "Сгенерировать", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
				AppendStyledText(rtb, " для построения блок-схемы в Visio.\n\n", rtb.Font, rtb.ForeColor);

				AppendStyledText(rtb, "Правила и синтаксис:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
				AppendStyledText(rtb, "• Каждая инструкция/команда должна начинаться с новой строки.\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "• Составные блоки (циклы, условия) обязательно должны использовать фигурные скобки { }.\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "  Например, пишите ", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "while(true) { }", new Font("Consolas", 9.5F), isDark ? Color.FromArgb(206, 145, 120) : Color.FromArgb(163, 21, 21));
				AppendStyledText(rtb, " вместо while(true);\n\n", rtb.Font, rtb.ForeColor);

				AppendStyledText(rtb, "Настройка ключевых слов:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
				AppendStyledText(rtb, "• Нажмите ", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "Настройка команд...", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
				AppendStyledText(rtb, " для сопоставления функций конкретным блокам в Visio (ввод, вывод, процесс, предопределенный процесс).\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "• Все нераспознанные функции будут автоматически отрисованы как обычные блоки процессов.\n", rtb.Font, rtb.ForeColor);
			}
			else
			{
				AppendStyledText(rtb, "Visio Flowchart Creator\n", new Font("Segoe UI", 12F, FontStyle.Bold), isDark ? Color.FromArgb(0, 150, 255) : Color.FromArgb(0, 102, 204));
				AppendStyledText(rtb, "Creates a Visio flowchart from C/C++ source code.\n\n", new Font("Segoe UI", 9.5F, FontStyle.Italic), isDark ? Color.FromArgb(180, 180, 180) : Color.FromArgb(100, 100, 100));

				AppendStyledText(rtb, " ⚠  READ BEFORE USING! \n", new Font("Segoe UI", 10F, FontStyle.Bold), Color.FromArgb(220, 53, 69));

				AppendStyledText(rtb, "How to use:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
				AppendStyledText(rtb, "1. Insert your C/C++ function code in the editor on the right.\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "2. Click ", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "Generate", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
				AppendStyledText(rtb, " to build the flowchart in Visio.\n\n", rtb.Font, rtb.ForeColor);

				AppendStyledText(rtb, "Rules & Syntax Guidelines:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
				AppendStyledText(rtb, "• Each instruction/command must start on a new line.\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "• Compound blocks (loops, conditionals) must use opening and closing curly braces { }.\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "  For example, write ", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "while(true) { }", new Font("Consolas", 9.5F), isDark ? Color.FromArgb(206, 145, 120) : Color.FromArgb(163, 21, 21));
				AppendStyledText(rtb, " instead of while(true);\n\n", rtb.Font, rtb.ForeColor);

				AppendStyledText(rtb, "Commands configuration:\n", new Font("Segoe UI", 10F, FontStyle.Bold), isDark ? Color.White : Color.Black);
				AppendStyledText(rtb, "• Click ", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "Configure Commands...", new Font("Segoe UI", 9.5F, FontStyle.Bold), rtb.ForeColor);
				AppendStyledText(rtb, " to map custom functions to specific Visio block types (input, output, process, subprocess).\n", rtb.Font, rtb.ForeColor);
				AppendStyledText(rtb, "• Unrecognized commands are automatically classified as standard Process blocks.\n", rtb.Font, rtb.ForeColor);
			}
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
			bool isRu = cmbLanguage.SelectedIndex == 0;
			// Simple brace validation check
			string braceWarning = StaticCodeValidator.ValidateBraces(textBox.Text);
			if (braceWarning != null)
			{
				string msg = isRu ? 
					braceWarning + "\n\nСхема может построиться некорректно или произойдет ошибка. Вы хотите продолжить генерацию?" :
					braceWarning + "\n\nThe flowchart might be generated incorrectly or an error might occur. Do you want to proceed?";
				string title = isRu ? "Предупреждение валидатора" : "Validator Warning";

				DialogResult result = MessageBox.Show(
					msg,
					title,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning
				);
				if (result == DialogResult.No)
				{
					return; // Abort generation
				}
			}

			SaveCurrentToHistory();

			MenuResult = EMenuResult.GenerateFromBuffer;
			try
			{
				File.WriteAllText(_filepath, textBox.Text);
			}
			catch (Exception ex)
			{
				string errorMsg = isRu ? "Не удалось записать буферный файл:\n" : "Failed to write temporary file:\n";
				string title = isRu ? "Ошибка" : "Error";
				MessageBox.Show(errorMsg + ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Close();
		}

		private void Btn_OpenCommandsFile_Click(object sender, EventArgs e)
		{
			bool isRu = cmbLanguage.SelectedIndex == 0;
			using (var editor = new CommandsEditorForm(_commandsJsonPath, textBox.Theme == CodeEditor.ColorTheme.Dark, isRu))
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

		private class HistoryItem
		{
			public string FilePath { get; set; }
			public string DisplayText { get; set; }
			public override string ToString() => DisplayText;
		}

		private bool _isLoadingHistory = false;

		private void LoadHistoryList(string selectFilePath = null)
		{
			if (cmbHistory == null) return;
			if (_isLoadingHistory) return;
			_isLoadingHistory = true;

			try
			{
				cmbHistory.Items.Clear();

				bool isRu = cmbLanguage.SelectedIndex == 0;
				string placeholderText = isRu ? "— История кода —" : "— Code History —";
				cmbHistory.Items.Add(placeholderText);

				string historyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlowchartCreatorAddIn", "History");
				if (Directory.Exists(historyDir))
				{
					var files = Directory.GetFiles(historyDir, "snippet_*.txt")
										 .Select(f => new FileInfo(f))
										 .OrderByDescending(f => f.LastWriteTime)
										 .Take(10)
										 .ToList();

					foreach (var file in files)
					{
						try
						{
							using (var reader = new StreamReader(file.FullName))
							{
								string title = reader.ReadLine() ?? "Untitled";
								string timeStr = file.LastWriteTime.ToString("g");
								cmbHistory.Items.Add(new HistoryItem
								{
									FilePath = file.FullName,
									DisplayText = $"{title} ({timeStr})"
								});
							}
						}
						catch { }
					}
				}

				int indexToSelect = 0;
				if (selectFilePath != null)
				{
					for (int i = 1; i < cmbHistory.Items.Count; i++)
					{
						if (cmbHistory.Items[i] is HistoryItem item && item.FilePath == selectFilePath)
						{
							indexToSelect = i;
							break;
						}
					}
				}
				cmbHistory.SelectedIndex = indexToSelect;
			}
			finally
			{
				_isLoadingHistory = false;
			}
		}

		private bool SaveCurrentToHistory()
		{
			string currentText = textBox.Text;
			if (string.IsNullOrWhiteSpace(currentText)) return false;

			string historyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlowchartCreatorAddIn", "History");
			if (!Directory.Exists(historyDir))
			{
				try { Directory.CreateDirectory(historyDir); } catch { return false; }
			}

			var files = Directory.GetFiles(historyDir, "snippet_*.txt")
								 .Select(f => new FileInfo(f))
								 .OrderByDescending(f => f.LastWriteTime)
								 .ToList();

			if (files.Count > 0)
			{
				try
				{
					string lastFileContent = File.ReadAllText(files[0].FullName);
					int firstNewline = lastFileContent.IndexOf('\n');
					string lastCode = firstNewline != -1 ? lastFileContent.Substring(firstNewline + 1) : lastFileContent;
					if (lastCode.Trim() == currentText.Trim())
					{
						return false; // Duplicate
					}
				}
				catch { }
			}

			string title = ExtractFunctionName(currentText);
			string fileName = $"snippet_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
			string filePath = Path.Combine(historyDir, fileName);
			try
			{
				File.WriteAllText(filePath, title + "\n" + currentText);

				var allFiles = Directory.GetFiles(historyDir, "snippet_*.txt")
									 .Select(f => new FileInfo(f))
									 .OrderByDescending(f => f.LastWriteTime)
									 .ToList();
				if (allFiles.Count > 10)
				{
					for (int i = 10; i < allFiles.Count; i++)
					{
						try { allFiles[i].Delete(); } catch { }
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		private string ExtractFunctionName(string code)
		{
			if (string.IsNullOrWhiteSpace(code)) return "Empty Snippet";

			var match = Regex.Match(code, @"\b(?:[a-zA-Z_]\w*(?:\s+|\s*\*+\s*))+([a-zA-Z_]\w*)\s*\([^)]*\)");
			if (match.Success)
			{
				return match.Value.Trim();
			}

			var firstLine = code.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => !string.IsNullOrEmpty(l));
			if (firstLine != null)
			{
				if (firstLine.Length > 40) return firstLine.Substring(0, 37) + "...";
				return firstLine;
			}

			return "Untitled";
		}

		private void CmbHistory_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_isLoadingHistory) return;
			if (cmbHistory.SelectedIndex <= 0) return;

			if (!(cmbHistory.SelectedItem is HistoryItem selectedItem)) return;

			bool saved = SaveCurrentToHistory();

			try
			{
				string content = File.ReadAllText(selectedItem.FilePath);
				int firstNewline = content.IndexOf('\n');
				string code = firstNewline != -1 ? content.Substring(firstNewline + 1) : content;

				textBox.Text = code;
			}
			catch (Exception ex)
			{
				bool isRu = cmbLanguage.SelectedIndex == 0;
				string errTitle = isRu ? "Ошибка загрузки" : "Load Error";
				string errMsg = isRu ? "Не удалось загрузить историю:" : "Failed to load history:";
				MessageBox.Show(errMsg + "\n" + ex.Message, errTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (saved)
			{
				LoadHistoryList(selectedItem.FilePath);
			}
		}
	}
}	
