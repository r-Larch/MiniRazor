using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MiniRazor.Primitives;


namespace MiniRazor.TagHelpers {
    /// <summary>
    /// Base implementation of <see cref="IMiniRazorTemplate"/>.
    /// </summary>
    public abstract partial class MiniRazorTemplateWithTagHelperBase : IMiniRazorTemplate {
        private string? _lastAttributeSuffix;

        /// <inheritdoc />
        public dynamic? Model { get; set; }

        /// <summary>
        /// Template Html Encoder
        /// </summary>
        protected HtmlEncoder HtmlEncoder { get; set; } = HtmlEncoder.Default;

        /// <summary>
        /// Template Output Writer
        /// </summary>
        protected IHtmlContentBuilder Writer { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="MiniRazorTemplateWithTagHelperBase"/>.
        /// </summary>
        protected MiniRazorTemplateWithTagHelperBase()
        {
            Writer = new HtmlContentBuilder();
        }

        /// <inheritdoc />
        public void WriteLiteral(string? literal = null)
        {
            if (literal != null)
                Writer.AppendHtml(encoded: literal);
        }

        private void Write(string? str = null)
        {
            if (str != null)
                WriteLiteral(HtmlEncoder.Encode(str));
        }

        /// <inheritdoc />
        public void Write(object? obj = null)
        {
            switch (obj) {
                case string s:
                    Write(s);
                    break;

                case RawString s:
                    WriteLiteral(s.Value);
                    break;

                case IHtmlContent content:
                    Writer.AppendHtml(content);
                    break;

                default:
                    Write(obj?.ToString());
                    break;
            }
        }

        /// <inheritdoc />
        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            _lastAttributeSuffix = suffix;
            WriteLiteral(prefix);
        }

        /// <inheritdoc />
        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            WriteLiteral(prefix);
            Write(value);
        }

        /// <inheritdoc />
        public void EndWriteAttribute()
        {
            if (_lastAttributeSuffix == null)
                return;

            WriteLiteral(_lastAttributeSuffix);
            _lastAttributeSuffix = null;
        }


        /// <summary>
        /// Wraps a string into a container that instructs the renderer to avoid encoding.
        /// </summary>
        public RawString Raw(string value) => new RawString(value);

        /// <inheritdoc />
        public abstract Task ExecuteAsync();

        /// <inheritdoc />
        public string GetOutput()
        {
            return Writer.GetStringBuilder(HtmlEncoder).ToString();
        }
    }


    public abstract partial class MiniRazorTemplateWithTagHelperBase {
        private readonly struct TagHelperScopeInfo {
            public TagHelperContent Buffer { get; }
            public HtmlEncoder PageHtmlEncoder { get; }
            public IHtmlContentBuilder PageWriter { get; }

            public TagHelperScopeInfo(TagHelperContent buffer, HtmlEncoder encoder, IHtmlContentBuilder pageWriter)
            {
                Buffer = buffer;
                PageHtmlEncoder = encoder;
                PageWriter = pageWriter;
            }
        }


        private Stack<TagHelperScopeInfo> TagHelperScopes { get; } = new Stack<TagHelperScopeInfo>();
        public ITagHelperFactory? TagHelperFactory { get; set; }


        public TTagHelper CreateTagHelper<TTagHelper>() where TTagHelper : ITagHelper
        {
            return this.TagHelperFactory!.CreateTagHelper<TTagHelper>();
        }

        public void StartTagHelperWritingScope(HtmlEncoder encoder)
        {
            var buffer = new DefaultTagHelperContent();

            TagHelperScopes.Push(new TagHelperScopeInfo(buffer, HtmlEncoder, Writer));
            if (encoder != null) {
                HtmlEncoder = encoder;
            }

            Writer = buffer;
        }

        public TagHelperContent EndTagHelperWritingScope()
        {
            if (TagHelperScopes.Count == 0) {
                throw new InvalidOperationException("There is no active writing scope to end!");
            }

            var scope = TagHelperScopes.Pop();

            // restore page writer
            this.HtmlEncoder = scope.PageHtmlEncoder;
            this.Writer = scope.PageWriter;

            return scope.Buffer;
        }


        private IHtmlContentBuilder? _pageWriter;
        private IHtmlContentBuilder? _valueBuffer;

        public void BeginWriteTagHelperAttribute()
        {
            if (_pageWriter != null) {
                throw new InvalidOperationException("Nesting attribute writing scopes not supported!");
            }

            _pageWriter = Writer;
            if (_valueBuffer == null) {
                _valueBuffer = new HtmlContentBuilder();
            }

            Writer = _valueBuffer;
        }

        public string EndWriteTagHelperAttribute()
        {
            if (_pageWriter == null || _valueBuffer == null) {
                throw new InvalidOperationException("There is no active writing scope to end!");
            }

            var result = _valueBuffer.GetStringBuilder(HtmlEncoder).ToString();
            _valueBuffer.Clear();

            // restore page writer
            Writer = _pageWriter;
            _pageWriter = null;

            return result;
        }
    }


    /// <summary>
    /// Generic version of <see cref="MiniRazorTemplateWithTagHelperBase"/>.
    /// </summary>
    public abstract class MiniRazorTemplateWithTagHelperBase<TModel> : MiniRazorTemplateWithTagHelperBase {
        /// <summary>
        /// Template model.
        /// </summary>
        public new TModel Model {
            get => (TModel) (object) base.Model!;
            set => base.Model = value;
        }
    }
}
