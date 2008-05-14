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
using Mono.GetOptions;
using Softec.AprSharp;
using Softec.SubversionSharp;

namespace Softec.SubversionSharp.SvnMockApp {

	abstract class CmdBase : Options
	{
		protected Application.SubCommand mSubCmd;
		
		protected SvnClient client;
		
		#region Options
		
		[Option("print debug information", 'd', "debug")]
		public bool oDebug = false;
		
		[Option("print as little as possible", 'q', "quiet")]
		public bool oQuiet = false;

		[Option("read user configuration files from {DIR}", "config-dir")]
		public string oConfigDir = null;
		
		public bool oInteractive = true;
		#endregion
		
		protected SvnRevision StringToRevision(string value)
		{
			try {
				return new SvnRevision(int.Parse(value));
			}
			catch {}
			
			switch(value.ToUpper())
			{
				case "HEAD":
					return new SvnRevision(Svn.Revision.Head);
				case "BASE":
					return new SvnRevision(Svn.Revision.Base);
				case "COMMITED":
					return new SvnRevision(Svn.Revision.Committed);
				case "PREV":
					return new SvnRevision(Svn.Revision.Previous);
					
				default:
					try 
					{
						return new SvnRevision(AprTime.FromDateTime(DateTime.Parse(value)));
					}
					catch( Exception e )
					{
						if( oDebug )
							Console.WriteLine(e);
						else
							Console.WriteLine(e.Message);
						System.Environment.Exit(1);
						return(-1);
					}
			}
		}
		
		protected abstract int Execute();
	
		public virtual int Run(Application.SubCommand sc, string[] args)
		{
			int res;
			
			mSubCmd = sc;
			BreakSingleDashManyLettersIntoManyOptions = true;
			ProcessArgs(args);

			try {
				AprPool p = Svn.PoolCreate();
		        SvnClientContext ctx = SvnClientContext.Create(p);
		        if( oConfigDir != null )
		        	ctx.Config = SvnConfig.GetConfig(new SvnPath(oConfigDir,p), p);
				else
					ctx.Config = SvnConfig.GetConfig(p);
				
				client = new SvnClient(ctx, p);
				
				client.AddSimpleProvider();
	        	client.AddUsernameProvider();
	        	client.AddSslServerTrustFileProvider();
	        	client.AddSslClientCertFileProvider();
	        	client.AddSslClientCertPwFileProvider();
	        	
	        	if( oInteractive )
	        	{
			        client.AddPromptProvider(
			        				new SvnAuthProviderObject.SimplePrompt(SimpleAuth),
			        				IntPtr.Zero, 2);
			        client.AddPromptProvider(
			        				new SvnAuthProviderObject.UsernamePrompt(UsernameAuth),
			        				IntPtr.Zero, 2);
			        client.AddPromptProvider(
			        				new SvnAuthProviderObject.SslServerTrustPrompt(SslServerTrustAuth),
			        				IntPtr.Zero);
			    }
		        client.OpenAuth();

				if( !oQuiet )
		        	client.Context.NotifyFunc = new SvnDelegate(new SvnWcNotify.Func(NotifyCallback));
		        	
	        	client.Context.LogMsgFunc = new SvnDelegate(new SvnClient.GetCommitLog(GetCommitLogCallback));
	        	client.Context.CancelFunc = new SvnDelegate(new SvnClient.CancelFunc(CancelCallback));
				
				res = Execute();
			}
			catch( Exception e )
			{
				if( oDebug )
					Console.WriteLine(e);
				else
					Console.WriteLine(e.Message);
				res = -1;
			}
			finally
			{			
				client.Pool.Destroy();
			}
			return(res);        
		}
		
		private WhatToDoNext UsageAppend(WhatToDoNext ret)
		{
			if(mSubCmd.Description != string.Empty)
				Console.WriteLine("\n{0} {1}\n",mSubCmd.LongName,mSubCmd.Description);
			return(ret);
		}
			
		public override WhatToDoNext DoUsage()
		{
			return UsageAppend(base.DoUsage());
		}

		public override WhatToDoNext DoHelp()
		{
			return UsageAppend(base.DoHelp());
		}
		
		
		private bool mInExternal = false;
		private bool mChanged = false;
		private bool mTxDelta = false;
		
		public void NotifyCallback(IntPtr baton, SvnPath Path,  
	        	 	 	           SvnWcNotify.Actions action, Svn.NodeKind kind,
	        			 		   AprString mimeType, SvnWcNotify.State contentState,
	        			 		   SvnWcNotify.State propState, int revNum)
	    {
	    	switch(action)
	    	{
	    		case SvnWcNotify.Actions.Add:
					if (!mimeType.IsNull && !mimeType.ToString().StartsWith("text/"))
						Console.WriteLine("A  (bin)  {0}", Path);
					else
						Console.WriteLine("A         {0}", Path);
					mChanged = true;
    				break;
	    		
	    		case SvnWcNotify.Actions.BlameRevision:
	    			break;
	    		
	    		case SvnWcNotify.Actions.CommitAdded:
					if (!mimeType.IsNull && !mimeType.ToString().StartsWith("text/"))
	    				Console.WriteLine("Adding  (bin)  {0}", Path);
					else
	    				Console.WriteLine("Adding         {0}", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.CommitDeleted:
	    				Console.WriteLine("Deleting       {0}", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.CommitModified:
	    			Console.WriteLine("Sending        {0}", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.CommitReplaced:
	    				Console.WriteLine("Replacing      {0}", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.Copy:
	    			break;
	    		
	    		case SvnWcNotify.Actions.Delete:
	    			Console.WriteLine("D         {0}", Path);
	    			mChanged = true;
	    			break;
	    		
	    		case SvnWcNotify.Actions.FailedRevert:
	    			Console.WriteLine("Failed to revert '{0}' -- try updating instead.", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.PostfixTxdelta:
	    			if( !mTxDelta )
	    			{
	    				Console.Write("Transmitting file data ");
	    				mTxDelta = true;
	    			}
	    			Console.Write(".");
	    			break;
	    		
	    		case SvnWcNotify.Actions.Resolved:
	    			Console.WriteLine("Resolved conflicted state of '{0}'", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.Restore:
	    			Console.WriteLine("Restored '{0}'", Path);
					break;
	    		
	    		case SvnWcNotify.Actions.Revert:
	    			Console.WriteLine("Reverted '{0}'", Path);
	    			break;
	    			
	    		case SvnWcNotify.Actions.Skip:
	    	    	if (contentState == SvnWcNotify.State.Missing)
        				Console.WriteLine("Skipped missing target: '{0}'", Path);
      				else
        				Console.WriteLine("Skipped '{0}'", Path);
					break;
	    		
	    		case SvnWcNotify.Actions.StatusCompleted:
            		if( revNum >= 0 )
		    			Console.WriteLine("Status against revision: {0}", revNum);
	    			break;
	    		
	    		case SvnWcNotify.Actions.StatusExternal:
					Console.WriteLine("\nPerforming status on external item at '{0}'", Path);
	    			break;
	    		
	    		case SvnWcNotify.Actions.UpdateAdd:
	    			Console.WriteLine("A {0}", Path);
	    			mChanged = true;
	    			break;
	    		
	    		case SvnWcNotify.Actions.UpdateCompleted:
            		if( revNum >= 0 )
              		{
						if( mSubCmd.LongName == "export" )
							Console.WriteLine("Exported {0}revision {1}.",
												(mInExternal) ? "external at " : "",
												revNum);
						else if( mSubCmd.LongName == "checkout" )
							Console.WriteLine("Checked out {0}revision {1}.",
												(mInExternal) ? "external at " : "",
												revNum);
						else
						{
							if( mChanged )
                      			Console.WriteLine("Updated {0}to revision {1}.",
													(mInExternal) ? "external at " : "",
													revNum);
							else
                      			Console.WriteLine("{0}t revision {1}.",
													(mInExternal) ? "External a" : "A",
													revNum);
						}
					}
					else  /* no revision */
					{
						if( mSubCmd.LongName == "export" )
                  			Console.WriteLine("{0}xport complete.",
												(mInExternal) ? "External e" : "E");
                		else if( mSubCmd.LongName == "checkout" )
                  			Console.WriteLine("{0}heckout complete.\n",
												(mInExternal) ? "External c" : "C");
                		else
                  			Console.WriteLine("{0}pdate complete.\n", 
												(mInExternal) ? "External u" : "U");
					}
					if( mInExternal )
						Console.WriteLine();
					mInExternal = false;
					mChanged = false;
	    			break;
	    		
	    		case SvnWcNotify.Actions.UpdateDelete:
	    			Console.WriteLine("D {0}", Path);
	    			mChanged = true;
	    			break;
	    		
	    		case SvnWcNotify.Actions.UpdateExternal:
	    			Console.WriteLine("\nFetching external item into '{0}'", Path);
	    			mInExternal = true;
	    			break;
	    		
	    		case SvnWcNotify.Actions.UpdateUpdate:
	    			string s1 = " ";
	    			string s2 = " ";
					if (! ((kind == Svn.NodeKind.Dir)
               				&& ((propState == SvnWcNotify.State.Inapplicable)
                   			 || (propState == SvnWcNotify.State.Unknown)
							 || (propState == SvnWcNotify.State.Unchanged))))
					{
						mChanged = true;
						if (kind == Svn.NodeKind.File)
						{
							if (contentState == SvnWcNotify.State.Conflicted)
								s1 = "C";
							else if (contentState == SvnWcNotify.State.Merged)
								s1 = "G";
							else if (contentState == SvnWcNotify.State.Changed)
								s1 = "U";
              			}
            
						if (propState == SvnWcNotify.State.Conflicted)
							s2 = "C";
						else if (propState == SvnWcNotify.State.Merged)
							s2 = "G";
						else if (propState == SvnWcNotify.State.Changed)
							s2 = "U";

						if (! ((contentState == SvnWcNotify.State.Unchanged
								|| contentState == SvnWcNotify.State.Unknown)
							&& (propState == SvnWcNotify.State.Unchanged
								|| propState == SvnWcNotify.State.Unknown)))
							Console.WriteLine("{0}{1} {2}", s1, s2, Path);
					}	    			
          			break;
	    	}
	    }
		
		public SvnError GetCommitLogCallback(out AprString logMessage, out SvnPath tmpFile,
							 		   	  	 AprArray commitItems, IntPtr baton,
									      	 AprPool pool)
		{
			if (!commitItems.IsNull)
			{
				foreach (SvnClientCommitItem item in commitItems)
				{
					Console.WriteLine("C1: {1} ({2}) r{3}",
						item.Path, item.Kind, item.Revision);
					Console.WriteLine("C2: {1} -> {2}",
						item.Url,
						item.CopyFromUrl);
				}
				Console.WriteLine();
			}
			
			Console.Write("Enter log message: ");
			logMessage = new AprString(Console.ReadLine(), pool);
			tmpFile = new SvnPath(pool);
			
			return(SvnError.NoError);
		}

		
		public SvnError CancelCallback(IntPtr baton)
		{
			return(SvnError.NoError);		
		}
		
		public SvnError SimpleAuth(out SvnAuthCredSimple cred, IntPtr baton, 
	        				   	   AprString realm, AprString username, 
	        				   	   bool maySave, AprPool pool)
		{
			Console.WriteLine("Simple Authentication");
			Console.WriteLine("---------------------");
			Console.WriteLine("Realm: {0}", realm);
			Console.WriteLine("");
			
			bool valid = false;
			string line = "";
			
			while(!valid)
			{
				if (!username.IsNull)
					Console.Write("Enter Username ({0}): ", username);
				else
					Console.Write("Enter Username: ");

				line = Console.ReadLine();

				if (line.Trim().Length == 0 && !username.IsNull)
				{
					line = username.ToString();
					valid = true;
				}
				else if (line.Trim().Length > 0)
				{
					valid = true;
				}
			}
			
			cred = SvnAuthCredSimple.Alloc(pool);
			cred.Username = new AprString(line, pool);
			Console.Write("Enter Password: ");
			cred.Password = new AprString(Console.ReadLine(), pool);
			cred.MaySave = maySave;
			return(SvnError.NoError);
		}
		
		public SvnError UsernameAuth(out SvnAuthCredUsername cred, IntPtr baton, 
									 AprString realm, bool maySave, AprPool pool)
		{
			Console.WriteLine("Username Authentication:");
			Console.WriteLine("------------------------");
			Console.WriteLine("Realm: {0}", realm);
			Console.WriteLine("");

			bool valid = false;
			string line = "";
			
			while(!valid)
			{
				Console.Write("Enter Username: ");

				line = Console.ReadLine();

				if (line.Trim().Length > 0)
				{
					valid = true;
				}
			}
			
			cred = SvnAuthCredUsername.Alloc(pool);
			cred.Username = new AprString(line, pool);
			cred.MaySave = maySave;
			return(SvnError.NoError);
		}
		
	    public SvnError SslServerTrustAuth(out SvnAuthCredSslServerTrust cred, 
						       			   IntPtr baton, AprString realm, 
										   SvnAuthCredSslServerTrust.CertFailures failures, 
										   SvnAuthSslServerCertInfo certInfo, 
										   bool maySave, IntPtr pool)
		{
			Console.WriteLine("Ssl Server Trust Prompt:");
			Console.WriteLine("------------------------");
			Console.WriteLine("");
			
			Console.WriteLine("Error validating server certificate for '{0}':", realm);
			if ((failures & SvnAuthCredSslServerTrust.CertFailures.UnknownCA) > 0)
				Console.WriteLine(" - The certificate is not issued by a trusted authority");
			if ((failures & SvnAuthCredSslServerTrust.CertFailures.CNMismatch) > 0)
				Console.WriteLine(" - The certificate hostname does not match");
			if ((failures & SvnAuthCredSslServerTrust.CertFailures.NotYetValid) > 0)
				Console.WriteLine(" - The certificate is not yet valid");
			if ((failures & SvnAuthCredSslServerTrust.CertFailures.Expired) > 0)
				Console.WriteLine(" - The certificate has expired");
			if ((failures & SvnAuthCredSslServerTrust.CertFailures.Other) > 0)
				Console.WriteLine(" - The certificate has an unknown error");
		
			Console.WriteLine("Certificate informations:");
			Console.WriteLine("\tHostName:    " + certInfo.Hostname);
			Console.WriteLine("\tIssuer:      " + certInfo.IssuerDName);
			Console.WriteLine("\tValid From:  " + certInfo.ValidFrom);
			Console.WriteLine("\tValid Until: " + certInfo.ValidUntil);
			Console.WriteLine("\tFingerprint: " + certInfo.Fingerprint);
		
			cred = SvnAuthCredSslServerTrust.Alloc(pool);
			bool valid = false;
			while (!valid)
			{
				if (maySave)
					Console.WriteLine("(R)eject, accept (t)emporarily or accept (p)ermanently? ");
				else
					Console.WriteLine("(R)eject or accept (t)emporarily? ");
					
				string line = Console.ReadLine();
				if (line.Length > 0)
				{
					char choice = line.ToLower()[0];
					if (choice == 'r')
					{
						cred.AcceptedFailures = 0;
						cred.MaySave=false;
						valid = true;
					}
					else if (choice == 't')
					{
						cred.AcceptedFailures = failures;
						cred.MaySave=false;
						valid = true;
					}
					else if (choice == 'p')
					{
						cred.AcceptedFailures = failures;
						cred.MaySave=true;
						valid = true;
					}
				}
			}
			return(SvnError.NoError);
		}
			
		}


		abstract class CmdBaseWithAuth : CmdBase
		{
			[Option("authentify using username {USER}", "username")]
			public string oUsername = null;
			
			[Option("authentify using password {PASS}", "password")]
			public string oPassword = null;
			
			[Option("do not cache authentication tokens", "no-auth-cache")]
			public bool oMayNotSave {
				set { oMaySave = !value; }
			}
			public bool oMaySave = true;
			
			[Option("do no interactive prompting", "non-interactive")]
			public bool oNotInteractive {
				set { oInteractive = !value; }
			}
		}	
}
