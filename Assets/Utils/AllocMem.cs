// http://wiki.unity3d.com/index.php/AllocationStats
using UnityEngine;
using System.Collections;
using System.Text;
 
//[ExecuteInEditMode ()]
public class AllocMem : MonoBehaviour {
 
	public bool showFPS = false;
	
	private float lastCollect = 0;
	private float lastCollectNum = 0;
	private float delta = 0;
	private float lastDeltaTime = 0;
	private int allocRate = 0;
	private int lastAllocMemory = 0;
	private float lastAllocSet = -9999;
	private int allocMem = 0;
	private int collectAlloc = 0;
	private int peakAlloc = 0;
	
	public Color color = Color.green;
	
	void Start () {
		
	}
	
 	public Rect drawRect = new Rect(10, 10, 400, 300);
	public bool debug = false;
	
//	public CheckBattery battery;
	
	void OnGUI()
	{
		if(!debug){ return; }
		
		//windowRect = GUILayout.Window(2, windowRect, DoMyWindow, "Memory debug");
		
		int mb = 1048576;
		string str = "Total allocated memory : " + ((float)(System.GC.GetTotalMemory(false) / (float)mb)).ToString("0.00") + " / " + (SystemInfo.systemMemorySize) + " MB";
		
		if(Debug.isDebugBuild){
			str += "\nHeapSize: "+ (Profiler.usedHeapSize / mb) + " Mb";
		}
		if (showFPS) {
			str += ("\n"+(1f/Time.deltaTime).ToString ("0.0")+" fps");
		}
		
		str += ("\nInput.gyro.enabled: "+Input.gyro.enabled);
		
//		if(battery != null){
//			if(battery.isEnableCheckBattery){
//				str += ("\nBattery: "+battery.GetBatteryLevel());
//			}else{
//				str += ("\nDisableCheckBattery");
//			}
//		}

		GUILayout.BeginArea(drawRect);
		GUI.color = color;
		GUILayout.Label(str);
		if(GUILayout.Button("System.GC.Collect")){
			System.GC.Collect();
		}
		GUILayout.EndArea();
		
	}
	void DoMyWindow(int windowID) {
		
		int mb = 1048576;
		string str = "Total allocated memory : " + ((float)(System.GC.GetTotalMemory(false) / (float)mb)).ToString("0.00") + " / " + (SystemInfo.systemMemorySize) + " MB";
		
		if(Debug.isDebugBuild){
			str += " HeapSize: "+ (Profiler.usedHeapSize / mb) + " Mb" + "\n";
		}
		
		GUILayout.Label(str);
		
		int collCount = System.GC.CollectionCount (0);
 
		if (lastCollectNum != collCount) {
			lastCollectNum = collCount;
			delta = Time.realtimeSinceStartup-lastCollect;
			lastCollect = Time.realtimeSinceStartup;
			lastDeltaTime = Time.deltaTime;
			collectAlloc = allocMem;
		}
 
		allocMem = (int)System.GC.GetTotalMemory (false);
 
		peakAlloc = allocMem > peakAlloc ? allocMem : peakAlloc;
 
		if (Time.realtimeSinceStartup - lastAllocSet > 0.3F) {
			int diff = allocMem - lastAllocMemory;
			lastAllocMemory = allocMem;
			lastAllocSet = Time.realtimeSinceStartup;
 
			if (diff >= 0) {
				allocRate = diff;
			}
		}
 
		StringBuilder text = new StringBuilder ();
 		
		// add inok
		text.Append("SystemMemorySize		");
		text.Append (SystemInfo.systemMemorySize);
		text.Append ("mb\n");
		
		if(Debug.isDebugBuild){
			text.Append ("HeapSize						");
			text.Append ((Profiler.usedHeapSize/1000000F).ToString ("0.0"));
			text.Append ("mb\n");
		}
		//
		
		text.Append ("Currently allocated			");
		text.Append ((allocMem/1000000F).ToString ("0"));
		text.Append ("mb\n");
 
		text.Append ("Peak allocated				");
		text.Append ((peakAlloc/1000000F).ToString ("0"));
		text.Append ("mb (last	collect ");
		text.Append ((collectAlloc/1000000F).ToString ("0"));
		text.Append (" mb)\n");
 
 
		text.Append ("Allocation rate				");
		text.Append ((allocRate/1000000F).ToString ("0.0"));
		text.Append ("mb\n");
 
		text.Append ("Collection frequency		");
		text.Append (delta.ToString ("0.00"));
		text.Append ("s\n");
 
		text.Append ("Last collect delta			");
		text.Append (lastDeltaTime.ToString ("0.000"));
		text.Append ("s (");
		text.Append ((1F/lastDeltaTime).ToString ("0.0"));
 
		text.Append (" fps)");
 		
		if (showFPS) {
			text.Append ("\n"+(1F/Time.deltaTime).ToString ("0.0")+" fps");
		}
 		
		GUILayout.Label(text.ToString ());
		
		if(GUILayout.Button("System.GC.Collect")){
			System.GC.Collect();
		}
		
		GUI.DragWindow();
	}
	
 
}