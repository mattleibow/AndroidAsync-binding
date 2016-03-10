using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidAsync.Http;
using AndroidAsync.Http.Body;
using AndroidAsync.Http.Cache;

namespace AndroidAsyncSample
{
	[Activity (Label = "@string/title_activity_main", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private static ResponseCacheMiddleware cacher;
		private static BasicAuthMiddleware auther;

		private ImageView rommanager;
		private ImageView tether;
		private ImageView desksms;
		private ImageView chart;
		private TextView socketsEcho;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			if (cacher == null) {
				try {
					cacher = ResponseCacheMiddleware.AddCache (AsyncHttpClient.DefaultInstance, GetFileStreamPath ("asynccache"), 1024 * 1024 * 10);
					cacher.Caching = false;
				} catch {
					Toast.MakeText (ApplicationContext, "unable to create cache", ToastLength.Short).Show ();
				}
			}

			if (auther == null) {
				try {
					auther = BasicAuthMiddleware.Add (AsyncHttpClient.DefaultInstance);
					auther.SetAuthorization ("http://www.google.com", "username", "password");
				} catch {
					Toast.MakeText (ApplicationContext, "unable to create basic auth", ToastLength.Short).Show ();
				}
			}

			SetContentView (Resource.Layout.activity_main);

			Button b = FindViewById<Button> (Resource.Id.go);
			b.Click += delegate {
				Refresh ();
			};

			Button s = FindViewById<Button> (Resource.Id.sockets);
			s.Click += delegate {
				ConnectSockets ();
			};

			rommanager = FindViewById<ImageView> (Resource.Id.rommanager);
			tether = FindViewById<ImageView> (Resource.Id.tether);
			desksms = FindViewById<ImageView> (Resource.Id.desksms);
			chart = FindViewById<ImageView> (Resource.Id.chart);
			socketsEcho = FindViewById<TextView> (Resource.Id.socketsEcho);

			ShowCacheToast ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add ("Toggle Caching");

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.TitleFormatted.ToString () == "Toggle Caching") {
				cacher.Caching = !cacher.Caching;
				ShowCacheToast ();
				return true;
			} else {
				return base.OnOptionsItemSelected (item);
			}
		}

		private void ShowCacheToast ()
		{
			bool caching = cacher.Caching;
			Toast.MakeText (ApplicationContext, "Caching: " + caching, ToastLength.Short).Show ();
		}

		private void AssignImageView (ImageView iv, BitmapDrawable bd)
		{
			iv.Handler.Post (() => {
				iv.SetImageDrawable (bd);
			});
		}

		private void GetFile (ImageView iv, string url, string filename)
		{
			AsyncHttpClient.DefaultInstance.ExecuteFile (new AsyncHttpGet (url), filename, (ex, response, file) => {
				if (ex != null) {
					Console.WriteLine (ex);
					return;
				}

				Bitmap bitmap = BitmapFactory.DecodeFile (filename);
				file.Delete ();

				if (bitmap == null)
					return;

				BitmapDrawable bd = new BitmapDrawable (bitmap);
				AssignImageView (iv, bd);
			});
		}

		private void GetChartFile ()
		{
			ImageView iv = chart;
			string filename = GetFileStreamPath (RandomFile ()).AbsolutePath;
			List<INameValuePair> pairs = new List<INameValuePair> {
				{ new BasicNameValuePair ("cht", "lc") },
				{ new BasicNameValuePair ("chtt", "This is a google chart") },
				{ new BasicNameValuePair ("chs", "512x512") },
				{ new BasicNameValuePair ("chxt", "x") },
				{ new BasicNameValuePair ("chd", "t:40,20,50,20,100") }
			};
			UrlEncodedFormBody writer = new UrlEncodedFormBody (pairs);
			try {
				AsyncHttpPost post = new AsyncHttpPost ("http://chart.googleapis.com/chart");
				post.Body = writer;
				AsyncHttpClient.DefaultInstance.ExecuteFile (post, filename, (ex, response, file) => {
					if (ex != null) {
						Console.WriteLine (ex);
						return;
					}
					Bitmap bitmap = BitmapFactory.DecodeFile (filename);
					file.Delete ();
					if (bitmap == null)
						return;
					BitmapDrawable bd = new BitmapDrawable (bitmap);
					AssignImageView (iv, bd);
				});
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		private String RandomFile ()
		{
			return new Random ().Next (1000) + ".png";
		}

		private void Refresh ()
		{
			rommanager.SetImageBitmap (null);
			tether.SetImageBitmap (null);
			desksms.SetImageBitmap (null);
			chart.SetImageBitmap (null);

			GetFile (rommanager, "https://raw.github.com/koush/AndroidAsync/master/rommanager.png", GetFileStreamPath (RandomFile ()).AbsolutePath);
			GetFile (tether, "https://raw.github.com/koush/AndroidAsync/master/tether.png", GetFileStreamPath (RandomFile ()).AbsolutePath);
			GetFile (desksms, "https://raw.github.com/koush/AndroidAsync/master/desksms.png", GetFileStreamPath (RandomFile ()).AbsolutePath);
			GetChartFile ();

			Console.WriteLine ("cache hit: " + cacher.CacheHitCount);
			Console.WriteLine ("cache store: " + cacher.CacheStoreCount);
			Console.WriteLine ("conditional cache hit: " + cacher.ConditionalCacheHitCount);
			Console.WriteLine ("network: " + cacher.NetworkCount);
		}

		private void ConnectSockets ()
		{
			AsyncHttpClient.DefaultInstance.Websocket ("wss://echo.websocket.org", "my-protocol", (ex, webSocket) => {
				if (ex != null) {
					Console.WriteLine (ex);
					return;
				}

				// attach handlers
				webSocket.SetStringCallback (str => {
					RunOnUiThread (() => {
						socketsEcho.Text = "I got a string: " + str;
					});
				});
				((WebSocketImpl)webSocket).SetDataCallback ((emitter, byteBufferList) => {
					int length = byteBufferList.GetAllByteArray ().Length;

					RunOnUiThread (() => {
						socketsEcho.Text += "\nI got some bytes: " + length;
					});

					// note that this data has been read
					byteBufferList.Recycle ();
				});

				// send some data
				webSocket.Send ("a string");
				webSocket.Send (new byte [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
			});

		}
	}
}
