using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graphics
{
	public enum CMD{
		MOVE_TO, LINE_TO
	}
	public List<CMD> cmds;
	public List<Vector2> points;

	public Graphics()
	{
		cmds = new List<CMD>();
		points = new List<Vector2>();
	}
	public void moveTo(float x, float y)
	{
		cmds.Add(CMD.MOVE_TO);
		points.Add(new Vector2(x, y));
	}
	public void lineTo(float x, float y)
	{
		cmds.Add(CMD.LINE_TO);
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