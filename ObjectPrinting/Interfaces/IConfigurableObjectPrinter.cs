using System;
using System.Linq.Expressions;

namespace ObjectPrinting.Interfaces
{
	public interface IConfigurableObjectPrinter<TOwner>
	{
		ObjectPrinter<TOwner> Excluding<TPropType>();
		ObjectPrinter<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector);
		PropertyPrinter<TOwner, TPropType> Print<TPropType>();
		PropertyPrinter<TOwner, TPropType> Print<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector);
	}
}