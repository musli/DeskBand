using System;
using System.Runtime.InteropServices;

namespace Win32
{	 
	public struct RECT 
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}
	public struct POINT 
	{
		public int x;
		public int y;
	}
	public struct SIZE 
	{
		public int cx;
		public int cy;
	}
	public struct FILETIME 
	{
		public int dwLowDateTime;
		public int dwHighDateTime;
	}
	public struct SYSTEMTIME 
	{
		public short wYear;
		public short wMonth;
		public short wDayOfWeek;
		public short wDay;
		public short wHour;
		public short wMinute;
		public short wSecond;
		public short wMilliseconds;
	}
}

