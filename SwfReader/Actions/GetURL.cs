/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct GetURL : IAction
	{
		public ActionKind ActionId{get{return ActionKind.GetURL;}}
		public uint Version {get{return 3;}}
		public uint Length
		{
			get
			{
                return (uint)(3 + UrlString.Length + 1 + TargetString.Length + 1);
			}
		}		
		public string UrlString;		
		public string TargetString;

		public GetURL(SwfReader r)
		{
			UrlString = r.GetString();
			TargetString = r.GetString();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("geturl");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.GetURL);
            w.AppendUI16(Length - 3);// don't incude this part

            w.AppendString(UrlString);
            w.AppendString(TargetString);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("GetUrl: " + UrlString + " targ: " + TargetString);
		}
	}
}
