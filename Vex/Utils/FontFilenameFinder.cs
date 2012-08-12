/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace DDW.Vex
{
	public class FontUtil
    {
		public Dictionary<string, string> FontMap = new Dictionary<string, string>();

		private static FontUtil inst;
		private FontUtil()
		{
		}

		public static FontUtil GetInstance()
		{
			if (inst == null)
			{
				inst = new FontUtil();
				inst.GetAllFonts();
			}
			return inst;
		}

		private void GetAllFonts()
		{
			//string fp = Environment.GetFolderPath((Environment.SpecialFolder) 0x14);//CSIDL_FONTS = 0x0014

			StringBuilder sb = new StringBuilder();
			SHGetFolderPath(IntPtr.Zero, 0x0014, IntPtr.Zero, 0x0000, sb);//CSIDL_FONTS = 0x0014
			string fontDir = sb.ToString();

			if (!Directory.Exists(fontDir))
			{
				return;
			}
			string[] fonts = Directory.GetFiles(fontDir);
			for (int i = 0; i < fonts.Length; i++)
			{
				string name = GetFontName(fonts[i]);
				if (name != "")
				{
					FontMap.Add(name, fonts[i]);
				}
			}
		}


		private TT_OFFSET_TABLE ttResult;
		private TT_TABLE_DIRECTORY tbName;
		private TT_NAME_TABLE_HEADER ttNTResult;
		private TT_NAME_RECORD ttNMResult;

		private string GetFontName(string fontPath)
        {
			string result = "";

			FileStream fs = new FileStream(fontPath, FileMode.Open, FileAccess.Read);
			BinaryReader r = new BinaryReader(fs, Encoding.UTF8);
			try
			{
				byte[] buff = r.ReadBytes(Marshal.SizeOf(ttResult));
				buff = BigEndian(buff);
				IntPtr ptr = Marshal.AllocHGlobal(buff.Length);
				Marshal.Copy(buff, 0x0, ptr, buff.Length);
				ttResult = (TT_OFFSET_TABLE)Marshal.PtrToStructure(ptr, typeof(TT_OFFSET_TABLE));
				Marshal.FreeHGlobal(ptr);

				//Must be maj =1 minor = 0
				if (ttResult.uMajorVersion != 1 || ttResult.uMinorVersion != 0)
					return "";

				bool bFound = false;
				tbName = new TT_TABLE_DIRECTORY();
				for (int i = 0; i < ttResult.uNumOfTables; i++)
				{
					byte[] bNameTable = r.ReadBytes(Marshal.SizeOf(tbName));
					IntPtr ptrName = Marshal.AllocHGlobal(bNameTable.Length);
					Marshal.Copy(bNameTable, 0x0, ptrName, bNameTable.Length);
					tbName = (TT_TABLE_DIRECTORY)Marshal.PtrToStructure(ptrName, typeof(TT_TABLE_DIRECTORY));
					Marshal.FreeHGlobal(ptrName);
					string szName =
						tbName.szTag1.ToString() +
						tbName.szTag2.ToString() +
						tbName.szTag3.ToString() +
						tbName.szTag4.ToString();
					if (szName != null)
					{
						if (szName.ToString() == "name")
						{
							bFound = true;
							byte[] btLength = BitConverter.GetBytes(tbName.uLength);
							byte[] btOffset = BitConverter.GetBytes(tbName.uOffset);
							Array.Reverse(btLength);
							Array.Reverse(btOffset);
							tbName.uLength = BitConverter.ToUInt32(btLength, 0);
							tbName.uOffset = BitConverter.ToUInt32(btOffset, 0);
							break;
						}
					}
				}
				if (bFound)
				{
					fs.Position = tbName.uOffset;
					byte[] btNTHeader = r.ReadBytes(Marshal.SizeOf(ttNTResult));
					btNTHeader = BigEndian(btNTHeader);
					IntPtr ptrNTHeader = Marshal.AllocHGlobal(btNTHeader.Length);
					Marshal.Copy(btNTHeader, 0x0, ptrNTHeader, btNTHeader.Length);
					ttNTResult = (TT_NAME_TABLE_HEADER)Marshal.PtrToStructure(ptrNTHeader, typeof(TT_NAME_TABLE_HEADER));
					Marshal.FreeHGlobal(ptrNTHeader);
					bFound = false;
					for (int i = 0; i < ttNTResult.uNRCount; i++)
					{
						byte[] btNMRecord = r.ReadBytes(Marshal.SizeOf(ttNMResult));
						btNMRecord = BigEndian(btNMRecord);
						IntPtr ptrNMRecord = Marshal.AllocHGlobal(btNMRecord.Length);
						Marshal.Copy(btNMRecord, 0x0, ptrNMRecord, btNMRecord.Length);
						ttNMResult = (TT_NAME_RECORD)Marshal.PtrToStructure(ptrNMRecord, typeof(TT_NAME_RECORD));
						Marshal.FreeHGlobal(ptrNMRecord);

						// this is where the font name is recovered
						// to get the font family name (not incl 'bold' etc) use ttNMResult.uNameID == 1
						// see http://www.microsoft.com/OpenType/OTSpec/name.htm
						if (ttNMResult.uNameID == 4)
						{
							long fPos = fs.Position;
							fs.Position = tbName.uOffset + ttNMResult.uStringOffset + ttNTResult.uStorageOffset;
							char[] szResult = r.ReadChars(ttNMResult.uStringLength);
							if (szResult.Length != 0)
							{
								// some fonts are \0 A \0 r \0 i \0 a \0 l.... UTf8 encoding doesn't help
								if (szResult[0] == '\0')
								{
									int count = 0;
									char[] temp = new char[szResult.Length / 2];
									for (int j = 1; j < szResult.Length; j += 2)
									{
										temp[count++] = szResult[j];
									}
									szResult = temp;
								}
								result = new String(szResult);
								break;
							}
						}
					}
				}
			}
			finally
			{
				r.Close();
				if (fs != null)
				{
					fs.Dispose();
				}
			}
			return result;
        }
		private byte[] BigEndian(byte[] bLittle)
        {
            byte[] bBig = new byte[bLittle.Length];
            for (int y = 0; y < (bLittle.Length-1); y += 2)
            {
                byte b1, b2;
                b1 = bLittle[y];
                b2 = bLittle[y + 1];
                bBig[y] = b2;
                bBig[y + 1] = b1;
            }
            return bBig;
        }

		[DllImport("shell32.dll")]
		static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken,
		   uint dwFlags, [Out] StringBuilder pszPath);

		[StructLayout(LayoutKind.Sequential, Pack = 0x1)]
		struct TT_OFFSET_TABLE
		{
			public ushort uMajorVersion;
			public ushort uMinorVersion;
			public ushort uNumOfTables;
			public ushort uSearchRange;
			public ushort uEntrySelector;
			public ushort uRangeShift;

		}
		[StructLayout(LayoutKind.Sequential, Pack = 0x1)]
		struct TT_TABLE_DIRECTORY
		{
			public char szTag1;
			public char szTag2;
			public char szTag3;
			public char szTag4;
			public uint uCheckSum; //Check sum
			public uint uOffset; //Offset from beginning of file
			public uint uLength; //length of the table in bytes
		}
		[StructLayout(LayoutKind.Sequential, Pack = 0x1)]
		struct TT_NAME_TABLE_HEADER
		{
			public ushort uFSelector;
			public ushort uNRCount;
			public ushort uStorageOffset;
		}
		[StructLayout(LayoutKind.Sequential, Pack = 0x1)]
		struct TT_NAME_RECORD
		{
			public ushort uPlatformID;
			public ushort uEncodingID;
			public ushort uLanguageID;
			public ushort uNameID;
			public ushort uStringLength;
			public ushort uStringOffset;
		}

    }

}