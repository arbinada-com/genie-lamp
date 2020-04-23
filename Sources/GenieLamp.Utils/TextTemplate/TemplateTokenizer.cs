using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextTemplate
{
    internal class TemplateTokenizer
    {
        private readonly string _TemplateContent;
        private bool _InCodeBlock;
        private int _Index;
        private int _LineNumber;
        private TemplateToken _Next;

        public TemplateTokenizer(string templateContent)
        {
            if (templateContent == null)
                throw new ArgumentNullException("templateContent");

            _TemplateContent = templateContent;
            _Index = 0;
            _LineNumber = 1;

            MoveNext();
        }

        public bool More
        {
            get
            {
                return _Next.Type != TemplateTokenType.End;
            }
        }

        public TemplateToken Peek()
        {
            return _Next;
        }

        public TemplateToken Next()
        {
            TemplateToken next = _Next;
            MoveNext();
            return next;
        }

        private void MoveNext()
        {
            _Next = GetNextToken();
        }

        public TemplateToken[] ToArray()
        {
            var result = new List<TemplateToken>();
            while (true)
            {
                TemplateToken token = Next();
                result.Add(token);
                if (token.Type == TemplateTokenType.End)
                    break;
            }
            return result.ToArray();
        }

        private TemplateToken GetNextToken()
        {
            if (_Index >= _TemplateContent.Length)
                return new TemplateToken(TemplateTokenType.End, _Index, _LineNumber, string.Empty);

            switch (_TemplateContent[_Index])
            {
                case '<':
                    if (!_InCodeBlock && _Index + 1 < _TemplateContent.Length && _TemplateContent[_Index + 1] == '%')
                    {
                        var result = new TemplateToken(TemplateTokenType.CodeBlockStart, _Index, _LineNumber, "<%");
                        _Index += 2;
                        _InCodeBlock = true;
                        return result;
                    }
                    else
                    {
                        var result = new TemplateToken(TemplateTokenType.Character, _Index, _LineNumber, "<");
                        _Index += 1;
                        return result;
                    }

                case '%':
                    if (_InCodeBlock && _Index + 1 < _TemplateContent.Length && _TemplateContent[_Index + 1] == '>')
                    {
                        var result = new TemplateToken(TemplateTokenType.CodeBlockEnd, _Index, _LineNumber, "%>");
                        _Index += 2;
                        _InCodeBlock = false;
                        return result;
                    }
                    else
                    {
                        var result = new TemplateToken(TemplateTokenType.Character, _Index, _LineNumber, "%");
                        _Index += 1;
                        return result;
                    }

                case '\n':
                    if (_Index + 1 < _TemplateContent.Length && _TemplateContent[_Index + 1] == '\r')
                    {
                        var result = new TemplateToken(TemplateTokenType.LineBreak, _Index, _LineNumber, "\n\r");
                        _Index += 2;
                        _LineNumber++;
                        return result;
                    }
                    else
                    {
                        var result = new TemplateToken(TemplateTokenType.LineBreak, _Index, _LineNumber, "\n");
                        _Index += 1;
                        _LineNumber++;
                        return result;
                    }

                case '\r':
                    if (_Index + 1 < _TemplateContent.Length && _TemplateContent[_Index + 1] == '\n')
                    {
                        var result = new TemplateToken(TemplateTokenType.LineBreak, _Index, _LineNumber, "\r\n");
                        _Index += 2;
                        _LineNumber++;
                        return result;
                    }
                    else
                    {
                        var result = new TemplateToken(TemplateTokenType.LineBreak, _Index, _LineNumber, "\r");
                        _Index += 1;
                        _LineNumber++;
                        return result;
                    }

                default:
                    var characterResult = new TemplateToken(TemplateTokenType.Character, _Index, _LineNumber, _TemplateContent[_Index].ToString());
                    _Index++;
                    return characterResult;
            }
        }
    }
}