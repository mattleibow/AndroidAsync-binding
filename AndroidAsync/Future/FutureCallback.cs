using System;
using Android.Runtime;

using AndroidAsync.Future;

namespace AndroidAsync.Future
{
	internal class FutureCallback<T> : Java.Lang.Object, IFutureCallback
		where T : class, IJavaObject
	{
		private readonly Action<Exception, T> onCompleted;

		public FutureCallback (Action<Exception, T> onCompleted)
		{
			this.onCompleted = onCompleted;
		}

		void IFutureCallback.OnCompleted (Java.Lang.Exception ex, Java.Lang.Object response)
		{
			var handler = onCompleted;
			if (handler != null) {
				handler (ex, response.JavaCast<T> ());
			}
		}
	}
}
