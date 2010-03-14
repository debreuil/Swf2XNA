using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DDW.FlaFormat
{
    public class JsonLexer
    {
        private static SortedList<string, TokenID> keywords = new SortedList<string, TokenID>();
        private int c;
        private int curLine = 0;
        private List<Token> tokens;
        private BufferedStream src;
        private List<string> strings;

        public JsonLexer(BufferedStream source)
        {
            this.src = source;
        }
        public List<string> StringLiterals
        {
            get
            {
                return strings;
            }
        }
        public List<Token> Lex()
        {
            tokens = new List<Token>();
            strings = new List<string>();
            StringBuilder sb = new StringBuilder();
            int loc = 0;

            c = src.ReadByte();
        readLoop:
            while (c != -1)
            {
                switch (c)
                {
                    #region EOS
                    case -1:
                        {
                            goto readLoop; // eos
                        }
                    #endregion

                    #region WHITESPACE
                    case (int)'\t':
                        {
                            //dont add whitespace tokens
                            c = src.ReadByte();
                            while (c == (int)'\t') { c = src.ReadByte(); } // check for dups of \t
                            break;
                        }
                    case (int)' ':
                        {
                            //dont add tokens whitespace
                            c = src.ReadByte();
                            while (c == (int)' ') { c = src.ReadByte(); }// check for dups of ' '
                            break;
                        }
                    case (int)'\r':
                        {
                            c = src.ReadByte();
                            if (c == (int)'\n')
                                c = src.ReadByte();
                            curLine++;
                            tokens.Add(new Token(TokenID.Newline));
                            break;
                        }
                    case (int)'\n':
                        {
                            c = src.ReadByte();
                            curLine++;
                            tokens.Add(new Token(TokenID.Newline));
                            break;
                        }
                    #endregion

                    #region	STRINGS
                    case (int)'@':
                    case (int)'\'':
                    case (int)'"':
                        {
                            bool isVerbatim = false;
                            if (c == (int)'@')
                            {
                                isVerbatim = true;
                                c = src.ReadByte(); // skip to follow quote
                            }
                            sb.Length = 0;
                            int quote = c;
                            bool isSingleQuote = (c == (int)'\'');
                            c = src.ReadByte();
                            while (c != -1)
                            {
                                if (c == (int)'\\' && !isVerbatim) // normal escaped chars
                                {
                                    c = src.ReadByte();
                                    switch (c)
                                    {
                                        //'"\0abfnrtv
                                        case -1:
                                            {
                                                goto readLoop;
                                            }
                                        case 0:
                                            {
                                                sb.Append('\0');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'a':
                                            {
                                                sb.Append('\a');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'b':
                                            {
                                                sb.Append('\b');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'f':
                                            {
                                                sb.Append('\f');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'n':
                                            {
                                                sb.Append('\n');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'r':
                                            {
                                                sb.Append('\r');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'t':
                                            {
                                                sb.Append('\t');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'v':
                                            {
                                                sb.Append('\v');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'\\':
                                            {
                                                sb.Append('\\');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'\'':
                                            {
                                                sb.Append('\'');
                                                c = src.ReadByte();
                                                break;
                                            }
                                        case (int)'\"':
                                            {
                                                // strings are always stored as verbatim for now, so the double quote is needed
                                                sb.Append("\"\"");
                                                c = src.ReadByte();
                                                break;
                                            }
                                        default:
                                            {
                                                sb.Append((char)c);
                                                break;
                                            }
                                    }
                                }
                                else if (c == (int)'\"')
                                {
                                    c = src.ReadByte();
                                    // two double quotes are escapes for quotes in verbatim mode
                                    if (c == (int)'\"' && isVerbatim)// verbatim escape
                                    {
                                        sb.Append("\"\"");
                                        c = src.ReadByte();
                                    }
                                    else if (isSingleQuote)
                                    {
                                        sb.Append('\"');
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else // non escaped
                                {
                                    if (c == quote)
                                    {
                                        break;
                                    }
                                    sb.Append((char)c);
                                    c = src.ReadByte();
                                }

                            }
                            if (c != -1)
                            {
                                if (c == quote)
                                {
                                    c = src.ReadByte(); // skip last quote
                                }

                                loc = strings.Count;
                                strings.Add(sb.ToString());
                                if (quote == '"')
                                    tokens.Add(new Token(TokenID.StringLiteral, loc));
                                else
                                    tokens.Add(new Token(TokenID.CharLiteral, loc));
                            }
                            break;
                        }
                    #endregion

                    #region PUNCTUATION
                    case (int)'!':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.NotEqual));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Not));
                            }
                            break;
                        }
                    case (int)'#':
                        {
                            // preprocessor
                            tokens.Add(new Token(TokenID.Hash));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'$':
                        {
                            tokens.Add(new Token(TokenID.Dollar)); // this is error in C#
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'%':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.PercentEqual));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Percent));
                            }
                            break;
                        }
                    case (int)'&':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.BAndEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'&')
                            {
                                tokens.Add(new Token(TokenID.And));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.BAnd));
                            }
                            break;
                        }
                    case (int)'(':
                        {
                            tokens.Add(new Token(TokenID.LParen));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)')':
                        {
                            tokens.Add(new Token(TokenID.RParen));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'*':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.StarEqual));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Star));
                            }
                            break;
                        }
                    case (int)'+':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.PlusEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'+')
                            {
                                tokens.Add(new Token(TokenID.PlusPlus));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Plus));
                            }
                            break;
                        }
                    case (int)',':
                        {
                            tokens.Add(new Token(TokenID.Comma));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'-':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.MinusEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'-')
                            {
                                tokens.Add(new Token(TokenID.MinusMinus));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'>')
                            {
                                tokens.Add(new Token(TokenID.MinusGreater));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Minus));
                            }
                            break;
                        }
                    case (int)'/':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.SlashEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'/')
                            {
                                c = src.ReadByte();
                                sb.Length = 0;
                                while (c != '\n' && c != '\r')
                                {
                                    sb.Append((char)c);
                                    c = src.ReadByte();
                                }
                                int index = this.strings.Count;
                                this.strings.Add(sb.ToString());
                                tokens.Add(new Token(TokenID.SingleComment, index));
                            }
                            else if (c == (int)'*')
                            {
                                c = src.ReadByte();
                                sb.Length = 0;
                                for (; ; )
                                {
                                    if (c == (int)'*')
                                    {
                                        c = src.ReadByte();
                                        if (c == -1 || c == (int)'/')
                                        {
                                            c = src.ReadByte();
                                            break;
                                        }
                                        else
                                        {
                                            sb.Append('*');
                                            sb.Append((char)c);
                                            c = src.ReadByte();
                                        }
                                    }
                                    else if (c == -1)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        sb.Append((char)c);
                                        c = src.ReadByte();
                                    }
                                }
                                int index = this.strings.Count;
                                this.strings.Add(sb.ToString());
                                tokens.Add(new Token(TokenID.MultiComment, index));
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Slash));
                            }
                            break;
                        }

                    case (int)':':
                        {
                            c = src.ReadByte();
                            if (c == (int)':')
                            {
                                tokens.Add(new Token(TokenID.ColonColon));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Colon));
                            }
                            break;
                        }
                    case (int)';':
                        {
                            tokens.Add(new Token(TokenID.Semi));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'<':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.LessEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'<')
                            {
                                c = src.ReadByte();
                                if (c == (int)'=')
                                {
                                    tokens.Add(new Token(TokenID.ShiftLeftEqual));
                                    c = src.ReadByte();
                                }
                                else
                                {
                                    tokens.Add(new Token(TokenID.ShiftLeft));
                                }
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Less));
                            }
                            break;
                        }
                    case (int)'=':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.EqualEqual));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Equal));
                            }
                            break;
                        }
                    case (int)'>':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.GreaterEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'>')
                            {
                                c = src.ReadByte();
                                if (c == (int)'=')
                                {
                                    tokens.Add(new Token(TokenID.ShiftRightEqual));
                                    c = src.ReadByte();
                                }
                                else
                                {
                                    tokens.Add(new Token(TokenID.ShiftRight));
                                }
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Greater));
                            }
                            break;
                        }
                    case (int)'?':
                        {
                            c = src.ReadByte();
                            if (c == (int)'?')
                            {
                                tokens.Add(new Token(TokenID.QuestionQuestion));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Question));
                            }
                            break;
                        }

                    case (int)'[':
                        {
                            tokens.Add(new Token(TokenID.LBracket));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'\\':
                        {
                            tokens.Add(new Token(TokenID.BSlash));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)']':
                        {
                            tokens.Add(new Token(TokenID.RBracket));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'^':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.BXorEqual));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.Not));
                            }
                            break;
                        }
                    case (int)'`':
                        {
                            tokens.Add(new Token(TokenID.BSQuote));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'{':
                        {
                            tokens.Add(new Token(TokenID.LCurly));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'|':
                        {
                            c = src.ReadByte();
                            if (c == (int)'=')
                            {
                                tokens.Add(new Token(TokenID.BOrEqual));
                                c = src.ReadByte();
                            }
                            else if (c == (int)'|')
                            {
                                tokens.Add(new Token(TokenID.Or));
                                c = src.ReadByte();
                            }
                            else
                            {
                                tokens.Add(new Token(TokenID.BOr));
                            }
                            break;
                        }
                    case (int)'}':
                        {
                            tokens.Add(new Token(TokenID.RCurly));
                            c = src.ReadByte();
                            break;
                        }
                    case (int)'~':
                        {
                            tokens.Add(new Token(TokenID.Tilde));
                            c = src.ReadByte();
                            break;
                        }
                    #endregion

                    #region NUMBERS
                    // todo: Infinity, -Infinity, NaN
                    case (int)'0':
                    case (int)'1':
                    case (int)'2':
                    case (int)'3':
                    case (int)'4':
                    case (int)'5':
                    case (int)'6':
                    case (int)'7':
                    case (int)'8':
                    case (int)'9':
                    case (int)'.':
                        {
                            sb.Length = 0;
                            TokenID numKind = TokenID.IntLiteral; // default
                            bool isReal = false;

                            // special case dot
                            if (c == (int)'.')
                            {
                                c = src.ReadByte();
                                if (c < '0' || c > '9')
                                {
                                    tokens.Add(new Token(TokenID.Dot));
                                    break;
                                }
                                else
                                {
                                    sb.Append('.');
                                    numKind = TokenID.RealLiteral;
                                    isReal = true;
                                }
                            }
                            bool isNum = true;
                            if (c == (int)'0')
                            {
                                sb.Append((char)c);
                                c = src.ReadByte();
                                if (c == (int)'x' || c == (int)'X')
                                {
                                    sb.Append((char)c);
                                    isNum = true;
                                    while (isNum)
                                    {
                                        c = src.ReadByte();
                                        switch (c)
                                        {
                                            case (int)'0':
                                            case (int)'1':
                                            case (int)'2':
                                            case (int)'3':
                                            case (int)'4':
                                            case (int)'5':
                                            case (int)'6':
                                            case (int)'7':
                                            case (int)'8':
                                            case (int)'9':
                                            case (int)'A':
                                            case (int)'B':
                                            case (int)'C':
                                            case (int)'D':
                                            case (int)'E':
                                            case (int)'F':
                                            case (int)'a':
                                            case (int)'b':
                                            case (int)'c':
                                            case (int)'d':
                                            case (int)'e':
                                            case (int)'f':
                                                {
                                                    sb.Append((char)c);
                                                    break;
                                                }
                                            default:
                                                {
                                                    isNum = false;
                                                    break;
                                                }
                                        }
                                    }
                                    // find possible U and Ls
                                    if (c == (int)'l' || c == (int)'L')
                                    {
                                        sb.Append((char)c);
                                        c = src.ReadByte();
                                        numKind = TokenID.LongLiteral;
                                        if (c == (int)'u' || c == (int)'U')
                                        {
                                            sb.Append((char)c);
                                            numKind = TokenID.ULongLiteral;
                                            c = src.ReadByte();
                                        }
                                    }
                                    else if (c == (int)'u' || c == (int)'U')
                                    {
                                        sb.Append((char)c);
                                        numKind = TokenID.UIntLiteral;
                                        c = src.ReadByte();
                                        if (c == (int)'l' || c == (int)'L')
                                        {
                                            sb.Append((char)c);
                                            numKind = TokenID.ULongLiteral;
                                            c = src.ReadByte();
                                        }
                                    }
                                    //numKind = TokenID.HexLiteral;
                                    loc = this.strings.Count;
                                    this.strings.Add(sb.ToString());
                                    tokens.Add(new Token(numKind, loc));
                                    break; // done number, exits
                                }
                            }

                            // if we get here, it is non hex, but it might be just zero

                            // read number part
                            isNum = true;
                            while (isNum)
                            {
                                switch (c)
                                {
                                    case (int)'0':
                                    case (int)'1':
                                    case (int)'2':
                                    case (int)'3':
                                    case (int)'4':
                                    case (int)'5':
                                    case (int)'6':
                                    case (int)'7':
                                    case (int)'8':
                                    case (int)'9':
                                        {
                                            sb.Append((char)c);
                                            c = src.ReadByte();
                                            break;
                                        }
                                    case (int)'.':
                                        {
                                            if (isReal) // only one dot allowed in numbers
                                            {
                                                numKind = TokenID.RealLiteral;
                                                loc = this.strings.Count;
                                                this.strings.Add(sb.ToString());
                                                tokens.Add(new Token(numKind, loc));
                                                goto readLoop;
                                            }

                                            // might have 77.toString() construct
                                            c = src.ReadByte();
                                            if (c < (int)'0' || c > (int)'9')
                                            {
                                                loc = this.strings.Count;
                                                this.strings.Add(sb.ToString());
                                                tokens.Add(new Token(numKind, loc));
                                                goto readLoop;
                                            }
                                            else
                                            {
                                                sb.Append('.');
                                                sb.Append((char)c);
                                                numKind = TokenID.RealLiteral;
                                                isReal = true;
                                            }
                                            c = src.ReadByte();
                                            break;
                                        }
                                    default:
                                        {
                                            isNum = false;
                                            break;
                                        }
                                }
                            }
                            // now test for letter endings

                            // first exponent
                            if (c == (int)'e' || c == (int)'E')
                            {
                                numKind = TokenID.RealLiteral;
                                sb.Append((char)c);
                                c = src.ReadByte();
                                if (c == '+' || c == '-')
                                {
                                    sb.Append((char)c);
                                    c = src.ReadByte();
                                }

                                isNum = true;
                                while (isNum)
                                {
                                    switch (c)
                                    {
                                        case (int)'0':
                                        case (int)'1':
                                        case (int)'2':
                                        case (int)'3':
                                        case (int)'4':
                                        case (int)'5':
                                        case (int)'6':
                                        case (int)'7':
                                        case (int)'8':
                                        case (int)'9':
                                            {
                                                sb.Append((char)c);
                                                c = src.ReadByte();
                                                break;
                                            }
                                        default:
                                            {
                                                isNum = false;
                                                break;
                                            }
                                    }
                                }
                            }
                            else if (c == (int)'d' || c == (int)'D' ||
                                        c == (int)'f' || c == (int)'F' ||
                                        c == (int)'m' || c == (int)'M')
                            {
                                numKind = TokenID.RealLiteral;
                                sb.Append((char)c);
                                c = src.ReadByte();
                            }
                            // or find possible U and Ls
                            else if (c == (int)'l' || c == (int)'L')
                            {
                                sb.Append((char)c);
                                numKind = TokenID.LongLiteral;
                                c = src.ReadByte();
                                if (c == (int)'u' || c == (int)'U')
                                {
                                    sb.Append((char)c);
                                    numKind = TokenID.ULongLiteral;
                                    c = src.ReadByte();
                                }
                            }
                            else if (c == (int)'u' || c == (int)'U')
                            {
                                sb.Append((char)c);
                                numKind = TokenID.UIntLiteral;
                                c = src.ReadByte();
                                if (c == (int)'l' || c == (int)'L')
                                {
                                    sb.Append((char)c);
                                    numKind = TokenID.ULongLiteral;
                                    c = src.ReadByte();
                                }
                            }

                            loc = this.strings.Count;
                            this.strings.Add(sb.ToString());
                            tokens.Add(new Token(numKind, loc));
                            isNum = false;
                            break;
                        }
                    #endregion

                    #region IDENTIFIERS/KEYWORDS
                    default:
                        {
                            // todo: deal with unicode chars
                            // check if this is an identifier char
                            switch (c)
                            {
                                case (int)'a':
                                case (int)'b':
                                case (int)'c':
                                case (int)'d':
                                case (int)'e':
                                case (int)'f':
                                case (int)'g':
                                case (int)'h':
                                case (int)'i':
                                case (int)'j':
                                case (int)'k':
                                case (int)'l':
                                case (int)'m':
                                case (int)'n':
                                case (int)'o':
                                case (int)'p':
                                case (int)'q':
                                case (int)'r':
                                case (int)'s':
                                case (int)'t':
                                case (int)'u':
                                case (int)'v':
                                case (int)'w':
                                case (int)'x':
                                case (int)'y':
                                case (int)'z':
                                case (int)'A':
                                case (int)'B':
                                case (int)'C':
                                case (int)'D':
                                case (int)'E':
                                case (int)'F':
                                case (int)'G':
                                case (int)'H':
                                case (int)'I':
                                case (int)'J':
                                case (int)'K':
                                case (int)'L':
                                case (int)'M':
                                case (int)'N':
                                case (int)'O':
                                case (int)'P':
                                case (int)'Q':
                                case (int)'R':
                                case (int)'S':
                                case (int)'T':
                                case (int)'U':
                                case (int)'V':
                                case (int)'W':
                                case (int)'X':
                                case (int)'Y':
                                case (int)'Z':
                                case (int)'_':
                                    {
                                        sb.Length = 0;
                                        sb.Append((char)c);
                                        c = src.ReadByte();
                                        bool endIdent = false;
                                        bool possibleKeyword = true;

                                        while (c != -1 && !endIdent)
                                        {
                                            switch (c)
                                            {
                                                case (int)'a':
                                                case (int)'b':
                                                case (int)'c':
                                                case (int)'d':
                                                case (int)'e':
                                                case (int)'f':
                                                case (int)'g':
                                                case (int)'h':
                                                case (int)'i':
                                                case (int)'j':
                                                case (int)'k':
                                                case (int)'l':
                                                case (int)'m':
                                                case (int)'n':
                                                case (int)'o':
                                                case (int)'p':
                                                case (int)'q':
                                                case (int)'r':
                                                case (int)'s':
                                                case (int)'t':
                                                case (int)'u':
                                                case (int)'v':
                                                case (int)'w':
                                                case (int)'x':
                                                case (int)'y':
                                                case (int)'z':
                                                    {
                                                        sb.Append((char)c);
                                                        c = src.ReadByte();
                                                        break;
                                                    }
                                                case (int)'A':
                                                case (int)'B':
                                                case (int)'C':
                                                case (int)'D':
                                                case (int)'E':
                                                case (int)'F':
                                                case (int)'G':
                                                case (int)'H':
                                                case (int)'I':
                                                case (int)'J':
                                                case (int)'K':
                                                case (int)'L':
                                                case (int)'M':
                                                case (int)'N':
                                                case (int)'O':
                                                case (int)'P':
                                                case (int)'Q':
                                                case (int)'R':
                                                case (int)'S':
                                                case (int)'T':
                                                case (int)'U':
                                                case (int)'V':
                                                case (int)'W':
                                                case (int)'X':
                                                case (int)'Y':
                                                case (int)'Z':
                                                case (int)'_':
                                                case (int)'0':
                                                case (int)'1':
                                                case (int)'2':
                                                case (int)'3':
                                                case (int)'4':
                                                case (int)'5':
                                                case (int)'6':
                                                case (int)'7':
                                                case (int)'8':
                                                case (int)'9':
                                                    {
                                                        possibleKeyword = false;
                                                        sb.Append((char)c);
                                                        c = src.ReadByte();
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        endIdent = true;
                                                        break;
                                                    }
                                            }
                                        }
                                        string identText = sb.ToString();
                                        bool isKeyword = possibleKeyword ? keywords.ContainsKey(identText) : false;
                                        if (isKeyword)
                                        {
                                            tokens.Add(new Token((TokenID)keywords[identText]));
                                        }
                                        else
                                        {
                                            loc = this.strings.Count;
                                            this.strings.Add(identText);
                                            tokens.Add(new Token(TokenID.Ident, loc));
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        // todo: if unicode char get ident

                                        // non unicode
                                        tokens.Add(new Token(TokenID.Invalid));
                                        c = src.ReadByte();
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                }

            }
            return tokens;
        }

        #region STATIC CTOR
        static void Lexer()
        {
            keywords.Add("true", TokenID.TrueLiteral);
            keywords.Add("false", TokenID.FalseLiteral);
            keywords.Add("null", TokenID.NullLiteral);
            keywords.Add("undefined", TokenID.UndefinedLiteral);

        }
        #endregion

    }

}
