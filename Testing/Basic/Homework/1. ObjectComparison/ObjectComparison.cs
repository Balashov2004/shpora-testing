using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Перепишите код на использование Fluent Assertions.

        actualTsar
            .Should()
            .BeEquivalentTo(expectedTsar, options => options
                .Excluding(info => info.Path.EndsWith(".Id") || info.Path == "Id")
            );
        // Мы заменили 8 строчек тестов одной, так же если параметры класса Person поменяются в будущем нам не потребуется писать доп тесты,
        // потому что FluentAssertions сравнивает все свойства,
        // через Excluding мы исключили параметр id, чтобы тест не падал
        
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода?
        
        // Нам придется переписывать метод AreEqual каждый раз когда меняется класс Person, при падении мы получим только
        // Что ожидалось True, а пришло False или наоборт, непонятно где ошибка
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
