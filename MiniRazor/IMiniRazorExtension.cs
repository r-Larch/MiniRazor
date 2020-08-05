using Microsoft.AspNetCore.Razor.Language;


namespace MiniRazor {
    /// <summary>
    ///
    /// </summary>
    public interface IMiniRazorExtension {
        void Configure(RazorProjectEngineBuilder builder, ExtensionContext context);
        void ActivateTemplate(IMiniRazorTemplate template);
    }
}
