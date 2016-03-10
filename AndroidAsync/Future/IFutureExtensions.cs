using System.Threading;
using System.Threading.Tasks;
using Android.Runtime;

namespace AndroidAsync.Future
{
	public static class IFutureExtensions
	{
		public static Java.Util.Concurrent.IFuture AsJavaFuture (this IFuture future)
		{
			return future.JavaCast<Java.Util.Concurrent.IFuture> ();
		}

		public static Task AsTask (this IFuture future, CancellationToken token = default (CancellationToken))
		{
			return future.AsTask<Java.Lang.Object> (token);
		}

		public static Task<T> AsTask<T> (this IFuture future, CancellationToken token = default (CancellationToken))
			where T : class, IJavaObject
		{
			var tcs = new TaskCompletionSource<T> ();

			future.SetCallback (new TaskFutureCallback<T> (tcs, future));
			token.Register (() => {
				future.Cancel ();
			});

			return tcs.Task;
		}

		private class TaskFutureCallback<T> : Java.Lang.Object, IFutureCallback
			where T : class, IJavaObject
		{
			private readonly TaskCompletionSource<T> tcs;
			private readonly IFuture future;

			public TaskFutureCallback (TaskCompletionSource<T> tcs, IFuture future)
			{
				this.tcs = tcs;
				this.future = future;
			}

			void IFutureCallback.OnCompleted (Java.Lang.Exception exception, Java.Lang.Object result)
			{
				if (future.IsCancelled) {
					tcs.SetCanceled ();
				} else if (exception != null) {
					tcs.SetException (exception);
				} else {
					tcs.SetResult (result.JavaCast<T> ());
				}
			}
		}
	}
}
