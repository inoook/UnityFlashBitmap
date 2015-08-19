# UnityFlashBitmap
Unityのtextureをflashのbitmaっぽく使う。

~~~cs
Texture2D texture; // unity texture

BitmapData srcBmp = new BitmapData();
srcBmp.SetTexture2D(texture);

// fillRect
srcBmp.fillRect(new Rectangle(0,0,10,10), Color.green);

// MatrixFilter
MatrixFilter grayScaleFilter = new MatrixFilter(new float[]{
  0.298912f, 0.586611f, 0.114478f, 0, 0,
  0.298912f, 0.586611f, 0.114478f, 0 ,0,
  0.298912f, 0.586611f, 0.114478f, 0 ,0,
  0 , 0, 0, 1, 0
});
distBmp.applyFilter(srcBmp, new Rectangle(0, 0, srcBmp.width, srcBmp.height), null, grayScaleFilter);
distBmp.unlock();// apply texture2D

// ConvolutionFilter - edge
int[] matrix = new int[]{-1, -1, -1,  -1,  8, -1,  -1, -1, -1}; // フィルタカーネル
int divisor = 1;
int bias = 0;
ConvolutionFilter	convolutionFilter = new ConvolutionFilter(matrix, divisor, bias);
distBmp.applyFilter(srcBmp, new Rectangle(0, 0, srcBmp.width, srcBmp.height), new Point(0, 0), convolutionFilter);
distBmp.unlock();// apply texture2D
~~~
