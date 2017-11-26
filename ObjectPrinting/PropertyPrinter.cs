using System;
using ObjectPrinting.Interfaces;

namespace ObjectPrinting
{
	public class PropertyPrinter<TOwner, TPropType> : IPropertyPrinter<TOwner, TPropType>
	{
		private readonly bool changeType;
		private readonly ObjectPrinter<TOwner> printingConfig;
		internal readonly string PropertyToChange;

		public PropertyPrinter(ObjectPrinter<TOwner> printingConfig, bool changeType, string propertyToChange)
		{
			this.printingConfig = printingConfig;
			this.changeType = changeType;
			PropertyToChange = propertyToChange;
		}

		ObjectPrinter<TOwner> IPropertyPrinter<TOwner, TPropType>.PrintingConfig => printingConfig;

		public ObjectPrinter<TOwner> Using(Func<TPropType, string> serializeFunc)
		{
			if (changeType)
				printingConfig.SerializationForTypes =
					printingConfig.SerializationForTypes.SetItem(typeof(TPropType), obj => serializeFunc((TPropType) obj));
			else
				printingConfig.SerializationForProperties =
					printingConfig.SerializationForProperties.SetItem(PropertyToChange,
						obj => serializeFunc((TPropType) obj));
			return printingConfig;
		}
	}
}