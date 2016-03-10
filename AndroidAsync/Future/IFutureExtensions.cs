using Java.Interop;

namespace AndroidAsync.Future
{
	public static class IFutureExtensions
	{
		public static Java.Util.Concurrent.IFuture AsJavaFuture (this IFuture future)
		{
			return future.JavaCast<Java.Util.Concurrent.IFuture> ();
		}
	}
}
