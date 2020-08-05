using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;


namespace MiniRazor.TagHelpers.Tests {
    public class TemplateTagHelper {
        [Fact]
        public async Task I_can_use_tag_helper()
        {
            var engine = new MiniRazorTemplateEngine();

            engine.AddTagHelpers();

            var template = engine.Compile(
                "@addTagHelper *, MiniRazor.TagHelpers.Tests\r\n" +
                "Hello, <my-tag-helper/>"
            );

            // Act
            var result = await template.RenderAsync();

            // Assert
            result.Should().Be("Hello, [ TagHelper Output Attribute='' ]");
        }


        [Fact]
        public async Task I_can_use_tag_helper_attributes()
        {
            var engine = new MiniRazorTemplateEngine();

            engine.AddTagHelpers();

            var template = engine.Compile(
                "@addTagHelper *, MiniRazor.TagHelpers.Tests\r\n" +
                "Hello, <my-tag-helper attribute=\"hoi\"/>"
            );

            // Act
            var result = await template.RenderAsync();

            // Assert
            result.Should().Be("Hello, [ TagHelper Output Attribute='hoi' ]");
        }
    }
}


[HtmlTargetElement("my-tag-helper")]
public class MyTagHelper : TagHelper {
    public string? Attribute { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.Content.AppendHtml($"[ TagHelper Output Attribute='{Attribute}' ]");

        base.Process(context, output);
    }
}
