using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
	{

		public string PrintToString(TOwner obj)
		{
			return PrintToString(obj, 0);
		}

		private string PrintToString(object obj, int nestingLevel)
		{
			//TODO apply configurations
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
				sb.Append(identation + propertyInfo.Name + " = " +
						  PrintToString(propertyInfo.GetValue(obj),
							  nestingLevel + 1));
			}
			return sb.ToString();
		}

		public PrintingConfig<TOwner> ExcludePropertyOfType<TPropType>()
		{
			return this;
		}

		public PrintingConfig<TOwner> UseNewSerializationFor<T>(Func<T, string> func)
		{
			return this;
		}

		public PrintingConfig<TOwner> ChangeCurrentCulture()
		{
			return this;
		}

		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>()
		{
			return new PropertyPrintingConfig<TOwner, TPropType>(this);
		}


		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>(Expression<Func<TOwner, TPropType>> func)
		{
			return new PropertyPrintingConfig<TOwner, TPropType>(this);
		}
	}
}