using System;
using System.Text;

namespace GenieLamp.Core.Utils
{
	public class SqlStringBuilder : ISqlStringBuilder
	{
		private const char indentChar = '\t';
		private const char delimiter = ',';
		private StringBuilder sb = new StringBuilder();
		private bool tokenSelect = false;
		private bool tokenFrom = false;
		private bool tokenWhere = false;
		private bool tokenOrderBy = false;

		#region Constructors
		public SqlStringBuilder()
		{
			Reset();
		}

		private void Reset()
		{
			tokenSelect = false;
			tokenFrom = false;
			tokenWhere = false;
			tokenOrderBy = false;
		}
		#endregion

		private void DoIndent()
		{
			for(int i = 0; i < Indent; i++)
				sb.Append(indentChar);
		}

		private void DeleteLastDelimiter()
		{
			for (int i = sb.Length - 1; i != 0; i--)
			{
				if (Char.IsWhiteSpace(sb[i]))
				    continue;
				if (sb[i].Equals(delimiter))
					sb.Remove(i, 1);
				break;
			}
		}

		public int Indent { get; set; }

		public override string ToString()
		{
			DeleteLastDelimiter();
			return sb.ToString();
		}

		#region ISqlStringBuilder implementation
		public void AppendLine(string format, params object[] args)
		{
			AppendLine(String.Format(format, args));
		}

		public void AppendLine(string value)
		{
			DoIndent();
			sb.AppendLine(value);
		}

		public void AppendLine()
		{
			sb.AppendLine();
		}

		public void Append(string format, params object[] args)
		{
			sb.Append(String.Format(format, args));
		}

		public void Append(string value)
		{
			sb.Append(value);
		}

		public void Select(string format, params object[] args)
		{
			Select(String.Format(format, args));
		}

		public void Select(string columnDescription)
		{
			if (!tokenSelect)
			{
				AppendLine("SELECT");
				Indent++;
				tokenSelect = true;
			}
			AppendLine("{0}{1}", columnDescription, delimiter);
		}


		public void From(string format, params object[] args)
		{
			From(String.Format(format, args));
		}

		public void From(string expression)
		{
			if (!tokenFrom)
			{
				DeleteLastDelimiter();
				Indent--;
				AppendLine("FROM");
				Indent++;
				tokenFrom = true;
			}
			AppendLine(expression);
		}

		public void WhereAnd(string format, params object[] args)
		{
			WhereAnd(String.Format(format, args));
		}

		public void WhereAnd(string expression)
		{
			if (BeginWhere())
				AppendLine("{0}", expression);
			else
				AppendLine("AND {0}", expression);
		}

		protected bool BeginWhere()
		{
			if (!tokenWhere)
			{
				DeleteLastDelimiter();
				Indent--;
				AppendLine("WHERE");
				Indent++;
				tokenWhere = true;
				return true;
			}
			return false;
		}

		public void OrderBy(string expression)
		{
			if (!tokenOrderBy)
			{
				DeleteLastDelimiter();
				Indent--;
				AppendLine("ORDER BY");
				Indent++;
				tokenFrom = true;
			}
			AppendLine(expression);
		}

		public void Union()
		{
			Reset();
			AppendLine("UNION");
		}

		public void UnionAll()
		{
			Reset();
			AppendLine("UNION ALL");
		}
		#endregion
	}
}

