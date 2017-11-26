using System;

namespace ObjectPrinting
{
	public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
	{
		private readonly bool changeType;
		private readonly PrintingConfig<TOwner> printingConfig;
		internal readonly string PropertyToChange;

		public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, bool changeType, string propertyToChange)
		{
			this.printingConfig = printingConfig;
			this.changeType = changeType;
			PropertyToChange = propertyToChange;
		}

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.PrintingConfig => printingConfig;

		public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
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