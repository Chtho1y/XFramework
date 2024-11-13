namespace XEngine.Engine
{
	public interface ILuaFunction
	{
		object[] Call(params object[] args);
	}
}