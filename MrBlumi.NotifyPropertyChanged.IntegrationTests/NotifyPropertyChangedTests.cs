using FluentAssertions;
using MrBlumi.NotifyPropertyChanged.Abstractions;
using System.ComponentModel;

namespace MrBlumi.NotifyPropertyChanged.IntegrationTests;

public class NotifyPropertyChangedTests
{
    [Fact]
    public void WhenAnnotatedPropertiesAreChangedTheEventIsRaised()
    {
        //arrange
        var dummy = new Dummy { StringProperty = "Foo" };

        var receivedSender = default(object)!;
        var receivedEventArgs = default(PropertyChangedEventArgs)!;

        dummy.PropertyChanged += (sender, eventArgs) => (receivedSender, receivedEventArgs) = (sender, eventArgs);

        //act
        dummy.StringProperty = "Bar";

        //assert
        receivedSender.Should().Be(dummy);
        receivedEventArgs.Should().BeEquivalentTo(new { PropertyName = nameof(Dummy.StringProperty) });
    }
}

public partial class Dummy
{
    [NotifyOnChange]
    public required partial string StringProperty { get; set; }
}