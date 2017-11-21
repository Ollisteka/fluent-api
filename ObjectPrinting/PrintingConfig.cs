using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
	{
		private readonly List<string> propertiesToExclude = new List<string>();
		private readonly List<Type> typesToExclude = new List<Type>();
		internal bool ChangeType;
		internal string PropertyToChange;

		internal Dictionary<string, Func<object, string>> SerializationForProperties =
			new Dictionary<string, Func<object, string>>();

		internal Dictionary<Type, Func<object, string>> SerializationForTypes = new Dictionary<Type, Func<object, string>>();

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

				if (typesToExclude.Contains(propertyType) || propertiesToExclude.Contains(propertyName))
					continue;
				if (SerializationForProperties.ContainsKey(propertyName))
				{
					sb.Append(identation)
						.Append(propertyInfo.Name)
						.Append(" = ")
						.Append(SerializationForProperties[propertyName](propertyInfo.GetValue(obj)))
						.Append(Environment.NewLine);
					continue;
				}
				if (SerializationForTypes.ContainsKey(propertyType))
				{
					sb.Append(identation)
						.Append(propertyInfo.Name)
						.Append(" = ")
						.Append(SerializationForTypes[propertyType](propertyInfo.GetValue(obj)))
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

		public PrintingConfig<TOwner> Excluding<TPropType>()
		{
			typesToExclude.Add(typeof(TPropType));
			return this;
		}

		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>()
		{
			ChangeType = true;
			return new PropertyPrintingConfig<TOwner, TPropType>(this);
		}


		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			ChangeType = false;
			PropertyToChange = ((MemberExpression) memberSelector.Body).Member.Name;
			return new PropertyPrintingConfig<TOwner, TPropType>(this);
		}

		public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			propertiesToExclude.Add(((MemberExpression) memberSelector.Body).Member.Name);
			return this;
		}
	}
}