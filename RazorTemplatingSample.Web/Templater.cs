using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace RazorTemplatingSample.Web
{
    public static class Templater
    {
        public static string Run()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
 // ADDED THE FOLLOWING LINE
using System;

class Greeter
{
    public void Greet()
    {
        Console.WriteLine(""Hello, World"");
    }
}");
            var assemblyName = Path.GetRandomFileName();
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options:  new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(IsError);
                    throw new InvalidOperationException();
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                Type type = assembly.GetType("Greeter");
                var obj = Activator.CreateInstance(type);
                type.InvokeMember("Greet",
                    BindingFlags.Default | BindingFlags.InvokeMethod,
                    null,
                    obj,
                    null);

            }

            return string.Empty;
        }

        private static bool IsError(Diagnostic diagnostic)
        {
            return diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error;
        }
    }
}
