using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic;

namespace TextTemplate
{
    /// <summary>
    /// This class implements <see cref="ITemplateLanguageHandler"/> for the Visual Basic .NET language.
    /// </summary>
    internal sealed class VisualBasicTemplateHandler : TemplateLanguageHandlerBase
    {
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
            string crlf = Environment.NewLine;

            namespaceImports = namespaceImports.Concat(new[] { "Microsoft.VisualBasic" }).Distinct();
            foreach (string usingDirective in namespaceImports)
                code.AppendFormat("Imports {0}{1}", usingDirective, crlf);

            code.Append(crlf);
            code.Append("Namespace GeneratedTextTemplateNamespace" + crlf);
            code.Append("    Public Class GeneratedTextTemplateClass" + crlf);
            code.Append("        Private _GeneratedTemplateOutput As System.Text.StringBuilder = New System.Text.StringBuilder()" + crlf);

            if (parameterName.Length > 0)
                code.Append("        Public Function ExecuteTemplateCode(ByVal " + parameterName + " As " + TypeHelper.TypeToString(parameterType, false) + ") As System.String" + crlf);
            else
            {
                code.Append("        Public Function ExecuteTemplateCode(ByVal templateState As " + TypeHelper.TypeToString(parameterType, false) + ") As System.String" + crlf);
                foreach (PropertyInfo property in parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.CanRead)
                        code.AppendFormat("            Dim {0} As {1} = templateState.{0}{2}", property.Name, TypeHelper.TypeToString(property.PropertyType, false), crlf);
                }
            }

            Action<IEnumerable<string>> addCodeParts = delegate(IEnumerable<string> parts)
            {
                foreach (string part in parts)
                {
                    if (part.StartsWith("<%=", StringComparison.Ordinal))
                    {
                        code.AppendFormat("            _GeneratedTemplateOutput.Append({0}){1}", part.Substring(3, part.Length - 5).Trim(), crlf);
                    }
                    else if (part.StartsWith("<%+", StringComparison.Ordinal))
                    {
                        code.AppendFormat("            {0}{1}", part.Substring(3, part.Length - 5), crlf);
                    }
                    else if (part.StartsWith("<%", StringComparison.Ordinal))
                    {
                        code.AppendFormat("            {0}{1}", part.Substring(2, part.Length - 4), crlf);
                    }
                    else
                    {
                        string literal = part.Replace("\"", "\"\"");
                        while (literal.Length > 0)
                        {
                            int index1 = literal.IndexOf('\n');
                            int index2 = literal.IndexOf('\r');
                            if (index1 < 0 && index2 < 0)
                            {
                                code.AppendFormat("            _GeneratedTemplateOutput.Append(\"{0}\"){1}", literal, crlf);
                                literal = string.Empty;
                            }
                            else
                            {
                                int first;
                                if (index1 < 0)
                                    first = index2;
                                else if (index2 < 0)
                                    first = index1;
                                else
                                    first = Math.Min(index1, index2);

                                if (first > 0)
                                    code.AppendFormat("            _GeneratedTemplateOutput.Append(\"{0}\"){1}", literal.Substring(0, first), crlf);
                                if (first + 1 < literal.Length && literal.Substring(first, 2) == "\r\n")
                                {
                                    code.AppendFormat("            _GeneratedTemplateOutput.Append(vbCrlf){1}", (int)literal[first], crlf);
                                    literal = literal.Substring(first + 2);
                                }
                                else if (literal[first] == '\r')
                                {
                                    code.AppendFormat("            _GeneratedTemplateOutput.Append(Constants.vbCr){1}", (int)literal[first], crlf);
                                    literal = literal.Substring(first + 1);
                                }
                                else
                                {
                                    code.AppendFormat("            generatedTemplateOutput.Append(Constants.vbLf){1}", (int)literal[first], crlf);
                                    literal = literal.Substring(first + 1);
                                }
                            }
                        }
                    }
                }
            };

            addCodeParts(codeParts.TakeWhile(p => !p.StartsWith("<%+", StringComparison.OrdinalIgnoreCase)));

            code.Append("            Return _GeneratedTemplateOutput.ToString()" + crlf);
            code.Append("        End Function" + crlf);

            addCodeParts(codeParts.SkipWhile(p => !p.StartsWith("<%+", StringComparison.OrdinalIgnoreCase)));

            code.Append("    End Class" + crlf);
            code.Append("End Namespace");

            return code.ToString();
        }

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
            return new VBCodeProvider(new Dictionary<string, string>
            {
                { "CompilerVersion", languageVersion.Substring(2) },
            });
        }
    }
}