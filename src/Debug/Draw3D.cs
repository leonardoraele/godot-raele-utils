// #define USE_GLOBAL_DEBUG_DRAW_3D
using Godot;

namespace Raele.GodotUtils.Debug;

public partial class Draw3D
{
	public static Draw3D Singleton => field ??= new();

	public static void AddText(string key, Variant value, int priority = 0, Color? color = null, float duration = 0.0f)
#if USE_GLOBAL_DEBUG_DRAW_3D
		=> global::DebugDraw2D.SetText(key, value, priority, color, duration);
#else
	{}
#endif

	public static void DrawArrow(Vector3 a, Vector3 b, Color? color = null, float arrow_size = 0.5f, bool is_absolute_size = false, float duration = 0.0f)
#if USE_GLOBAL_DEBUG_DRAW_3D
		=> global::DebugDraw3D.DrawArrow(a, b, color, arrow_size, is_absolute_size, duration);
#else
	{}
#endif
}
