using System;
using System.IO;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class LoggingExtensions
{
	extension(GodotObject self)
	{
		// TODO Use GD.Print instead when `OS.IsDebugBuild() == false`
		public void DebugLog(string message, params object[] details)
			=> GD.PrintRich([
				$"[bgcolor={GetNextColorCode()}]",
				message,
				$"[color=dim_gray]",
				..details.Select(@object => $"\n ├ {@object}"),
				$"\n └ {GetFrameString()} {GetTimeString()} @ {self.GetIdentityString()}"
				// $"\n\t[hint={detailsObj.ToString().ToJsonString()}]{message}[/hint]"
			]);

		private string GetIdentityString()
			=> self is Node node ? $"\"{node.GetPath()}\""
				: self is Resource res ? $"{res.GetType().Name} \"{(!res.ResourceName.IsWhiteSpace() ? res.ResourceName : Path.GetFileName(res.ResourcePath))}\" {{{res.GetRid()}}}"
				: self.GetType().Name;
	}

	private static readonly Color[] COLORS = Enumerable.Range(0, 8).Select(i => Color.FromHsv(i / 8f, 1f, .2f)).ToArray();
	private static int colorIndex = 0;
	private static string GetNextColorCode() => COLORS[colorIndex = (colorIndex + 3) % COLORS.Length].ToHtml();

	// private static IEnumerable<string> ToChunks(string message, int size = 80)
	// 	// => message.Chunk(80).Select(chars => new string(chars)) // -TODO Bechmark performance of this vs current implementation
	// 	=> Enumerable.Range(0, message.Length / size + 1)
	// 		.Select(i => message.Substring(i * size, Math.Min(size, message.Length - i * size)));

	private static string GetFrameString()
		=> $"#{Engine.GetPhysicsFrames():N0}";
	private static string GetTimeString()
		=> DateTime.Now.ToString("HH:mm:ss.fff");
}
