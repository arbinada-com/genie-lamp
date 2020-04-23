using System;
using System.Text;

namespace GenieLamp.Core.Layers
{
	class LayerMethod : ILayerMethod
	{
		#region Constructors
		public LayerMethod(string name, string methodParams)
		{
			this.Name = name;
			this.Parameters = methodParams;
			this.Signature = String.Format("{0}({1})", name, methodParams);

		}
		#endregion


		#region ILayerMethod implementation
		public string Name
		{
			get;
			private set;
		}

		public string Parameters
		{
			get;
			private set;
		}

		public string Signature
		{
			get;
			private set;
		}

		public string Call(params object[] args)
		{
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < args.Length; i++)
			{
				sb.AppendFormat("{0}", args[i]);
				if (i != args.Length - 1)
					sb.Append(", ");
			}
			return String.Format("{0}({1})", Name, sb.ToString());
		}
		#endregion
	}
}

