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
	partial class AsyncHttpClient
	{
		public class WebSocketConnectCallback : Java.Lang.Object, IWebSocketConnectCallback
		{
			private readonly Action<Exception, IWebSocket> onCompleted;

			public WebSocketConnectCallback (Action<Exception, IWebSocket> onCompleted)
			{
				this.onCompleted = onCompleted;
			}

			public void OnCompleted (Java.Lang.Exception ex, IWebSocket webSocket)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (ex, webSocket);
				}
			}
		}
	}
}
