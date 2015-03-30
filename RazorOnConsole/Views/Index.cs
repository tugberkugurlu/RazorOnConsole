namespace RazorOnConsole
{
#line 1 "Index.cshtml"
using System.Linq

#line default
#line hidden
    ;
    using System;
    using System.Threading.Tasks;

    public class Index : RazorOnConsole.Views.BaseView
    {
#line 9 "Index.cshtml"

    public Index(string model)
    {
        Model = model;
    }

    public string Model { get; set; }

#line default
#line hidden
        #line hidden
        public Index()
        {
        }

        #pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#line 3 "Index.cshtml"
  
    var foo = "bar" + "foo";
    var link = "http://tugberkugurlu.com";

#line default
#line hidden

            WriteLiteral("\r\n\r\n");
            WriteLiteral("\r\n<a");
            WriteAttribute("href", Tuple.Create(" href=\"", 238), Tuple.Create("\"", 250), 
            Tuple.Create(Tuple.Create("", 245), Tuple.Create<System.Object, System.Int32>(link, 245), false));
            WriteLiteral(">");
#line 18 "Index.cshtml"
           Write(foo);

#line default
#line hidden
            WriteLiteral("</a>\r\n\r\n");
#line 20 "Index.cshtml"
 foreach (var i in Enumerable.Range(0, 10))
{

#line default
#line hidden

            WriteLiteral("     <a href=\"#");
#line 22 "Index.cshtml"
             Write(i);

#line default
#line hidden
            WriteLiteral("\">");
#line 22 "Index.cshtml"
                   Write(i);

#line default
#line hidden
            WriteLiteral("</a>\r\n");
#line 23 "Index.cshtml"
}

#line default
#line hidden

            WriteLiteral("\r\n<p>This is model: ");
#line 25 "Index.cshtml"
             Write(Model);

#line default
#line hidden
            WriteLiteral("..</p>");
        }
        #pragma warning restore 1998
    }
}
