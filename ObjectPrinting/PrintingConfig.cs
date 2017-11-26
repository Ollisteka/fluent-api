using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
	{
		private ImmutableList<string> propertiesToExclude = ImmutableList<string>.Empty;
		private ImmutableList<Type> typesToExclude = ImmutableList<Type>.Empty;

		internal ImmutableDictionary<string, Func<object, string>> SerializationForProperties
			= ImmutableDictionary<string, Func<object, string>>.Empty;

		internal ImmutableDictionary<Type, Func<object, string>> SerializationForTypes
			= ImmutableDictionary<Type, Func<object, string>>.Empty;

		internal IReadOnlyList<string> PropertiesToExclude => propertiesToExclude;
		internal IReadOnlyList<Type> TypesToExclude => typesToExclude;

		public ObjectPrinter<TOwner> Build()
		{
			return new ObjectPrinter<TOwner>(this);
		}

		public PrintingConfig<TOwner> Excluding<TPropType>()
		{
			var newConfig = CopyCurrentConfig();
			newConfig.typesToExclude = newConfig.typesToExclude.Add(typeof(TPropType));
			return newConfig;
		}

		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>()
		{
			return new PropertyPrintingConfig<TOwner, TPropType>(CopyCurrentConfig(), true, null);
		}

		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			var propertyToChange = ((MemberExpression) memberSelector.Body).Member.Name;
			return new PropertyPrintingConfig<TOwner, TPropType>(CopyCurrentConfig(), false, propertyToChange);
		}

		public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			var newConfig = CopyCurrentConfig();
			newConfig.propertiesToExclude =
				newConfig.propertiesToExclude.Add(((MemberExpression) memberSelector.Body).Member.Name);
			return newConfig;
		}

		private PrintingConfig<TOwner> CopyCurrentConfig()
		{
			return (PrintingConfig<TOwner>) MemberwiseClone();
		}
	}
}