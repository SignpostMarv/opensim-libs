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

	[SubCommand("status", "st",
  @"[PATH...]
  Print the status of working copy files and directories.
  With no args, print only locally modified items (no network access).
  With -u, add working revision and server out-of-date information.
  With -v, print full revision information on every item.
 
  The first five columns in the output are each one character wide:
    First column: Says if item was added, deleted, or otherwise changed
      ' ' no modifications
      'A' Added
      'C' Conflicted
      'D' Deleted
      'G' Merged
      'I' Ignored
      'M' Modified
      'R' Replaced
      'X' item is unversioned, but is used by an externals definition
      '?' item is not under version control
      '!' item is missing (removed by non-svn command) or incomplete
      '~' versioned item obstructed by some item of a different kind
    Second column: Modifications of a file's or directory's properties
      ' ' no modifications
      'C' Conflicted
      'M' Modified
    Third column: Whether the working copy directory is locked
      ' ' not locked
      'L' locked
    Fourth column: Scheduled commit will contain addition-with-history
      ' ' no history scheduled with commit
      '+' history scheduled with commit
    Fifth column: Whether the item is switched relative to its parent
      ' ' normal
      'S' switched
 
  The out-of-date information appears in the eighth column (with -u):
      '*' a newer revision exists on the server
      ' ' the working copy is up to date
 
  Remaining fields are variable width and delimited by spaces:
    The working revision (with -u or -v)
    The last committed revision and last committed author (with -v)
    The working copy path is always the final field, so it can
      include spaces.
 
  Example output:
    svn status wc
     M     wc/bar.c
    A  +   wc/qax.c
 
    svn status -u wc
     M           965    wc/bar.c
           *     965    wc/foo.c
    A  +         965    wc/qax.c
    Head revision:   981
 
    svn status --show-updates --verbose wc
     M           965       938 kfogel       wc/bar.c
           *     965       922 sussman      wc/foo.c
    A  +         965       687 joe          wc/qax.c
                 965       687 joe          wc/zig.c
    Head revision:   981"
	)]
	class StatusCmd : CmdBaseWithAuth
	{
		[Option("operate on single directory only", 'N', "non-recursive")]
		public bool oNoRecurse {
			set { oRecurse = !value; }
		} 
		public bool oRecurse = true;

		[Option("display update information", 'u', "show-updates")]
		public bool oShowUpdates = false;
		
		[Option("print extra information", 'v', "verbose")]
		public bool oVerbose = false;

		[Option("disregard default and svn:ignore property ignores", "no-ignore")]
		public bool oNoIgnore = false;
		
		protected override int Execute()
		{
			int nbPath = RemainingArguments.Length;
		
			if( nbPath == 1 )
			{
				client.Status("", 
							  new SvnRevision(Svn.Revision.Head),
				  			  new SvnWcStatus.Func(StatusCallback), IntPtr.Zero,
				  			  oRecurse, oVerbose, oShowUpdates, oNoIgnore);
			}
			else
			{
				for(int i=1; i<nbPath; i++)
				{
					try 
					{
						client.Status(RemainingArguments[i],
									  new SvnRevision(Svn.Revision.Head),
						  			  new SvnWcStatus.Func(StatusCallback), IntPtr.Zero,
						  			  oRecurse, oVerbose, oShowUpdates, oNoIgnore);
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
			return(0);
		}
		
		private void StatusCallback(IntPtr baton, SvnPath path, SvnWcStatus status)
		{
			if (status.IsNull 
				|| (oQuiet && status.Entry.IsNull)
      			|| ((status.TextStatus == SvnWcStatus.Kind.None)
          		    && (status.ReposTextStatus == SvnWcStatus.Kind.None)))
          	{
				return;
			}
				
			string wrkRevNum = "";
			string ciRevNum = "";
			string ciAuthor = "";
			char oodStatus = '@';
				
			if (oShowUpdates || oVerbose)
			{
				if( status.Entry.IsNull )
					wrkRevNum = "";
				else if( status.Entry.Revision < 0 )
					wrkRevNum = " ? ";
				else if( status.Copied )
					wrkRevNum = "-";
				else
					wrkRevNum = status.Entry.Revision.ToString();
					
				if( status.ReposTextStatus != SvnWcStatus.Kind.None
					|| status.ReposPropStatus != SvnWcStatus.Kind.None )
					oodStatus = '*';
				else
					oodStatus = ' ';
					
				if( oVerbose )
				{
					if( status.Entry.IsNull )
						ciRevNum = "";
					else if( status.Entry.CommitRev < 0 )
						ciRevNum = " ? ";
					else
						ciRevNum = status.Entry.CommitRev.ToString();
						
					if( status.Entry.IsNull )
						ciAuthor = "";
					else if( status.Entry.CommitAuthor.IsNull )
						ciAuthor = " ? ";
					else
						ciAuthor = status.Entry.CommitAuthor.ToString();
				}
			}
			
			if(oVerbose)
			{
				Console.WriteLine("{0}{1}{2}{3}{4}  {5}   {6,6}   {7,6:X} {8,-12} {9}", 
								  WcStatusChar(status.TextStatus),
								  WcStatusChar(status.PropStatus),
								  status.Locked ? 'L' : ' ',
								  status.Copied ? '+' : ' ',
								  status.Switched ? 'S' : ' ',
								  oodStatus,
								  wrkRevNum,
								  ciRevNum,
								  ciAuthor,
								  path);
			}
			else if (oShowUpdates)
			{
				Console.WriteLine("{0}{1}{2}{3}{4}  {5}   {6,6}   {7}",
								  WcStatusChar(status.TextStatus),
								  WcStatusChar(status.PropStatus),
								  status.Locked ? 'L' : ' ',
								  status.Copied ? '+' : ' ',
								  status.Switched ? 'S' : ' ',
								  oodStatus,
								  wrkRevNum,
								  path);
			}
			else
			{
				Console.WriteLine("{0}{1}{2}{3}{4}  {5}",
								  WcStatusChar(status.TextStatus),
								  WcStatusChar(status.PropStatus),
								  status.Locked ? 'L' : ' ',
								  status.Copied ? '+' : ' ',
								  status.Switched ? 'S' : ' ',
								  path);
			}
		}
		
		private char WcStatusChar(SvnWcStatus.Kind kind)
		{
			switch( kind )
			{
				case SvnWcStatus.Kind.None			: return ' ';
				case SvnWcStatus.Kind.Unversioned	: return '?';
				case SvnWcStatus.Kind.Normal		: return ' ';
				case SvnWcStatus.Kind.Added			: return 'A';
				case SvnWcStatus.Kind.Missing		: return '!';
				case SvnWcStatus.Kind.Deleted		: return 'D';
				case SvnWcStatus.Kind.Replaced		: return 'R';
				case SvnWcStatus.Kind.Modified		: return 'M';
				case SvnWcStatus.Kind.Merged		: return 'G';
				case SvnWcStatus.Kind.Conflicted	: return 'C';
				case SvnWcStatus.Kind.Ignored		: return 'I';
				case SvnWcStatus.Kind.Obstructed	: return '~';
				case SvnWcStatus.Kind.External		: return 'X';
				case SvnWcStatus.Kind.Incomplete	: return 'I';
			}
			return '?';
		}
	}	
}
