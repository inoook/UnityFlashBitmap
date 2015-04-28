using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FlashInt
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
	
	public class BitmapData
	{
		public float[] _data;
		public uint[] pixcels;
		
		public int width = 0;
		public int height = 0;
		
		public Texture2D texture;
		
		private Rectangle _rect;
		
		public BitmapData()
		{
			
		}
		public BitmapData(int width, int height) : this(width, height, Color.black)
		{
			
		}
		public BitmapData(int width, int height, Color fillColor)
		{
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.anisoLevel = 0;
			
			int n = width * height;
			Color[] colors = new Color[n];
			for(int i = 0; i < n; i++){
				colors[i] = fillColor;
			}
			texture.SetPixels(colors);
			texture.Apply();
			
			SetTexture2D(texture);
		}
		
		#region for unity
		public void SetTexture2D(Texture2D texture_)
		{
			texture = texture_;
			//texture.wrapMode = TextureWrapMode.Clamp;
			
			/*
			Color[] colors = texture.GetPixels(0);
			_data = new float[colors.Length*4];
			
			for(int i = 0; i < colors.Length; i++){
				Color color = colors[i];
				_data[i*4    ] = color.r;
				_data[i*4 + 1] = color.g;
				_data[i*4 + 2] = color.b;
				_data[i*4 + 3] = color.a;
			}
			*/
			Color32[] colors32 = texture.GetPixels32(0);
			pixcels = new uint[colors32.Length];
			for(int i = 0; i < pixcels.Length; i++){
				Color32 c = colors32[i];
				pixcels[i] = (uint)(c.a << 24 | c.r << 16 | c.g << 8 | c.b);
			}
			
			width = texture.width;
			height = texture.height;
			
			_rect = new Rectangle(0,0,width, height);
		}
		public Texture2D GetTexture2D()
		{
			return texture;
		}
		
		public Texture2D GetTexture2D_2()
		{
			unlock();
			return texture;
		}
		#endregion
		
		public BitmapData clone()
		{
			/*
			BitmapData bmd = new BitmapData();
			bmd._data = new float[this._data.Length];
			for(int i = 0; i < bmd._data.Length; i++){
				bmd._data[i] = this._data[i];
			}
			bmd.width = this.width;
			bmd.height = this.height;
			
			bmd.texture = this.texture;
			
			//bmd.texture.wrapMode = TextureWrapMode.Clamp;
			
			return bmd;
			*/
			return this;
		}
		
		public Color getPixel(float x, float y)
		{
			return getPixel((int)x, (int)y);
		}
		
		public Color getPixel(int x, int y)
		{
			if(x >= 0 && y >= 0 && x < width && y < height){
				int w = this.width;
				int index = x +y*w;
				uint pixcel = pixcels[index];
				byte a = (byte)(pixcel >> 24 & 0xFF);
				byte r = (byte)(pixcel >> 16 & 0xFF);
				byte g = (byte)(pixcel >> 8 & 0xFF);
				byte b = (byte)(pixcel & 0xFF);
					
				return new Color32(r, g, b, a);
			}else{
				return new Color32(0xFF, 0xFF, 0xFF, 0x00);
			}
		}
		
		public float getPixelSums(float x, float y)
		{
			return getPixelSums((int)x, (int)y);
		}
		
		public uint getPixelSums(int x, int y)
		{
			if(x >= 0 && y >= 0 && x < width && y < height){
				int w = this.width;
				int index = x +y*w;
				uint pixcel = pixcels[index];
				//uint a = (pixcel >> 24 & 0xFF);
				uint r = (pixcel >> 16 & 0xFF);
				uint g = (pixcel >> 8 & 0xFF);
				uint b = (pixcel & 0xFF);
				
				return r + g + b;
			}else{
				return 0xFF;
			}
		}
		
		public void dispose()
		{
			_data = null;
		}
		
		// http://livedocs.adobe.com/flash/9.0_jp/ActionScriptLangRefV3/flash/filters/ColorMatrixFilter.html#ColorMatrixFilter()
		public void applyFilter(MatrixFilter ff){
			applyFilter(this, new Rectangle(0, 0, this.width, this.height), null, ff);
		}
		
		public void applyFilter(Rectangle rect, Point pt, MatrixFilter ff){
			applyFilter(this, rect, pt, ff);
		}
		
		public void applyFilter(BitmapData src, Rectangle rect, Point pt, MatrixFilter ff){
			uint[] srcPix = src.pixcels;
			
			float[] f = ff.filter;
			
			int w = this.width;
			for(int y = rect.y; y < rect.y + rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					int index = x +y*w;
					uint pixcel = srcPix[index];
					
					// old
					uint r = (pixcel >> 16 & 0xFF);
					uint g = (pixcel >> 8 & 0xFF);
					uint b = (pixcel & 0xFF);
					uint a = (pixcel >> 24 & 0xFF);
					
					// new
					uint new_r = (uint)(r*f[0] + g*f[1] + b*f[2] +a*f[3]+f[4]); //Rnew
					uint new_g = (uint)(r*f[5] + g*f[6] + b*f[7] +a*f[8]+f[9]); //Gnew
					uint new_b = (uint)(r*f[10] + g*f[11] + b*f[12] +a*f[13]+f[14]); //Bnew
					uint new_a = (uint)(r*f[15] + g*f[16] + b*f[17] +a*f[18]+f[19]); //Anew
					
					pixcels[index] = (new_a << 24 | new_r << 16 | new_g << 8 | new_b);
				}
			}
		}
		
		public void applyFilter(ConvolutionFilter cf){
			applyFilter(this, new Rectangle(0, 0, this.width, this.height), null, cf);
		}
		
		// http://rest-term.com/archives/2566/
		public void applyFilter__bk(BitmapData src, Rectangle rect, Point pt, ConvolutionFilter cf)
		{
			BitmapData dst = this;
			int w = src.width;
			//int h = src.height;
			
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
		 
			for (int y = 1 + rect.y; y < rect.y + rect.height - 1; y++) {
				step = y * w;
				for (int x = 1 + rect.x; x < rect.x + rect.width - 1; x++) {
					r = 0;
					g = 0;
					b = 0;
					i = (step + x) << 2;
					k = 0;
					for (int ky = -1; ky <= 1; ky++) {
						kStep = ky * w;
						for (int kx = -1; kx <= 1; kx++) {
							j = (kStep << 2) + (kx << 2);
							r += srcData [i + j] * cf.matrix [k];
							g += srcData [i + j + 1] * cf.matrix [k];
							b += srcData [i + j + 2] * cf.matrix [k++];
						}
					}
					// new
					this._data [i] = r / cf.divisor + cf.bias;
					this._data [i + 1] = g / cf.divisor + cf.bias;
					this._data [i + 2] = b / cf.divisor + cf.bias;
					this._data [i + 3] = 1.0f;
				}
			}
			for (var l=0; l<len; l++) {
				var val = this._data [l];
				this._data [l] = (val < 0) ? 0 : (val > 1) ? 1 : val;
			}
		}
		
		public void applyFilter(BitmapData src, Rectangle rect, Point pt, ConvolutionFilter cf)
		{
			//BitmapData dst = this;
			int w = src.width;
			//int h = src.height;
			
			uint[] srcPix = src.pixcels;
			float[] srcData = UintToColorPixels(srcPix);
			float[] tmpData = new float[srcData.Length];
			//float[] srcData = src._data;
			
			float r;
			float g;
			float b;
			int i;
			int j;
			int k;
			int step;
			int kStep;
		 
			for (int y = 1 + rect.y; y < rect.y + rect.height - 1; y++) {
				step = y * w;
				for (int x = 1 + rect.x; x < rect.x + rect.width - 1; x++) {
					r = 0;
					g = 0;
					b = 0;
					i = (step + x) << 2;
					k = 0;
					for (int ky = -1; ky <= 1; ky++) {
						kStep = ky * w;
						for (int kx = -1; kx <= 1; kx++) {
							j = (kStep << 2) + (kx << 2);
							
							r += srcData [i + j] * cf.matrix [k];
							g += srcData [i + j + 1] * cf.matrix [k];
							b += srcData [i + j + 2] * cf.matrix [k++];
						}
					}
					// new
					tmpData [i] = r / cf.divisor + cf.bias;
					tmpData [i + 1] = g / cf.divisor + cf.bias;
					tmpData [i + 2] = b / cf.divisor + cf.bias;
					tmpData [i + 3] = 1.0f;
				}
			}
			/*
			float[] dstData = dst._data;
			int len = dstData.Length;
			for (var l=0; l<len; l++) {
				var val = this._data [l];
				this._data [l] = (val < 0) ? 0 : (val > 1) ? 1 : val;
			}
			*/
			pixcels = ColorPixelsToUint(tmpData);
		}
		
		
		public void colorTransform(Rectangle rect, ColorTransform colorTransform)
		{
			float redMultiplier = colorTransform.redMultiplier;
			float greenMultiplier = colorTransform.greenMultiplier; 
			float blueMultiplier = colorTransform.blueMultiplier; 
			float alphaMultiplier = colorTransform.alphaMultiplier; 
			int redOffset = colorTransform.redOffset;
			int greenOffset = colorTransform.greenOffset; 
			int blueOffset = colorTransform.blueOffset;
			int alphaOffset = colorTransform.alphaOffset;
			
			int w = this.width;
			for(int y = rect.y; y < rect.y +rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					
					int index = x +y*w;
					uint pixcel = pixcels[index];
					
					// old
					uint r = (pixcel >> 16 & 0xFF);
					uint g = (pixcel >> 8 & 0xFF);
					uint b = (pixcel & 0xFF);
					uint a = (pixcel >> 24 & 0xFF);
					
					// new
					int new_r = (int)(r * redMultiplier + redOffset); //Rnew
					int new_g = (int)(g * greenMultiplier + greenOffset); //Gnew
					int new_b = (int)(b * blueMultiplier + blueOffset); //Bnew
					int new_a = (int)(a * alphaMultiplier + alphaOffset); //Anew
					
					pixcels[index] = (uint)(new_a << 24 | new_r << 16 | new_g << 8 | new_b);
				}
			}
		}
		
		public void f_lock()
		{
			
		}
		
		public Rectangle rect
		{
			get{ return _rect; }	
		}
		
		public void fillRect(Rectangle rect, Color color)
		{
			uint fillcolor = ColorToUint(color);
			
			int w = this.width;
			if(rect.x >= 0 && rect.y >= 0 && (rect.x + rect.width <= this.width) && (rect.y + rect.height <= this.height)){
				for(int y = rect.y; y < rect.y + rect.height; y++){
					for(int x = rect.x; x < rect.x + rect.width; x++){
						// new
						int index = x +y*w;
						pixcels[index] = fillcolor;
					}
				}
			}
		}
		
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
		
		public static float[] UintToColorPixels(uint[] uintColors)
		{
			float[] pixelsData = new float[uintColors.Length * 4];
			for(int i = 0; i < uintColors.Length; i++){
				Color32 c = UintToColor(uintColors[i]);
				pixelsData[i * 4] = c.r/255.0f;
				pixelsData[i * 4 + 1] = c.g/255.0f;
				pixelsData[i * 4 + 2] = c.b/255.0f;
				pixelsData[i * 4 + 3] = c.a/255.0f;
			}
			return pixelsData;
		}
		public static uint[] ColorPixelsToUint(float[] data)
		{
			uint[] uintPix = new uint[data.Length/4];
			for(int i = 0; i < uintPix.Length; i++){
				int index = i * 4;
				uintPix[i] = ColorToUint(new Color(data[index], data[index+1], data[index+2], data[index+3]));
			}
			return uintPix;
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
		
		
		
		
		public void unlock()
		{
			// updatePixcel (rewrite Texture2D)
			//int num = ((width)*(height));
			
			Color32[] colors = ConvColorsInt(pixcels);
			texture.SetPixels32(colors);
			texture.Apply(false, false);
		}
		
		// http://d.hatena.ne.jp/flashrod/20061015
		public void threshold(BitmapData sourceBitmapData, Rectangle sourceRect, float threshold, Color color, Color mask)
		{
			threshold *= 255;
			uint uColor;
			uint uintColor = ColorToUint(color);
			uint uintColorMask = ColorToUint(mask);
			
			int w = this.width;
			for(int y = rect.y; y < rect.y +rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					uint sum = sourceBitmapData.getPixelSums(x, y) / 3;
					if (sum <= threshold) {
						uColor = uintColor;
					}else{
						uColor = uintColorMask;
					}
					this.pixcels[x+y*w] = uColor;
				}
			}
		}
		
		// extra 
		// http://d.hatena.ne.jp/flashrod/20081109
		public void resolution(BitmapData src, int m, int n) {
			
			int u = src.width / m; // 1ブロックあたりの水平方向画素数
		    int v = src.height / n; // 1ブロックあたりの垂直方向画素数
			
			int n_u = src.width % m;
			int n_v = src.height % n;
			
			BitmapData dst = this;
		    for (int j = 0; j < n; j++) {
		        for (int i = 0; i < m; i++) {
		            Rectangle rect = new Rectangle(i * u, j * v, u, v);
					Color rgb = average(src, rect);
		            dst.fillRect(rect, rgb);
		        }
		    }
			
			// 余りピクセル
			Rectangle t_rect;
			Color32 t_rgb;
			if(n_v > 0){
		        for (int i = 0; i < m; i++) {
					t_rect = new Rectangle(i*u, n*v, u, n_v);
					t_rgb = average(src, t_rect);
		            dst.fillRect(t_rect, t_rgb);
		        }
			}
			if(n_u > 0){
				for (int j = 0; j < n; j++) {
					t_rect = new Rectangle(m*u, j*v, n_u, v);
					t_rgb = average(src, t_rect);
		            dst.fillRect(t_rect, t_rgb);
		        }
			}
			if(n_v > 0 && n_u > 0){
				t_rect = new Rectangle((m*u), (n*v), n_u, n_v);
				t_rgb = average(src, t_rect);
		        dst.fillRect(t_rect, t_rgb);
			}
		    
		}
		
		Color32 average(BitmapData bd, Rectangle rect) {
			uint[] bdPixcel = bd.pixcels;
		    uint r = 0;
		    uint g = 0;
		    uint b = 0;
			uint a = 0;
			int w = this.width;
			
		    for (int j = 0; j < rect.height; j++) {
		        for (int i = 0; i < rect.width; i++) {
					int x = rect.x + i;
					int y = rect.y + j;
					
					int index = x + y * w;
					
					if(index < pixcels.Length){
						uint pixcel = bdPixcel[index];
						a += (pixcel >> 24 & 0xFF);
						r += (pixcel >> 16 & 0xFF);
						g += (pixcel >> 8 & 0xFF);
						b += (pixcel & 0xFF);
					}
		        }
		    }
			
		    float n = rect.width * rect.height;
			
			Color32 c = new Color32((byte)(r/n), (byte)(g/n), (byte)(b/n), 255);
			return c;
		}
		
		// fast blur
		public void fastblur (BitmapData img, int radius)
		{
			if (radius < 1) {
				return;
			}
			int w = img.width;
			int h = img.height;
			int wm = w - 1;
			int hm = h - 1;
			int wh = w * h;
			int div = radius + radius + 1;
			int[] r = new int[wh];
			int[] g = new int[wh];
			int[] b = new int[wh];
			int rsum, gsum, bsum, x, y, i, p, p1, p2, yp, yi, yw;
			int[] vmin = new int[Mathf.Max (w, h)];
			int[] vmax = new int[Mathf.Max (w, h)];
			//int[] pix = img.pixelsInt;
			
			int[] pix = img.GetPixelsInt();/// mmm?
			
			int[] dv = new int[256 * div];
			for (i=0; i<256*div; i++) {
				dv [i] = (i / div);
			}
		
			yw = yi = 0;
		
			for (y=0; y<h; y++) {
				rsum = gsum = bsum = 0;
				for (i=-radius; i<=radius; i++) {
					p = pix [yi + Mathf.Min (wm, Mathf.Max (i, 0))];
					rsum += (p & 0xff0000) >> 16;
					gsum += (p & 0x00ff00) >> 8;
					bsum += p & 0x0000ff;
				}
				for (x=0; x<w; x++) {
					
					r [yi] = dv [rsum];
					g [yi] = dv [gsum];
					b [yi] = dv [bsum];
		
					if (y == 0) {
						vmin [x] = Mathf.Min (x + radius + 1, wm);
						vmax [x] = Mathf.Max (x - radius, 0);
					}
					p1 = pix [yw + vmin [x]];
					p2 = pix [yw + vmax [x]];
		
					rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
					gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
					bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);
					yi++;
				}
				yw += w;
			}
		
			for (x=0; x<w; x++) {
				rsum = gsum = bsum = 0;
				yp = -radius * w;
				for (i=-radius; i<=radius; i++) {
					yi = Mathf.Max (0, yp) + x;
					rsum += r [yi];
					gsum += g [yi];
					bsum += b [yi];
					yp += w;
				}
				yi = x;
				for (y=0; y<h; y++) {
					pix [yi] = (int)( (uint)0xff000000 | (uint)(dv [rsum] << 16) | (uint)(dv [gsum] << 8) | (uint)(dv [bsum]) );
					//pix[yi] = (int)(0xff0000 * Random.value);
					if (x == 0) {
						vmin [y] = Mathf.Min (y + radius + 1, hm) * w;
						vmax [y] = Mathf.Max (y - radius, 0) * w;
					}
					p1 = x + vmin [y];
					p2 = x + vmax [y];
		
					rsum += r [p1] - r [p2];
					gsum += g [p1] - g [p2];
					bsum += b [p1] - b [p2];
		
					yi += w;
				}
			}
			
			this.SetPixelsIntToData(pix);/// mmm?
		}
		
		
		int[] GetPixelsInt()
		{
			int[] pixelsInt = new int[pixcels.Length];
			for(int i = 0; i < pixelsInt.Length; i++){
				uint p = pixcels[i];
				pixelsInt[i] = (int)p;
			}
			return pixelsInt;
		}
		
		void SetPixelsIntToData(int[] pixelsInt)
		{
			pixcels = new uint[pixelsInt.Length];
			for(int i = 0; i < pixelsInt.Length; i++){
				pixcels[i] = (uint)(pixelsInt[i]);
			}
		}
	}
	
	public class Graphics
	{
		public enum CMD{
			MOVE_TO, LINE_TO
		}
		public List<CMD> cmds;
		public List<Vector2> points;
		
		public float bezierDelta = 0.02f;
		
		public Graphics()
		{
			cmds = new List<CMD>();
			points = new List<Vector2>();
		}
		public void moveTo(float x, float y)
		{
			cmds.Add(CMD.MOVE_TO);
			//Debug.Log("moveTo: "+x + " / "+ y);
			points.Add(new Vector2(x, y));
		}
		public void lineTo(float x, float y)
		{
			cmds.Add(CMD.LINE_TO);
			//Debug.Log("lineTo: "+x + " / "+ y);
			points.Add(new Vector2(x, y));
		}
		
		public void clear()
		{
			cmds.Clear();
			points.Clear();
		}
		
		public void renderDrawLine()
		{
			renderDrawLine(Vector3.one);
		}
		public void renderDrawLine(Vector2 scale)
		{
			for(int i = 0; i < points.Count-1; i++){
				Vector2 pt0 = points[i] * scale.x;
				Vector2 pt1 = points[i+1] * scale.y;
				if(cmds[i+1] == CMD.MOVE_TO){
					
				}else{
					Debug.DrawLine(pt0, pt1);
				}
			}
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