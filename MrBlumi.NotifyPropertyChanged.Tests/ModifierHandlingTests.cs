using MrBlumi.NotifyPropertyChanged.Tests.Helpers;

namespace MrBlumi.NotifyPropertyChanged.Tests;

public class ModifierHandlingTests : TestBase
{
    [Fact]
    public Task RespectClassModifiers() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;

            internal partial sealed class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; set; }
            }
            """);

    [Fact]
    public Task RespectPropertyModifiers() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;

            public partial class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                internal partial string StringProperty { get; set; }

                [NotifyOnChange]
                private partial int IntProperty { get; set; }
            }
            """);

    [Fact]
    public Task RespectAccessorModifiers() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;

            public partial class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; private set; }
            
                [NotifyOnChange]
                public partial int IntProperty { internal get; set; }
            }
            """);
}
