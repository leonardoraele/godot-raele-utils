using System.Diagnostics.CodeAnalysis;
using Godot;
using Raele.GodotUtils.Adapters;

namespace Raele.GodotUtils.Extensions;

public static class ProjectSettingsExtensionMethods
{
	extension(ProjectSettings)
	{
		public static T GetSettingOrDefault<[MustBeVariant] T>(string setting)
			=> ProjectSettings.GetSetting(setting, Variant.From(default(T))).As<T>();
		public static bool TryGetSetting<[MustBeVariant] T>(string setting, [MaybeNullWhen(false)] out T value)
		{
			bool hasSetting = ProjectSettings.HasSetting(setting);
			value = hasSetting ? ProjectSettings.GetSetting(setting).As<T>() : default;
			return hasSetting;
		}
		public static void DefineSetting(GodotPropertyInfo property)
		{
			Variant defaultValue = property.HasDefaultValue ? property.DefaultValue : Variant.GetEmpty(property.Type);
			if (!ProjectSettings.HasSetting(property.Name))
				ProjectSettings.SetSetting(property.Name, defaultValue);
			ProjectSettings.SetInitialValue(property.Name, defaultValue);
			ProjectSettings.AddPropertyInfo(property.ToGodotDictionary());
		}
	}
}
