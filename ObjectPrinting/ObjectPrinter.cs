using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ObjectPrinting.Interfaces;

namespace ObjectPrinting
{
	public class ObjectPrinter
	{
		public static ObjectPrinter<TOwner> For<TOwner>()
		{
			return new ObjectPrinter<TOwner>();
		}
	}

	public class ObjectPrinter<TOwner> : IObjectPrinter<TOwner>
	{
		private ImmutableList<string> propertiesToExclude = ImmutableList<string>.Empty;

		internal ImmutableDictionary<string, Func<object, string>> SerializationForProperties
			= ImmutableDictionary<string, Func<object, string>>.Empty;

		internal ImmutableDictionary<Type, Func<object, string>> SerializationForTypes
			= ImmutableDictionary<Type, Func<object, string>>.Empty;

		private ImmutableList<Type> typesToExclude = ImmutableList<Type>.Empty;

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

				if (typesToExclude.Contains(propertyType)
					|| propertiesToExclude.Contains(propertyName))
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


		public ObjectPrinter<TOwner> Excluding<TPropType>()
		{
			var newConfig = CopyCurrentConfig();
			newConfig.typesToExclude = newConfig.typesToExclude.Add(typeof(TPropType));
			return newConfig;
		}

		public ObjectPrinter<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			var newConfig = CopyCurrentConfig();
			newConfig.propertiesToExclude =
				propertiesToExclude.Add(((MemberExpression) memberSelector.Body).Member.Name);
			return newConfig;
		}

		public PropertyPrinter<TOwner, TPropType> Print<TPropType>()
		{
			return new PropertyPrinter<TOwner, TPropType>(this, true, null);
		}

		public PropertyPrinter<TOwner, TPropType> Print<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			var propertyToChange = ((MemberExpression) memberSelector.Body).Member.Name;
			return new PropertyPrinter<TOwner, TPropType>(this, false, propertyToChange);
		}

		internal ObjectPrinter<TOwner> CopyCurrentConfig()
		{
			return (ObjectPrinter<TOwner>) MemberwiseClone();
		}
	}
}