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
		public void PrintEnumerable()
		{
			var e = new E
			{
				arr = new[] {1, 2, 3}
			};

			var printer = ObjectPrinter.For<E>();
			printer.PrintToString(e).Should().Be("E\r\n\tarr = Int32[]\r\n\t\tSystem.Int32[] = 1, 2, 3\r\n");
		}

		[Test]
		public void PropertyConfig_ShoudBe_Immutable_When_ChangingCultureInfo()
		{
			person.Height = 11.2;
			var printer = ObjectPrinter
				.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print<double>();
			var p1 = printer.Using(CultureInfo.InvariantCulture);
			var p2 = printer.Using(x => "NUM");
			p1.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 11.2\r\n");
			p2.PrintToString(person).Should().Be("Person\r\n\tName = Alex\r\n\tHeight = NUM\r\n");
		}

		[Test]
		public void PrintingConfig_ShouldBe_Immutable_When_CustomPropertySerialization()
		{
			var firstConfig = ObjectPrinter.For<Person>()
				.Excluding<Guid>()
				.Excluding<int>()
				.Print(p => p.Name)
				.Using(str => "FIRST");
			var secondConfig = firstConfig.Print(p => p.Name).Using(str => "TALL");
			var firstResult = firstConfig.PrintToString(person);
			var secondResult = secondConfig.PrintToString(person);
			firstResult.Should().Be("Person\r\n\tName = FIRST\r\n\tHeight = 11\r\n");
			secondResult.Should().Be("Person\r\n\tName = TALL\r\n\tHeight = 11\r\n");
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
		public void PropertyConfig_Shoud_BeImmutable()
		{
			var printer = ObjectPrinter
				.For<Entry>()
				.Print<string>();
			var p1 = printer.Using(x => x.ToLower());
			var p2 = printer.Using(x => x.ToUpper());
			var entry = new Entry {s = "aBc"};
			p1.PrintToString(entry).Should().Be("Entry\r\n\ts = abc\r\n");
			p2.PrintToString(entry).Should().Be("Entry\r\n\ts = ABC\r\n");
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