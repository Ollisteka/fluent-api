using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		public string SayHi(object obj)
		{
			return "Hi";
		}
		[Test]
		public void Demo()
		{
			var person = new Person { Name = "Alex", Age = 19 };

			var printer = ObjectPrinter.For<Person>()
				//1. Исключить из сериализации свойства определенного типа
				.ExcludePropertyOfType<Guid>()
				//2. Указать альтернативный способ сериализации для определенного типа
				.Print<int>()
					.Using(i => "IT IS INT")
				//3. Для числовых типов указать культуру
				.Print<double>()
					.Using(CultureInfo.CurrentCulture)
				//4. Настроить сериализацию конкретного свойства
				.Print(obj => obj.Age)
					.Using(age => age.ToString());
				//5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
				//6. Исключить из сериализации конкретного свойства



			string s1 = printer.PrintToString(person);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
			//8. ...с конфигурированием
		}

		[Test]
		public void ExcludeProperty()
		{
			var person = new Person { Name = "Alex", Age = 19 };

			var printer = ObjectPrinter.For<Person>().ExcludePropertyOfType<Guid>();

			string s1 = printer.PrintToString(person);
			s1.Should().Be("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 0\r\n");
		}
	}
}