using System;

namespace DDW.FlaFormat
{
    [System.Diagnostics.DebuggerDisplay("ID = {ID}, Line = {Line}, Column={Col}")]
    public struct Token
    {
        public TokenID ID;
        public int Data; // index into data table
        public int Line;
        public int Col;

        public Token(TokenID id)
        {
            this.ID = id;
            this.Data = -1;
            this.Line = 0;
            this.Col = 0;
        }
        public Token(TokenID id, int data)
        {
            this.ID = id;
            this.Data = data;
            this.Line = 0;
            this.Col = 0;
        }

        public Token(TokenID id, int data, int line, int col)
        {
            this.ID = id;
            this.Data = data;
            this.Line = line;
            this.Col = col;
        }

        public Token(TokenID id, int line, int col)
        {
            this.ID = id;
            this.Data = -1;
            this.Line = line;
            this.Col = col;
        }

        public override string ToString()
        {
            return this.ID.ToString();
        }
    }
}
