using System;
using System.Globalization;
using NUnit.Framework;
using FluentAssertions;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		[Test]
		public void Demo()
		{
			var person = new Person { Name = "Alex", Age = 19 };

			var printer = ObjectPrinter.For<Person>()
				//1. Исключить из сериализации свойства определенного типа
				.Excluding<Guid>()
				//2. Указать альтернативный способ сериализации для определенного типа
				.Print<int>()
					.Using(i => "IT IS INT")
				//3. Для числовых типов указать культуру
				.Print<double>()
					.Using(CultureInfo.CurrentCulture)
				//4. Настроить сериализацию конкретного свойства
				.Print(obj => obj.Age)
					.Using(age => age.ToString())
				//5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
				.Print(obj => obj.Name)
					.TakeSubstring(1)
				//6. Исключить из сериализации конкретного свойства
				.Excluding(obj => obj.Age);



			string s1 = printer.PrintToString(person);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
			//8. ...с конфигурированием
		}

		[Test]
		public void ChangeSerializationForProperty()
		{
			var person = new Person { Name = "Alex", Age = 19, Height = 11 };

			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				//4. Настроить сериализацию конкретного свойства
				.Print(obj => obj.Name)
				.Using(name => "NAME\r\n");

			printer.PrintToString(person).Should().Be("Person\r\n\tName = NAME\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void AlternativeSerializationForType()
		{
			var person = new Person { Name = "Alex", Age = 19, Height = 11 };

			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print<string>()
					.Using(str => "STR\r\n");

			printer.PrintToString(person).Should().Be("Person\r\n\tName = STR\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void ExcludeProperty()
		{
			var person = new Person { Name = "Alex", Age = 19, Height = 11 };

			var printer = ObjectPrinter.For<Person>().Excluding<Guid>().Excluding<int>();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
		}
	}
}