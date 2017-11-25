using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		[SetUp]
		public void SetUp()
		{
			person = new Person {Name = "Alex", Height = 11, Age = 19};
		}

		private Person person;

		[Test]
		public void ChangeCulturalInfo()
		{
			person.Height = 11.2;

			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Build();
			printer.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11,2\r\n");

			printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print<double>()
				.Using(CultureInfo.InvariantCulture)
				.Build();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11.2\r\n");
		}

		[Test]
		public void ChangeSerializationFor_Property()
		{
			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print(obj => obj.Height)
				.Using(name => "NAME")
				.Build();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = NAME\r\n");
		}

		[Test]
		public void ChangeSerializationFor_Type()
		{
			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print<string>()
				.Using(str => "STR")
				.Build();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = STR\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void Demo()
		{
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
				//6. Исключить из сериализации конкретное свойство
				.Excluding(obj => obj.Age)
				.Build();


			var s1 = printer.PrintToString(person);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
			//8. ...с конфигурированием
		}

		[Test]
		public void Exclude_Property()
		{
			var printer = ObjectPrinter.For<Person>()
				.Excluding(p => p.Id)
				.Excluding<int>()
				.Build();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void Exclude_Type()
		{
			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Build();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_CustomPropertySerialization()
		{
			var firstConfig = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print(p => p.Name)
				.Using(str => "FIRST");
			var secondConfig = firstConfig.Print(p => p.Height).Using(str => "TALL");
			var firstResult = firstConfig.Build().PrintToString(person);
			var secondResult = secondConfig.Build().PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = FIRST\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = FIRST\r\n\tHeight = TALL\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_CustomTypeSerialization()
		{
			var firstConfig = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print<string>()
				.Using(str => "FIRST");
			var secondConfig = firstConfig.Print<string>().Using(str => "SECOND");
			var firstResult = firstConfig.Build().PrintToString(person);
			var secondResult = secondConfig.Build().PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = FIRST\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = SECOND\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_ExcludingProperties()
		{
			var firstConfig = ObjectPrinter.For<Person>().Excluding(p => p.Id).Excluding(p => p.Age);
			var secondConfig = firstConfig.Excluding(p => p.Height);
			var firstResult = firstConfig.Build().PrintToString(person);
			var secondResult = secondConfig.Build().PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = Alex\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_ExcludingTypes()
		{
			var firstConfig = ObjectPrinter.For<Person>().Excluding<Guid>().Excluding<int>();
			var secondConfig = firstConfig.Excluding<double>();
			var firstResult = firstConfig.Build().PrintToString(person);
			var secondResult = secondConfig.Build().PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = Alex\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_TakingSubstring()
		{
			var firstConfig = ObjectPrinter.For<Person>().Excluding<Guid>().Excluding<int>().Excluding<double>();
			var secondConfig = firstConfig.Print(p => p.Name).TakeSubstring(4);
			var firstResult = firstConfig.Print(p => p.Name).TakeSubstring(2).Build().PrintToString(person);
			var secondResult = secondConfig.Build().PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = Al\r\n");
			secondResult.Should().Be("Person\r\n\tName = Alex\r\n");
		}

		[Test]
		public void TakeSubstring()
		{
			var printer = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print(obj => obj.Name)
				.TakeSubstring(2)
				.Build();

			printer.PrintToString(person).Should().Be("Person\r\n\tName = Al\r\n\tHeight = 11\r\n");
		}
	}
}