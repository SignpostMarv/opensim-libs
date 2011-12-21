using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace MCI
{
	public delegate void PositionEventHandler(object sender, PositionChangedEventArgs e);

	public class Media
	{
		private System.Windows.Forms.Timer timer1;
		public bool FileIsOpen = false;

		public event PositionEventHandler PositionChanged;

		[DllImport("winmm.dll")]
		private static extern long mciSendString(string strCommand,StringBuilder strReturn,int iReturnLength, IntPtr hwndCallback);
		[DllImport("Winmm.dll")]
		private static extern long PlaySound(byte[] data, IntPtr hMod, UInt32 dwFlags);
		private string sCommand;
		private StringBuilder sBuffer = new StringBuilder(128);
		private bool m_Repeat = false;
		private bool m_Pause = false;
		private int seconds;

		public Media()
		{
			timer1 = new System.Windows.Forms.Timer();
			timer1.Enabled = false;
			timer1.Interval = 100;
			timer1.Tick += new System.EventHandler(this.timer1_Tick);
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			PositionChangedEventArgs pArgs = new PositionChangedEventArgs(this.Position);

			if(PositionChanged != null)
                PositionChanged(this,pArgs);
		}

		public void Close()
		{
			sCommand = "close MediaFile";
			mciSendString(sCommand, null, 0, IntPtr.Zero);

			timer1.Enabled = false;
			FileIsOpen = false;
		}

		public void Stop()
		{
			this.Position = 0;
			sCommand = "stop MediaFile";
			mciSendString(sCommand, null, 0, IntPtr.Zero);

			timer1.Enabled = false;
		}

		public bool Pause
		{
			get
			{
				return m_Pause;
			}
			set
			{
				m_Pause = value;

				if(m_Pause)
                    sCommand = "pause MediaFile";
				else
					sCommand = "resume MediaFile";

				mciSendString(sCommand, null, 0, IntPtr.Zero);
			}
		}

		public void Open(string sFileName)
		{
			if(this.Status() == "playing")
				this.Close();

			sCommand = "open \"" + sFileName + "\" type mpegvideo alias MediaFile";
			mciSendString(sCommand, null, 0, IntPtr.Zero);

			FileIsOpen = true;
		}

		public void Open(string sFileName,System.Windows.Forms.PictureBox  videobox)
		{
			if(this.Status() == "playing")
				this.Close();

			sCommand = "open \"" + sFileName + "\" type mpegvideo alias MediaFile style child parent " + videobox.Handle.ToInt32();
			mciSendString(sCommand, null, 0, IntPtr.Zero);
			sCommand = "put MediaFile window at 0 0 " + videobox.Width + " " +videobox.Height;
			mciSendString(sCommand, null, 0, IntPtr.Zero);

			FileIsOpen = true;
		}

		public void Play()
		{
			if(FileIsOpen)
			{
				sCommand = "play MediaFile";

				if (Repeat)
					sCommand += " REPEAT";

				mciSendString(sCommand, null, 0, IntPtr.Zero);

				timer1.Enabled = true;
			}
		}

		public void FullScreen()
		{
			sCommand = "play MediaFile FullScreen";
			mciSendString(sCommand, null, 0, IntPtr.Zero);
		}

		public int Position
		{
			set
			{
				seconds = value;
				seconds = seconds * 1000;
				sCommand = "play MediaFile from " + seconds.ToString();
				mciSendString(sCommand, null, 0, IntPtr.Zero);
			}

			get
			{
				sCommand = "status MediaFile position";
				mciSendString(sCommand, sBuffer, sBuffer.Capacity, IntPtr.Zero);

				seconds = int.Parse(sBuffer.ToString());
				seconds = seconds /1000;
				return seconds;
			}
		}

		public int Duration()
		{
			int ReturnSeconds;
			sCommand = "status MediaFile length";
			mciSendString(sCommand, sBuffer, 128, IntPtr.Zero);
		
			ReturnSeconds = int.Parse(sBuffer.ToString());
			ReturnSeconds = ReturnSeconds /1000;
			return ReturnSeconds;
		}
		
		public string Status()
		{
			mciSendString("status MediaFile mode", sBuffer, sBuffer.Capacity, IntPtr.Zero);
			return sBuffer.ToString();
		}

		public int Volume 
		{
			get
			{
				if(Status() != "")
				{
					mciSendString("status MediaFile volume", sBuffer, sBuffer.Capacity, IntPtr.Zero);
					return int.Parse(sBuffer.ToString());
				}
				else
				{
					return 0;
				}
			}
			set
			{
				if(value <= 1000)
				{
					mciSendString("setaudio MediaFile volume to " + value.ToString(), null, 0, IntPtr.Zero);
				}
				else
				{
					System.Windows.Forms.MessageBox.Show("Volume value must be smaller than 1000");
				}
			}
		}
		
		public bool Repeat
		{
			get 
			{
				return m_Repeat;
			}
			set 
			{
				m_Repeat = value;
			}
		}

		public string SecondsToTime(int seconds)
		{
			int hours,minutes;

			hours = seconds / 3600;
			minutes = seconds / 60;
			minutes = minutes % 60;
			seconds = seconds % 60;

			return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
		}

		public void PlayWavResource(string wav)
		{
			// get the namespace 
			string strNameSpace = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();

			// get the resource into a stream
			Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( strNameSpace +"."+ wav );
	
			if ( str == null )
				return;
			// bring stream into a byte array
			byte[] bStr = new Byte[str.Length];
			str.Read(bStr, 0, (int)str.Length);
			
			// play the resource
			PlaySound(bStr, IntPtr.Zero, 1 | 4 | 8);
		}
	}

	public class PositionChangedEventArgs : EventArgs
	{
		private int _position;

		public PositionChangedEventArgs(int num)
		{
			this._position = num;
		}

		public int newPosition
		{
			get
			{
				return _position;
			}
		}
	}
}
