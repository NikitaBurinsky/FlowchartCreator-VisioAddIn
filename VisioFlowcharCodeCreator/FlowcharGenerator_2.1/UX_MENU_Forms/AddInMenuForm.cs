using System;
using System.IO;
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
		}

		private void InitializeCustomSettingsControls()
		{
			// Word Wrap
			chkWordWrap = new CheckBox();
			chkWordWrap.Text = "Перенос строк";
			chkWordWrap.Location = new System.Drawing.Point(22, 530);
			chkWordWrap.Size = new System.Drawing.Size(200, 24);
			chkWordWrap.Checked = textBox.WordWrap;
			chkWordWrap.CheckedChanged += (s, e) => { textBox.WordWrap = chkWordWrap.Checked; };
			this.Controls.Add(chkWordWrap);

			// Auto Close Brackets
			chkAutoCloseBrackets = new CheckBox();
			chkAutoCloseBrackets.Text = "Автозакрытие скобок";
			chkAutoCloseBrackets.Location = new System.Drawing.Point(22, 560);
			chkAutoCloseBrackets.Size = new System.Drawing.Size(200, 24);
			chkAutoCloseBrackets.Checked = textBox.EnableAutoCloseBrackets;
			chkAutoCloseBrackets.CheckedChanged += (s, e) => { textBox.EnableAutoCloseBrackets = chkAutoCloseBrackets.Checked; };
			this.Controls.Add(chkAutoCloseBrackets);

			// Theme label
			Label lblTheme = new Label();
			lblTheme.Text = "Тема редактора:";
			lblTheme.Location = new System.Drawing.Point(22, 595);
			lblTheme.Size = new System.Drawing.Size(120, 20);
			this.Controls.Add(lblTheme);

			// Theme combo
			cmbTheme = new ComboBox();
			cmbTheme.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbTheme.Items.AddRange(new object[] { "Светлая", "Темная" });
			cmbTheme.SelectedIndex = 0; // Light
			cmbTheme.Location = new System.Drawing.Point(160, 592);
			cmbTheme.Size = new System.Drawing.Size(120, 25);
			cmbTheme.SelectedIndexChanged += (s, e) => {
				textBox.Theme = cmbTheme.SelectedIndex == 1 ? CodeEditor.ColorTheme.Dark : CodeEditor.ColorTheme.Light;
			};
			this.Controls.Add(cmbTheme);

			// Font label
			Label lblFont = new Label();
			lblFont.Text = "Шрифт кода:";
			lblFont.Location = new System.Drawing.Point(22, 630);
			lblFont.Size = new System.Drawing.Size(120, 20);
			this.Controls.Add(lblFont);

			// Font combo
			cmbFont = new ComboBox();
			cmbFont.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbFont.Items.AddRange(new object[] { "Consolas", "Courier New" });
			cmbFont.SelectedIndex = 0; // Consolas
			cmbFont.Location = new System.Drawing.Point(160, 627);
			cmbFont.Size = new System.Drawing.Size(120, 25);
			cmbFont.SelectedIndexChanged += (s, e) => {
				textBox.EditorFontFamily = cmbFont.SelectedItem.ToString();
			};
			this.Controls.Add(cmbFont);
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
			//TEST
			System.Diagnostics.Process.Start(_commandsJsonPath);
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
