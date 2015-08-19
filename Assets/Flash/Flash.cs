using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FlashClass;

namespace Flash
{
	public class BitmapData
	{
		public float[] _data;
		
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
			Color[] colors = texture.GetPixels(0);
			_data = new float[colors.Length*4];
			
			for(int i = 0; i < colors.Length; i++){
				Color color = colors[i];
				_data[i*4    ] = color.r;
				_data[i*4 + 1] = color.g;
				_data[i*4 + 2] = color.b;
				_data[i*4 + 3] = color.a;
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
			//return texture.GetPixel(x,y);
			
			if(x >= 0 && y >= 0 && x < width && y < height){
				int w = this.width;
				//Debug.Log( (x + y*w) * 4);
				float r = _data[x*4 +y*w*4];
				float g = _data[x*4 +y*w*4 + 1];
				float b = _data[x*4 +y*w*4 + 2];
				float a = _data[x*4 +y*w*4 + 3];
				
				return new Color(r, g, b, a);
			}else{
				return new Color(1,1,1,0);
			}
		}
		
		public float getPixelSums(float x, float y)
		{
			return getPixelSums((int)x, (int)y);
		}
		public float getPixelSums(int x, int y)
		{
			if(x >= 0 && y >= 0 && x < width && y < height){
				int w = this.width;
				
				float r = _data[x*4 +y*w*4];
				float g = _data[x*4 +y*w*4 + 1];
				float b = _data[x*4 +y*w*4 + 2];
				//float a = _data[x*4 +y*w*4 + 3];
				
				return r + g + b;
			}else{
				return 1;
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
			float[] srcData = src._data;
			
			float[] f = ff.filter;
			int w = this.width;
			for(int y = rect.y; y < rect.y + rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					// old
					float r = srcData[x*4 +y*w*4],
						  g = srcData[x*4 +y*w*4+1],
						  b = srcData[x*4 +y*w*4+2],
						  a = srcData[x*4 +y*w*4+3];
					this._data[x*4 +y*w*4] = r*f[0] + g*f[1] + b*f[2] +a*f[3]+f[4]; //Rnew
					this._data[x*4 +y*w*4+1] = r*f[5] + g*f[6] + b*f[7] +a*f[8]+f[9]; //Gnew
					this._data[x*4 +y*w*4+2] = r*f[10] + g*f[11] + b*f[12] +a*f[13]+f[14]; //Bnew
					this._data[x*4 +y*w*4+3] = r*f[15] + g*f[16] + b*f[17] +a*f[18]+f[19]; //Anew
				}
			}
		}
		
		
		public void applyFilter(ConvolutionFilter cf){
			applyFilter(this, new Rectangle(0, 0, this.width, this.height), null, cf);
		}
		// // http://rest-term.com/archives/2566/
		public void applyFilter(BitmapData src, Rectangle rect, Point pt, ConvolutionFilter cf)
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
		
		
		public void colorTransform(Rectangle rect, ColorTransform colorTransform)
		{
			float redMultiplier = colorTransform.redMultiplier;
			float greenMultiplier = colorTransform.greenMultiplier; 
			float blueMultiplier = colorTransform.blueMultiplier; 
			float alphaMultiplier = colorTransform.alphaMultiplier;
			float redOffset = colorTransform.redOffset / 255.0f;
			float greenOffset = colorTransform.greenOffset / 255.0f; 
			float blueOffset = colorTransform.blueOffset / 255.0f;
			float alphaOffset = colorTransform.alphaOffset / 255.0f;
			
			int w = this.width;
			for(int y = rect.y; y < rect.y +rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					int index = x*4 + y*w*4;
					// old
					float r = this._data[index  ],
						  g = this._data[index+1],
						  b = this._data[index+2],
						  a = this._data[index+3];
					// new
					this._data[index  ] = r * redMultiplier + redOffset; //Rnew
					this._data[index+1] = g * greenMultiplier + greenOffset; //Gnew
					this._data[index+2] = b * blueMultiplier + blueOffset; //Bnew
					this._data[index+3] = a * alphaMultiplier + alphaOffset; //Anew
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
			float r = color.r,
				  g = color.g,
				  b = color.b,
				  a = color.a;
			
			int w = this.width;
			if(rect.x >= 0 && rect.y >= 0 && (rect.x + rect.width <= this.width) && (rect.y + rect.height <= this.height)){
				for(int y = rect.y; y < rect.y + rect.height; y++){
					for(int x = rect.x; x < rect.x + rect.width; x++){
						
						// new
						int index = x*4 +y*w*4;
						this._data[index] = r; //Rnew
						this._data[index+1] = g; //Gnew
						this._data[index+2] = b; //Bnew
						this._data[index+3] = a; //Anew
					}
				}
			}
		}

		public void clear(Color32 color)
		{
			fillRect(new Rectangle(0,0,width, height), color);
		}

		public void unlock()
		{
			// updatePixcel (rewrite Texture2D)
			int num = ((width)*(height));
			/*
			Color[] colors = new Color[num];
			for(int i = 0; i < num; i++){
				float r = _data[i * 4];
				float g = _data[i * 4 + 1];
				float b = _data[i * 4 + 2];
				float a = _data[i * 4 + 3];
				colors[i] = new Color(r, g, b, a);
			}
			texture.SetPixels(colors);
			texture.Apply(false, false);
			*/
			Color32[] colors = new Color32[num];
			for(int i = 0; i < num; i++){
				byte r = (byte)(_data[i * 4] * 0xFF);
				byte g = (byte)(_data[i * 4 + 1] * 0xFF);
				byte b = (byte)(_data[i * 4 + 2] * 0xFF);
				byte a = (byte)(_data[i * 4 + 3] * 0xFF);
				
				colors[i] = new Color32(r, g, b, a);
			}
			texture.SetPixels32(colors);
			texture.Apply(false, false);
		}
		
		// http://d.hatena.ne.jp/flashrod/20061015
		public void threshold(BitmapData sourceBitmapData, Rectangle sourceRect, float threshold, Color color, Color mask)
		{
			Color c;
			int w = this.width;
			//float[] data = sourceBitmapData._data;
			for(int y = rect.y; y < rect.y +rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					float sum = sourceBitmapData.getPixelSums(x, y) / 3.0f;
					if (sum <= threshold) {
						c = color;
					}else{
						c = mask;
					}
					this._data[x*4 +y*w*4] = c.r; //Rnew
					this._data[x*4 +y*w*4+1] = c.g; //Gnew
					this._data[x*4 +y*w*4+2] = c.b; //Bnew
					this._data[x*4 +y*w*4+3] = c.a; //Anew
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
			//Debug.Log(src.width + " / "+src.height + " // "+u + " / "+v +  " // "+ n_u + " / "+n_v);
		    //BitmapData d = new BitmapData(src.width, src.height);
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
			Color t_rgb;
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
		    
			//this._data = dst._data;
		}
		
		Color average(BitmapData bd, Rectangle rect) {
			float[] data = bd._data;
		    float r = 0;
		    float g = 0;
		    float b = 0;
			float a = 0;
			int w = this.width;
		    for (int j = 0; j < rect.height; j++) {
		        for (int i = 0; i < rect.width; i++) {
					int x = rect.x + i;
					int y = rect.y + j;
					int index = x*4 +y*w*4;
					if(index < data.Length){
						r += data[x*4 +y*w*4];
						g += data[x*4 +y*w*4 + 1];
						b += data[x*4 +y*w*4 + 2];
						a += data[x*4 +y*w*4 + 3];
					}
		        }
		    }
			
		    float n = rect.width * rect.height;
			return new Color(r/n, g/n, b/n, a/n);
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
			int rsum, gsum, bsum;
			int x, y, i, p, p1, p2, yp, yi, yw;
			int[] vmin = new int[Mathf.Max (w, h)];
			int[] vmax = new int[Mathf.Max (w, h)];
			//int[] pix = img.pixelsInt;
			int[] pix = img.GetPixelsInt();
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
			
			this.SetPixelsIntToData(pix);
		}
		
		// pixelInt
		int[] GetPixelsInt()
		{
			int[] pixelsInt = new int[_data.Length / 4];
			float r,g,b,a;
			for(int i = 0; i < pixelsInt.Length; i++){
				r = _data[i*4];
				g = _data[i*4 + 1];
				b = _data[i*4 + 2];
				a = _data[i*4 + 3];
				pixelsInt[i] = ((int)(a*255)) << 24 | ((int)(r*255)) << 16 | ((int)(g*255)) << 8 | ((int)(b*255));
			}
			return pixelsInt;
		}
		void SetPixelsIntToData(int[] pixelsInt)
		{
			_data = new float[pixelsInt.Length * 4];
			for(int i = 0; i < pixelsInt.Length; i++){
				int c = pixelsInt[i];
			 	int r = (c >> 16) & 0xFF;
	            int g = (c >> 8) & 0xFF;
	            int b = c & 0xFF;
				_data[i*4] = r / 255.0f;
				_data[i*4 + 1] = g / 255.0f;
				_data[i*4 + 2] = b / 255.0f;
				_data[i*4 + 3] = 1.0f;
			}
		}
		
	}
}