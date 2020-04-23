using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TextTemplate
{
    /// <summary>
    /// This interface must be implemented by classes that handles specific languages.
    /// </summary>
    public interface ITemplateLanguageHandler
    {
        /// <summary>
        /// Rewrite the specific code parts into a source code file that can
        /// be compiled, compiles it, and returns the compiled assembly.
        /// </summary>
        /// <param name="language">
        /// The language to write the code for.
        /// </param>
        /// <param name="namespaceImports">
        /// A collection of strings naming namespaces to import into the generated code.
        /// </param>
        /// <param name="assemblyReferences">
        /// A collection of strings naming assemblies to reference when compiling the code.
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
        /// <param name="code">
        /// Upon return of this method, this parameter will contain the code that was compiled, or was attempted compiled.
        /// </param>
        /// <returns>
        /// The generated <see cref="Assembly"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="language"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="namespaceImports"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="assemblyReferences"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="codeParts"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="parameterType"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="parameterName"/> is <c>null</c>.</para>
        /// </exception>
        Assembly RewriteAndCompile(string language, IEnumerable<string> namespaceImports, IEnumerable<string> assemblyReferences, IEnumerable<string> codeParts, Type parameterType, string parameterName, out string code);
    }
}