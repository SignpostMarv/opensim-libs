//  SvnTest, a client program used to test SubversionSharp library
#region Copyright (C) 2004 SOFTEC sa.
//
//  SvnTest, a client program used to test SubversionSharp library
//  Copyright 2004 by SOFTEC sa
//
//  This program is free software; you can redistribute it and/or
//  modify it under the terms of the GNU General Public License as
//  published by the Free Software Foundation; either version 2 of
// the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of 
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//  See the GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public
//  License along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//  Sources, support options and lastest version of the complete library
//  is available from:
//		http://www.softec.st/SubversionSharp
//		Support@softec.st
//
//  Initial authors : 
//		Denis Gervalle
//		Olivier Desaive
#endregion
//
using System;
using System.Collections;
using System.IO;
using Mono.GetOptions;
using Softec.AprSharp;
using Softec.SubversionSharp;

namespace Softec.SubversionSharp.SvnMockApp {

	[SubCommand("checkout","co", 
  @"URL... [PATH]
  Check out a working copy from a repository.

  Note: If PATH is omitted, the basename of the URL will be used as
  the destination. If multiple URLs are given each will be checked
  out into a sub-directory of PATH, with the name of the sub-directory
  being the basename of the URL."
	)]
	class CheckoutCmd : CmdBaseWithAuth
	{
		[Option("checkout revision {REV}", 'r', "revision")]
		public string oStrRevision {
			set
			{
				oRevision = StringToRevision(value);
			}
		}
		public SvnRevision oRevision = new SvnRevision(Svn.Revision.Head);
		
		[Option("operate on single directory only", 'N', "non-recursive")]
		public bool oNoRecurse {
			set { oRecurse = !value; }
		} 
		public bool oRecurse = true;
		
		
		protected override int Execute()
		{
			int nbUrl = RemainingArguments.Length-1;
			Uri[] urls = new Uri[nbUrl];
			string path;
		
			for(int i=1; i<nbUrl; i++)
			{
				urls[i-1] = new Uri(RemainingArguments[i]);
			}
			
			try {
				urls[nbUrl-1] = 
					new Uri(RemainingArguments[nbUrl]);
				path = string.Empty;
			}
			catch(System.UriFormatException) {
				path = RemainingArguments[nbUrl--];
			}
			
			if( nbUrl > 1
				|| RemainingArguments.Length == 2)
			{
				if( path != string.Empty )
					path += Path.DirectorySeparatorChar;
				for(int i = 0; i < nbUrl; i++)							
				{
					string[] seg = urls[i].Segments;
					try 
					{
						client.Checkout(urls[i].AbsoluteUri, path + seg[seg.Length-1],
										oRevision,
										oRecurse);
					}
					catch( Exception e )
					{
						if( oDebug )
							Console.WriteLine(e);
						else
							Console.WriteLine(e.Message);
					}
					client.Clear();
				}
			}
			else
			{
				client.Checkout(urls[0].AbsoluteUri, path,
								oRevision,
								oRecurse);
			}
			return(0);
		}
	}	
}
