using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Instance : IVexObject
	{
		public Matrix matrix;

		public Instance()
		{
			this.matrix = Matrix.Identitiy;
		}
	}
}
