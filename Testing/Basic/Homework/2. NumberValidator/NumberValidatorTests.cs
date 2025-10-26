
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    // тут решил добавить проверку на текст с ошибкой потому что иначе  если первое верно срабатывала вторая проверка падала и
    // получалось неверно и непонятно что упало
    // например precision 1 и scale 1 и тест первый ниже проходит хотя должен не проходить
    [TestCase(-1, 2, "precision must be a positive number",
        TestName = "precision не может быть отрицательным")]
    [TestCase(0, 2, "precision must be a positive number",
        TestName = "precision не может быть равен 0")]
    [TestCase(1, 2, "precision must be a non-negative number less or equal than precision",
        TestName = "scale не может быть больше precision")]
    [TestCase(1, 1, "precision must be a non-negative number less or equal than precision",
        TestName = "scale не может быть равен precision")]
    [TestCase(1, -1, "precision must be a non-negative number less or equal than precision",
        TestName = "scale не может быть отрицательным")]
    public void ConstructorWhenParametersInvalid(int precision, int scale, string expectedMessage)
    {
        Assert.That(
            () => new NumberValidator(precision, scale, true),
            Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(expectedMessage)
        );
    }

    [TestCase(1, 0, TestName = "scale может быть равен 0")]
    [TestCase(7, 5, TestName = "общий тест")]
    public void ConstructorWhenParametersValid(int precision, int scale)
    {
        Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
    }

    
    [TestCase(6, 2, false, "-1.23", true, TestName = "Валидное отрицательное число")]
    [TestCase(6, 2, true, "+1.23", true, TestName = "Валидное положительное число")]
    [TestCase(1, 0, true, "0", true, TestName = "Валидное целое число")]
    [TestCase(4, 2, true, "+1.23", true, TestName = "Валидное число где знак и цифры укладываются в precision")]
    [TestCase(6, 2, true, "+1,23", true, TestName = "Валидное число через запятую")]
    [TestCase(3, 2, true, "00.00", false, TestName = "Ошибка: вышло за пределы precision")]
    [TestCase(3, 2, true, "-0.00", false, TestName = "Ошибка: отрицательное число при onlyPositive=true")]
    [TestCase(3, 2, true, "+0.00", false, TestName = "Ошибка: превышена точность из-за знака")]
    [TestCase(6, 2, true, "0.000", false, TestName = "Ошибка: дробная часть превышает scale")]
    [TestCase(3, 2, true, "-1.23", false, TestName = "Ошибка: отрицательное число при onlyPositive=true")]
    [TestCase(3, 2, true, "a.sd", false, TestName = "Ошибка: нечисловая строка")]

    public void IsValidNumber_Tests(int precision, int scale, bool onlyPositive, string input, bool expectedResult)
        {
            NumberValidator validator = new NumberValidator(precision, scale, onlyPositive);
            Boolean result = validator.IsValidNumber(input);
            
            ClassicAssert.AreEqual(expectedResult, result,
                $"Упал для значения '{input}' с параметрами precision = {precision}, scale = {scale}, onlyPositive = {onlyPositive}");
    }
}