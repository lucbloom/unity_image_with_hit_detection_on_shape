using StoryGiant.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ImageWithHitDetectionOnShape : Image
{
#if DEVELOP_HIT_DETECTION
	int m_HitState;
	Vector2 m_InputScreenPoint;
	Vector2 m_CalculatedPointInSprite;
	Camera m_UsedCam;
#endif // DEVELOP_HIT_DETECTION

	Vector2 GetSizeForCalc()
	{
		if (preserveAspect && (type == Image.Type.Simple || type == Image.Type.Filled))
		{
			return sprite.rect.size * Mathf.Min(rectTransform.sizeDelta.x / sprite.rect.width, rectTransform.sizeDelta.y / sprite.rect.height);
		}
		return rectTransform.sizeDelta;
	}

	override public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		var verts = sprite.vertices;
		var tris = sprite.triangles;

#if DEVELOP_HIT_DETECTION
		m_InputScreenPoint = screenPoint;
#endif // DEVELOP_HIT_DETECTION

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var localPos))
		{
#if DEVELOP_HIT_DETECTION
			m_HitState = 0; // Outside rect
#else
			return false;
#endif // DEVELOP_HIT_DETECTION
		}

		var size = GetSizeForCalc();
		var pointInSpriteSpace = localPos;
		pointInSpriteSpace *= sprite.bounds.extents.xy() * 2 / size;
		pointInSpriteSpace -= Vector2.one;
		pointInSpriteSpace += rectTransform.pivot * 2;
		pointInSpriteSpace += sprite.textureRectOffset / Vec.xy(sprite.texture.width, sprite.texture.height);
		pointInSpriteSpace += sprite.bounds.center.xy();

#if DEVELOP_HIT_DETECTION
		m_CalculatedPointInSprite = pointInSpriteSpace;
#endif // DEVELOP_HIT_DETECTION
		for (var i = 0; i < tris.Length; i += 3)
		{
			var a = verts[tris[i + 0]];
			var b = verts[tris[i + 1]];
			var c = verts[tris[i + 2]];
			if (IsPointInTriangle(pointInSpriteSpace, a, b, c))
			{
#if DEVELOP_HIT_DETECTION
				m_HitState = 2; // Hit
#endif // DEVELOP_HIT_DETECTION
				return true;
			}
		}

#if DEVELOP_HIT_DETECTION
		m_HitState = 1; // Outside triangles
#endif // DEVELOP_HIT_DETECTION
		return false;
	}

#if DEVELOP_HIT_DETECTION
	void OnGUI()
	{
		if (GUI.Button(new Rect(10.0f, 10.0f, 200.0f, 30.0f), $"Draw Debug ({m_CalculatedPointInSprite})"))
		{
		}
		DrawDebug();
	}

	void DrawDebug()
	{
		var triangles = sprite.triangles;
		var vertices = sprite.vertices;

		var size = GetSizeForCalc();
		Vector2 ToScreenPoint(Vector2 p)
		{
			p -= sprite.bounds.center.xy();
			p -= sprite.textureRectOffset / Vec.xy(sprite.texture.width, sprite.texture.height);
			p -= rectTransform.pivot * 2;
			p += Vector2.one;
			p /= sprite.bounds.extents.xy() * 2 / size;
			p = transform.TransformPoint(p);
			if (m_UsedCam)
			{
				p = m_UsedCam.WorldToScreenPoint(p);
			}
			return p;
		};

		// Draw the triangles using grabbed vertices
		for (int i = 0; i < triangles.Length; i += 3)
		{
			var list = new[] {
				vertices[triangles[i + 0]],
				vertices[triangles[i + 1]],
				vertices[triangles[i + 2]],
			};
	
			var color = IsPointInTriangle(m_CalculatedPointInSprite, list[0], list[1], list[2]) ? Color.green : Color.red;

			// To see these you must view the game in the Scene tab while in Play mode
			list.For((i, p) => list[i] = ToScreenPoint(p));
			list.For((i, p) => Debug.DrawLine(p, list.Mod(i+1), color, 0.2f));
		}
	
		if (m_CalculatedPointInSprite != Vector2.zero)
		{
			var color = m_HitState == 2 ? Color.blue : (m_HitState == 1 ? Color.yellow : Color.gray);
			var screenPt = ToScreenPoint(m_CalculatedPointInSprite);
			Debug.DrawLine(screenPt + new Vector2(-10, 10), screenPt + new Vector2(10, -10), color, 0.2f);
			Debug.DrawLine(screenPt + new Vector2(-10, -10), screenPt + new Vector2(10, 10), color, 0.2f);
		}

		if (m_InputScreenPoint != Vector2.zero)
		{
			Debug.DrawLine(m_InputScreenPoint + new Vector2(-5, 0), m_InputScreenPoint + new Vector2(5, 0), Color.cyan, 0.2f);
			Debug.DrawLine(m_InputScreenPoint + new Vector2(0, -5), m_InputScreenPoint + new Vector2(0, 5), Color.cyan, 0.2f);
		}
	}
#endif // DEVELOP_HIT_DETECTION

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
