using Android.Runtime;
using Java.IO;

namespace AndroidAsync.Http.Body
{
	partial class StreamBody
	{
		public InputStream Get ()
		{
			var stream =  GetAsStream () as InputStreamInvoker;
			return stream.BaseInputStream;
		}
	}
}
