using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Runtime.InteropServices;

namespace FlashClass
{
	public enum BitmapDataChannel
	{
		ALPHA = 1, RED = 1 << 1, GREEN = 1 << 2, BLUE = 1 << 3, ALL = ALPHA | RED | GREEN | BLUE
	}

	public class Point
	{
		public float x = 0;
		public float y = 0;
		public Point(float x_, float y_)
		{
			x = x_;
			y = y_;
		}
		public Point()
		{
			x = 0;
			y = 0;
		}
		
		public Point clone()
		{
			return new Point(x, y);
		}
		public void offset(float x_, float y_)
		{
			x += x_;
			y += y_;
		}
		public bool equals(Point pt)
		{
			return (pt.x == x && pt.y == y);
		}
		
		public void transformPoint(Matrix mtx)
		{
			float[] matrix = mtx.matrix;
			float x = this.x;
			float y = this.y;
			this.x = x*matrix[0] + y*matrix[2];
			this.y = x*matrix[1] + y*matrix[3];
		}
		
		public override string ToString()
		{
			return "Point: "+ x + ", "+y;
		}
	}

	public class Matrix
	{
		/*
			var x = this.x;
			var y = this.y;
			this.x = x*matrix[0] + y*matrix[2];
			this.y = x*matrix[1] + y*matrix[3];
			*/
		public float[] matrix;
		public Matrix(float a, float b, float c, float d)
		{
			matrix = new float[]{ a, b, c, d };
		}
		
		public Point transformPoint(Point pt)
		{
			Point newPt = new Point();
			newPt.x = pt.x*matrix[0] + pt.y*matrix[2];
			newPt.y = pt.x*matrix[1] + pt.y*matrix[3];
			return newPt;
		}
	}

	public class Rectangle
	{
		public int x;
		public int y;
		public int width;
		public int height;
		
		public Rectangle()
		{
			x = 0;
			y = 0;
			width = 0;
			height = 0;
		}
		
		public Rectangle(int x_, int y_, int widht_, int height_)
		{
			x = x_;
			y = y_;
			width = widht_;
			height = height_;
		}
		
		public override string ToString()
		{
			return "Rectangle: "+x+", "+y+", "+width+", "+height;
		}
	}

	public class Math
	{
		public static float min(float a, float b){
			
			return Mathf.Min(a, b);
		}
		public static int min(int a, int b){
			
			return Mathf.Min(a, b);
		}
		
		public static float max(float a, float b){
			
			return Mathf.Max(a, b);
		}
		public static int max(int a, int b){
			
			return Mathf.Max(a, b);
		}
		
		public static float sqrt(float f)
		{
			return Mathf.Sqrt(f);
		}
	}

	public class MatrixFilter
	{
		//public int[] filter;
		public float[] filter;
		
		public MatrixFilter(float[] filter_)
		{
			filter = filter_;
		}
	}
	
	// http://help.adobe.com/ja_JP/FlashPlatform/reference/actionscript/3/spark/filters/ConvolutionFilter.html
	public class ColorMatrixFilter : MatrixFilter
	{
		public ColorMatrixFilter(float[] filter_) : base(filter_)
		{
			filter = filter_;
		}
	}

	// http://rest-term.com/archives/2566/
	public class ConvolutionFilter
	{
		public int[] matrix;
		public int divisor = 0;
		public int bias = 0;
		
		public ConvolutionFilter (int[] matrix, int divisor, int bias)
		{
			this.matrix = matrix;
			this.divisor = divisor;
			this.bias = bias;
		}
		/*
		public void apply (BitmapData src, BitmapData dst)
		{
			int w = src.width;
			int h = src.height;
			float[] srcData = src._data;
			float[] dstData = dst._data;
			int len = dstData.Length;
			float r;
			float g;
			float b;
			int i;
			int j;
			int k;
			int step;
			int kStep;
		 
			for (int y=1; y<h-1; y++) {
				step = y * w;
				for (int x=1; x<w-1; x++) {
					r = 0;
					g = 0;
					b = 0;
					i = (step + x) << 2;
					k = 0;
					for (int ky=-1; ky<=1; ky++) {
						kStep = ky * w;
						for (int kx=-1; kx<=1; kx++) {
							j = (kStep << 2) + (kx << 2);
							r += srcData [i + j] * this.matrix [k];
							g += srcData [i + j + 1] * this.matrix [k];
							b += srcData [i + j + 2] * this.matrix [k++];
						}
					}
					dstData [i] = r / this.divisor + this.bias;
					dstData [i + 1] = g / this.divisor + this.bias;
					dstData [i + 2] = b / this.divisor + this.bias;
					dstData [i + 3] = 1.0f;
				}
			}
			for (var l=0; l<len; l++) {
				var val = dstData [l];
				dstData [l] = (val < 0) ? 0 : (val > 1) ? 1 : val;
			}
		}
		*/
	}
	
	public class Sprte
	{
		public float mouseX = 0;
		public float mouseY = 0;
	}

	
	public class ColorTransform
	{
		public float redMultiplier = 1.0f;
		public float greenMultiplier = 1.0f; 
		public float blueMultiplier = 1.0f; 
		public float alphaMultiplier = 1.0f; 
		public int redOffset = 0;//0-255
		public int greenOffset = 0; 
		public int blueOffset = 0;
		public int alphaOffset = 0;
		
		public ColorTransform(  float redMultiplier = 1.0f, float greenMultiplier = 1.0f, float blueMultiplier = 1.0f, float alphaMultiplier = 1.0f, 
		                      int redOffset = 0, int greenOffset = 0, int blueOffset = 0, int alphaOffset = 0)
		{
			this.redMultiplier = redMultiplier;
			this.greenMultiplier = greenMultiplier; 
			this.blueMultiplier = blueMultiplier; 
			this.alphaMultiplier = alphaMultiplier; 
			this.redOffset = redOffset;
			this.greenOffset = greenOffset; 
			this.blueOffset = blueOffset;
			this.alphaOffset = alphaOffset;
		}
	}

	//
	public class Utils
	{
		#region utils
		public static uint ColorToUint(Color32 c)
		{
			return (uint)(c.a << 24 | c.r << 16 | c.g << 8 | c.b);
		}

		public static Color32 UintToColor(uint uintColor)
		{
			uint a = (uintColor >> 24 & 0xFF);
			uint r = (uintColor >> 16 & 0xFF);
			uint g = (uintColor >> 8 & 0xFF);
			uint b = (uintColor & 0xFF);
			return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
		}

		public static Color32[] ConvColorsInt(uint[] intColors)
		{
			Color32[] colors = new Color32[intColors.Length];
			for(int i = 0; i < intColors.Length; i++){
				uint intColor = intColors[i];
				colors[i] = UintToColor(intColor);
			}
			return colors;
		}

		public static byte[] ConvColorsToBytes(uint[] intColors)
		{
			byte[] bytes = new byte[intColors.Length*4];
			for(int i = 0; i < intColors.Length; i++){
				uint uintColor = intColors[i];
				byte a = (byte)(uintColor >> 24 & 0xFF);
				byte r = (byte)(uintColor >> 16 & 0xFF);
				byte g = (byte)(uintColor >> 8 & 0xFF);
				byte b = (byte)(uintColor & 0xFF);
				
				bytes[i*4+0] = r;
				bytes[i*4+1] = g;
				bytes[i*4+2] = b;
				bytes[i*4+3] = a;
			}
			return bytes;
		}

		// http://stackoverflow.com/questions/21512259/fast-copy-of-color32-array-to-byte-array
		public static byte[] Color32ArrayToByteArray(Color32[] colors)
		{
			if (colors == null || colors.Length == 0)
				return null;
			
			int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
			int length = lengthOfColor32 * colors.Length;
			byte[] bytes = new byte[length];
			
			GCHandle handle = default(GCHandle);
			try
			{
				handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
				IntPtr ptr = handle.AddrOfPinnedObject();
				Marshal.Copy(ptr, bytes, 0, length);
			}
			finally
			{
				if (handle != default(GCHandle))
					handle.Free();
			}
			
			return bytes;
		}
		#endregion
	}
}