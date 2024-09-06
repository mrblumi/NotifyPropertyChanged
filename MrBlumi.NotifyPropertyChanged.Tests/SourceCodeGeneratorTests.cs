using MrBlumi.NotifyPropertyChanged.Tests.Helpers;

namespace MrBlumi.NotifyPropertyChanged.Tests;

public class SourceCodeGeneratorTests : TestBase
{
    [Fact]
    public Task ImplementNotifyPropertyChangedPattern() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;
            
            namespace Test;
            
            public partial class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; set; }
            }
            """);

    [Fact]
    public Task ImplementNotifyPropertyChangedPatternForMultipleProperties() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;
            
            namespace Test;
            
            public partial class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; set; }
            
                [NotifyOnChange]
                public partial int IntProperty { get; set; }
            }
            """);

    [Fact]
    public Task ImplementNotifyPropertyChangedPatternForMultipleTypes() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;
            
            namespace Test;
            
            public partial class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; set; }
            }
            """,
            """
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;
            
            namespace Test;
            
            public partial record Record : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; set; }
            }
            """);
}