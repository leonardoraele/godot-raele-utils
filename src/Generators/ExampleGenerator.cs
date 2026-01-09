// using System;
// using System.Linq;
// using System.Text;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.Text;

// namespace Raele.GodotUtilsGenerators;

// [Generator]
// public class ExampleGenerator : ISourceGenerator
// {
// 	public void Initialize(GeneratorInitializationContext context)
// 	{
// 		// Initialization code can go here if needed
// 	}

// 	public void Execute(GeneratorExecutionContext context)
// 	{
// 		string sourceBuilder = new StringBuilder($@"
// using System;

// namespace HelloWorldGenerated;

// public static class HelloWorld
// {{
// 	public static void SayHello()
// 	{{
// 		Console.WriteLine(""Hello from generated code!"");
// 		Console.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
// 		{context.Compilation.SyntaxTrees.Select(tree => $"Console.WriteLine(\" - {tree.FilePath}\");")}

// 	}}
// }}
// 		").ToString();
// 		context.AddSource(nameof(ExampleGenerator), SourceText.From(sourceBuilder, Encoding.UTF8));
// 	}
// }
