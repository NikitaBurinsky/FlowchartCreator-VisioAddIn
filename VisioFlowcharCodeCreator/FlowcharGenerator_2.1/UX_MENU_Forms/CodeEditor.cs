using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FlowchartGenerator.UX_MENU_Forms
{
	public class CodeEditor : UserControl
	{
		private RichTextBox richTextBox;
		private PictureBox lineNumbersPB;
		private Panel statusBarPanel;
		private Label lblPosition;
		private Label lblValidationStatus;
		private ToolTip validationToolTip;

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
			this.richTextBox.AllowDrop = true;
			this.richTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.RichTextBox_DragEnter);
			this.richTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.RichTextBox_DragDrop);
			this.richTextBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.RichTextBox_MouseWheel);
			
			this.richTextBox.SelectionChanged += new System.EventHandler(this.RichTextBox_SelectionChanged);
			
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
			// statusBarPanel
			// 
			this.statusBarPanel = new System.Windows.Forms.Panel();
			this.lblPosition = new System.Windows.Forms.Label();
			this.lblValidationStatus = new System.Windows.Forms.Label();
			this.validationToolTip = new System.Windows.Forms.ToolTip();

			this.statusBarPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.statusBarPanel.Height = 25;
			this.statusBarPanel.Name = "statusBarPanel";
			this.statusBarPanel.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
			
			// 
			// lblValidationStatus
			// 
			this.lblValidationStatus.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblValidationStatus.Name = "lblValidationStatus";
			this.lblValidationStatus.AutoSize = true;
			this.lblValidationStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblValidationStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
			
			// 
			// lblPosition
			// 
			this.lblPosition.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.AutoSize = true;
			this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblPosition.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
			
			this.statusBarPanel.Controls.Add(this.lblValidationStatus);
			this.statusBarPanel.Controls.Add(this.lblPosition);
			
			// 
			// CodeEditor
			// 
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.lineNumbersPB);
			this.Controls.Add(this.statusBarPanel);
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
			UpdateValidationStatus();
			UpdateCursorPosition();
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

		private void RichTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void RichTextBox_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files != null && files.Length > 0)
			{
				LoadFile(files[0]);
			}
		}

		private void RichTextBox_MouseWheel(object sender, MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Control)
			{
				float newSize = richTextBox.Font.Size + (e.Delta > 0 ? 1 : -1);
				if (newSize >= 6 && newSize <= 30)
				{
					richTextBox.Font = new Font(richTextBox.Font.FontFamily, newSize);
					lineNumbersPB.Invalidate();
				}
				((HandledMouseEventArgs)e).Handled = true;
			}
		}

		public void LoadFile(string filePath)
		{
			try
			{
				string content = File.ReadAllText(filePath);
				var functions = CFunctionExtractor.ExtractFunctions(content);
				if (functions.Count > 1)
				{
					using (var selector = new FunctionSelectForm(functions, currentTheme == ColorTheme.Dark))
					{
						if (selector.ShowDialog(this) == DialogResult.OK && selector.SelectedFunction != null)
						{
							this.Text = selector.SelectedFunction.Body;
						}
					}
				}
				else if (functions.Count == 1)
				{
					this.Text = functions[0].Body;
				}
				else
				{
					this.Text = content;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка чтения файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			Color statusBg, statusFg;
			if (currentTheme == ColorTheme.Dark)
			{
				richTextBox.BackColor = Color.FromArgb(30, 30, 30);
				richTextBox.ForeColor = Color.White;
				lineNumbersPB.BackColor = Color.FromArgb(40, 40, 40);
				statusBg = Color.FromArgb(45, 45, 48);
				statusFg = Color.FromArgb(200, 200, 200);
			}
			else
			{
				richTextBox.BackColor = Color.White;
				richTextBox.ForeColor = Color.Black;
				lineNumbersPB.BackColor = Color.FromArgb(240, 240, 240);
				statusBg = Color.FromArgb(225, 225, 225);
				statusFg = Color.FromArgb(60, 60, 60);
			}

			if (statusBarPanel != null)
			{
				statusBarPanel.BackColor = statusBg;
				lblPosition.ForeColor = statusFg;
			}

			ColorizeCode();
			UpdateValidationStatus();
			UpdateCursorPosition();
			lineNumbersPB.Invalidate();
		}

		private void RichTextBox_SelectionChanged(object sender, EventArgs e)
		{
			UpdateCursorPosition();
		}

		private void UpdateCursorPosition()
		{
			if (lblPosition == null) return;
			int index = richTextBox.SelectionStart;
			int line = richTextBox.GetLineFromCharIndex(index);
			int lineStart = richTextBox.GetFirstCharIndexFromLine(line);
			int col = index - lineStart;
			lblPosition.Text = $"Стр {line + 1}, Кол {col + 1}";
		}

		private void UpdateValidationStatus()
		{
			if (lblValidationStatus == null) return;

			string validationError = StaticCodeValidator.ValidateBraces(richTextBox.Text);
			if (string.IsNullOrEmpty(validationError))
			{
				lblValidationStatus.Text = "✔ Скобки сбалансированы";
				lblValidationStatus.ForeColor = currentTheme == ColorTheme.Dark ? Color.FromArgb(106, 153, 85) : Color.Green;
				validationToolTip.SetToolTip(lblValidationStatus, "Код прошел базовую проверку скобок.");
			}
			else
			{
				lblValidationStatus.Text = "⚠ Ошибка согласования скобок!";
				lblValidationStatus.ForeColor = currentTheme == ColorTheme.Dark ? Color.FromArgb(244, 75, 75) : Color.Red;
				validationToolTip.SetToolTip(lblValidationStatus, validationError);
			}
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

	public class CFunctionExtractor
	{
		private static readonly HashSet<string> ReservedKeywords = new HashSet<string>(StringComparer.Ordinal)
		{
			"if", "for", "while", "switch", "catch", "return", "sizeof", "struct", "class", "union", "enum", "typedef", "template"
		};

		public class CFunction
		{
			public string Name { get; set; }
			public string Signature { get; set; }
			public string Body { get; set; }
			public int StartIndex { get; set; }
			public int EndIndex { get; set; }
		}

		public static List<CFunction> ExtractFunctions(string code)
		{
			var functions = new List<CFunction>();
			var regex = new Regex(
				@"\b((?:const\s+)?(?:unsigned\s+|signed\s+|struct\s+|class\s+)?(?:[a-zA-Z_][a-zA-Z0-9_:]*)\s*[\*\&]*)\s+([a-zA-Z_][a-zA-Z0-9_:]*)\s*\([^)]*\)\s*\{",
				RegexOptions.Compiled);

			var matches = regex.Matches(code);
			foreach (Match match in matches)
			{
				if (IsIndexInsideCommentOrString(code, match.Index))
					continue;

				string funcName = match.Groups[2].Value;
				if (ReservedKeywords.Contains(funcName))
					continue;
				int openBraceIndex = match.Index + match.Length - 1; // Index of '{'
				
				// Find matching closing brace
				int closeBraceIndex = FindMatchingBrace(code, openBraceIndex);
				if (closeBraceIndex != -1)
				{
					string body = code.Substring(match.Index, closeBraceIndex - match.Index + 1);
					functions.Add(new CFunction
					{
						Name = funcName,
						Signature = match.Value,
						Body = body,
						StartIndex = match.Index,
						EndIndex = closeBraceIndex
					});
				}
			}
			return functions;
		}

		private static bool IsIndexInsideCommentOrString(string code, int index)
		{
			bool inString = false;
			bool inChar = false;
			bool inSingleComment = false;
			bool inMultiComment = false;

			for (int i = 0; i < index && i < code.Length; i++)
			{
				char c = code[i];

				if (inSingleComment)
				{
					if (c == '\n' || c == '\r') inSingleComment = false;
					continue;
				}
				if (inMultiComment)
				{
					if (c == '/' && i > 0 && code[i - 1] == '*') inMultiComment = false;
					continue;
				}
				if (inString)
				{
					if (c == '"' && code[i - 1] != '\\') inString = false;
					continue;
				}
				if (inChar)
				{
					if (c == '\'' && code[i - 1] != '\\') inChar = false;
					continue;
				}

				if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
				{
					inSingleComment = true;
					i++;
					continue;
				}
				if (c == '/' && i + 1 < code.Length && code[i + 1] == '*')
				{
					inMultiComment = true;
					i++;
					continue;
				}
				if (c == '"')
				{
					inString = true;
					continue;
				}
				if (c == '\'')
				{
					inChar = true;
					continue;
				}
			}
			return inString || inChar || inSingleComment || inMultiComment;
		}

		private static int FindMatchingBrace(string code, int openBraceIndex)
		{
			int depth = 1;
			bool inString = false;
			bool inChar = false;
			bool inSingleComment = false;
			bool inMultiComment = false;

			for (int i = openBraceIndex + 1; i < code.Length; i++)
			{
				char c = code[i];

				if (inSingleComment)
				{
					if (c == '\n' || c == '\r')
					{
						inSingleComment = false;
					}
					continue;
				}

				if (inMultiComment)
				{
					if (c == '/' && code[i - 1] == '*')
					{
						inMultiComment = false;
					}
					continue;
				}

				if (inString)
				{
					if (c == '"' && code[i - 1] != '\\')
					{
						inString = false;
					}
					continue;
				}

				if (inChar)
				{
					if (c == '\'' && code[i - 1] != '\\')
					{
						inChar = false;
					}
					continue;
				}

				// Check comments
				if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
				{
					inSingleComment = true;
					i++;
					continue;
				}
				if (c == '/' && i + 1 < code.Length && code[i + 1] == '*')
				{
					inMultiComment = true;
					i++;
					continue;
				}

				// Check strings
				if (c == '"')
				{
					inString = true;
					continue;
				}
				if (c == '\'')
				{
					inChar = true;
					continue;
				}

				// Check braces
				if (c == '{')
				{
					depth++;
				}
				else if (c == '}')
				{
					depth--;
					if (depth == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}
	}

	public class FunctionSelectForm : Form
	{
		private ListBox lstFunctions;
		private Button btnOk;
		private Button btnCancel;
		private Label lblTitle;
		
		public CFunctionExtractor.CFunction SelectedFunction { get; private set; }

		public FunctionSelectForm(List<CFunctionExtractor.CFunction> functions, bool isDarkTheme)
		{
			this.Text = "Выбор функции";
			this.Size = new Size(500, 400);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.StartPosition = FormStartPosition.CenterParent;
			this.MaximizeBox = false;
			this.MinimizeBox = false;

			// Set colors based on theme
			Color bgColor = isDarkTheme ? Color.FromArgb(30, 30, 30) : Color.FromArgb(245, 245, 245);
			Color fgColor = isDarkTheme ? Color.FromArgb(220, 220, 220) : Color.FromArgb(30, 30, 30);
			Color accentColor = Color.FromArgb(0, 122, 204); // Blue Accent
			Color controlBgColor = isDarkTheme ? Color.FromArgb(45, 45, 48) : Color.White;

			this.BackColor = bgColor;

			lblTitle = new Label
			{
				Text = "Выберите функцию для импорта:",
				Location = new Point(15, 15),
				Size = new Size(460, 20),
				ForeColor = fgColor,
				Font = new Font("Segoe UI", 10F, FontStyle.Bold)
			};

			lstFunctions = new ListBox
			{
				Location = new Point(15, 45),
				Size = new Size(454, 250),
				BackColor = controlBgColor,
				ForeColor = fgColor,
				BorderStyle = BorderStyle.FixedSingle,
				Font = new Font("Consolas", 10F),
				ItemHeight = 22
			};

			foreach (var func in functions)
			{
				lstFunctions.Items.Add(func.Name + "()");
			}
			if (lstFunctions.Items.Count > 0)
			{
				lstFunctions.SelectedIndex = 0;
			}

			btnOk = new Button
			{
				Text = "Выбрать",
				Location = new Point(279, 315),
				Size = new Size(90, 30),
				BackColor = accentColor,
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold)
			};
			btnOk.FlatAppearance.BorderSize = 0;
			btnOk.Click += (s, e) =>
			{
				if (lstFunctions.SelectedIndex >= 0)
				{
					SelectedFunction = functions[lstFunctions.SelectedIndex];
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
			};

			btnCancel = new Button
			{
				Text = "Отмена",
				Location = new Point(379, 315),
				Size = new Size(90, 30),
				BackColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(200, 200, 200),
				ForeColor = fgColor,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F)
			};
			btnCancel.FlatAppearance.BorderSize = 0;
			btnCancel.Click += (s, e) =>
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			};

			Label lblTip = new Label
			{
				Text = "* Если нужной функции нет в списке, импортируйте её код вручную.",
				Location = new Point(15, 298),
				Size = new Size(460, 15),
				ForeColor = isDarkTheme ? Color.FromArgb(140, 140, 140) : Color.FromArgb(100, 100, 100),
				Font = new Font("Segoe UI", 7.5F, FontStyle.Italic)
			};

			this.Controls.Add(lblTitle);
			this.Controls.Add(lstFunctions);
			this.Controls.Add(lblTip);
			this.Controls.Add(btnOk);
			this.Controls.Add(btnCancel);

			lstFunctions.DoubleClick += (s, e) => btnOk.PerformClick();
		}
	}
}
