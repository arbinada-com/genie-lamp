using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace TextTemplate
{
    /// <summary>
    /// This <see cref="TemplateException"/> descendant is used to flag syntax errors
    /// in the template structure, usually an imbalance in code block tokens.
    /// </summary>
    [Serializable]
    public class TemplateSyntaxException : TemplateException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateSyntaxException"/> class.
        /// </summary>
        public TemplateSyntaxException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateSyntaxException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public TemplateSyntaxException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateSyntaxException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException parameter is not a null reference
        /// (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public TemplateSyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateSyntaxException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The SerializationInfo that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The StreamingContext that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <para><paramref name="info"/> is null.</para>
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// <para>The class name is null or System.Exception.HResult is zero (0).</para>
        /// </exception>
        protected TemplateSyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}