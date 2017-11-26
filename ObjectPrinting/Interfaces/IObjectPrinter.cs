namespace ObjectPrinting.Interfaces
{
	public interface IObjectPrinter<T>
	{
		string PrintToString(object obj);
	}
}