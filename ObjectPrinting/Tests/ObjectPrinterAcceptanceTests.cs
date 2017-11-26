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

			var config = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>();
			config.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11,2\r\n");

			config.Print<double>()
				.Using(CultureInfo.InvariantCulture)
				.PrintToString(person)
				.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11.2\r\n");
		}

		[Test]
		public void ChangeSerializationFor_Property()
		{
			ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print(obj => obj.Height)
				.Using(name => "NAME")
				.PrintToString(person)
				.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = NAME\r\n");
		}

		[Test]
		public void ChangeSerializationFor_Type()
		{
			ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print<string>()
				.Using(str => "STR")
				.PrintToString(person).Should().Be("Person\r\n\tName = STR\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void Demo()
		{
			var config = ObjectPrinter.For<Person>()
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
				.Excluding(obj => obj.Age);


			var s1 = config.PrintToString(person);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
			//8. ...с конфигурированием
		}

		[Test]
		public void DoSomething_WhenSomething()
		{

			var printer = ObjectPrinter
				.For<Entry>()
				.Print<string>();
			var p1 = printer.Using(x => x.ToLower());
			var p2 = printer.Using(x => x.ToUpper());
			Console.WriteLine(p1.PrintToString(new Entry { s = "aBc" }));
			Console.WriteLine(p2.PrintToString(new Entry { s = "aBc" }));
		}
		[Test]
		public void Exclude_Property()
		{
			ObjectPrinter.For<Person>()
				.Excluding(p => p.Id)
				.Excluding<int>().PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void Exclude_Type()
		{
			ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>().PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
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
			var firstResult = firstConfig.PrintToString(person);
			var secondResult = secondConfig.PrintToString(person);
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
			var firstResult = firstConfig.PrintToString(person);
			var secondResult = secondConfig.PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = FIRST\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = SECOND\r\n\tHeight = 11\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_ExcludingProperties()
		{
			var firstConfig = ObjectPrinter.For<Person>().Excluding(p => p.Id).Excluding(p => p.Age);
			var secondConfig = firstConfig.Excluding(p => p.Height);
			var firstResult = firstConfig.PrintToString(person);
			var secondResult = secondConfig.PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = Alex\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_ExcludingTypes()
		{
			var firstConfig = ObjectPrinter.For<Person>().Excluding<Guid>().Excluding<int>();
			var secondConfig = firstConfig.Excluding<double>();
			var firstResult = firstConfig.PrintToString(person);
			var secondResult = secondConfig.PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = Alex\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBeImmutable_When_TakingSubstring()
		{
			var firstConfig = ObjectPrinter.For<Person>().Excluding<Guid>().Excluding<int>().Excluding<double>();
			var secondConfig = firstConfig.Print(p => p.Name).TakeSubstring(4);
			var firstResult = firstConfig.Print(p => p.Name).TakeSubstring(2).PrintToString(person);
			var secondResult = secondConfig.PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = Al\r\n");
			secondResult.Should().Be("Person\r\n\tName = Alex\r\n");
		}

		[Test]
		public void TakeSubstring()
		{
			ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print(obj => obj.Name)
				.TakeSubstring(2).PrintToString(person).Should().Be("Person\r\n\tName = Al\r\n\tHeight = 11\r\n");
		}
	}
}