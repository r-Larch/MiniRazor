﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;


namespace MiniRazor.TagHelpers {
    public static class ModelDirective {
        public static readonly DirectiveDescriptor Directive = DirectiveDescriptor.CreateDirective(
            "model",
            DirectiveKind.SingleLine,
            builder => {
                builder.AddTypeToken("TypeName", "The model type.");
                builder.Usage = DirectiveUsage.FileScopedSinglyOccurring;
                builder.Description = "Specify the view or page model for the page.";
            });

        public static RazorProjectEngineBuilder Register(RazorProjectEngineBuilder builder)
        {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddDirective(Directive);
            builder.Features.Add(new Pass());
            return builder;
        }

        public static string GetModelType(DocumentIntermediateNode document)
        {
            if (document == null) {
                throw new ArgumentNullException(nameof(document));
            }

            var visitor = new Visitor();
            return GetModelType(document, visitor);
        }

        private static string GetModelType(DocumentIntermediateNode document, Visitor visitor)
        {
            visitor.Visit(document);

            for (var i = visitor.ModelDirectives.Count - 1; i >= 0; i--) {
                var directive = visitor.ModelDirectives[i];

                var tokens = directive.Tokens.ToArray();
                if (tokens.Length >= 1) {
                    return tokens[0].Content;
                }
            }

            return "dynamic";
        }

        internal class Pass : IntermediateNodePassBase, IRazorDirectiveClassifierPass {
            // Runs after the @inherits directive
            public override int Order => 5;

            protected override void ExecuteCore(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode)
            {
                var visitor = new Visitor();
                var modelType = GetModelType(documentNode, visitor);

                var baseType = visitor.Class.BaseType?.Replace("<TModel>", $"<{modelType}>");
                visitor.Class.BaseType = baseType;
            }
        }

        private class Visitor : IntermediateNodeWalker {
            public NamespaceDeclarationIntermediateNode Namespace { get; private set; }

            public ClassDeclarationIntermediateNode Class { get; private set; }

            public IList<DirectiveIntermediateNode> ModelDirectives { get; } = new List<DirectiveIntermediateNode>();

            public override void VisitNamespaceDeclaration(NamespaceDeclarationIntermediateNode node)
            {
                if (Namespace == null) {
                    Namespace = node;
                }

                base.VisitNamespaceDeclaration(node);
            }

            public override void VisitClassDeclaration(ClassDeclarationIntermediateNode node)
            {
                if (Class == null) {
                    Class = node;
                }

                base.VisitClassDeclaration(node);
            }

            public override void VisitDirective(DirectiveIntermediateNode node)
            {
                if (node.Directive == Directive) {
                    ModelDirectives.Add(node);
                }
            }
        }
    }
}
