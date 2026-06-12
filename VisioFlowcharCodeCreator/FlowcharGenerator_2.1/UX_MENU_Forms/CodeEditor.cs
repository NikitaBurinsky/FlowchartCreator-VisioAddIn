using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowchartGenerator.UX_MENU_Forms
{
	public class CodeEditor : UserControl
	{
		private RichTextBox richTextBox;
		private PictureBox lineNumbersPB;

		public enum ColorTheme { Light, Dark }
		private ColorTheme currentTheme = ColorTheme.Light;
		public bool EnableAutoCloseBrackets { get; set; } = true;

		public CodeEditor()
		{
			InitializeComponent();
			ApplyTheme();
		}

		public override string Text
		{
			get { return richTextBox.Text; }
			set { richTextBox.Text = value; ColorizeCode(); }
		}

		public int TextLength
		{
			get { return richTextBox.TextLength; }
		}

		public bool WordWrap
		{
			get { return richTextBox.WordWrap; }
			set { richTextBox.WordWrap = value; lineNumbersPB.Invalidate(); }
		}

		public ColorTheme Theme
		{
			get { return currentTheme; }
			set { currentTheme = value; ApplyTheme(); }
		}

		public string EditorFontFamily
		{
			get { return richTextBox.Font.FontFamily.Name; }
			set
			{
				try
				{
					richTextBox.Font = new Font(value, richTextBox.Font.Size);
					lineNumbersPB.Invalidate();
				}
				catch { }
			}
		}

		private void InitializeComponent()
		{
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.lineNumbersPB = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.lineNumbersPB)).BeginInit();
			this.SuspendLayout();
			
			// 
			// richTextBox
			// 
			this.richTextBox.AcceptsTab = true;
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox.Location = new System.Drawing.Point(40, 0);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
			this.richTextBox.Size = new System.Drawing.Size(658, 735);
			this.richTextBox.TabIndex = 0;
			this.richTextBox.Text = "";
			this.richTextBox.WordWrap = false;
			this.richTextBox.TextChanged += new System.EventHandler(this.RichTextBox_TextChanged);
			this.richTextBox.VScroll += new System.EventHandler(this.RichTextBox_VScroll);
			this.richTextBox.Resize += new System.EventHandler(this.RichTextBox_Resize);
			this.richTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox_KeyDown);
			this.richTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RichTextBox_KeyPress);
			this.richTextBox.HandleCreated += new System.EventHandler(this.RichTextBox_HandleCreated);
			
			// 
			// lineNumbersPB
			// 
			this.lineNumbersPB.Dock = System.Windows.Forms.DockStyle.Left;
			this.lineNumbersPB.Location = new System.Drawing.Point(0, 0);
			this.lineNumbersPB.Name = "lineNumbersPB";
			this.lineNumbersPB.Size = new System.Drawing.Size(40, 735);
			this.lineNumbersPB.TabIndex = 1;
			this.lineNumbersPB.TabStop = false;
			this.lineNumbersPB.Paint += new System.Windows.Forms.PaintEventHandler(this.LineNumbersPB_Paint);
			
			// 
			// CodeEditor
			// 
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.lineNumbersPB);
			this.Name = "CodeEditor";
			this.Size = new System.Drawing.Size(698, 735);
			((System.ComponentModel.ISupportInitialize)(this.lineNumbersPB)).EndInit();
			this.ResumeLayout(false);
		}

		private void RichTextBox_HandleCreated(object sender, EventArgs e)
		{
			// Set tab size to 4 spaces (16 dialog template units)
			int[] tabStops = { 16 };
			SendMessage(richTextBox.Handle, EM_SETTABSTOPS, 1, tabStops);
		}

		private void RichTextBox_TextChanged(object sender, EventArgs e)
		{
			lineNumbersPB.Invalidate();
			ColorizeCode();
		}

		private void RichTextBox_VScroll(object sender, EventArgs e)
		{
			lineNumbersPB.Invalidate();
		}

		private void RichTextBox_Resize(object sender, EventArgs e)
		{
			lineNumbersPB.Invalidate();
		}

		private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			// 1. Zoom with Ctrl + Plus/Minus
			if (e.Control)
			{
				if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
				{
					e.SuppressKeyPress = true;
					float newSize = richTextBox.Font.Size + 1;
					if (newSize <= 30) richTextBox.Font = new Font(richTextBox.Font.FontFamily, newSize);
					lineNumbersPB.Invalidate();
					return;
				}
				else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
				{
					e.SuppressKeyPress = true;
					float newSize = richTextBox.Font.Size - 1;
					if (newSize >= 6) richTextBox.Font = new Font(richTextBox.Font.FontFamily, newSize);
					lineNumbersPB.Invalidate();
					return;
				}
			}

			// 2. Block Indentation (Tab / Shift + Tab)
			if (e.KeyCode == Keys.Tab)
			{
				int selStart = richTextBox.SelectionStart;
				int selLen = richTextBox.SelectionLength;
				if (selLen > 0)
				{
					e.SuppressKeyPress = true;
					int startLine = richTextBox.GetLineFromCharIndex(selStart);
					int endLine = richTextBox.GetLineFromCharIndex(selStart + selLen);

					SendMessage(richTextBox.Handle, WM_SETREDRAW, false, 0);

					int deltaChars = 0;
					for (int i = startLine; i <= endLine; i++)
					{
						int lineStart = richTextBox.GetFirstCharIndexFromLine(i);
						if (lineStart == -1) continue;

						if (e.Shift)
						{
							richTextBox.Select(lineStart, 1);
							if (richTextBox.SelectedText == "\t")
							{
								richTextBox.SelectedText = "";
								deltaChars--;
							}
							else
							{
								richTextBox.Select(lineStart, 4);
								if (richTextBox.SelectedText == "    ")
								{
									richTextBox.SelectedText = "";
									deltaChars -= 4;
								}
							}
						}
						else
						{
							richTextBox.Select(lineStart, 0);
							richTextBox.SelectedText = "\t";
							deltaChars++;
						}
					}

					richTextBox.Select(selStart, selLen + deltaChars);
					SendMessage(richTextBox.Handle, WM_SETREDRAW, true, 0);
					richTextBox.Refresh();
					return;
				}
			}

			// 3. Comment / Uncomment block (Ctrl + /)
			if (e.Control && e.KeyCode == Keys.OemQuestion)
			{
				e.SuppressKeyPress = true;
				int selStart = richTextBox.SelectionStart;
				int selLen = richTextBox.SelectionLength;

				int startLine = richTextBox.GetLineFromCharIndex(selStart);
				int endLine = richTextBox.GetLineFromCharIndex(selStart + selLen);

				SendMessage(richTextBox.Handle, WM_SETREDRAW, false, 0);

				int deltaChars = 0;
				for (int i = startLine; i <= endLine; i++)
				{
					int lineStart = richTextBox.GetFirstCharIndexFromLine(i);
					if (lineStart == -1) continue;

					richTextBox.Select(lineStart, 2);
					if (richTextBox.SelectedText == "//")
					{
						richTextBox.SelectedText = "";
						deltaChars -= 2;
					}
					else
					{
						richTextBox.Select(lineStart, 0);
						richTextBox.SelectedText = "//";
						deltaChars += 2;
					}
				}

				richTextBox.Select(selStart, selLen + deltaChars);
				SendMessage(richTextBox.Handle, WM_SETREDRAW, true, 0);
				richTextBox.Refresh();
				ColorizeCode();
				return;
			}

			// 4. Auto-Indent on Enter
			if (e.KeyCode == Keys.Enter)
			{
				int index = richTextBox.SelectionStart;
				int lineIndex = richTextBox.GetLineFromCharIndex(index);
				if (lineIndex >= 0 && lineIndex < richTextBox.Lines.Length)
				{
					string line = richTextBox.Lines[lineIndex];
					int spaceCount = 0;
					while (spaceCount < line.Length && (line[spaceCount] == ' ' || line[spaceCount] == '\t'))
					{
						spaceCount++;
					}
					if (spaceCount > 0)
					{
						string indent = line.Substring(0, spaceCount);
						e.SuppressKeyPress = true;
						richTextBox.SelectedText = Environment.NewLine + indent;
					}
				}
			}
		}

		private void RichTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!EnableAutoCloseBrackets) return;

			char c = e.KeyChar;
			char closingChar = '\0';
			if (c == '{') closingChar = '}';
			else if (c == '(') closingChar = ')';
			else if (c == '[') closingChar = ']';
			else if (c == '"') closingChar = '"';
			else if (c == '\'') closingChar = '\'';

			if (closingChar != '\0')
			{
				int selStart = richTextBox.SelectionStart;
				int selLen = richTextBox.SelectionLength;
				string selText = richTextBox.SelectedText;

				e.Handled = true;
				richTextBox.SelectedText = c + selText + closingChar;
				richTextBox.SelectionStart = selStart + 1;
				richTextBox.SelectionLength = selLen;
			}
		}

		private void LineNumbersPB_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Color lineNumberBg = currentTheme == ColorTheme.Dark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(240, 240, 240);
			g.Clear(lineNumberBg);

			Point pt = new Point(0, 0);
			int firstChar = richTextBox.GetCharIndexFromPosition(pt);
			int firstLine = richTextBox.GetLineFromCharIndex(firstChar);

			Point firstLinePt = richTextBox.GetPositionFromCharIndex(firstChar);
			int y = firstLinePt.Y;

			int lineCount = richTextBox.Lines.Length;
			Font font = new Font("Consolas", 9F, FontStyle.Regular);
			Color lineNumberText = currentTheme == ColorTheme.Dark ? Color.FromArgb(140, 140, 140) : Color.FromArgb(120, 120, 120);
			Brush brush = new SolidBrush(lineNumberText);

			int currentLine = firstLine;
			while (y < richTextBox.Height && currentLine < lineCount)
			{
				string numberStr = (currentLine + 1).ToString();
				SizeF size = g.MeasureString(numberStr, font);
				float x = 35 - size.Width;
				g.DrawString(numberStr, font, brush, x, y + 2);

				currentLine++;
				int nextLineStartChar = richTextBox.GetFirstCharIndexFromLine(currentLine);
				if (nextLineStartChar == -1) break;
				Point nextLinePt = richTextBox.GetPositionFromCharIndex(nextLineStartChar);
				y = nextLinePt.Y;
			}
		}

		private void ApplyTheme()
		{
			if (currentTheme == ColorTheme.Dark)
			{
				richTextBox.BackColor = Color.FromArgb(30, 30, 30);
				richTextBox.ForeColor = Color.White;
				lineNumbersPB.BackColor = Color.FromArgb(40, 40, 40);
			}
			else
			{
				richTextBox.BackColor = Color.White;
				richTextBox.ForeColor = Color.Black;
				lineNumbersPB.BackColor = Color.FromArgb(240, 240, 240);
			}
			ColorizeCode();
			lineNumbersPB.Invalidate();
		}

		private bool isColorizing = false;
		private void ColorizeCode()
		{
			if (isColorizing) return;
			isColorizing = true;

			int selStart = richTextBox.SelectionStart;
			int selLen = richTextBox.SelectionLength;

			SendMessage(richTextBox.Handle, WM_SETREDRAW, false, 0);

			try
			{
				Color keywordColor = currentTheme == ColorTheme.Dark ? Color.FromArgb(86, 156, 214) : Color.Blue;
				Color commentColor = currentTheme == ColorTheme.Dark ? Color.FromArgb(106, 153, 85) : Color.FromArgb(0, 128, 0);
				Color stringColor = currentTheme == ColorTheme.Dark ? Color.FromArgb(214, 157, 133) : Color.FromArgb(163, 21, 21);
				Color defaultColor = currentTheme == ColorTheme.Dark ? Color.White : Color.Black;

				richTextBox.SelectAll();
				richTextBox.SelectionColor = defaultColor;

				string text = richTextBox.Text;

				string[] keywords = { "int", "float", "double", "char", "void", "if", "else", "while", "for", "switch", "case", "default", "return", "break", "continue", "struct" };
				foreach (var kw in keywords)
				{
					int index = 0;
					while ((index = text.IndexOf(kw, index)) != -1)
					{
						bool isWordStart = (index == 0 || !char.IsLetterOrDigit(text[index - 1]) && text[index - 1] != '_');
						bool isWordEnd = (index + kw.Length == text.Length || !char.IsLetterOrDigit(text[index + kw.Length]) && text[index + kw.Length] != '_');

						if (isWordStart && isWordEnd)
						{
							richTextBox.Select(index, kw.Length);
							richTextBox.SelectionColor = keywordColor;
						}
						index += kw.Length;
					}
				}

				int commentIndex = 0;
				while ((commentIndex = text.IndexOf("//", commentIndex)) != -1)
				{
					int endOfLine = text.IndexOf('\n', commentIndex);
					if (endOfLine == -1) endOfLine = text.Length;
					richTextBox.Select(commentIndex, endOfLine - commentIndex);
					richTextBox.SelectionColor = commentColor;
					commentIndex = endOfLine;
				}

				int startComment = 0;
				while ((startComment = text.IndexOf("/*", startComment)) != -1)
				{
					int endComment = text.IndexOf("*/", startComment);
					if (endComment == -1) endComment = text.Length;
					else endComment += 2;
					richTextBox.Select(startComment, endComment - startComment);
					richTextBox.SelectionColor = commentColor;
					startComment = endComment;
				}

				int strIndex = 0;
				while ((strIndex = text.IndexOf('"', strIndex)) != -1)
				{
					int nextQuote = text.IndexOf('"', strIndex + 1);
					if (nextQuote == -1) nextQuote = text.Length;
					else nextQuote += 1;
					richTextBox.Select(strIndex, nextQuote - strIndex);
					richTextBox.SelectionColor = stringColor;
					strIndex = nextQuote;
				}
			}
			finally
			{
				richTextBox.Select(selStart, selLen);
				SendMessage(richTextBox.Handle, WM_SETREDRAW, true, 0);
				richTextBox.Refresh();
				isColorizing = false;
			}
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
		
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int[] lParam);

		private const int WM_SETREDRAW = 11;
		private const int EM_SETTABSTOPS = 0x00CB;
	}
}
