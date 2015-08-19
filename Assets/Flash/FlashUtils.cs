using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FlashClass
{
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
}