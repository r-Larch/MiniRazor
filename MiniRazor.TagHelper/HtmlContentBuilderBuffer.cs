using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;


namespace MiniRazor.TagHelpers {
    public static class HtmlContentBuilderExtensions {
        public static StringBuilder GetStringBuilder(this IHtmlContentBuilder builder, HtmlEncoder encoder)
        {
            var writer = new StringWriter();
            builder.WriteTo(writer, encoder);
            return writer.GetStringBuilder();
        }
    }
}
