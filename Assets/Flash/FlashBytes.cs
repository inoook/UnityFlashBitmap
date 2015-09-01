using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FlashClass;

namespace FlashBytes
{
	public class BitmapData
	{
		public byte[] _data;
		
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
		public BitmapData(int width, int height, Color32 fillColor) : this()
		{
			// createTexture RGBA32
			texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.anisoLevel = 0;

			int n = width * height;

			_data = new byte[n*4];
			byte r = fillColor.r;
			byte g = fillColor.g;
			byte b = fillColor.b;
			byte a = fillColor.a;
			for(int i = 0; i < n; i++){
				_data[i*4+0] = r;
				_data[i*4+1] = g;
				_data[i*4+2] = b;
				_data[i*4+3] = a;
			}

			this.width = width;
			this.height = height;
			
			_rect = new Rectangle(0,0,width, height);

			Unlock();
		}
		
		#region for unity
		public void SetTexture2D(Texture2D srcTextrue)
		{
			// createTexture RGBA32
			texture = new Texture2D(srcTextrue.width, srcTextrue.height, TextureFormat.RGBA32, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.anisoLevel = 0;
			
			Color32[] colors = srcTextrue.GetPixels32(0);
//			_data = new byte[colors.Length*4];
//			
//			for(int i = 0; i < colors.Length; i++){
//				Color32 color = colors[i];
//				_data[i*4 + 0] = color.r;
//				_data[i*4 + 1] = color.g;
//				_data[i*4 + 2] = color.b;
//				_data[i*4 + 3] = color.a;
//			}
			_data = Utils.Color32ArrayToByteArray(colors);

			this.width = srcTextrue.width;
			this.height = srcTextrue.height;
			
			_rect = new Rectangle(0,0,width, height);

			Unlock();
		}
		public Texture2D GetTexture2D()
		{
			return texture;
		}
		
		public Texture2D GetTexture2D_2()
		{
			Unlock();
			return texture;
		}
		#endregion
		
		public BitmapData Clone()
		{
			BitmapData bmd = new BitmapData();
			bmd._data = new byte[this._data.Length];
			for(int i = 0; i < bmd._data.Length; i++){
				bmd._data[i] = this._data[i];
			}
			bmd.width = this.width;
			bmd.height = this.height;

			//bmd.texture.wrapMode = TextureWrapMode.Clamp;

			Unlock();

			return bmd;

//			return this;
		}
		
		public Color GetPixel(float x, float y)
		{
			return GetPixel((int)x, (int)y);
		}
		public Color GetPixel(int x, int y)
		{
			if(x >= 0 && y >= 0 && x < width && y < height){
				int w = this.width;
				byte r = _data[x*4 +y*w*4 + 0];
				byte g = _data[x*4 +y*w*4 + 1];
				byte b = _data[x*4 +y*w*4 + 2];
				byte a = _data[x*4 +y*w*4 + 3];
				
				return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
			}else{
				return new Color32(0xff,0xff,0xff,0);
			}
		}
		
		public int GetPixelSums(float x, float y)
		{
			return GetPixelSums((int)x, (int)y);
		}
		public int GetPixelSums(int x, int y)
		{
			if(x >= 0 && y >= 0 && x < width && y < height){
				int w = this.width;
				
				byte r = _data[x*4 +y*w*4];
				byte g = _data[x*4 +y*w*4 + 1];
				byte b = _data[x*4 +y*w*4 + 2];
				
				return ((int)r + (int)g + (int)b);
			}else{
				return 255;
			}
		}
		
		public void Dispose()
		{
			_data = null;
		}
		
		// http://livedocs.adobe.com/flash/9.0_jp/ActionScriptLangRefV3/flash/filters/ColorMatrixFilter.html#ColorMatrixFilter()
		public void ApplyFilter(MatrixFilter ff){
			ApplyFilter(this, new Rectangle(0, 0, this.width, this.height), null, ff);
		}
		
		public void ApplyFilter(Rectangle rect, Point pt, MatrixFilter ff){
			ApplyFilter(this, rect, pt, ff);
		}
		public void ApplyFilter(BitmapData src, Rectangle rect, Point pt, MatrixFilter ff){
			byte[] srcData = src._data;
			
			float[] f = ff.filter;
			int w = this.width;
			for(int y = rect.y; y < rect.y + rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					// old
					int r = (int)srcData[x*4 +y*w*4],
					g = (int)srcData[x*4 +y*w*4+1],
					b = (int)srcData[x*4 +y*w*4+2],
					a = (int)srcData[x*4 +y*w*4+3];
					this._data[x*4 +y*w*4] = (byte)(r*f[0] + g*f[1] + b*f[2] +a*f[3]+f[4]); //Rnew
					this._data[x*4 +y*w*4+1] = (byte)(r*f[5] + g*f[6] + b*f[7] +a*f[8]+f[9]); //Gnew
					this._data[x*4 +y*w*4+2] = (byte)(r*f[10] + g*f[11] + b*f[12] +a*f[13]+f[14]); //Bnew
					this._data[x*4 +y*w*4+3] = (byte)(r*f[15] + g*f[16] + b*f[17] +a*f[18]+f[19]); //Anew
				}
			}
		}

		public void ApplyFilter(ConvolutionFilter cf){
			ApplyFilter(this, new Rectangle(0, 0, this.width, this.height), null, cf);
		}
		// // http://rest-term.com/archives/2566/
		public void ApplyFilter(BitmapData src, Rectangle rect, Point pt, ConvolutionFilter cf)
		{
			BitmapData dst = this;
			int w = src.width;
			
			byte[] srcData = src._data;
			int[] tmpData = new int[srcData.Length];
			
			int r;
			int g;
			int b;
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
							r += (int)(srcData [i + j + 0]) * cf.matrix [k];
							g += (int)(srcData [i + j + 1]) * cf.matrix [k];
							b += (int)(srcData [i + j + 2]) * cf.matrix [k++];
						}
					}
					// new
					tmpData [i + 0] = (r / cf.divisor + cf.bias);
					tmpData [i + 1] = (g / cf.divisor + cf.bias);
					tmpData [i + 2] = (b / cf.divisor + cf.bias);
					tmpData [i + 3] = 255;
				}
			}
			
			byte[] dstData = dst._data;
			int len = dstData.Length;

			for (int l=0; l<len; l++) {
				int val = tmpData [l];
				// clamp
				dstData[l] = (byte)( (val < 0) ? 0 : (val > 255) ? 255 : val );
			}
//			this._data = tmpData;
		}
		
		
		public void ColorTransform(Rectangle rect, ColorTransform colorTransform)
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
					int index = x*4 + y*w*4;
					// old
					int r = this._data[index+0],
						g = this._data[index+1],
						b = this._data[index+2],
						a = this._data[index+3];
					// new
					this._data[index+0] = (byte)((int)(r * redMultiplier) + redOffset); //Rnew
					this._data[index+1] = (byte)((int)(g * greenMultiplier) + greenOffset); //Gnew
					this._data[index+2] = (byte)((int)(b * blueMultiplier) + blueOffset); //Bnew
					this._data[index+3] = (byte)((int)(a * alphaMultiplier) + alphaOffset); //Anew
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
		
		public void FillRect(Rectangle rect, Color32 color)
		{
			byte r = color.r,
			g = color.g,
			b = color.b,
			a = color.a;

			if(rect.x + rect.width > this.width){
				rect.width = this.width - rect.x;
			}
			if(rect.y + rect.height > this.height){
				rect.height = this.height - rect.y;
			}
			if(rect.x < 0){
				rect.width = rect.width + rect.x;
				rect.x = 0;
			}
			if(rect.y < 0){
				rect.height = rect.height + rect.y;
				rect.y = 0;
			}
			
			int w = this.width;
			if(rect.x >= 0 && rect.y >= 0 && (rect.x + rect.width <= this.width) && (rect.y + rect.height <= this.height)){
				for(int y = rect.y; y < rect.y + rect.height; y++){
					for(int x = rect.x; x < rect.x + rect.width; x++){
						
						// new
						int index = (y * w + x) * 4;
						this._data[index+0] = r; //Rnew
						this._data[index+1] = g; //Gnew
						this._data[index+2] = b; //Bnew
						this._data[index+3] = a; //Anew
					}
				}
			}
		}

		public void FillRectTexture(Rectangle rect, byte[] textureBytes, int fillTextureWidth, int textureX, int textureY, BitmapDataChannel channels = BitmapDataChannel.ALL)
		{
			if(rect.x + rect.width > this.width){
				rect.width = this.width - rect.x;
			}
			if(rect.y + rect.height > this.height){
				rect.height = this.height - rect.y;
			}
			if(rect.x < 0){
				rect.width = rect.width + rect.x;
				textureX = textureX - rect.x;
				rect.x = 0;
			}
			if(rect.y < 0){
				rect.height = rect.height + rect.y;
				textureY = textureY - rect.y;
				rect.y = 0;
			}

			int w = this.width;
			if(rect.x >= 0 && rect.y >= 0 && (rect.x + rect.width <= this.width) && (rect.y + rect.height <= this.height)){
				for(int y = 0; y < rect.height; y++){
					for(int x = 0; x < rect.width; x++){
						int txIndex = (((y+textureY) * fillTextureWidth + (x+textureX))) * 4;
						byte r = textureBytes[txIndex + 0];
						byte g = textureBytes[txIndex + 1];
						byte b = textureBytes[txIndex + 2];
						byte a = textureBytes[txIndex + 3];
						
						// new
						int index = ((y+rect.y) * w + (x+rect.x)) * 4;
						if((channels & BitmapDataChannel.RED) != 0)
							this._data[index+0] = r; //Rnew
						if((channels & BitmapDataChannel.GREEN) != 0)
							this._data[index+1] = g; //Gnew
						if((channels & BitmapDataChannel.BLUE) != 0)
							this._data[index+2] = b; //Bnew
						if((channels & BitmapDataChannel.ALPHA) != 0)
							this._data[index+3] = a; //Anew
					}
				}
			}
		}

		public void FillRectTexture(int x, int y, Texture2D fillTexture, BitmapDataChannel channels = BitmapDataChannel.ALL)
		{
			byte[] textureBytes = GetTextureBytes(fillTexture);
			Rectangle rect = new Rectangle(x, y, fillTexture.width, fillTexture.height);
			FillRectTexture(rect, textureBytes, fillTexture.width, 0, 0, channels);
		}

		public void FillRectTexture(Rectangle rect, Texture2D fillTexture, BitmapDataChannel channels = BitmapDataChannel.ALL)
		{
			byte[] textureBytes = GetTextureBytes(fillTexture);
			FillRectTexture(rect, textureBytes, fillTexture.width, 0, 0, channels);
		}

		private byte[] GetTextureBytes(Texture2D texture)
		{
			if( texture.format == TextureFormat.RGBA32 ){
				return texture.GetRawTextureData();
			}else{
				Color32[] colors = texture.GetPixels32(0);
				return Utils.Color32ArrayToByteArray(colors);
			}
		}
		
		public void Clear(Color32 color)
		{
			FillRect(new Rectangle(0,0,width, height), color);
		}
		
		public void Unlock()
		{
			// updatePixcel (rewrite Texture2D)
			texture.LoadRawTextureData(_data);
			texture.Apply(false, false);
		}
		
		// http://d.hatena.ne.jp/flashrod/20061015
		public void Threshold(BitmapData sourceBitmapData, Rectangle sourceRect, float threshold, Color32 color, Color32 mask)
		{
			threshold *= 255;
			
			Color32 c;
			int w = this.width;
			//float[] data = sourceBitmapData._data;
			for(int y = rect.y; y < rect.y +rect.height; y++){
				for(int x = rect.x; x < rect.x + rect.width; x++){
					float sum = sourceBitmapData.GetPixelSums(x, y) / 3.0f;
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
		public void ChangeResolution(BitmapData src, int m, int n) {
			
			int u = src.width / m; // 1ブロックあたりの水平方向画素数
			int v = src.height / n; // 1ブロックあたりの垂直方向画素数
			
			int n_u = src.width % m;
			int n_v = src.height % n;
			//BitmapData d = new BitmapData(src.width, src.height);
			BitmapData dst = this;
			for (int j = 0; j < n; j++) {
				for (int i = 0; i < m; i++) {
					Rectangle rect = new Rectangle(i * u, j * v, u, v);
					Color32 rgb = Average(src, rect);
					dst.FillRect(rect, rgb);
				}
			}
			
			// 余りピクセル
			Rectangle t_rect;
			Color32 t_rgb;
			if(n_v > 0){
				for (int i = 0; i < m; i++) {
					t_rect = new Rectangle(i*u, n*v, u, n_v);
					t_rgb = Average(src, t_rect);
					dst.FillRect(t_rect, t_rgb);
				}
			}
			if(n_u > 0){
				for (int j = 0; j < n; j++) {
					t_rect = new Rectangle(m*u, j*v, n_u, v);
					t_rgb = Average(src, t_rect);
					dst.FillRect(t_rect, t_rgb);
				}
			}
			if(n_v > 0 && n_u > 0){
				t_rect = new Rectangle((m*u), (n*v), n_u, n_v);
				t_rgb = Average(src, t_rect);
				dst.FillRect(t_rect, t_rgb);
			}
			
			//this._data = dst._data;
		}
		
		Color32 Average(BitmapData bd, Rectangle rect) {
			byte[] data = bd._data;
			int r = 0;
			int g = 0;
			int b = 0;
			int a = 0;
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
			return new Color32((byte)(r/n), (byte)(g/n), (byte)(b/n), (byte)(a/n));
		}

		public void Fastblur (BitmapData img, int radius)
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

			byte[] data = img._data;
			
			int[] dv = new int[256 * div];
			for (i=0; i<256*div; i++) {
				dv [i] = (i / div);
			}
			
			yw = yi = 0;
			
			for (y=0; y<h; y++) {
				rsum = gsum = bsum = 0;
				for (i=-radius; i<=radius; i++) {
					int index = yi + Mathf.Min (wm, Mathf.Max (i, 0));
//					p = pix [index];
					rsum += (int)(data[index*4+0]);
					gsum += (int)(data[index*4+1]);
					bsum += (int)(data[index*4+2]);
				}
				for (x=0; x<w; x++) {
					
					r [yi] = dv [rsum];
					g [yi] = dv [gsum];
					b [yi] = dv [bsum];
					
					if (y == 0) {
						vmin [x] = Mathf.Min (x + radius + 1, wm);
						vmax [x] = Mathf.Max (x - radius, 0);
					}
					int index1 = (yw + vmin [x])*4;
					int index2 = (yw + vmax [x])*4;

					rsum += (int)(data[index1+0]) - (int)(data[index2+0]);
					gsum += (int)(data[index1+1]) - (int)(data[index2+1]);
					bsum += (int)(data[index1+2]) - (int)(data[index2+2]);

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
					_data [yi*4+0] = (byte)((int)dv [rsum]);
					_data [yi*4+1] = (byte)((int)dv [gsum]);
					_data [yi*4+2] = (byte)((int)dv [bsum]);
					_data [yi*4+3] = 0xff;

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
		}
		
		// pixelInt
		int[] GetPixelsInt()
		{
			int[] pixelsInt = new int[_data.Length / 4];
			int r,g,b,a;
			for(int i = 0; i < pixelsInt.Length; i++){
				r = _data[i*4 + 0];
				g = _data[i*4 + 1];
				b = _data[i*4 + 2];
				a = _data[i*4 + 3];
				pixelsInt[i] = ((int)(a)) << 24 | ((int)(r)) << 16 | ((int)(g)) << 8 | ((int)(b));
			}
			return pixelsInt;
		}

		void SetPixelsIntToData(int[] pixelsInt)
		{
			_data = new byte[pixelsInt.Length * 4];
			for(int i = 0; i < pixelsInt.Length; i++){
				int c = pixelsInt[i];
				byte r = (byte)((c >> 16) & 0xFF);
				byte g = (byte)((c >> 8) & 0xFF);
				byte b = (byte)(c & 0xFF);
				_data[i*4] = r;
				_data[i*4 + 1] = g;
				_data[i*4 + 2] = b;
				_data[i*4 + 3] = 0xff;
			}
		}
		
	}
}