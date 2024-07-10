﻿using System.Text;

namespace NetDocGen.Markdown
{
	public enum MarkdownTableAlignment
	{
		Center,
		Right,
		Left,
	}

	public class MarkdownFileBuilder
	{
		private const string codeStart = "```";
		private const string codeEnd = "```";

		private StringBuilder _sb = new StringBuilder();

		public string MarkdownCodeQuote(string code)
		{
			return "`" + code + "`";
		}

		public void Append(string text)
		{
			this._sb.Append(text);
		}

		public void Append(string text, MarkdownTextStyle style)
		{
			this._sb.Append($"{this.getStylePrefix(style)}{text}{this.getStyleSufix(style)}");
		}

		public void HorizontalRule()
		{
			this._sb.AppendLine();
			this._sb.AppendLine("---");
			this._sb.AppendLine();
		}

		public void AppendLine()
		{
			this._sb.AppendLine();
		}

		public void AppendLine(string text)
		{
			this._sb.AppendLine(text);
			this._sb.AppendLine();
		}

		public void AppendLine(string text, MarkdownTextStyle style)
		{
			this._sb.AppendLine($"{this.getStylePrefix(style)}{text}{this.getStyleSufix(style)}");
		}

		public void TextWithHeader(int level, string header, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			this.Header(level, header);
			this.AppendLine(text);
		}

		public void Header(int level, string text)
		{
			for (int i = 0; i < level; i++)
			{
				this._sb.Append("#");
			}
			this._sb.Append(" ");
			this._sb.AppendLine(text);
			this._sb.AppendLine();
		}

		public void Header(int level, string text, string url)
		{
			for (int i = 0; i < level; i++)
			{
				this._sb.Append("#");
			}

			this._sb.Append(" ");
			this.Link(text, url);
			this._sb.AppendLine();
			this._sb.AppendLine();
		}

		public void Link(string text, string url)
		{
			this._sb.Append(LinkString(text, url));
		}

		public static string LinkString(string text, string url)
		{
			StringBuilder strbuilder = new StringBuilder();

			strbuilder.Append("[");
			strbuilder.Append(process(text));
			strbuilder.Append("]");
			strbuilder.Append("(");
			strbuilder.Append(url);
			strbuilder.Append(")");

			return strbuilder.ToString();
		}

		public void Image(string altText, string imageUrl)
		{
			this._sb.Append("!");
			Link(altText, imageUrl);
		}

		public void Code(string text, string coding = null)
		{
			this._sb.AppendLine();
			this._sb.AppendLine(string.IsNullOrEmpty(coding) ? codeStart : $"{codeStart}{coding}");
			this._sb.AppendLine(text);
			this._sb.AppendLine(codeEnd);
			this._sb.AppendLine();
		}

		public void Quote(string text)
		{
			this._sb.AppendLine();
			this._sb.AppendLine($"> {text}");
			this._sb.AppendLine();
			this._sb.AppendLine();
		}

		public void Table(IEnumerable<string> headers, IEnumerable<string> alignment = null, IEnumerable<IEnumerable<string>> items = null)
		{
			this._sb.Append("| ");
			foreach (var item in headers)
			{
				this._sb.Append(item);
				this._sb.Append(" | ");
			}
			this._sb.AppendLine();

			if (alignment == null)
			{
				List<string> columnAlignments = new List<string>();
				foreach (var item in headers)
				{
					columnAlignments.Add(":-:");
				}
				alignment = columnAlignments;
			}

			this._sb.Append("| ");
			foreach (var columnAlignment in alignment)
			{
				this._sb.Append(columnAlignment);
				this._sb.Append(" | ");
			}
			this._sb.AppendLine();

			if (items != null)
			{
				foreach (var item in items)
				{
					this._sb.Append("| ");
					foreach (var item2 in item)
					{
						this._sb.Append(item2);
						this._sb.Append(" | ");
					}
					this._sb.AppendLine();
				}
				this._sb.AppendLine();
			}
		}

		public void List(string text, int level = 0) // nest zero
		{
			for (int i = 0; i < level; i++)
			{
				this._sb.Append("\t");
			}

			this._sb.Append($"- ");
			this._sb.AppendLine(text);
		}

		public void ListLink(string text, string url) // nest zero
		{
			this.ListLink(text, url, string.Empty);
		}

		public void ListLink(string text, string url, string comment) // next zero 
		{
			this._sb.Append("- ");
			Link(text, url);
			if (!string.IsNullOrWhiteSpace(comment))
			{
				this._sb.Append($" - {comment}");
			}
			this._sb.AppendLine();
		}

		public void StartCollapsibleSection(string caption)
		{
			this._sb.AppendLine("<details>");
			this._sb.AppendLine("\t<summary>" + caption + "</summary>");
			this._sb.AppendLine("");
		}

		public void EndCollapsibleSection()
		{
			this._sb.AppendLine("</details>");
			this._sb.AppendLine("");
		}

		public void Clear()
		{
			this._sb.Clear();
		}

		public override string ToString()
		{
			return this._sb.ToString();
		}

		private static string process(string text)
		{
			if (text == null) return "";
			text = text.Replace("<", "\\<");
			text = text.Replace(">", "\\>");
			text = text.Replace("&gt;", ">");
			text = text.Replace("&lt;", "<");
			text = text.Replace("|", "\\|");
			text = text.Replace(Environment.NewLine, "<br>");
			return text.Replace("\n", "<br>");
		}

		private string getStylePrefix(MarkdownTextStyle style)
		{
			switch (style)
			{
				case MarkdownTextStyle.Bold:
					return "**";
				case MarkdownTextStyle.Code:
					return "`";
				case MarkdownTextStyle.Italic:
					return "*";
				case MarkdownTextStyle.Strikethrough:
					return "~~";
				case MarkdownTextStyle.BoldAndItalic:
					return "***";
				case MarkdownTextStyle.Subscript:
					return "<sub>";
				case MarkdownTextStyle.Superscript:
					return "<sup>";
				case MarkdownTextStyle.None:
				default:
					return string.Empty;
			}
		}

		private string getStyleSufix(MarkdownTextStyle style)
		{
			switch (style)
			{
				case MarkdownTextStyle.Bold:
					return "**";
				case MarkdownTextStyle.Code:
					return "`";
				case MarkdownTextStyle.Italic:
					return "*";
				case MarkdownTextStyle.Strikethrough:
					return "~~";
				case MarkdownTextStyle.BoldAndItalic:
					return "***";
				case MarkdownTextStyle.Subscript:
					return "</sub>";
				case MarkdownTextStyle.Superscript:
					return "</sup>";
				case MarkdownTextStyle.None:
				default:
					return string.Empty;
			}
		}
	}
}