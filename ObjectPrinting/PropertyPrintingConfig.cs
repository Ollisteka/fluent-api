using System;

namespace ObjectPrinting
{
	public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
	{
		private readonly PrintingConfig<TOwner> printingConfig;

		public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
		{
			this.printingConfig = printingConfig;
		}

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.PrintingConfig => printingConfig;

		public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
		{
			if (printingConfig.ChangeType)
				printingConfig.SerializationForTypes[typeof(TPropType)] = obj => serializeFunc((TPropType) obj);
			else
				printingConfig.SerializationForProperties[printingConfig.PropertyToChange] = obj => serializeFunc((TPropType) obj);
			return printingConfig;
		}
	}
}