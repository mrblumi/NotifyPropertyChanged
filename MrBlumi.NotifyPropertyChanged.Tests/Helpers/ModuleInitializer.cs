using System.Runtime.CompilerServices;

namespace MrBlumi.NotifyPropertyChanged.Tests.Helpers;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifySourceGenerators.Initialize();
        Verifier.UseProjectRelativeDirectory("Snapshots");
    }
}
