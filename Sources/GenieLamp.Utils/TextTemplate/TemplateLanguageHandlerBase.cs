using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TextTemplate
{
    /// <summary>
    /// This is a base class that implements some general utility methods and skeleton code
    /// for a <see cref="ITemplateLanguageHandler"/> implementation.
    /// </summary>
    public abstract class TemplateLanguageHandlerBase : ITemplateLanguageHandler
    {
        /// <summary>
        /// Gets the <see cref="Environment.NewLine"/> value, just to make code easier to write.
        /// </summary>
        protected string NewLine
        {
            get
            {
                return Environment.NewLine;
            }
        }

        #region ITemplateLanguageHandler Members

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
        /// The generated assembly.
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
        public Assembly RewriteAndCompile(
            string language, IEnumerable<string> namespaceImports, IEnumerable<string> assemblyReferences, IEnumerable<string> codeParts, Type parameterType,
            string parameterName, out string code)
        {
            if (StringEx.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException("language");
            if (namespaceImports == null)
                throw new ArgumentNullException("namespaceImports");
            if (assemblyReferences == null)
                throw new ArgumentNullException("assemblyReferences");
            if (codeParts == null)
                throw new ArgumentNullException("codeParts");
            if (parameterType == null)
                throw new ArgumentNullException("parameterType");
            if (parameterName == null)
                throw new ArgumentNullException("parameterName");

            code = GenerateCode(namespaceImports, codeParts, parameterType, parameterName);

            var referencedAssemblies = new List<string>();
            referencedAssemblies.Add("System.dll");
            referencedAssemblies.Add("TextTemplate.dll");
            referencedAssemblies.Add(Path.GetFileName(parameterType.Assembly.Location));
            foreach (string reference in assemblyReferences)
                referencedAssemblies.Add(reference);

            var parameters = new CompilerParameters();
            foreach (string assemblyName in referencedAssemblies.Distinct())
                parameters.ReferencedAssemblies.Add(assemblyName);

            // Make local references absolute
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            for (int index = 0; index < parameters.ReferencedAssemblies.Count; index++)
            {
                string filename = parameters.ReferencedAssemblies[index];
                if (!Path.IsPathRooted(filename))
                {
                    if (File.Exists(Path.Combine(appPath, filename)))
                        parameters.ReferencedAssemblies[index] = Path.Combine(appPath, filename);
                }
            }
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = false;
            parameters.TreatWarningsAsErrors = false;

            CodeDomProvider codeProvider = CreateCodeProvider(language);

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.Count > 0)
            {
                throw new TemplateCompilerException(
                    "Errors during compilation of template application domain " + AppDomain.CurrentDomain.FriendlyName, code, results.Errors);
            }

            Assembly assembly = results.CompiledAssembly;

            foreach (string reference in parameters.ReferencedAssemblies)
            {
                if (Path.IsPathRooted(reference))
                {
                    bool success = Assembly.LoadFile(reference) != null;
                    if (!success)
                        throw new InvalidOperationException("Unable to load referenced assembly: " + reference);
                }
            }

            return assembly;
        }

        #endregion

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
        protected abstract string GenerateCode(IEnumerable<string> namespaceImports, IEnumerable<string> codeParts, Type parameterType, string parameterName);

        /// <summary>
        /// Creates the specific <see cref="CodeDomProvider"/> based on the language version.
        /// </summary>
        /// <param name="languageVersion">
        /// The language and version used by the template.
        /// </param>
        /// <returns>
        /// The created <see cref="CodeDomProvider"/>.
        /// </returns>
        protected abstract CodeDomProvider CreateCodeProvider(string languageVersion);
    }
}