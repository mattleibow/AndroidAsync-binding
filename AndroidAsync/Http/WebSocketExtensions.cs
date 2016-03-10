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
	public static class WebSocketExtensions
	{
		public static void SetPingCallback (this IWebSocket webSocket, Action<string> onPingReceived)
		{
			webSocket.SetPingCallback (new WebSocketPingCallback (onPingReceived));
		}

		public static void SetPongCallback (this IWebSocket webSocket, Action<string> onPongReceived)
		{
			webSocket.SetPongCallback (new WebSocketPongCallback (onPongReceived));
		}

		public static void SetStringCallback (this IWebSocket webSocket, Action<string> onStringAvailable)
		{
			webSocket.SetStringCallback (new WebSocketStringCallback (onStringAvailable));
		}


		public static void SetClosedCallback (this WebSocketImpl webSocket, Action<Exception> onCompleted)
		{
			webSocket.SetClosedCallback (new CompletedCallback (onCompleted));
		}

		public static void SetDataCallback (this WebSocketImpl webSocket, Action<IDataEmitter, ByteBufferList> onDataAvailable)
		{
			webSocket.SetDataCallback (new WebSocketDataCallback (onDataAvailable));
		}

		public static void SetEndCallback (this WebSocketImpl webSocket, Action<Exception> onCompleted)
		{
			webSocket.SetEndCallback (new CompletedCallback (onCompleted));
		}

		public static void SetWriteableCallback (this WebSocketImpl webSocket, Action onWriteable)
		{
			webSocket.SetWriteableCallback (new WritableCallback (onWriteable));
		}


		internal class WebSocketPingCallback : Java.Lang.Object, IWebSocketPingCallback
		{
			private readonly Action<string> onPingReceived;

			public WebSocketPingCallback (Action<string> onPingReceived = null)
			{
				this.onPingReceived = onPingReceived;
			}

			void IWebSocketPingCallback.OnPingReceived (string @string)
			{
				var handler = onPingReceived;
				if (handler != null) {
					handler (@string);
				}
			}
		}

		internal class WebSocketPongCallback : Java.Lang.Object, IWebSocketPongCallback
		{
			private readonly Action<string> onPongReceived;

			public WebSocketPongCallback (Action<string> onPongReceived = null)
			{
				this.onPongReceived = onPongReceived;
			}

			void IWebSocketPongCallback.OnPongReceived (string @string)
			{
				var handler = onPongReceived;
				if (handler != null) {
					handler (@string);
				}
			}
		}

		internal class WebSocketStringCallback : Java.Lang.Object, IWebSocketStringCallback
		{
			private readonly Action<string> onStringAvailable;

			public WebSocketStringCallback (Action<string> onStringAvailable = null)
			{
				this.onStringAvailable = onStringAvailable;
			}

			void IWebSocketStringCallback.OnStringAvailable (string @string)
			{
				var handler = onStringAvailable;
				if (handler != null) {
					handler (@string);
				}
			}
		}


		internal class CompletedCallback : Java.Lang.Object, ICompletedCallback
		{
			private readonly Action<Exception> onCompleted;

			public CompletedCallback (Action<Exception> onCompleted = null)
			{
				this.onCompleted = onCompleted;
			}

			void ICompletedCallback.OnCompleted (Java.Lang.Exception exception)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (exception);
				}
			}
		}

		internal class WebSocketDataCallback : Java.Lang.Object, IDataCallback
		{
			private readonly Action<IDataEmitter, ByteBufferList> onDataAvailable;

			public WebSocketDataCallback (Action<IDataEmitter, ByteBufferList> onDataAvailable = null)
			{
				this.onDataAvailable = onDataAvailable;
			}

			void IDataCallback.OnDataAvailable (IDataEmitter emitter, ByteBufferList bb)
			{
				var handler = onDataAvailable;
				if (handler != null) {
					handler (emitter, bb);
				}
			}
		}

		internal class WritableCallback : Java.Lang.Object, IWritableCallback
		{
			private readonly Action onWriteable;

			public WritableCallback (Action onWriteable = null)
			{
				this.onWriteable = onWriteable;
			}

			void IWritableCallback.OnWriteable ()
			{
				var handler = onWriteable;
				if (handler != null) {
					handler ();
				}
			}
		}

	}
}
