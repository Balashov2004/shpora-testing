
using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

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
    public void Constructor_WhenInvalidPrecisionProvided_ShouldThrow(int precision, int scale, string expectedMessage)
    {
        Action act = () => new NumberValidator(precision, scale, true);
        act
            .Should()
            .Throw<ArgumentException>()
            .WithMessage(expectedMessage
            );
    }
    
    
    [TestCase(1, 2, "precision must be a non-negative number less or equal than precision",
        TestName = "scale не может быть больше precision")]
    [TestCase(1, 1, "precision must be a non-negative number less or equal than precision",
        TestName = "scale не может быть равен precision")]
    [TestCase(1, -1, "precision must be a non-negative number less or equal than precision",
        TestName = "scale не может быть отрицательным")]
    public void Constructor_WhenInvalidScaleProvided_ShouldThrow(int precision, int scale, string expectedMessage)
    {
        Action act = () => new NumberValidator(precision, scale, true);
        
        act
            .Should()
            .Throw<ArgumentException>()
            .WithMessage(expectedMessage
            );
    }
    

    [TestCase(1, 0, TestName = "scale может быть равен 0")]
    [TestCase(7, 5, TestName = "Тест на создание объекта с правильными параметрами")]
    public void Constructor_WhenValidParametersProvided_ShouldNotThrow(int precision, int scale)
    {
        Action act = () => new NumberValidator(precision, scale, true);

        act
            .Should()
            .NotThrow<Exception>(
            $"Не должно быть исключения при создании NumberValidator с precision={precision}, scale={scale}");
    }

    [TestCase(6, 2, false, "-1.23", true, TestName = "Валидное отрицательное число")]
    [TestCase(6, 2, true, "+1.23", true, TestName = "Валидное положительное число")]
    [TestCase(1, 0, true, "0", true, TestName = "Валидное целое число")]
    [TestCase(4, 2, true, "+1.23", true, TestName = "Валидное число где знак и цифры укладываются в precision")]
    [TestCase(6, 2, true, "+1,23", true, TestName = "Валидное число через запятую")]
    public void IsValidNumber_WhenValidInputProvided_ShouldReturnTrue(int precision, int scale, bool onlyPositive, string input, bool expectedResult)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        var result = validator.IsValidNumber(input);

        result
            .Should()
            .BeTrue(
            $"Ожидалось, что '{input}' будет валидным при precision={precision}, scale={scale}, onlyPositive={onlyPositive}"
            );
    }
    
    [TestCase(3, 2, true, "00.00", false, TestName = "Ошибка: вышло за пределы precision")]
    [TestCase(4, 2, true, "-0.00", false, TestName = "Ошибка: отрицательное число при onlyPositive=true")]
    [TestCase(3, 2, true, "+0.00", false, TestName = "Ошибка: превышена точность из-за знака")]
    [TestCase(6, 2, true, "0.000", false, TestName = "Ошибка: дробная часть превышает scale")]
    [TestCase(3, 2, true, "a.sd", false, TestName = "Ошибка: нечисловая строка")]
    public void IsValidNumber_WhenInvalidInputProvided_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string input, bool expectedResult)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        var result = validator.IsValidNumber(input);

        result
            .Should()
            .BeFalse(
            $"Ожидалось, что '{input}' будет невалидным при precision={precision}, scale={scale}, onlyPositive={onlyPositive}"
            );
    }
}