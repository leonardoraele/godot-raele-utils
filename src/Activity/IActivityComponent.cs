
namespace Raele.GodotUtils;

public interface IActivityComponent
{
	public IActivity? ParentActivity { get; }
}

public static class IActivityComponentExtensions
{
	public static IActivityComponent AsActivityComponent(this IActivityComponent component) => component;
}
