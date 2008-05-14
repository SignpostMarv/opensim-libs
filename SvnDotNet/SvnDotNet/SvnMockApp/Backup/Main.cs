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
using System.Reflection;
using Mono.GetOptions;

namespace Softec.SubversionSharp.SvnMockApp {

	class Application : Options
	{
		public class SubCommand {
			public string LongName;
			public string ShortName;
			public string Description;
			public Type Impl;
			public SubCommand(string longName, string shortName, string desc, Type impl)
			{
				LongName = longName;
				ShortName = shortName;
				Description = desc;
				Impl = impl;
			} 
		}
		
		private SortedList SubCommands = new SortedList();
		private SortedList ShortCommands = new SortedList();
		private bool mPendingHelp = false;
	
		public Application()
		{
			Assembly a = Assembly.GetCallingAssembly();
			foreach(Type t in a.GetTypes())
			{
				object[] o;
				if((o = t.GetCustomAttributes(typeof(SubCommandAttribute),true)) != null
				   && o.Length != 0)
				{
					SubCommandAttribute sc = (SubCommandAttribute)o[0];
					SubCommands.Add(sc.LongName,new SubCommand(sc.LongName,sc.ShortName,sc.Description,t));
					if (sc.ShortName != string.Empty)
						ShortCommands.Add(sc.ShortName,sc.LongName);
				}
			}
		}
	
		private WhatToDoNext UsageAppend(WhatToDoNext ret)
		{
			Console.Write("Subcommands: ");
			bool addSep = false;
			foreach(SubCommand sc in SubCommands.Values)
			{
				if( addSep )
					Console.Write(", ");
				else
					addSep = true;
				Console.Write(sc.LongName);
				if( sc.ShortName != string.Empty )
					Console.Write("[{0}]",sc.ShortName);
			}
			Console.WriteLine("\nFor help on subcommands, use the -?/--help subcommand option.");
			return(ret);
		}
			
		public override WhatToDoNext DoUsage()
		{
			return WhatToDoNext.GoAhead;
		}

		public override WhatToDoNext DoHelp()
		{
			mPendingHelp = true;
			return WhatToDoNext.GoAhead;
		}

		public int Run(string[] args)
		{
			ProcessArgs(args);

			if( RemainingArguments.Length == 0 )
			{
				if( mPendingHelp )
					UsageAppend(base.DoHelp());
				else
					UsageAppend(base.DoUsage());
				return(0);
			}

			SubCommand sc;
			if ((sc = (SubCommand) SubCommands[RemainingArguments[0].ToLower()]) == null)
			{
				string shortCmd = (string) ShortCommands[RemainingArguments[0].ToLower()];
				if( shortCmd == null
					|| (sc = (SubCommand) SubCommands[shortCmd]) == null ) 
				{
					if( mPendingHelp )
						UsageAppend(base.DoHelp());
					else
						UsageAppend(base.DoUsage());
					return(1);
				}
			}

			ConstructorInfo ctor = sc.Impl.GetConstructor(new Type[0]);
			MethodInfo run = sc.Impl.GetMethod("Run",new Type[] { typeof(SubCommand), typeof(string[]) });
	
			if(ctor == null || run == null || run.ReturnType != typeof(int))
				throw new InvalidOperationException("Invalid subcommand class");		
							
			return((int)run.Invoke(ctor.Invoke(new object[0]),new object[] { sc, args }));			
		}

		public static int Main(string[] args)
		{
			Application progInst = new Application();
			return( progInst.Run(args) );				
		}		
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class SubCommandAttribute : Attribute
	{
		public string ShortName;
		public string LongName;
		public string Description;
		
		public SubCommandAttribute(string longName)
		{
			ShortName = string.Empty;
			LongName = longName.ToLower();
			Description = string.Empty;
		}
		
		public SubCommandAttribute(string longName, string desc)
		{
			ShortName = string.Empty;
			LongName = longName.ToLower();
			Description = desc;
		}

		public SubCommandAttribute(string longName, string shortName, string desc)
		{
			ShortName = shortName.ToLower();
			LongName = longName.ToLower();
			Description = desc;
		}
	}
}