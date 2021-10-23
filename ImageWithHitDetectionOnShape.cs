using StoryGiant.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ImageWithHitDetectionOnShape : Image
{
	//PolygonCollider2D polygoncollider;
	//LineRenderer lr;
	//Vector2 pt;

    //protected override void Awake()
	//{
	//	//polygoncollider = gameObject.AddComponent<PolygonCollider2D>();
	//	lr = gameObject.GetComponent<LineRenderer>();
	//}

	override public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		var verts = sprite.vertices;
		var tris = sprite.triangles;

		//var pathCount = sprite.GetPhysicsShapeCount();
		//for (int i = 0; i < pathCount; i++)
		//{
		//	SGDebug.Log(LogTag.Luc, i);
		//	var points = new List<Vector2>();
		//	sprite.GetPhysicsShape(i, points);
		//	lr.SetPositions(points.Select(points => new Vector3(points.x, points.y)).ToArray());
		//	foreach (var v in points)
		//	{
		//		SGDebug.Log(LogTag.Luc, v);
		//	}
		//}
		//
		//var pts = new List<Vector2>();
		//for (var i = 0; i < tris.Length; i += 3)
		//{
		//	for (var t = 0; t < 3; ++t)
		//	{
		//		pts.Add(verts[tris[i + t]]);
		//		pts.Add(verts[tris[i + ((t + 1) % 3)]]);
		//	}
		//}
		//var rw = sprite.rect.width;
		//var rh = sprite.rect.height;
		//var fx = rw / (2.0f * ext.x);
		//var fy = rh / (2.0f * ext.y);
		//var ox = -ctr.x - (sprite.textureRectOffset.x / sprite.texture.width) + ext.x;
		//var oy = -ctr.y - (sprite.textureRectOffset.y / sprite.texture.height) + ext.y;
		//lr.SetPositions(pts.Select(p => new Vector3(
		//	Mathf.Clamp((p.x + ox) * fx, 0.0f, rw),
		//	 Mathf.Clamp((p.y + oy) * fy, 0.0f, rh))).ToArray());
		//
		//var worldSpace = (eventCamera ?? Camera.main).ScreenToWorldPoint(screenPoint);
		//var pointInImageSpace = rectTransform.InverseTransformPoint(worldSpace);
		//var pointInSpriteSpace = ctr - ext + 2 * ext * pointInImageSpace / sprite.rect.size; // TODO!


		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var local))
		{
			return false;
		}
		var ext = Vec.XY(sprite.bounds.extents);
		var ctr = Vec.XY(sprite.bounds.center);
		var offset = ctr + sprite.textureRectOffset / new Vector2(sprite.texture.width, sprite.texture.height);
		var pointInSpriteSpace = offset + 2 * ext * local / sprite.rect.size; // TODO: 

		//pt = pointInSpriteSpace;
		for (var i = 0; i < tris.Length; i += 3)
		{
			var a = verts[tris[i + 0]];
			var b = verts[tris[i + 1]];
			var c = verts[tris[i + 2]];
			if (IsPointInTriangle(pointInSpriteSpace, a, b, c))
			{
				return true;
			}
		}

		return false;
	}

	//void OnGUI()
	//{
	//	if (GUI.Button(new Rect(10.0f, 10.0f, 200.0f, 30.0f), $"Draw Debug ({pt})"))
	//	{
	//	}
	//	DrawDebug();
	//}

	//void DrawDebug()
	//{
	//	ushort[] triangles = sprite.triangles;
	//	Vector2[] vertices = sprite.vertices;
	//
	//	// Draw the triangles using grabbed vertices
	//	for (int i = 0; i < triangles.Length; i = i + 3)
	//	{
	//		var a = vertices[triangles[i + 0]];
	//		var b = vertices[triangles[i + 1]];
	//		var c = vertices[triangles[i + 2]];
	//
	//		var color = IsPointInTriangle(pt, a, b, c) ? Color.green : Color.red;
	//		//To see these you must view the game in the Scene tab while in Play mode
	//		Debug.DrawLine(a, b, color, 0.2f);
	//		Debug.DrawLine(b, c, color, 0.2f);
	//		Debug.DrawLine(c, a, color, 0.2f);
	//	}
	//
	//	if (pt != Vector2.zero)
	//	{
	//		Debug.DrawLine(pt + new Vector2(-100, 100), pt + new Vector2(10, -10), Color.blue, 0.2f);
	//		Debug.DrawLine(pt + new Vector2(-100, -100), pt + new Vector2(10, 10), Color.blue, 0.2f);
	//	}
	//}

	bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
	{
		// Compute vectors
		var v0 = c - a;
		var v1 = b - a;
		var v2 = p - a;

		// Compute dot products
		var dot00 = Vector2.Dot(v0, v0);
		var dot01 = Vector2.Dot(v0, v1);
		var dot02 = Vector2.Dot(v0, v2);
		var dot11 = Vector2.Dot(v1, v1);
		var dot12 = Vector2.Dot(v1, v2);

		// Compute barycentric coordinates
		var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
		var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
		var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

		// Check if point is in triangle
		return (u >= 0) && (v >= 0) && (u + v < 1);
	}
}
