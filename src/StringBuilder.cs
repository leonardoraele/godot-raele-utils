using System.Collections.Generic;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public class StringBuilder
{
	private readonly List<string> Parts = [];
	private string? Prefix;
	private string? Suffix;

	public StringBuilder Append(string part)
	{
		this.Parts.Add(part);
		return this;
	}

	public StringBuilder AppendMany(IEnumerable<string> parts)
	{
		this.Parts.AddRange(parts);
		return this;
	}

	public StringBuilder AppendIf(bool condition, string part)
	{
		if (condition)
			return this.Append(part);
		return this;
	}

	public StringBuilder AppendIfNotEmpty(string? part)
		=> this.AppendIf(!string.IsNullOrEmpty(part), part ?? "");

	public StringBuilder WithSuffix(string suffix)
	{
		this.Suffix = suffix;
		return this;
	}

	public StringBuilder WithPrefix(string prefix)
	{
		this.Prefix = prefix;
		return this;
	}

	public StringBuilder Wrap(string prefix, string suffix)
	{
		this.Prefix = prefix;
		this.Suffix = suffix;
		return this;
	}

	public string Join(string separator)
		=> this.Parts.Count > 0 ? $"{this.Prefix}{this.Parts.JoinIntoString(separator)}{this.Suffix}" : "";

	public override string ToString()
		=> this.Join("");
}
