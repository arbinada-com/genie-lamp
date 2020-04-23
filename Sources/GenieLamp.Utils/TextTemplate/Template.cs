using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TextTemplate
{
    /// <summary>
    /// This class implements the public interface for the text templating engine.
    /// </summary>
    /// <typeparam name="T">
    /// The type of parameter object to pass to the template.
    /// </typeparam>
    /// <example> Basic template using C# as programming language
    /// <code>
    ///   using (var template = new Template())
    ///   {
    ///     template.Content = "Current date and time: &lt;%= DateTime.Now %&gt;";
    ///     var output = template.Execute();
    ///     // output should be: "Current date and time: 08.02.2011 18:00:00", or similar
    ///   }
    /// </code>
    /// Template with flow control
    /// <code>
    ///   using (var template = new Template())
    ///   {
    ///     template.Content = @"&lt;% if (DateTime.Now.Hour &lt; 12) { %&gt;
    ///   Good morning!
    ///   &lt;% } else if (DateTime.Now.Hour &lt; 18) { %&gt;
    ///   Good afternoon
    ///   &lt;% } else { %&gt;
    ///   Good evening
    ///   &lt;% } %&gt;
    ///   ";
    ///     var output = template.Execute();
    ///     // output should be either "Good morning", "Good afternoon" or "Good evening"
    ///   }
    /// </code>
    /// The same template with VB code instead of C# code
    /// <code>
    ///   using (var template = new Template())
    ///   {
    ///     template.Content = @"&lt;%@ language VBv3.5 %&gt;
    ///   &lt;% If DateTime.Now.Hour &lt; 12 Then %&gt;
    ///   Good morning!
    ///   &lt;% ElseIf DateTime.Now.Hour &lt; 18 Then %&gt;
    ///   Good afternoon
    ///   &lt;% Else %&gt;
    ///   Good evening
    ///   &lt;% End If %&gt;
    ///   ";
    ///     var output = template.Execute();
    ///     // output should be either "Good morning", "Good afternoon" or "Good evening"
    ///   }
    /// </code>
    /// Passing parameters to template
    /// <code>
    ///   using (var template = new Template&lt;string&gt;("Name"))
    ///   {
    ///     template.Content = "Hello &lt;%= Name %&gt;!";
    ///     var output = template.Execute("Anders");
    ///     // output is now: "Hello, Anders!"
    ///   }
    /// </code>
    /// Referencing other assemblies and importing namespaces
    /// <code>
    ///   using (var template = new Template())
    ///   {
    ///     template.Content = "&lt;%@ reference System.Drawing %&gt;&lt;%@ using System.Drawing %&gt;&lt;%= Color.Red.ToString() %&gt;";
    ///     var output = template.Execute();
    ///     // output is now: "Color [Red]"
    ///   }
    /// </code>
    /// Adding nested classes and extra methods
    /// <code>
    ///   using (var template = new Template())
    ///   {
    ///     template.Content = @"&lt;% var person = new Person(); Dump(person); %&gt;
    ///   &lt;%+
    ///   public class Person
    ///   {
    ///       public Person()
    ///       {
    ///           Name = ""Anders"";
    ///       }
    ///       
    ///       public string Name { get; set; }
    ///   }
    ///   
    ///   public void Dump(Person p)
    ///   {
    ///   %&gt;
    ///   &lt;%= p.Name %&gt;
    ///   &lt;%
    ///   }
    ///   %&gt;";
    ///     var output = template.Execute();
    ///     // output is now: "Anders"
    ///   }
    /// </code>
    /// </example>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1644:DocumentationHeadersMustNotContainBlankLines", Justification = "Example scode")]
    public class Template<T> : IDisposable
    {
        /// <summary>
        /// The name of the parameter to pass to the template.
        /// </summary>
        private static string _ParameterName;

        /// <summary>
        /// Increasing number to give each <see cref="AppDomain"/> its own unique name.
        /// </summary>
        private static int _TemplateIndex;

        /// <summary>
        /// This is the backing field for the <see cref="Content"/> property.
        /// </summary>
        private string _Content = string.Empty;

        /// <summary>
        /// This field holds a value indicating whether this <see cref="Template{T}"/> instance has been disposed.
        /// </summary>
        private bool _IsDisposed;

        /// <summary>
        /// This field holds the <see cref="AppDomain"/> that will host the template code.
        /// </summary>
        private AppDomain _TemplateDomain;

        /// <summary>
        /// This <see cref="ITemplateProxy"/> object communicates with the <see cref="TemplateProxy"/>
        /// that is hosted in <see cref="_TemplateDomain"/>.
        /// </summary>
        private ITemplateProxy _TemplateProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="Template&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the parameter to pass to the template.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="parameterName"/> is <c>null</c> or empty.</para>
        /// </exception>
        public Template(string parameterName)
        {
            if (StringEx.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentNullException("parameterName");

            _ParameterName = parameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template&lt;T&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// Note that with this constructor, a parameter is still passed to the template, but
        /// all public readable properties of the object will be lifted out as individual parameters to it.
        /// </remarks>
        public Template()
        {
            _ParameterName = string.Empty;
        }

        /// <summary>
        /// Gets a value indicating whether the template has been compiled and is
        /// ready for execution.
        /// </summary>
        /// <value>
        /// <c>true</c> if the template has been compiled;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsCompiled
        {
            get
            {
                AssertNotDisposed();
                return _TemplateDomain != null;
            }
        }

        /// <summary>
        /// Gets or sets the template content.
        /// </summary>
        /// <value>
        /// The template content.
        /// </value>
        public string Content
        {
            get
            {
                AssertNotDisposed();
                return _Content;
            }

            set
            {
                AssertNotDisposed();
                if (value != _Content)
                {
                    Invalidate();
                    _Content = (value ?? string.Empty).Trim();
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_IsDisposed)
            {
                Invalidate();
                _IsDisposed = true;
            }
        }

        /// <summary>
        /// Checks the internal disposed flag and throws <see cref="ObjectDisposedException"/> if it has been set.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// This <see cref="Template{T}"/> instance has been disposed.
        /// </exception>
        protected void AssertNotDisposed()
        {
            if (_IsDisposed)
                throw new ObjectDisposedException(TypeHelper.TypeToString(GetType(), true));
        }

        /// <summary>
        /// Executes the template, returning the generated text, passing the
        /// specified state object to the template code.
        /// </summary>
        /// <param name="parameterValue">
        /// A parameter that will be passed to the template.
        /// </param>
        /// <returns>
        /// The output generated by executing the template.
        /// </returns>
        /// <remarks>
        /// If the template has not yet been compiled, this will be done before
        /// execution, as part of the call to this method.
        /// </remarks>
        public string Execute(T parameterValue)
        {
            AssertNotDisposed();
            if (!IsCompiled)
                Compile();

            Debug.Assert(_TemplateDomain != null, "no domain loaded, Compile should've failed");
            Debug.Assert(_TemplateProxy != null, "no proxy loaded, Compile should've failed");

            return _TemplateProxy.Execute(parameterValue);
        }

        /// <summary>
        /// Invalidates internal data structures, releasing compiled versions of
        /// the template and associated information.
        /// </summary>
        public void Invalidate()
        {
            AssertNotDisposed();
            _TemplateProxy = null;

            if (_TemplateDomain != null)
                AppDomain.Unload(_TemplateDomain);
            _TemplateDomain = null;
        }

        /// <summary>
        /// Compiles the template.
        /// </summary>
        /// <exception cref="TemplateCompilerException">
        /// The template failed to compile, see <see cref="TemplateCompilerException.Errors"/>
        /// for more information about the cause.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// An unsupported language tag or version was specified in the template.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>The compiler was unable to load all the referenced assemblies.</para>
        /// <para>- or -</para>
        /// <para>An invalid directive was specified in the template.</para>
        /// <para>- or -</para>
        /// <para>The code blocks was mismatched in the template.</para>
        /// </exception>
        public void Compile()
        {
            AssertNotDisposed();
            if (IsCompiled)
                return;

            try
            {
                string code = PreProcessCode(_Content, new HashSet<string>());
                CreateDomain();
                _TemplateProxy.Compile(code, LanguageHandlers.GetRegisteredHandlers(), typeof(T), _ParameterName);
            }
            catch (TemplateCompilerException ex)
            {
                Invalidate();
                foreach (CompilerError err in ex.Errors)
                    Debug.WriteLine(err.ErrorNumber + " @ " + err.FileName + "#" + err.Line + "," + err.Column + ": " + err.ErrorText);
                throw;
            }
            catch
            {
                Invalidate();
                throw;
            }
        }

        /// <summary>
        /// Gets the resulting code after transforming the template content into
        /// code for compilation and execution.
        /// </summary>
        public string Code
        {
            get
            {
                return _TemplateProxy.GetCode();
            }
        }

        /// <summary>
        /// Pre-processes the template code to handle include-directives.
        /// </summary>
        /// <param name="content">
        /// The content to pre-process.
        /// </param>
        /// <param name="inclusionStack">
        /// This structure is used to detect cycles in included content, where one included
        /// piece of content includes another, which again includes the first.
        /// </param>
        /// <returns>
        /// The preprocessed code.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="content"/> is <c>null</c>.</para>
        /// </exception>
        private string PreProcessCode(string content, HashSet<string> inclusionStack)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            var parser = new TemplateParser(content);
            parser.Parse();

            var sb = new StringBuilder();
            if (inclusionStack.Count == 0)
                sb.AppendFormat("<%@ language {0} %>{1}", parser.Language, Environment.NewLine);

            foreach (string reference in parser.AssemblyReferences)
                sb.AppendFormat("<%@ references {0} %>{1}", reference, Environment.NewLine);
            foreach (string import in parser.NamespaceImports)
                sb.AppendFormat("<%@ using {0} %>{1}", import, Environment.NewLine);

            foreach (string part in parser.CodeParts)
            {
                if (part.StartsWith("<%@", StringComparison.Ordinal))
                {
                    string subContent = part.Substring(3, part.Length - 5).Trim();
                    if (subContent.StartsWith("include ", StringComparison.OrdinalIgnoreCase))
                    {
                        string name = subContent.Substring(7).Trim();
                        if (inclusionStack.Contains(name))
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "A cycle is detected in included content, was triggered by {0}, stack already contains {1}", name, string.Join(", ", inclusionStack.ToArray())));
                        var includeEventArgs = new IncludeEventArgs(name);
                        OnInclude(includeEventArgs);
                        if (!includeEventArgs.Handled)
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to include content named '{0}'", name));
                        inclusionStack.Add(name);
                        sb.Append(PreProcessCode(includeEventArgs.Content, inclusionStack));
                        inclusionStack.Remove(name);
                    }
                    else
                        sb.Append(part);
                }
                else
                    sb.Append(part);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates the <see cref="AppDomain"/> that will host the compiled code.
        /// </summary>
        private void CreateDomain()
        {
            string templateDomainFriendlyName = "TextTemplate TemplateDomain #" + (++_TemplateIndex);
            var setup = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), };
            _TemplateDomain = AppDomain.CreateDomain(templateDomainFriendlyName, null, setup);
            _TemplateProxy = (ITemplateProxy)_TemplateDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "TextTemplate.TemplateProxy");
        }

        /// <summary>
        /// This event is fired if the template attempts to include files. If this event is
        /// not handled, the inclusion attempt will fail.
        /// </summary>
        public event EventHandler<IncludeEventArgs> Include;

        /// <summary>
        /// Raises the <see cref="Include"/> event.
        /// </summary>
        /// <param name="e">The <see cref="TextTemplate.IncludeEventArgs"/> instance containing the event data.</param>
        protected virtual void OnInclude(IncludeEventArgs e)
        {
            if (Include != null)
                Include(this, e);
        }
    }
}