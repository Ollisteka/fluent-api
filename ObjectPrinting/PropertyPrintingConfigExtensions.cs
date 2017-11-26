using System;
using System.Collections.Generic;
using System.Globalization;

namespace ObjectPrinting
{
	public static class PropertyPrintingConfigExtensions
	{
		private static readonly Dictionary<Type, Func<object, CultureInfo, string>> ConvertingFuncs =
			new Dictionary<Type, Func<object, CultureInfo, string>>
			{
				{typeof(int), (o, info) => ((int) o).ToString(info)},
				{typeof(double), (o, info) => ((double) o).ToString(info)},
				{typeof(long), (o, info) => ((long) o).ToString(info)},
				{typeof(float), (o, info) => ((float) o).ToString(info)}
			};

		private static PrintingConfig<TOwner> ChangeCultureForType<TOwner, TPropType>(
			PropertyPrintingConfig<TOwner, TPropType> propConfig,
			CultureInfo cultInfo)
		{
			if (!ConvertingFuncs.ContainsKey(typeof(TPropType)))
				throw new ArgumentException("You can use this func only for numbers!");
			((IPropertyPrintingConfig<TOwner, TPropType>) propConfig).PrintingConfig.SerializationForTypes =
				((IPropertyPrintingConfig<TOwner, TPropType>) propConfig).PrintingConfig.SerializationForTypes.SetItem(
					typeof(TPropType),
					obj => ConvertingFuncs[typeof(TPropType)](obj, cultInfo));
			return ((IPropertyPrintingConfig<TOwner, TPropType>) propConfig).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, int> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, double> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, float> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static PrintingConfig<TOwner> Using<TOwner>
			(this PropertyPrintingConfig<TOwner, long> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static PrintingConfig<TOwner> TakeSubstring<TOwner>
			(this PropertyPrintingConfig<TOwner, string> propConfig, int maxLength)
		{
			var printingConfig = ((IPropertyPrintingConfig<TOwner, string>) propConfig).PrintingConfig;
			printingConfig.SerializationForProperties = printingConfig.SerializationForProperties.SetItem(
				propConfig.PropertyToChange,
				obj => ((string) obj).Substring(0, maxLength));
			return printingConfig;
		}
	}

	public interface IPropertyPrintingConfig<TOwner, TPropType>
	{
		PrintingConfig<TOwner> PrintingConfig { get; }
	}
}