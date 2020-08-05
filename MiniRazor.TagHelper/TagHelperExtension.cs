using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;


namespace MiniRazor.TagHelpers {
    internal class TagHelperExtension : IMiniRazorExtension {
        public void Configure(RazorProjectEngineBuilder builder, ExtensionContext context)
        {
            context.MetadataReferences.Add(
                MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Razor.TagHelpers.ITagHelper).Assembly.Location)
            );
            context.MetadataReferences.Add(
                MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner).Assembly.Location)
            );

            context.TagHelperDescriptors = new CompilationTagHelperFeature(context.MetadataReferences)
                .GetDescriptors()
                .ToList();

            builder.SetBaseType($"{typeof(MiniRazorTemplateWithTagHelperBase).FullName}<TModel>");

            ModelDirective.Register(builder);
        }


        public void ActivateTemplate(IMiniRazorTemplate template)
        {
            if (template is MiniRazorTemplateWithTagHelperBase x) {
                x.TagHelperFactory = new DefaultTagHelperFactory();
            }
        }
    }
}
