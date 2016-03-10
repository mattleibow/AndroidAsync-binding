using System;
using System.Collections.Generic;
using System.Text;

using AndroidAsync.Http;

namespace AndroidAsyncSample
{
	public class BasicAuthMiddleware : SimpleMiddleware
	{
		private readonly Dictionary<string, string> auths = new Dictionary<string, string> ();

		public override void OnRequest (AsyncHttpClientMiddlewareOnRequestData data)
		{
			base.OnRequest (data);

			// do more checking here, since uri may not necessarily be http or have a host, etc.
			string host = data.Request.Uri.Host;
			if (auths.ContainsKey (host)) {
				string auth = auths [host];
				if (!string.IsNullOrEmpty (auth))
					data.Request.SetHeader ("Authorization", auth);
			}
		}

		public void SetAuthorization (string host, string username, string password)
		{
			string text = string.Format ("{0}:{1}", username, password);
			byte [] bytes = Encoding.ASCII.GetBytes (text);
			string base64 = Convert.ToBase64String (bytes);
			string auth = "Basic " + base64;

			auths [host] = auth;
		}

		public static BasicAuthMiddleware Add (AsyncHttpClient client)
		{
			BasicAuthMiddleware ret = new BasicAuthMiddleware ();
			client.Middleware.Add (ret);
			return ret;
		}
	}
}
