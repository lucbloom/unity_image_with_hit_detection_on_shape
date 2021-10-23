using StoryGiant.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ImageWithHitDetectionOnShape : Image
{
	override public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		var verts = sprite.vertices;
		var tris = sprite.triangles;

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var local))
		{
			return false;
		}
		var ext = Vec.XY(sprite.bounds.extents);
		var ctr = Vec.XY(sprite.bounds.center);
		var offset = ctr + sprite.textureRectOffset / new Vector2(sprite.texture.width, sprite.texture.height);
		var pointInSpriteSpace = offset + 2 * ext * local / sprite.rect.size;
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
