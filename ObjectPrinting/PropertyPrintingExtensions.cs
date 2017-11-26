using System;
using System.Collections.Generic;
using System.Globalization;
using ObjectPrinting.Interfaces;

namespace ObjectPrinting
{
	public static class PropertyPrintingExtensions
	{
		private static readonly Dictionary<Type, Func<object, CultureInfo, string>> ConvertingFuncs =
			new Dictionary<Type, Func<object, CultureInfo, string>>
			{
				{typeof(int), (o, info) => ((int) o).ToString(info)},
				{typeof(double), (o, info) => ((double) o).ToString(info)},
				{typeof(long), (o, info) => ((long) o).ToString(info)},
				{typeof(float), (o, info) => ((float) o).ToString(info)}
			};

		private static ObjectPrinter<TOwner> ChangeCultureForType<TOwner, TPropType>(
			IPropertyPrinter<TOwner, TPropType> propConfig,
			CultureInfo cultInfo)
		{
			if (!ConvertingFuncs.ContainsKey(typeof(TPropType)))
				throw new ArgumentException("You can use this func only for numbers!");
			propConfig.PrintingConfig.SerializationForTypes =
				propConfig.PrintingConfig.SerializationForTypes.SetItem(
					typeof(TPropType),
					obj => ConvertingFuncs[typeof(TPropType)](obj, cultInfo));
			return propConfig.PrintingConfig;
		}

		public static ObjectPrinter<TOwner> Using<TOwner>
			(this PropertyPrinter<TOwner, int> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static ObjectPrinter<TOwner> Using<TOwner>
			(this PropertyPrinter<TOwner, double> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static ObjectPrinter<TOwner> Using<TOwner>
			(this PropertyPrinter<TOwner, float> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static ObjectPrinter<TOwner> Using<TOwner>
			(this PropertyPrinter<TOwner, long> propConfig, CultureInfo cultInfo)
		{
			return ChangeCultureForType(propConfig, cultInfo);
		}

		public static ObjectPrinter<TOwner> TakeSubstring<TOwner>
			(this PropertyPrinter<TOwner, string> propConfig, int maxLength)
		{
			var newConfig = propConfig.CopyCurrentPropertyPrint();
			var printingConfig = ((IPropertyPrinter<TOwner, string>)newConfig).PrintingConfig.CopyCurrentConfig();
			printingConfig.SerializationForProperties = printingConfig.SerializationForProperties.SetItem(
				propConfig.PropertyToChange,
				obj => ((string) obj).Substring(0, maxLength));
			return printingConfig;
		}
	}
}