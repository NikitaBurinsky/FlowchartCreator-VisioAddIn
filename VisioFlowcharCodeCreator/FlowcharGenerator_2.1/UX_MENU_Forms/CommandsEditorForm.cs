using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FlowchartGenerator.UX_MENU_Forms
{
	public class CommandsEditorForm : Form
	{
		private DataGridView dgvCommands;
		private Button btnAdd;
		private Button btnDelete;
		private Button btnSave;
		private Button btnCancel;
		private Label lblTitle;
		private string jsonPath;
		private bool isDarkTheme;
		private bool isRussian;

		public CommandsEditorForm(string path, bool darkTheme, bool isRussian)
		{
			this.jsonPath = path;
			this.isDarkTheme = darkTheme;
			this.isRussian = isRussian;

			InitializeComponent();
			LoadData();
		}

		private void InitializeComponent()
		{
			this.Text = isRussian ? "Настройка команд и функций" : "Configure Commands & Macros";
			this.Size = new Size(500, 550);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.StartPosition = FormStartPosition.CenterParent;
			this.MaximizeBox = false;
			this.MinimizeBox = false;

			// Colors
			Color bgColor = isDarkTheme ? Color.FromArgb(30, 30, 30) : Color.FromArgb(245, 245, 245);
			Color fgColor = isDarkTheme ? Color.FromArgb(220, 220, 220) : Color.FromArgb(30, 30, 30);
			Color controlBgColor = isDarkTheme ? Color.FromArgb(45, 45, 48) : Color.White;
			Color accentColor = Color.FromArgb(0, 122, 204);

			this.BackColor = bgColor;

			lblTitle = new Label
			{
				Text = isRussian ? "Список сопоставления функций типам блоков в Visio:" : "Mapping rules of functions to Visio block types:",
				Location = new Point(15, 15),
				Size = new Size(460, 20),
				ForeColor = fgColor,
				Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
			};

			dgvCommands = new DataGridView
			{
				Location = new Point(15, 45),
				Size = new Size(454, 380),
				BackgroundColor = controlBgColor,
				ForeColor = fgColor,
				BorderStyle = BorderStyle.FixedSingle,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				RowHeadersVisible = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				Font = new Font("Segoe UI", 9.5F)
			};

			// Setup DataGridView theme
			dgvCommands.EnableHeadersVisualStyles = false;
			dgvCommands.ColumnHeadersDefaultCellStyle.BackColor = isDarkTheme ? Color.FromArgb(50, 50, 50) : Color.FromArgb(230, 230, 230);
			dgvCommands.ColumnHeadersDefaultCellStyle.ForeColor = fgColor;
			dgvCommands.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
			dgvCommands.DefaultCellStyle.BackColor = controlBgColor;
			dgvCommands.DefaultCellStyle.ForeColor = fgColor;
			dgvCommands.DefaultCellStyle.SelectionBackColor = accentColor;
			dgvCommands.DefaultCellStyle.SelectionForeColor = Color.White;
			dgvCommands.GridColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 220, 220);

			// Columns
			var txtCol = new DataGridViewTextBoxColumn
			{
				Name = "FuncName",
				HeaderText = isRussian ? "Функция / Макрос" : "Function / Macro",
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			dgvCommands.Columns.Add(txtCol);

			var cmbCol = new DataGridViewComboBoxColumn
			{
				Name = "BlockType",
				HeaderText = isRussian ? "Тип блока в Visio" : "Visio Block Type",
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			cmbCol.Items.AddRange("INPUT", "OUTPUT", "SUBPROCESS", "PROCESS");
			dgvCommands.Columns.Add(cmbCol);

			// Buttons
			btnAdd = new Button
			{
				Text = isRussian ? "Добавить" : "Add",
				Location = new Point(15, 435),
				Size = new Size(95, 30),
				BackColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 220, 220),
				ForeColor = fgColor,
				FlatStyle = FlatStyle.Flat
			};
			btnAdd.FlatAppearance.BorderSize = 0;
			btnAdd.Click += BtnAdd_Click;

			btnDelete = new Button
			{
				Text = isRussian ? "Удалить" : "Delete",
				Location = new Point(120, 435),
				Size = new Size(95, 30),
				BackColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 220, 220),
				ForeColor = fgColor,
				FlatStyle = FlatStyle.Flat
			};
			btnDelete.FlatAppearance.BorderSize = 0;
			btnDelete.Click += BtnDelete_Click;

			btnSave = new Button
			{
				Text = isRussian ? "Сохранить" : "Save",
				Location = new Point(274, 435),
				Size = new Size(95, 30),
				BackColor = accentColor,
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold)
			};
			btnSave.FlatAppearance.BorderSize = 0;
			btnSave.Click += BtnSave_Click;

			btnCancel = new Button
			{
				Text = isRussian ? "Отмена" : "Cancel",
				Location = new Point(374, 435),
				Size = new Size(95, 30),
				BackColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(200, 200, 200),
				ForeColor = fgColor,
				FlatStyle = FlatStyle.Flat
			};
			btnCancel.FlatAppearance.BorderSize = 0;
			btnCancel.Click += (s, e) => this.Close();

			this.Controls.Add(lblTitle);
			this.Controls.Add(dgvCommands);
			this.Controls.Add(btnAdd);
			this.Controls.Add(btnDelete);
			this.Controls.Add(btnSave);
			this.Controls.Add(btnCancel);
		}

		private void LoadData()
		{
			try
			{
				if (File.Exists(jsonPath))
				{
					string json = File.ReadAllText(jsonPath);
					var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
					if (dict != null)
					{
						foreach (var kvp in dict)
						{
							dgvCommands.Rows.Add(kvp.Key, kvp.Value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string msg = isRussian ? "Ошибка чтения Commands.json:\n" : "Error reading Commands.json:\n";
				string title = isRussian ? "Ошибка" : "Error";
				MessageBox.Show(msg + ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnAdd_Click(object sender, EventArgs e)
		{
			int rowIndex = dgvCommands.Rows.Add("", "PROCESS");
			dgvCommands.ClearSelection();
			dgvCommands.Rows[rowIndex].Selected = true;
			dgvCommands.CurrentCell = dgvCommands.Rows[rowIndex].Cells[0];
			dgvCommands.BeginEdit(true);
		}

		private void BtnDelete_Click(object sender, EventArgs e)
		{
			if (dgvCommands.SelectedRows.Count > 0)
			{
				foreach (DataGridViewRow row in dgvCommands.SelectedRows)
				{
					dgvCommands.Rows.Remove(row);
				}
			}
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			// Validate and collect
			for (int i = 0; i < dgvCommands.Rows.Count; i++)
			{
				var row = dgvCommands.Rows[i];
				var keyObj = row.Cells["FuncName"].Value;
				var valObj = row.Cells["BlockType"].Value;

				string key = keyObj?.ToString().Trim();
				string val = valObj?.ToString();

				if (string.IsNullOrEmpty(key))
				{
					string emptyMsg = isRussian ? $"Строка {i + 1}: Имя функции не может быть пустым." : $"Row {i + 1}: Function name cannot be empty.";
					string title = isRussian ? "Ошибка валидации" : "Validation Error";
					MessageBox.Show(emptyMsg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				if (dict.ContainsKey(key))
				{
					string dupMsg = isRussian ? 
						$"Дублирующееся правило для функции '{key}'. Каждая функция может иметь только один тип блока." : 
						$"Duplicate mapping for function '{key}'. Each function can have only one block type.";
					string title = isRussian ? "Ошибка валидации" : "Validation Error";
					MessageBox.Show(dupMsg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				dict.Add(key, val ?? "PROCESS");
			}

			try
			{
				string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
				File.WriteAllText(jsonPath, json);
				string successMsg = isRussian ? "Настройки команд успешно сохранены!" : "Commands settings saved successfully!";
				string title = isRussian ? "Успех" : "Success";
				MessageBox.Show(successMsg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception ex)
			{
				string errMsg = isRussian ? "Ошибка сохранения файла:\n" : "Error saving file:\n";
				string title = isRussian ? "Ошибка" : "Error";
				MessageBox.Show(errMsg + ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
