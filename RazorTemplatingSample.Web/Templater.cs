using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.Generator;
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
using RazorTemplatingSample.Web;

class Greeter
{
    public void Greet()
    {
        var person = new Person { Name = ""foo"" };
        Console.WriteLine(""Hello, World"" + person.Name);
    }
}");
            var razorGeneratedCode = GetRazorSyntaxTree();
            var razorSyntaxTree = CSharpSyntaxTree.ParseText(razorGeneratedCode);
            var assemblyName = Path.GetRandomFileName();
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Templater).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { razorSyntaxTree },
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

                //Type type = assembly.GetType("Greeter");
                //var obj = Activator.CreateInstance(type);
                //type.InvokeMember("Greet",
                //    BindingFlags.Default | BindingFlags.InvokeMethod,
                //    null,
                //    obj,
                //    null);

                Type[] types = assembly.GetTypes();
            }

            return string.Empty;
        }

        private static string GetRazorSyntaxTree()
        {
            const string rootNamespace = "RazorOnConsole";
            const string mainClassNamePrefix = "ASPV_";
            var viewPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Views\Index.cshtml");
            var fileName = Path.GetFileName(viewPath);
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            var className = mainClassNamePrefix + fileNameNoExtension;

            var codeLang = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(codeLang)
            {
                DefaultBaseClass = typeof(BaseView).FullName,
                GeneratedClassContext = new GeneratedClassContext(
                    executeMethodName: GeneratedClassContext.DefaultExecuteMethodName,
                    writeMethodName: GeneratedClassContext.DefaultWriteMethodName,
                    writeLiteralMethodName: GeneratedClassContext.DefaultWriteLiteralMethodName,
                    writeToMethodName: "WriteTo",
                    writeLiteralToMethodName: "WriteLiteralTo",
                    templateTypeName: "HelperResult",
                    defineSectionMethodName: "DefineSection",
                    generatedTagHelperContext: new GeneratedTagHelperContext())
            };

            host.NamespaceImports.Add("System");

            var engine = new RazorTemplateEngine(host);

            using (var fileStream = File.OpenText(viewPath))
            {
                GeneratorResults code = engine.GenerateCode(
                    input: fileStream,
                    className: className,
                    rootNamespace: rootNamespace,
                    sourceFileName: fileName);

                return code.GeneratedCode;
            }
        }

        private static bool IsError(Diagnostic diagnostic)
        {
            return diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error;
        }
    }
}
