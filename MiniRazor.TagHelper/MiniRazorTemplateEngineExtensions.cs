using System;


namespace MiniRazor.TagHelpers {
    public static class MiniRazorTemplateEngineExtensions {
        public static MiniRazorTemplateEngine AddTagHelpers(this MiniRazorTemplateEngine engine)
        {
            engine.Extensions.Add(new TagHelperExtension());

            return engine;
        }
    }
}
