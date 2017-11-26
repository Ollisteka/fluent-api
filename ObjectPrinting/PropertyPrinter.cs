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
			var newConfig = printingConfig.CopyCurrentConfig();
			if (changeType)
				newConfig.SerializationForTypes =
					newConfig.SerializationForTypes.SetItem(typeof(TPropType), obj => serializeFunc((TPropType) obj));
			else
				newConfig.SerializationForProperties =
					newConfig.SerializationForProperties.SetItem(PropertyToChange,
						obj => serializeFunc((TPropType) obj));
			return newConfig;
		}
		internal PropertyPrinter<TOwner, TPropType> CopyCurrentPropertyPrint()
		{
			return (PropertyPrinter<TOwner, TPropType>)MemberwiseClone();
		}
	}
}