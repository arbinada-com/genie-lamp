using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextTemplate
{
    /// <summary>
    /// This enum is used by the <see cref="TemplateParser"/> and <see cref="TemplateTokenizer"/> to split
    /// the contents of a template into manageable chunks.
    /// </summary>
    internal enum TemplateTokenType
    {
        /// <summary>
        /// The end of the template has been reached.
        /// </summary>
        End,

        /// <summary>
        /// A code block starts with this token.
        /// </summary>
        CodeBlockStart,

        /// <summary>
        /// A code block ends with this token.
        /// </summary>
        CodeBlockEnd,

        /// <summary>
        /// This is a single character token.
        /// </summary>
        Character,

        /// <summary>
        /// This is a line break token, can consist of one or two characters.
        /// </summary>
        LineBreak,
    }
}