using System;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem.TimingStrategies;

[Tool][GlobalClass]
public partial class ExpressionStrategy : TimingStrategy
{
	private Node? Context = null;
	private Variant Param = new Variant();
	private string Expression
		{ get; set { field = value; this.Interpreter = null!; } }
		= "";		private Expression Interpreter
	{
		get
		{
			if (field == null)
			{
				field = new();
				field.Parse(this.Expression, ["param"]);
			}
			return field;
		}
		set;
	}
	public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
		=> new GodotPropertyInfo[]
		{
			new()
			{
				Name = nameof(Context),
				Type = Variant.Type.Object,
				Hint = PropertyHint.NodeType,
			},
			new()
			{
				Name = nameof(Param),
				Type = Variant.Type.Nil,
				Usage = [PropertyUsageFlags.Default, PropertyUsageFlags.NilIsVariant],
			},
			new()
			{
				Name = nameof(Expression),
				Type = Variant.Type.String,
				Hint = PropertyHint.Expression,
			},
		}
		.Select(GodotPropertyInfo.ToGodotDictionary)
		.ToGodotArrayT();
	public override Variant _Get(StringName property)
		=> property.ToString() switch
		{
			nameof(Context) => this.Context ?? Variant.NULL,
			nameof(Param) => this.Param,
			nameof(Expression) => this.Expression,
			_ => new Variant(),
		};
	public override bool _Set(StringName property, Variant value)
	{
		switch (property.ToString())
		{
			case nameof(Context):
				this.Context = value.AsGodotObject<Node>();
				return true;
			case nameof(Param):
				this.Param = value;
				return true;
			case nameof(Expression):
				this.Expression = value.AsString();
				return true;
		}
		return false;
	}
	public override bool _PropertyCanRevert(StringName property)
		=> property.ToString() switch
		{
			nameof(Context) => this.Context != this._PropertyGetRevert(nameof(Context)).AsGodotObject(),
			nameof(Param) => !this.Param.Equals(this._PropertyGetRevert(nameof(Param))),
			nameof(Expression) => this.Expression != this._PropertyGetRevert(nameof(Expression)).AsString(),
			_ => false,
		};
	public override Variant _PropertyGetRevert(StringName property)
		=> property.ToString() switch
		{
			nameof(Context) => Variant.NULL,
			nameof(Param) => Variant.NULL,
			nameof(Expression) => "",
			_ => Variant.NULL,
		};
	public override bool Test(TimeSpan activeTime)
		=> this.Interpreter.Execute([this.Param], this.Context).AsBool();
}
