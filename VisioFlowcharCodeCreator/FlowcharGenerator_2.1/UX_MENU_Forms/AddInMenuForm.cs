using System;
using System.IO;
using System.Windows.Forms;

namespace FlowchartGenerator.MENU
{
	public partial class AddInMenuForm : Form
	{	public enum EMenuResult : int { GenerateFromBuffer, GenerateFromFile, Exit};
		
		public EMenuResult MenuResult = EMenuResult.GenerateFromBuffer;

		//R
        string _filepath;
		string _commandsJsonPath;
        SettingsSystem _settings;
        public AddInMenuForm(string filepath_, string commandspath_, SettingsSystem settings)
		{
			InitializeComponent();
			_commandsJsonPath = commandspath_;
			_filepath = filepath_;
			_settings = settings;
		}

        private void AddInMenuForm_Load(object sender, EventArgs e) {}
        private void TextCodeBuffer_TextChanged(object sender, EventArgs e){}

		private void Btn_Generate_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(textBox.Text))
			{
				MessageBox.Show("Пожалуйста, вставьте код функции перед генерацией.",
					"Пустой ввод", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			StreamWriter streamWriter = new StreamWriter(_filepath);
			int gap = 300;
			int s = 0;
			for (; (s + gap) < textBox.TextLength; s += gap)
			{
				streamWriter.Write(textBox.Text.Substring(s, gap));
			}
			if (s >= 0)
				streamWriter.Write(textBox.Text.Substring(s, textBox.TextLength - s));
			streamWriter.Close();
			Close();
		}

		private void Btn_OpenCommandsFile_Click(object sender, EventArgs e)
		{
			if (!File.Exists(_commandsJsonPath))
			{
				MessageBox.Show($"Не найден файл команд:\n{_commandsJsonPath}",
					"Файл не найден", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				System.Diagnostics.Process.Start(_commandsJsonPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Не удалось открыть файл команд.\n{ex.Message}",
					"Ошибка открытия файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
