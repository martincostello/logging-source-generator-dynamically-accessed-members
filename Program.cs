using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var services = new ServiceCollection()
    .AddLogging((builder) => builder.AddConsole())
    .AddSingleton<IAnimalProvider<Cat>, AnimalProvider<Cat>>()
    .AddSingleton<IAnimalProvider<Dog>, AnimalProvider<Dog>>()
    .AddSingleton<Describer<IAnimal>, AnimalDescriber>()
    .BuildServiceProvider();

var cattery = services.GetRequiredService<IAnimalProvider<Cat>>();
var kennel = services.GetRequiredService<IAnimalProvider<Dog>>();
var describer = services.GetRequiredService<Describer<IAnimal>>();

var cat = cattery.Create();
var dog = kennel.Create();

Console.WriteLine(describer.Describe(cat));
Console.WriteLine(describer.Describe(dog));

public interface IAnimal
{
    string Name { get; }
}

public class Dog : IAnimal
{
    public string Name { get; set; } = "Clifford";
    public string Size { get; set; } = "Big";
    public string Colour { get; set; } = "Red";
}

public class Cat : IAnimal
{
    public string Name { get; set; } = "Garfield";
    public string Food { get; set; } = "Lasagne";
}

// Without [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)], the following warning is generated from dotnet publish:
// Trim analysis warning IL2075: Describer`1.Describe(!0): 'this' argument does not satisfy
// 'DynamicallyAccessedMemberTypes.PublicProperties' in call to 'System.Type.GetProperties()'.
// The return value of method 'System.Object.GetType()' does not have matching annotations.
// The source value must declare at least the same requirements as those declared on the target location it is assigned to.
//
// With [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)], the application fails to compile:
// Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,31): error CS0246: The type or namespace name 'DynamicallyAccessedMembersAttribute' could not be found (are you missing a using directive or an assembly reference?)
// Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,31): error CS0246: The type or namespace name 'DynamicallyAccessedMembers' could not be found (are you missing a using directive or an assembly reference?)
// Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,58): error CS0103: The name 'DynamicallyAccessedMemberTypes' does not exist in the current context
public abstract partial class Describer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(ILogger<Describer<T>> logger)
{
    public string Describe(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var type = value.GetType();

        Log.Describing(logger, type);

        var builder = new StringBuilder()
            .AppendLine($"Type: {type.Name}");

        foreach (var property in type.GetProperties())
        {
            builder.AppendLine($"{property.Name}: {property.GetValue(value)}");
        }

        return builder.ToString();
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Describing an animal of type {Type}.")]
        public static partial void Describing(ILogger logger, Type type);
    }
}

public class AnimalDescriber(ILogger<AnimalDescriber> logger) : Describer<IAnimal>(logger)
{
}

public interface IAnimalProvider<T> where T : IAnimal, new()
{
    T Create();
}

public partial class AnimalProvider<T>(ILogger<AnimalProvider<T>> logger) : IAnimalProvider<T>
    where T : IAnimal, new()
{
    public T Create()
    {
        Log.CreatingAnimal(logger, typeof(T));
        return new();
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Returning an animal of type {Type}.")]
        public static partial void CreatingAnimal(ILogger logger, Type type);
    }
}
