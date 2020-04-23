using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace TextTemplate
{
    /// <summary>
    /// This class implements <see cref="ITemplateLanguageHandler"/> for the C# language.
    /// </summary>
    internal sealed class CSharpTemplateHandler : TemplateLanguageHandlerBase
    {
        /// <summary>
        /// Creates the specific <see cref="CodeDomProvider"/> based on the language version.
        /// </summary>
        /// <param name="languageVersion">
        /// The language and version used by the template.
        /// </param>
        /// <returns>
        /// The created <see cref="CodeDomProvider"/>.
        /// </returns>
        protected override CodeDomProvider CreateCodeProvider(string languageVersion)
        {
            return new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", languageVersion.Substring(2) }, });
        }

        /// <summary>
        /// Generates the code for the template.
        /// </summary>
        /// <param name="namespaceImports">
        /// A collection of strings naming namespaces to import into the generated code.
        /// </param>
        /// <param name="codeParts">
        /// A collection of <see cref="string"/>s containing the code parts to generate the code for.
        /// </param>
        /// <param name="parameterType">
        /// The <see cref="Type"/> of the parameter to the template.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter to the template. If left as <see cref="String.Empty"/>, the public readable
        /// properties of the <paramref name="parameterType"/> will be exposed as their own parameters instead.
        /// </param>
        /// <returns>
        /// The generated code.
        /// </returns>
        protected override string GenerateCode(IEnumerable<string> namespaceImports, IEnumerable<string> codeParts, Type parameterType, string parameterName)
        {
            var code = new StringBuilder();
            foreach (string usingDirective in namespaceImports)
                code.AppendFormat("using {0};{1}", usingDirective, NewLine);

            code.Append(NewLine);
            code.Append("namespace GeneratedTextTemplateNamespace" + NewLine);
            code.Append("{" + NewLine);
            code.Append("    public class GeneratedTextTemplateClass" + NewLine);
            code.Append("    {" + NewLine);
            code.Append("        private global::System.Text.StringBuilder _GeneratedTemplateOutput = new global::System.Text.StringBuilder();" + NewLine);
            if (parameterName.Length > 0)
            {
                code.Append("        public global::System.String ExecuteTemplateCode(" + TypeHelper.TypeToString(parameterType, true) + " " + parameterName + ")" + NewLine);
                code.Append("        {" + NewLine);
            }
            else
            {
                code.Append("        public global::System.String ExecuteTemplateCode(" + TypeHelper.TypeToString(parameterType, true) + " templateState)" + NewLine);
                code.Append("        {" + NewLine);
                foreach (PropertyInfo property in parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.CanRead)
                        code.AppendFormat("            var {0} = templateState.{0};{1}", property.Name, NewLine);
                }
            }

            Action<IEnumerable<string>> addCodeParts = delegate(IEnumerable<string> parts)
            {
                foreach (string part in parts)
                {
                    if (part.StartsWith("<%=", StringComparison.Ordinal))
                    {
                        code.AppendFormat("            _GeneratedTemplateOutput.Append({0});{1}", part.Substring(3, part.Length - 5).Trim(), NewLine);
                    }
                    else if (part.StartsWith("<%+", StringComparison.Ordinal))
                    {
                        code.AppendFormat("            {0}{1}", part.Substring(3, part.Length - 5), NewLine);
                    }
                    else if (part.StartsWith("<%", StringComparison.Ordinal))
                    {
                        code.AppendFormat("            {0}{1}", part.Substring(2, part.Length - 4), NewLine);
                    }
                    else
                    {
                        string literal = part.Replace("\"", "\"\"");
                        code.AppendFormat("            _GeneratedTemplateOutput.Append(@\"{0}\");{1}", literal, NewLine);
                    }
                }
            };

            addCodeParts(codeParts.TakeWhile(p => !p.StartsWith("<%+", StringComparison.OrdinalIgnoreCase)));

            code.Append("            return _GeneratedTemplateOutput.ToString();" + NewLine);
            code.Append("        }" + NewLine);

            addCodeParts(codeParts.SkipWhile(p => !p.StartsWith("<%+", StringComparison.OrdinalIgnoreCase)));

            code.Append("    }" + NewLine);
            code.Append("}");
            return code.ToString();
        }
    }
}