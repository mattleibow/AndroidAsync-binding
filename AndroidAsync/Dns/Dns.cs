using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Runtime;
using Java.IO;
using Org.Json;

using AndroidAsync;
using AndroidAsync.Future;

namespace AndroidAsync.Dns
{
	partial class Dns
	{
		public static IFuture Lookup (AsyncServer server, string host, bool multicast, Action<Exception, DnsResponse> onCompleted)
		{
			return Lookup (server, host, multicast, new FutureCallback<DnsResponse> (onCompleted));
		}

		public static Task<DnsResponse> LookupAsync (AsyncServer server, string host, CancellationToken token = default (CancellationToken))
		{
			return Lookup (server, host).AsTask<DnsResponse> (token);
		}

		public static Task<DnsResponse> LookupAsync (string host, CancellationToken token = default (CancellationToken))
		{
			return Lookup (host).AsTask<DnsResponse> (token);
		}

		public static ICancellable MulticastLookup (AsyncServer server, string host, Action<Exception, DnsResponse> onCompleted)
		{
			return MulticastLookup (server, host, new FutureCallback<DnsResponse> (onCompleted));
		}

		public static ICancellable MulticastLookup (string host, Action<Exception, DnsResponse> onCompleted)
		{
			return MulticastLookup (host, new FutureCallback<DnsResponse> (onCompleted));
		}
	}
}
