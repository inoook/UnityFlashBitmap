using UnityEngine;
using System.Collections;

using FlashClass;
using FlashBytes;

public class Test : MonoBehaviour {

	public Texture2D texture;
	
	public BitmapData srcBmp;
	public BitmapData distBmp;
	
	private ConvolutionFilter convolutionFilter;
	private MatrixFilter grayScaleFilter;

	private ColorMatrixFilter filter;

	public Texture2D fillTexture;
	public Vector2 pos = Vector2.zero;
	
	// Use this for initialization
	void Start () {
		srcBmp = new BitmapData();
		srcBmp.SetTexture2D(texture);
		srcBmp.Unlock();
		
		distBmp = new BitmapData(srcBmp.width, srcBmp.height, Color.black);
		
//		Debug.Log(FlashInt.BitmapData.UintToColor(0xffff0000));
//		Debug.Log((Color32)Color.red);
//		
//		Debug.Log(FlashInt.BitmapData.ColorToUint(Color.red));
//		Debug.Log(0xffff0000);
		
		// edge
		
		int[] matrix = new int[]{-1, -1, -1,  -1,  8, -1,  -1, -1, -1}; // フィルタカーネル
		int divisor = 1;
		int bias = 0;
		
		/*
		// 'blur':
		int[] matrix = new int[]{	1, 1, 1,
                      				1, 1, 1,
                      				1, 1, 1};
		int divisor = 9;
		int bias = 0;
		
		/*
		//'sharpness':
        int[] matrix = new int[]{-1, -1, -1,
                      -1,  9, -1,
                      -1, -1, -1};
        int divisor = 1;
        int bias = 0;
        */
		/*
		// emboss
		int[] matrix = new int[]{-2, -1, 0,
								-1,  1, 1,
								0,  1, 2};
		int divisor = 1;
		int bias = 0;
		*/
		
		convolutionFilter = new ConvolutionFilter(matrix, divisor, bias);
		
		grayScaleFilter = new MatrixFilter(new float[]{
				0.298912f, 0.586611f, 0.114478f, 0, 0,
				0.298912f, 0.586611f, 0.114478f, 0 ,0,
				0.298912f, 0.586611f, 0.114478f, 0 ,0,
				0 , 0, 0, 1, 0
		});
		filter = new ColorMatrixFilter(new float[]{
			-1, 0, 0, 0, 1,
			0 ,-1, 0 ,0 ,1,
			0 , 0,-1 ,0 ,1,
			0 , 0, 0, 1, 0
		});
	}
	
	// Update is called once per frame
	void Update () {
		/*
		distBmp.fastblur(srcBmp, blur);
		distBmp.unlock();// apply texture2D
		*/
		/*
		split = Mathf.Clamp(split, 1, 256);
		distBmp.resolution(srcBmp, 256/split, 256/split);
		distBmp.unlock();// apply texture2D
		*/
		/*
//		distBmp.fillRectTexture((int)pos.x, (int)pos.y, fillTexture);
		distBmp.fillRect(new Rectangle(0, 0, distBmp.width, distBmp.height), Color.black);
		distBmp.fillRect(new Rectangle((int)pos.x, (int)pos.y, distBmp.width/2, distBmp.height/2), Color.green);
		distBmp.unlock();// apply texture2D
		*/
	}
	
	public int split = 1;
	public int blur = 3;
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10,10, 1100,800));
		GUILayout.BeginHorizontal();
		GUILayout.Label(srcBmp.texture);
		GUILayout.Label(distBmp.texture);
		GUILayout.EndHorizontal();
		if(GUILayout.Button("Apply convolutionFilter")){
			/*
			convolutionFilter.apply(srcBmp, distBmp);
			distBmp.unlock();// apply texture2D
			*/
			distBmp.ApplyFilter(srcBmp, new Rectangle(0, 0, srcBmp.width, srcBmp.height), new Point(0, 0), convolutionFilter);
			distBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply MatrixFilter")){
//			Debug.Log("Apply MatrixFilter");
			distBmp.ApplyFilter(srcBmp, new Rectangle(0, 0, srcBmp.width, srcBmp.height), null, grayScaleFilter);
			distBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply threshold")){
			Color c = Color.black;
			c.a = 0;
			distBmp.Threshold(srcBmp, new Rectangle(0, 0, distBmp.width, distBmp.height), 0.5f, c, Color.white);
			distBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply resolution")){
			distBmp.ChangeResolution(srcBmp, 256/split, 256/split);
			distBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply FastBlur")){
			distBmp.Fastblur(srcBmp, blur);
			distBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply ColorTransform")){
//			distBmp.ColorTransform(new Rectangle(0, 0, distBmp.width, distBmp.height), new ColorTransform(-1, -1, -1, 1, 255, 255, 255, 0));
//			distBmp.Unlock();// apply texture2D

			srcBmp.ColorTransform(new Rectangle(0, 0, distBmp.width, distBmp.height), new ColorTransform(-1, -1, -1, 1, 255, 255, 255, 0));
			srcBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply Fill")){
			distBmp.FillRect(new Rectangle(0,0,distBmp.width, distBmp.height), Color.green);
			distBmp.Unlock();// apply texture2D
		}
		if(GUILayout.Button("Apply Fill textureBytes")){
//			distBmp.FillRectTexture(new Rectangle(0,0,distBmp.width/2, distBmp.height/2), srcBmp._data, srcBmp.width, distBmp.width/2, distBmp.height/2);
//			distBmp.FillRectTexture(-100, -100, fillTexture, BitmapDataChannel.ALPHA);
			distBmp.FillRectTexture(-100, -100, fillTexture);
			distBmp.Unlock();// apply texture2D
		}
		GUILayout.EndArea();
	}
}
