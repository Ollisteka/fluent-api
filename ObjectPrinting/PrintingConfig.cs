using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
	{
		internal bool ChangeType;
		private ImmutableList<string> propertiesToExclude = ImmutableList<string>.Empty;
		internal string PropertyToChange;

		internal ImmutableDictionary<string, Func<object, string>> SerializationForProperties
			= ImmutableDictionary<string, Func<object, string>>.Empty;

		internal ImmutableDictionary<Type, Func<object, string>> SerializationForTypes
			= ImmutableDictionary<Type, Func<object, string>>.Empty;

		private ImmutableList<Type> typesToExclude = ImmutableList<Type>.Empty;

		internal IReadOnlyList<string> PropertiesToExclude => propertiesToExclude;
		internal IReadOnlyList<Type> TypesToExclude => typesToExclude;

		public ObjectPrinter<TOwner> Build()
		{
			return new ObjectPrinter<TOwner>(this);
		}

		public PrintingConfig<TOwner> Excluding<TPropType>()
		{
			var newConfig = (PrintingConfig<TOwner>) MemberwiseClone();
			newConfig.typesToExclude = newConfig.typesToExclude.Add(typeof(TPropType));
			return newConfig;
		}

		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>()
		{
			ChangeType = true;
			return new PropertyPrintingConfig<TOwner, TPropType>((PrintingConfig<TOwner>) MemberwiseClone());
		}

		public PropertyPrintingConfig<TOwner, TPropType> Print<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			ChangeType = false;
			PropertyToChange = ((MemberExpression) memberSelector.Body).Member.Name;
			return new PropertyPrintingConfig<TOwner, TPropType>((PrintingConfig<TOwner>) MemberwiseClone());
		}

		public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
		{
			var newConfig = (PrintingConfig<TOwner>) MemberwiseClone();
			newConfig.propertiesToExclude =
				newConfig.propertiesToExclude.Add(((MemberExpression) memberSelector.Body).Member.Name);
			return newConfig;
		}
	}
}