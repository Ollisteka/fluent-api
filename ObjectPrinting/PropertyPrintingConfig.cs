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

		public PrintingConfig<TOwner> Using(Func<TPropType, string> deserializeFunc)
		{
			return printingConfig;
		}

//		public PrintingConfig<TOwner> ChangeCulture(CultureInfo cultureInfo)
//		{
//			return printingConfig;
//		}
		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.PrintingConfig => printingConfig;
	}
}