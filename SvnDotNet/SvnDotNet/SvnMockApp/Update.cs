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

	[SubCommand("update","up", 
  @"[PATH...]
  If no revision given, bring working copy up-to-date with HEAD rev.
  Else synchronize working copy to revision given by -r.
 
  For each updated item a line will start with a character reporting the
  action taken.  These characters have the following meaning:
 
    A  Added
    D  Deleted
    U  Updated
    C  Conflict
    G  Merged
 
  A character in the first column signifies an update to the actual file,
  while updates to the file's properties are shown in the second column.
  Check out a working copy from a repository."
	)]
	class UpdateCmd : CmdBaseWithAuth
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
			if( RemainingArguments.Length > 1 )
			{
				for(int i = 1; i < RemainingArguments.Length; i++)							
				{
					try 
					{
						client.Update(RemainingArguments[i],
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
				client.Update("", 
							  oRevision,
							  oRecurse);
			}
			return(0);
		}
	}	
}
