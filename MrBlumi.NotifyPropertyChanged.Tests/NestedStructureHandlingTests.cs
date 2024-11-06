using MrBlumi.NotifyPropertyChanged.Tests.Helpers;

namespace MrBlumi.NotifyPropertyChanged.Tests;

public class NestedStructureHandlingTests : TestBase
{
    
    [Fact]
    public Task SupportRootNamespace() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;

            public partial class Class : INotifyPropertyChanged
            {
                [NotifyOnChange]
                public partial string StringProperty { get; set; }
            }
            """);
    
    [Fact]
    public Task SupportNestedNamespaces() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;

            namespace Outer
            {
                namespace Inner
                {
                    public partial class Class : INotifyPropertyChanged
                    {
                        [NotifyOnChange]
                        public partial string StringProperty { get; set; }
                    }
                }
            }
            """);
    
    [Fact]
    public Task SupportNestedTypeStructures() =>
        Verify("""
            using System.ComponentModel;
            using MrBlumi.NotifyPropertyChanged.Abstractions;

            namespace Test;
            
            public partial struct Outer
            { 
                public partial record Middle
                {
                    public partial class Inner : INotifyPropertyChanged
                    {
                        [NotifyOnChange]
                        public partial string StringProperty { get; set; }
                    }
                }
            }
            """);
}