using System;

namespace Warp3D
{
	/// <summary>
	/// Summary description for warp_Environment.
	/// </summary>
	public class warp_Environment
	{
		public int ambient=0;
		public int fogcolor=0;
		public int fogfact=0;
		public int bgcolor=unchecked((int)0xffffffff);

		public warp_Texture background=null;

		public void setBackground(warp_Texture t)
		{
			background=t;
		}
	}
}
