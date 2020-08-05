using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;


namespace MiniRazor {
    /// <summary>
    /// Context information for Extensions
    /// </summary>
    public class ExtensionContext {
        /// <summary>
        /// The references for the new Compilation
        /// </summary>
        public IList<MetadataReference> MetadataReferences { get; }

        /// <summary>
        /// ImportSources for compilation
        /// </summary>
        public IList<RazorSourceDocument> ImportSources { get; set; } = new List<RazorSourceDocument>();

        /// <summary>
        /// TagHelperDescriptor available for compilation
        /// </summary>
        public IList<TagHelperDescriptor> TagHelperDescriptors { get; set; } = new List<TagHelperDescriptor>();

        /// <summary>
        /// Initializes an instance of <see cref="ExtensionContext"/>.
        /// </summary>
        public ExtensionContext(IEnumerable<MetadataReference> metadataReferences)
        {
            MetadataReferences = metadataReferences.ToList();
        }
    }
}
