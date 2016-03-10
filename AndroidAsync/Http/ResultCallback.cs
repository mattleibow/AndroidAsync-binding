using System;
using Android.Runtime;
using Java.IO;
using Org.Json;

using AndroidAsync;
using AndroidAsync.Callback;
using AndroidAsync.Future;
using AndroidAsync.Http.Callback;
using AndroidAsync.Parser;

namespace AndroidAsync.Http
{
	internal class ResultCallback<S, T> : Java.Lang.Object, IResultCallback
		where S : class, IJavaObject
		where T : class, IJavaObject
	{
		private readonly Action<Exception, S, T> onCompleted;

		public ResultCallback (Action<Exception, S, T> onCompleted = null)
		{
			this.onCompleted = onCompleted;
		}

		void IResultCallback.OnCompleted (Java.Lang.Exception exception, Java.Lang.Object s, Java.Lang.Object t)
		{
			var handler = onCompleted;
			if (handler != null) {
				handler (exception, s.JavaCast<S> (), t.JavaCast<T> ());
			}
		}
	}
}
