using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniRazor.Internal.Extensions;

namespace MiniRazor
{
    /// <summary>
    /// Compiled Razor template which can be used to render output.
    /// </summary>
    public class MiniRazorTemplateDescriptor
    {
        private readonly Type _templateType;
        private readonly ICollection<IMiniRazorExtension> _extensions;

        /// <summary>
        /// Initializes an instance of <see cref="MiniRazorTemplateDescriptor"/>.
        /// </summary>
        public MiniRazorTemplateDescriptor(Type templateType, ICollection<IMiniRazorExtension> extensions)
        {
            _templateType = templateType;
            _extensions = extensions;
        }

        private IMiniRazorTemplate ActivateTemplate()
        {
            var template = (IMiniRazorTemplate) (Activator.CreateInstance(_templateType) ?? throw new InvalidOperationException($"Could not instantiate template of type '{_templateType}'."));

            foreach (var extension in _extensions) {
                extension.ActivateTemplate(template);
            }

            return template;
        }

        /// <summary>
        /// Renders the template with the specified model.
        /// </summary>
        public async Task<string> RenderAsync(object? model = null)
        {
            var template = ActivateTemplate();

            template.Model = model?.GetType().IsAnonymousType() == true
                ? model?.ToExpando()
                : model;

            await template.ExecuteAsync();

            return template.GetOutput();
        }
    }
}