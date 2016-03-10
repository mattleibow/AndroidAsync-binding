using System;
using System.Threading;
using System.Threading.Tasks;
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
		public IFuture Execute (string uri,
								Action<Exception, IAsyncHttpResponse> onConnectCompleted)
		{
			return Execute (uri, new HttpConnectCallback (onConnectCompleted));
		}

		public Task<IAsyncHttpResponse> ExecuteAsync (string uri, CancellationToken token = default (CancellationToken))
		{
			return Execute (uri, (IHttpConnectCallback)null).AsTask<IAsyncHttpResponse> (token);
		}

		public IFuture Execute (AsyncHttpRequest request,
								Action<Exception, IAsyncHttpResponse> onConnectCompleted)
		{
			return Execute (request, new HttpConnectCallback (onConnectCompleted));
		}

		public Task<IAsyncHttpResponse> ExecuteAsync (AsyncHttpRequest request, CancellationToken token = default (CancellationToken))
		{
			return Execute (request, (IHttpConnectCallback)null).AsTask<IAsyncHttpResponse> (token);
		}

		public SimpleFuture Execute<T> (AsyncHttpRequest req,
										IAsyncParser parser,
										Action<Exception, IAsyncHttpResponse, T> onCompleted,
										Action<IAsyncHttpResponse> onConnect = null,
										Action<IAsyncHttpResponse, long, long> onProgress = null)
			where T : class, IJavaObject
		{
			return Execute (req, parser, new RequestCallback<T> (onConnect, onProgress, onCompleted));
		}

		public Task<T> ExecuteAsync<T> (AsyncHttpRequest req, IAsyncParser parser, CancellationToken token = default (CancellationToken))
			where T : class, IJavaObject
		{
			return Execute (req, parser, (IRequestCallback)null).AsTask<T> (token);
		}

		public IFuture ExecuteByteBufferList (AsyncHttpRequest req,
											  Action<Exception, IAsyncHttpResponse, ByteBufferList> onCompleted,
											  Action<IAsyncHttpResponse> onConnect = null,
											  Action<IAsyncHttpResponse, long, long> onProgress = null)
		{
			return ExecuteByteBufferList (req, new ActionDownloadCallback (onConnect, onProgress, onCompleted));
		}

		public Task<ByteBufferList> ExecuteByteBufferListAsync (AsyncHttpRequest req, CancellationToken token = default (CancellationToken))
		{
			return ExecuteByteBufferList (req, (DownloadCallback)null).AsTask<ByteBufferList> (token);
		}

		public IFuture ExecuteFile (AsyncHttpRequest req,
									string filename,
									Action<Exception, IAsyncHttpResponse, File> onCompleted,
									Action<IAsyncHttpResponse> onConnect = null,
									Action<IAsyncHttpResponse, long, long> onProgress = null)
		{
			return ExecuteFile (req, filename, new ActionFileCallback (onConnect, onProgress, onCompleted));
		}

		public Task<File> ExecuteFileAsync (AsyncHttpRequest req, string filename, CancellationToken token = default (CancellationToken))
		{
			return ExecuteFile (req, filename, (FileCallback)null).AsTask<File> (token);
		}

		public IFuture ExecuteJSONArray (AsyncHttpRequest req,
										 Action<Exception, IAsyncHttpResponse, JSONArray> onCompleted,
										 Action<IAsyncHttpResponse> onConnect = null,
										 Action<IAsyncHttpResponse, long, long> onProgress = null)
		{
			return ExecuteJSONArray (req, new ActionJSONArrayCallback (onConnect, onProgress, onCompleted));
		}

		public Task<JSONArray> ExecuteJSONArrayAsync (AsyncHttpRequest req, CancellationToken token = default (CancellationToken))
		{
			return ExecuteJSONArray (req, (JSONArrayCallback)null).AsTask<JSONArray> (token);
		}

		public IFuture ExecuteJSONObject (AsyncHttpRequest req,
										  Action<Exception, IAsyncHttpResponse, JSONObject> onCompleted,
										  Action<IAsyncHttpResponse> onConnect = null,
										  Action<IAsyncHttpResponse, long, long> onProgress = null)
		{
			return ExecuteJSONObject (req, new ActionJSONObjectCallback (onConnect, onProgress, onCompleted));
		}

		public Task<JSONObject> ExecuteJSONObjectAsync (AsyncHttpRequest req, CancellationToken token = default (CancellationToken))
		{
			return ExecuteJSONObject (req, (JSONObjectCallback)null).AsTask<JSONObject> (token);
		}

		public IFuture ExecuteString (AsyncHttpRequest req,
									  Action<Exception, IAsyncHttpResponse, string> onCompleted,
									  Action<IAsyncHttpResponse> onConnect = null,
									  Action<IAsyncHttpResponse, long, long> onProgress = null)
		{
			return ExecuteString (req, new ActionStringCallback (onConnect, onProgress, onCompleted));
		}

		public async Task<string> ExecuteStringAsync (AsyncHttpRequest req, CancellationToken token = default (CancellationToken))
		{
			return (await ExecuteString (req, (StringCallback)null).AsTask<Java.Lang.String> (token)).ToString ();
		}

		public IFuture Websocket (AsyncHttpRequest req,
								  string protocol,
								  Action<Exception, IWebSocket> onCompleted)
		{
			return Websocket (req, protocol, new WebSocketConnectCallback (onCompleted));
		}

		public Task<IWebSocket> WebsocketAsync (AsyncHttpRequest req, string protocol, CancellationToken token = default (CancellationToken))
		{
			return Websocket (req, protocol, (IWebSocketConnectCallback)null).AsTask<IWebSocket> (token);
		}

		public IFuture Websocket (string uri,
								  string protocol,
								  Action<Exception, IWebSocket> onCompleted)
		{
			return Websocket (uri, protocol, new WebSocketConnectCallback (onCompleted));
		}

		public Task<IWebSocket> WebsocketAsync (string uri, string protocol, CancellationToken token = default (CancellationToken))
		{
			return Websocket (uri, protocol, (IWebSocketConnectCallback)null).AsTask<IWebSocket> (token);
		}


		private class ActionJSONArrayCallback : JSONArrayCallback
		{
			private readonly Action<IAsyncHttpResponse> onConnect;
			private readonly Action<IAsyncHttpResponse, long, long> onProgress;
			private readonly Action<Exception, IAsyncHttpResponse, JSONArray> onCompleted;

			public ActionJSONArrayCallback (Action<IAsyncHttpResponse> onConnect = null,
											Action<IAsyncHttpResponse, long, long> onProgress = null,
											Action<Exception, IAsyncHttpResponse, JSONArray> onCompleted = null)
			{
				this.onConnect = onConnect;
				this.onProgress = onProgress;
				this.onCompleted = onCompleted;
			}

			public override void OnConnect (IAsyncHttpResponse response)
			{
				var handler = onConnect;
				if (handler != null) {
					handler (response);
				}
			}

			public override void OnProgress (IAsyncHttpResponse response, long downloaded, long total)
			{
				var handler = onProgress;
				if (handler != null) {
					handler (response, downloaded, total);
				}
			}

			public override void OnCompleted (Java.Lang.Exception exception, IAsyncHttpResponse s, JSONArray t)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (exception, s, t);
				}
			}
		}

		private class ActionJSONObjectCallback : JSONObjectCallback
		{
			private readonly Action<IAsyncHttpResponse> onConnect;
			private readonly Action<IAsyncHttpResponse, long, long> onProgress;
			private readonly Action<Exception, IAsyncHttpResponse, JSONObject> onCompleted;

			public ActionJSONObjectCallback (Action<IAsyncHttpResponse> onConnect = null,
											 Action<IAsyncHttpResponse, long, long> onProgress = null,
											 Action<Exception, IAsyncHttpResponse, JSONObject> onCompleted = null)
			{
				this.onConnect = onConnect;
				this.onProgress = onProgress;
				this.onCompleted = onCompleted;
			}

			public override void OnConnect (IAsyncHttpResponse response)
			{
				var handler = onConnect;
				if (handler != null) {
					handler (response);
				}
			}

			public override void OnProgress (IAsyncHttpResponse response, long downloaded, long total)
			{
				var handler = onProgress;
				if (handler != null) {
					handler (response, downloaded, total);
				}
			}

			public override void OnCompleted (Java.Lang.Exception exception, IAsyncHttpResponse s, JSONObject t)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (exception, s, t);
				}
			}
		}

		private class ActionFileCallback : FileCallback
		{
			private readonly Action<IAsyncHttpResponse> onConnect;
			private readonly Action<IAsyncHttpResponse, long, long> onProgress;
			private readonly Action<Exception, IAsyncHttpResponse, File> onCompleted;

			public ActionFileCallback (Action<IAsyncHttpResponse> onConnect = null,
									   Action<IAsyncHttpResponse, long, long> onProgress = null,
									   Action<Exception, IAsyncHttpResponse, File> onCompleted = null)
			{
				this.onConnect = onConnect;
				this.onProgress = onProgress;
				this.onCompleted = onCompleted;
			}

			public override void OnConnect (IAsyncHttpResponse response)
			{
				var handler = onConnect;
				if (handler != null) {
					handler (response);
				}
			}

			public override void OnProgress (IAsyncHttpResponse response, long downloaded, long total)
			{
				var handler = onProgress;
				if (handler != null) {
					handler (response, downloaded, total);
				}
			}

			public override void OnCompleted (Java.Lang.Exception exception, IAsyncHttpResponse s, File t)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (exception, s, t);
				}
			}
		}

		private class ActionDownloadCallback : DownloadCallback
		{
			private readonly Action<IAsyncHttpResponse> onConnect;
			private readonly Action<IAsyncHttpResponse, long, long> onProgress;
			private readonly Action<Exception, IAsyncHttpResponse, ByteBufferList> onCompleted;

			public ActionDownloadCallback (Action<IAsyncHttpResponse> onConnect = null,
										   Action<IAsyncHttpResponse, long, long> onProgress = null,
										   Action<Exception, IAsyncHttpResponse, ByteBufferList> onCompleted = null)
			{
				this.onConnect = onConnect;
				this.onProgress = onProgress;
				this.onCompleted = onCompleted;
			}

			public override void OnConnect (IAsyncHttpResponse response)
			{
				var handler = onConnect;
				if (handler != null) {
					handler (response);
				}
			}

			public override void OnProgress (IAsyncHttpResponse response, long downloaded, long total)
			{
				var handler = onProgress;
				if (handler != null) {
					handler (response, downloaded, total);
				}
			}

			public override void OnCompleted (Java.Lang.Exception exception, IAsyncHttpResponse s, ByteBufferList t)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (exception, s, t);
				}
			}
		}

		private class ActionStringCallback : StringCallback
		{
			private readonly Action<IAsyncHttpResponse> onConnect;
			private readonly Action<IAsyncHttpResponse, long, long> onProgress;
			private readonly Action<Exception, IAsyncHttpResponse, string> onCompleted;

			public ActionStringCallback (Action<IAsyncHttpResponse> onConnect = null,
										 Action<IAsyncHttpResponse, long, long> onProgress = null,
										 Action<Exception, IAsyncHttpResponse, string> onCompleted = null)
			{
				this.onConnect = onConnect;
				this.onProgress = onProgress;
				this.onCompleted = onCompleted;
			}

			public override void OnConnect (IAsyncHttpResponse response)
			{
				var handler = onConnect;
				if (handler != null) {
					handler (response);
				}
			}

			public override void OnProgress (IAsyncHttpResponse response, long downloaded, long total)
			{
				var handler = onProgress;
				if (handler != null) {
					handler (response, downloaded, total);
				}
			}

			public override void OnCompleted (Java.Lang.Exception exception, IAsyncHttpResponse s, string t)
			{
				var handler = onCompleted;
				if (handler != null) {
					handler (exception, s, t);
				}
			}
		}

		partial class FileCallback : IResultCallback, IRequestCallback
		{
			void IResultCallback.OnCompleted (Java.Lang.Exception e, Java.Lang.Object source, Java.Lang.Object result)
			{
				OnCompleted (e, source.JavaCast<IAsyncHttpResponse> (), result.JavaCast<File> ());
			}
		}

		partial class StringCallback : IResultCallback, IRequestCallback
		{
			void IResultCallback.OnCompleted (Java.Lang.Exception e, Java.Lang.Object source, Java.Lang.Object result)
			{
				OnCompleted (e, source.JavaCast<IAsyncHttpResponse> (), result.JavaCast<Java.Lang.String> ().ToString ());
			}
		}

		partial class DownloadCallback : IResultCallback, IRequestCallback
		{
			void IResultCallback.OnCompleted (Java.Lang.Exception e, Java.Lang.Object source, Java.Lang.Object result)
			{
				OnCompleted (e, source.JavaCast<IAsyncHttpResponse> (), result.JavaCast<ByteBufferList> ());
			}
		}

		partial class JSONArrayCallback : IResultCallback, IRequestCallback
		{
			void IResultCallback.OnCompleted (Java.Lang.Exception e, Java.Lang.Object source, Java.Lang.Object result)
			{
				OnCompleted (e, source.JavaCast<IAsyncHttpResponse> (), result.JavaCast<JSONArray> ());
			}
		}

		partial class JSONObjectCallback : IResultCallback, IRequestCallback
		{
			void IResultCallback.OnCompleted (Java.Lang.Exception e, Java.Lang.Object source, Java.Lang.Object result)
			{
				OnCompleted (e, source.JavaCast<IAsyncHttpResponse> (), result.JavaCast<JSONObject> ());
			}
		}
	}
}
