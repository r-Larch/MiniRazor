using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Razor;


namespace MiniRazor.TagHelpers {
    internal sealed class CompilationTagHelperFeature : RazorEngineFeatureBase, ITagHelperFeature {
        private readonly IEnumerable<MetadataReference> _references;

        public CompilationTagHelperFeature(IEnumerable<MetadataReference> references)
        {
            _references = references;
        }

        public IReadOnlyList<TagHelperDescriptor> GetDescriptors()
        {
            var compilation = CSharpCompilation.Create("__TagHelpers", references: _references);
            if (IsValidCompilation(compilation)) {
                var results = new List<TagHelperDescriptor>();
                var context = TagHelperDescriptorProviderContext.Create(results);
                context.SetCompilation(compilation);
                new DefaultTagHelperDescriptorProvider().Execute(context);
                return results;
            }

            return new TagHelperDescriptor[0];
        }

        internal static bool IsValidCompilation(Compilation compilation)
        {
            var @string = compilation.GetSpecialType(SpecialType.System_String);

            // Do some minimal tests to verify the compilation is valid. If symbols for System.String
            // is missing or errored, the compilation may be missing references.
            return @string != null && @string.TypeKind != TypeKind.Error;
        }
    }
}
