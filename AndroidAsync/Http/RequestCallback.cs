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
	internal class RequestCallback<T> : ResultCallback<IAsyncHttpResponse, T>, IRequestCallback
		where T : class, IJavaObject
	{
		private readonly Action<IAsyncHttpResponse> onConnect;
		private readonly Action<IAsyncHttpResponse, long, long> onProgress;

		public RequestCallback (Action<IAsyncHttpResponse> onConnect = null,
								Action<IAsyncHttpResponse, long, long> onProgress = null,
								Action<Exception, IAsyncHttpResponse, T> onCompleted = null)
			: base (onCompleted)
		{
			this.onConnect = onConnect;
			this.onProgress = onProgress;
		}

		void IRequestCallback.OnConnect (IAsyncHttpResponse response)
		{
			var handler = onConnect;
			if (handler != null) {
				handler (response);
			}
		}

		void IRequestCallback.OnProgress (IAsyncHttpResponse response, long downloaded, long total)
		{
			var handler = onProgress;
			if (handler != null) {
				handler (response, downloaded, total);
			}
		}
	}
}
