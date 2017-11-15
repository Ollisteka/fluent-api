using System.Globalization;

namespace ObjectPrinting
{
	public static class PropertyPrintingConfigExtenxion
	{
		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, int> propConfig, CultureInfo cultInfo)
		{
			return ((IPropertyPrintingConfig<TOwner, int>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, double> propConfig, CultureInfo cultInfo)
		{
			return ((IPropertyPrintingConfig<TOwner, double>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, float> propConfig, CultureInfo cultInfo)
		{
			return ((IPropertyPrintingConfig<TOwner, float>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, long> propConfig, CultureInfo cultInfo)
		{
			return ((IPropertyPrintingConfig<TOwner, long>) propConfig).PrintingConfig;
		}
	}

	public interface IPropertyPrintingConfig<TOwner, TPropType>
	{
		PrintingConfig<TOwner> PrintingConfig { get; }
	}
}