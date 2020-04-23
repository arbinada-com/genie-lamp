using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextTemplate
{
    /// <summary>
    /// This interface is implemented by <see cref="TemplateProxy"/> and used
    /// by <see cref="Template{T}"/> to talk to the proxy hosted in its own
    /// <see cref="AppDomain"/>.
    /// </summary>
    internal interface ITemplateProxy
    {
        /// <summary>
        /// Compiles the template content.
        /// </summary>
        /// <param name="content">
        /// The template content to compile.
        /// </param>
        /// <param name="handlers">
        /// The set of known languages and their handler types.
        /// </param>
        /// <param name="parameterType">
        /// The <see cref="Type"/> of the parameter to the template.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter to the template. If left as <see cref="String.Empty"/>, the public readable
        /// properties of the <paramref name="parameterType"/> will be exposed as their own parameters instead.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="content"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="handlers"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="parameterType"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="parameterName"/> is <c>null</c>.</para>
        /// </exception>
        void Compile(string content, Dictionary<string, Type> handlers, Type parameterType, string parameterName);

        /// <summary>
        /// Executes the template and returns the generated output.
        /// </summary>
        /// <param name="templateState">
        /// The state for the template to process.
        /// </param>
        /// <returns>
        /// The generated output.
        /// </returns>
        string Execute(object templateState);

        /// <summary>
        /// Returns the code that was compiled, for debugging purposes.
        /// </summary>
        /// <returns>
        /// The code that was compiled.
        /// </returns>
        string GetCode();
    }
}