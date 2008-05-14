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

	[SubCommand("add", 
  @"PATH...
  Put files and directories under version control, scheduling
  them for addition to repository.  They will be added in next commit."
	)]
	class AddCmd : CmdBase
	{
		[Option("operate on single directory only", 'N', "non-recursive")]
		public bool oNoRecurse {
			set { oRecurse = !value; }
		} 
		public bool oRecurse = true;
				
		protected override int Execute()
		{
			int nbPath = RemainingArguments.Length;
		
			for(int i=1; i<nbPath; i++)
			{
				try 
				{
					client.Add(RemainingArguments[i], oRecurse);
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
			return(0);
		}
	}	
}
