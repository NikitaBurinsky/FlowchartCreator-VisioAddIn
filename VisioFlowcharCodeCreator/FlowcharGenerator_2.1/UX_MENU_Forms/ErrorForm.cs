using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowchartGenerator.UX_MENU_Forms
{
	public partial class ErrorForm : Form
	{
		private string technicalDetails;

		public ErrorForm(string userFriendlyMessage, string technicalDetails)
		{
			InitializeComponent();
			this.lblMessage.Text = userFriendlyMessage;
			this.technicalDetails = technicalDetails;
			this.txtTechDetails.Text = technicalDetails;
		}

		private void BtnCopyLog_Click(object sender, EventArgs e)
		{
			try
			{
				Clipboard.SetText(technicalDetails);
				MessageBox.Show("Лог ошибки успешно скопирован в буфер обмена.", "Копирование", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось скопировать лог: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void LblShowDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (txtTechDetails.Visible)
			{
				txtTechDetails.Visible = false;
				lblShowDetails.Text = "Показать технические подробности >>";
				this.Height -= 150;
			}
			else
			{
				txtTechDetails.Visible = true;
				lblShowDetails.Text = "<< Скрыть технические подробности";
				this.Height += 150;
			}
		}
	}
}
