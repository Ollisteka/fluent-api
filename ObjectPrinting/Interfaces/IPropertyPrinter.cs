namespace ObjectPrinting.Interfaces
{
	public interface IPropertyPrinter<TOwner, TPropType>
	{
		ObjectPrinter<TOwner> PrintingConfig { get; }
	}
}