using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextTemplate
{
    /// <summary>
    /// This class holds the registered <see cref="ITemplateLanguageHandler"/> objects.
    /// </summary>
    public static class LanguageHandlers
    {
        /// <summary>
        /// This dictionary holds registered languages and their handler types.
        /// </summary>
        private static readonly Dictionary<string, Type> _RegisteredHandlers = new Dictionary<string, Type>
        {
            { "C#v3.5", typeof(CSharpTemplateHandler) },
            { "C#v4.0", typeof(CSharpTemplateHandler) },
            { "VBv3.5", typeof(VisualBasicTemplateHandler) },
            { "VBv4.0", typeof(VisualBasicTemplateHandler) },
        };

        /// <summary>
        /// Gets the registered handlers.
        /// </summary>
        /// <returns>
        /// The dictionary containing the registered language handlers.
        /// </returns>
        internal static Dictionary<string, Type> GetRegisteredHandlers()
        {
            return _RegisteredHandlers;
        }

        /// <summary>
        /// Registers the language and its handler.
        /// </summary>
        /// <param name="language">
        /// The language identifier.
        /// </param>
        /// <param name="handlerType">
        /// The type that will process the given <paramref name="language"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="language"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="handlerType"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="handlerType"/> is abstract.</para>
        /// <para>- or -</para>
        /// <para><paramref name="handlerType"/> is generic.</para>
        /// <para>- or -</para>
        /// <para><paramref name="handlerType"/> does not implement <see cref="ITemplateLanguageHandler"/>.</para>
        /// </exception>
        public static void RegisterHandler(string language, Type handlerType)
        {
            if (StringEx.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException("language");
            if (handlerType == null)
                throw new ArgumentNullException("handlerType");
            if (!typeof(ITemplateLanguageHandler).IsAssignableFrom(handlerType))
                throw new ArgumentException("The specified handlerType does not implement ITemplateLanguageHandler");
            if (handlerType.IsGenericType || handlerType.IsAbstract)
                throw new ArgumentException("The specified handlerType is generic, or abstract");

            _RegisteredHandlers[language] = handlerType;
        }
    }
}