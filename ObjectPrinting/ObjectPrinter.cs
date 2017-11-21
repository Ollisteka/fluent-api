using System;
using System.Linq;
using System.Text;

namespace ObjectPrinting
{
	public class ObjectPrinter<TOwner>
	{
		private readonly PrintingConfig<TOwner> printingConfig;

		public ObjectPrinter(PrintingConfig<TOwner> printingConfig)
		{
			this.printingConfig = printingConfig;
		}

		public static PrintingConfig<TOwner> Configure()
		{
			return new PrintingConfig<TOwner>();
		}

		public string PrintToString(TOwner obj)
		{
			return PrintToString(obj, 0);
		}

		private string PrintToString(object obj, int nestingLevel)
		{
			if (obj == null)
				return "null" + Environment.NewLine;

			var finalTypes = new[]
			{
				typeof(int), typeof(double), typeof(float), typeof(string),
				typeof(DateTime), typeof(TimeSpan)
			};
			if (finalTypes.Contains(obj.GetType()))
				return obj + Environment.NewLine;

			var identation = new string('\t', nestingLevel + 1);
			var sb = new StringBuilder();
			var type = obj.GetType();
			sb.AppendLine(type.Name);
			foreach (var propertyInfo in type.GetProperties())
			{
				var propertyType = propertyInfo.PropertyType;
				var propertyName = propertyInfo.Name;

				if (printingConfig.TypesToExclude.Contains(propertyType)
					|| printingConfig.PropertiesToExclude.Contains(propertyName))
					continue;
				if (printingConfig.SerializationForProperties.ContainsKey(propertyName))
				{
					sb.Append(identation)
						.Append(propertyInfo.Name)
						.Append(" = ")
						.Append(printingConfig.SerializationForProperties[propertyName](propertyInfo.GetValue(obj)))
						.Append(Environment.NewLine);
					continue;
				}
				if (printingConfig.SerializationForTypes.ContainsKey(propertyType))
				{
					sb.Append(identation)
						.Append(propertyInfo.Name)
						.Append(" = ")
						.Append(printingConfig.SerializationForTypes[propertyType](propertyInfo.GetValue(obj)))
						.Append(Environment.NewLine);
					continue;
				}
				sb.Append(identation)
					.Append(propertyInfo.Name)
					.Append(" = ")
					.Append(PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1));
			}
			return sb.ToString();
		}
	}
}