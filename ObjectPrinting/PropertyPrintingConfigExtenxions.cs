using System.Globalization;

namespace ObjectPrinting
{
	public static class PropertyPrintingConfigExtenxions
	{
		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, int> propConfig, CultureInfo cultInfo)
		{
			((IPropertyPrintingConfig<TOwner, int>) propConfig).PrintingConfig.SerializationForTypes =
				((IPropertyPrintingConfig<TOwner, int>) propConfig).PrintingConfig.SerializationForTypes.SetItem(typeof(int),
					obj => ((int) obj).ToString(cultInfo));
			return ((IPropertyPrintingConfig<TOwner, int>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, double> propConfig, CultureInfo cultInfo)
		{
			((IPropertyPrintingConfig<TOwner, double>) propConfig).PrintingConfig.SerializationForTypes =
				((IPropertyPrintingConfig<TOwner, double>) propConfig).PrintingConfig.SerializationForTypes.SetItem(typeof(double),
					obj => ((double) obj).ToString(cultInfo));
			return ((IPropertyPrintingConfig<TOwner, double>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, float> propConfig, CultureInfo cultInfo)
		{
			((IPropertyPrintingConfig<TOwner, float>) propConfig).PrintingConfig.SerializationForTypes =
				((IPropertyPrintingConfig<TOwner, float>) propConfig).PrintingConfig.SerializationForTypes.SetItem(typeof(float),
					obj => ((float) obj).ToString(cultInfo));
			return ((IPropertyPrintingConfig<TOwner, float>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, long> propConfig, CultureInfo cultInfo)
		{
			((IPropertyPrintingConfig<TOwner, long>) propConfig).PrintingConfig.SerializationForTypes =
				((IPropertyPrintingConfig<TOwner, long>) propConfig).PrintingConfig.SerializationForTypes.SetItem(typeof(long),
					obj => ((long) obj).ToString(cultInfo));
			return ((IPropertyPrintingConfig<TOwner, long>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> TakeSubstring<TOwner>
			(this PropertyPrintingConfig<TOwner, string> propConfig, int maxLength)
		{
			var printingConfig = ((IPropertyPrintingConfig<TOwner, string>) propConfig).PrintingConfig;
			printingConfig.SerializationForProperties = printingConfig.SerializationForProperties.SetItem(
				printingConfig.PropertyToChange,
				obj => ((string) obj).Substring(0, maxLength));
			return printingConfig;
		}
	}

	public interface IPropertyPrintingConfig<TOwner, TPropType>
	{
		PrintingConfig<TOwner> PrintingConfig { get; }
	}
}