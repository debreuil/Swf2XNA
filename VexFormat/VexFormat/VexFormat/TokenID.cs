using System;

namespace DDW.FlaFormat
{
    public enum TokenID : byte
    {
        Whitespace = 0x00,
        Newline = 0x01,
        SingleComment = 0x02,
        MultiComment = 0x03,
        DocComment = 0x04,

        Ident = 0x05,
        TrueLiteral = 0x06,
        FalseLiteral = 0x07,
        NullLiteral = 0x08,
        UndefinedLiteral = 0x08,

        //SByteLiteral	= 0x09, // not used
        //ByteLiteral	= 0x0A,
        //ShortLiteral	= 0x0B,
        //UShortLiteral	= 0x0C,
        HexLiteral = 0x0C,
        IntLiteral = 0x0D,
        UIntLiteral = 0x0F,
        LongLiteral = 0x10,
        ULongLiteral = 0x11,

        DecimalLiteral = 0x12,
        RealLiteral = 0x13,

        CharLiteral = 0x14,
        StringLiteral = 0x15,

        ColonColon = 0x1F, // "::" 
        QuestionQuestion = 0x20, // "??" 

        Not = (int)'!', // 0x21
        Quote = (int)'"', // 0x22
        Hash = (int)'#', // 0x23
        Dollar = (int)'$', // 0x24
        Percent = (int)'%', // 0x25
        BAnd = (int)'&', // 0x26
        SQuote = (int)'\'', // 0x27
        LParen = (int)'(', // 0x28
        RParen = (int)')', // 0x29
        Star = (int)'*', // 0x2A
        Plus = (int)'+', // 0x2Bz 
        Comma = (int)',', // 0x2C
        Minus = (int)'-', // 0x2D
        Dot = (int)'.', // 0x2E
        Slash = (int)'/', // 0x2F

        PlusPlus = 0x30, // "++"
        MinusMinus = 0x31, // "--"
        And = 0x32, // "&&"
        Or = 0x33, // "||"
        MinusGreater = 0x34, // "->"
        EqualEqual = 0x35, // "=="
        NotEqual = 0x36, // "!="
        LessEqual = 0x37, // "<="
        GreaterEqual = 0x38, // ">="
        PlusEqual = 0x39, // "+="

        Colon = (int)':', // 0x3A
        Semi = (int)';', // 0x3B
        Less = (int)'<', // 0x3C
        Equal = (int)'=', // 0x3D
        Greater = (int)'>', // 0x3E
        Question = (int)'?', // 0x3F
        //At				= (int)'@', // 0x40

        MinusEqual = 0x41, // "-="
        StarEqual = 0x42, // "*="
        SlashEqual = 0x43, // "/="
        PercentEqual = 0x44, // "%="
        BAndEqual = 0x45, // "&="
        BOrEqual = 0x46, // "|="
        BXorEqual = 0x47, // "^="
        ShiftLeft = 0x48, // "<<"
        ShiftLeftEqual = 0x49, // "<<="
        ShiftRight = 0x4A, // ">>"
        ShiftRightEqual = 0x4B, // ">>="
        

        LBracket = (int)'[', // 0x5B
        BSlash = (int)'\\', // 0x5C
        RBracket = (int)']', // 0x5D
        BXor = (int)'^', // 0x5E
        //'_', 0x5F
        BSQuote = (int)'`', // 0x60

        LCurly = (int)'{', // 0x7B
        BOr = (int)'|', // 0x7C
        RCurly = (int)'}', // 0x7D
        Tilde = (int)'~', // 0x7E

        Eof = 0xFE, // error token
        Invalid = 0xFF, // error token

    }
}
