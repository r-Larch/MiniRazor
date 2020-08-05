using System;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace MiniRazor.TagHelpers {
    /// <summary>
    /// Provides methods to create and initialize tag helpers.
    /// </summary>
    public interface ITagHelperFactory {
        TTagHelper CreateTagHelper<TTagHelper>() where TTagHelper : ITagHelper;
    }


    public class DefaultTagHelperFactory : ITagHelperFactory {
        public TTagHelper CreateTagHelper<TTagHelper>() where TTagHelper : ITagHelper
        {
            //var instance = (TTagHelper) _serviceProvider.GetService(typeof(TTagHelper));
            //if (instance != null) {
            //    return instance;
            //}

            return Activator.CreateInstance<TTagHelper>();
        }
    }
}
