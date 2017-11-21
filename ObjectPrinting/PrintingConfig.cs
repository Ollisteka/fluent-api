using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
		internal IReadOnlyList<string> PropertiesToExclude => propertiesToExclude;
		internal IReadOnlyList<Type> TypesToExclude => typesToExclude;

		public ObjectPrinter<TOwner> Build()
		{
			return new ObjectPrinter<TOwner>(this);
		}

		public PrintingConfig<TOwner> Excluding<TPropType>()
		{
			var newConfig = (PrintingConfig<TOwner>)MemberwiseClone();
			newConfig.typesToExclude.Add(typeof(TPropType));
			return newConfig;
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
			var newConfig = (PrintingConfig<TOwner>)MemberwiseClone();
			newConfig.propertiesToExclude.Add(((MemberExpression) memberSelector.Body).Member.Name);
			return newConfig;
		}
	}
}