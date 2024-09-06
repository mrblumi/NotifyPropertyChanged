using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using MrBlumi.NotifyPropertyChanged.Abstractions;
using Test.Generator.NotifyPropertyChanged;

namespace MrBlumi.NotifyPropertyChanged.Tests.Helpers;

public abstract class TestBase
{
    protected static Task Verify(params IEnumerable<string> source)
    {
        var sourceGenerator = new SourceCodeGenerator();

        var compilation = CSharpCompilation.Create(
            assemblyName: nameof(SourceCodeGeneratorTests),
            syntaxTrees: source
                .Select(x => CSharpSyntaxTree.ParseText(x, cancellationToken: TestContext.Current.CancellationToken))
                .ToArray(),
            references: [MetadataReference.CreateFromFile(typeof(NotifyOnChangeAttribute).Assembly.Location)]);

        var driver = CSharpGeneratorDriver
            .Create(sourceGenerator)
            .RunGenerators(compilation, TestContext.Current.CancellationToken);

        return Verifier.Verify(driver);
    }
}