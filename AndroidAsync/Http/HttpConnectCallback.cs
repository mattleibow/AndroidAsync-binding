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
	internal class HttpConnectCallback : Java.Lang.Object, IHttpConnectCallback
	{
		private readonly Action<Exception, IAsyncHttpResponse> onConnectCompleted;

		public HttpConnectCallback (Action<Exception, IAsyncHttpResponse> onConnectCompleted)
		{
			this.onConnectCompleted = onConnectCompleted;
		}

		void IHttpConnectCallback.OnConnectCompleted (Java.Lang.Exception ex, IAsyncHttpResponse response)
		{
			var handler = onConnectCompleted;
			if (handler != null) {
				handler (ex, response);
			}
		}
	}
}
