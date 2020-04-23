using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace TextTemplate
{
    internal struct TemplateToken : IEquatable<TemplateToken>
    {
        public int LineNumber;
        public int Position;
        public string Token;
        public TemplateTokenType Type;

        public TemplateToken(TemplateTokenType type, int position, int lineNumber, string token)
        {
            Type = type;
            Position = position;
            LineNumber = lineNumber;
            Token = token;
        }

        #region IEquatable<TemplateToken> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(TemplateToken other)
        {
            return Equals(other.Type, Type) && other.LineNumber == LineNumber && other.Position == Position && Equals(other.Token, Token);
        }

        #endregion

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// Another object to compare to. 
        /// </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(TemplateToken)) return false;
            return Equals((TemplateToken)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Type.GetHashCode();
                result = (result * 397) ^ LineNumber;
                result = (result * 397) ^ Position;
                result = (result * 397) ^ Token.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Type: {0}, LineNumber: {1}, Position: {2}, Token: {3}", Type, LineNumber, Position, Token.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r"));
        }
    }
}