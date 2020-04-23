using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TextTemplate
{
    /// <summary>
    /// This class handles rewriting the template content into normal compilable code.
    /// </summary>
    internal class TemplateParser
    {
        private readonly List<string> _AssemblyReferences = new List<string>();
        private readonly List<string> _NamespaceImports = new List<string>();
        private readonly string _TemplateContent;
        private List<string> _CodeParts = new List<string>();
        private string _Language = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateParser"/> class.
        /// </summary>
        /// <param name="templateContent">
        /// The template content to rewrite.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="templateContent"/> is <c>null</c>.</para>
        /// </exception>
        public TemplateParser(string templateContent)
        {
            if (templateContent == null)
                throw new ArgumentNullException("templateContent");

            _TemplateContent = templateContent;
        }

        /// <summary>
        /// Gets the language to override the template language with, or <c>null</c> if the
        /// one specified by the <see cref="Template{T}"/> instance should be used.
        /// </summary>
        public string Language
        {
            get
            {
                return _Language;
            }
        }

        /// <summary>
        /// Gets the collection of code parts from the template.
        /// </summary>
        public IEnumerable<string> CodeParts
        {
            get
            {
                return _CodeParts;
            }
        }

        /// <summary>
        /// Gets the collection of namespaces to import when compiling the template.
        /// </summary>
        public IEnumerable<string> NamespaceImports
        {
            get
            {
                return _NamespaceImports;
            }
        }

        /// <summary>
        /// Gets a collection of referenced assemblies.
        /// </summary>
        public IEnumerable<string> AssemblyReferences
        {
            get
            {
                return _AssemblyReferences;
            }
        }

        /// <summary>
        /// Parses the content of the template into separate parts.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>An invalid code block was found, either it contained another code block, or it did not complete.</para>
        /// </exception>
        public void Parse()
        {
            var codeParts = new List<string>();

            var tokenizer = new TemplateTokenizer(_TemplateContent);
            while (tokenizer.More)
            {
                TemplateToken token = tokenizer.Peek();
                if (token.Type == TemplateTokenType.CodeBlockStart)
                {
                    TemplateToken startToken = tokenizer.Next();

                    TemplateToken firstToken = tokenizer.Peek();
                    bool skipLineBreakAfterBlock = firstToken.Type != TemplateTokenType.Character || firstToken.Token != "=";

                    var codeBlock = new StringBuilder(token.Token);
                    bool keepParsing = true;
                    while (keepParsing)
                    {
                        token = tokenizer.Peek();
                        switch (token.Type)
                        {
                            case TemplateTokenType.Character:
                            case TemplateTokenType.LineBreak:
                                codeBlock.Append(token.Token);
                                tokenizer.Next();
                                break;

                            case TemplateTokenType.CodeBlockEnd:
                                codeBlock.Append(token.Token);
                                tokenizer.Next();
                                keepParsing = false;
                                break;

                            case TemplateTokenType.End:
                                throw new TemplateSyntaxException(string.Format(CultureInfo.InvariantCulture, "Code block that starts at position {0}, line {1}, does not complete", startToken.Position, startToken.LineNumber));

                            default:
                                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported token from template parser: {0}", token));
                        }
                    }

                    codeParts.Add(codeBlock.ToString());
                    if (skipLineBreakAfterBlock)
                    {
                        if (tokenizer.Peek().Type == TemplateTokenType.LineBreak)
                            tokenizer.Next();
                    }
                }
                else
                {
                    var literal = new StringBuilder();
                    bool keepParsing = true;
                    while (keepParsing)
                    {
                        token = tokenizer.Peek();
                        switch (token.Type)
                        {
                            case TemplateTokenType.Character:
                            case TemplateTokenType.LineBreak:
                                literal.Append(token.Token);
                                tokenizer.Next();
                                break;

                            case TemplateTokenType.CodeBlockStart:
                                keepParsing = false;
                                break;

                            case TemplateTokenType.End:
                                keepParsing = false;
                                break;

                            default:
                                throw new TemplateSyntaxException(string.Format(CultureInfo.InvariantCulture, "Unsupported token from template parser: {0}", token));
                        }
                    }

                    codeParts.Add(literal.ToString());
                }
            }

            IEnumerable<string> statementBlocks =
                from part in codeParts
                where
                    !part.StartsWith("<%@", StringComparison.Ordinal) ||
                    (part.StartsWith("<%@", StringComparison.Ordinal) &&
                     part.Substring(3, part.Length - 5).Trim().StartsWith("include ", StringComparison.OrdinalIgnoreCase))
                select part;
            IEnumerable<string> directives =
                from part in codeParts
                where
                    part.StartsWith("<%@", StringComparison.Ordinal) &&
                    !part.Substring(3, part.Length - 5).Trim().StartsWith("include ", StringComparison.OrdinalIgnoreCase)
                select part.Substring(3, part.Length - 5).Trim();

            var imports = new HashSet<string>();
            _Language = "C#v3.5";
            var references = new HashSet<string>();

            imports.Add("System");
            imports.Add("System.Text");
            imports.Add("TextTemplate");

            foreach (string directive in directives)
            {
                string[] parts = directive.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                bool directiveIsGood = false;
                if (parts.Length == 2)
                {
                    switch (parts[0])
                    {
                        case "use":
                        case "using":
                        case "import":
                            directiveIsGood = true;
                            imports.Add(parts[1]);
                            break;

                        case "language":
                            switch (parts[1].ToUpper(CultureInfo.InvariantCulture))
                            {
                                case "C#":
                                case "C#V3.5":
                                case "C#3.5":
                                    _Language = "C#v3.5";
                                    break;

                                case "C#V4.0":
                                case "C#4":
                                case "C#V4":
                                case "C#4.0":
                                    _Language = "C#v4.0";
                                    break;

                                default:
                                    _Language = parts[1];
                                    break;
                            }
                            directiveIsGood = true;
                            break;

                        case "ref":
                        case "reference":
                        case "references":
                            string reference = parts[1];
                            if (Path.GetExtension(reference).ToLower(CultureInfo.InvariantCulture) != ".dll")
                                reference += ".dll";
                            references.Add(reference);
                            directiveIsGood = true;
                            break;
                    }
                }
                if (!directiveIsGood)
                    throw new TemplateSyntaxException(string.Format(CultureInfo.InvariantCulture, "Invalid directive found in template: {0}", directive));
            }

            _NamespaceImports.Clear();
            _NamespaceImports.AddRange(imports);

            _AssemblyReferences.Clear();
            _AssemblyReferences.AddRange(references);

            _CodeParts = new List<string>(statementBlocks);
        }
    }
}