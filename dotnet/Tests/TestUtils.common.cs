﻿using Applitools.Metadata;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Applitools.Tests.Utils
{
    internal partial class TestUtils
    {
        public static readonly bool RUNS_ON_CI = Environment.GetEnvironmentVariable("CI") != null;
        public const string COVERED_BY_GENERATED_TESTS_MESSAGE = "covered by generated tests";

        public static string InitLogPath([CallerMemberName] string testName = null)
        {
            string dateString = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
            string extendedTestName = $"{testName}_{dateString}";
            string logsPath = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";
            string path = Path.Combine(logsPath, "DotNet", extendedTestName);
            return path;
        }

        public static ILogHandler InitLogHandler([CallerMemberName] string testName = null, string logPath = null)
        {
            if (!RUNS_ON_CI)
            {
                string path = logPath ?? InitLogPath(testName);
                return new FileLogHandler(Path.Combine(path, "log.log"), true, true);
            }
            return new NunitLogHandler(false);
        }

        public static void SetupLogging(EyesBase eyes, [CallerMemberName] string testName = null)
        {
            ILogHandler logHandler = null;
            if (!RUNS_ON_CI)
            {
                string path = InitLogPath(testName);
                //eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider()
                //{
                //    Path = path,
                //    Prefix = testName + "_"
                //};
                logHandler = new FileLogHandler(Path.Combine(path, testName + ".log"), true, true);
            }
            else
            {
                logHandler = new NunitLogHandler(false);
            }

            if (logHandler != null)
            {
                //eyes.SetLogHandler(logHandler);
            }
        }

        public static SessionResults GetSessionResults(string apiKey, Applitools.TestResults testResults)
        {
            string apiSessionUrl = testResults?.ApiUrls?.Session;
            if (string.IsNullOrWhiteSpace(apiSessionUrl)) return null;
            UriBuilder uriBuilder = new UriBuilder(apiSessionUrl);
            NameValueCollection query = UrlUtility.ParseQueryString(uriBuilder.Query);
            query["format"] = "json";
            query["AccessToken"] = testResults.SecretToken;
            query["apiKey"] = apiKey;
            uriBuilder.Query = query.ToString();
            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            SessionResults sessionResults = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            TimeSpan timeout = TimeSpan.FromSeconds(40);
            while (sessionResults == null && stopwatch.Elapsed < timeout)
            {
                using (HttpResponseMessage metaResults = client.Get(uriBuilder.ToString()))
                {
                    sessionResults = metaResults.DeserializeBody<SessionResults>(false);
                }
                if (sessionResults != null && sessionResults.ActualAppOutput.Length > 0) break;
                System.Threading.Thread.Sleep(500);
            }
            return sessionResults;
        }

        public static string GetDom(string apiKey, Applitools.TestResults testResults, string domId)
        {
            string apiSessionUrl = testResults?.AppUrls?.Session;
            if (string.IsNullOrWhiteSpace(apiSessionUrl)) return null;
            UriBuilder uriBuilder = new UriBuilder(apiSessionUrl);
            NameValueCollection query = UrlUtility.ParseQueryString(uriBuilder.Query);
            query["apiKey"] = apiKey;
            uriBuilder.Query = query.ToString();
            uriBuilder.Path = $"/api/images/dom/{domId}/";

            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            using (HttpResponseMessage response = client.Get(uriBuilder.ToString()))
            using (Stream s = response.GetResponseStream())
            {
                var json = new StreamReader(s).ReadToEnd();
                return json;
            }
        }

        public static BatchInfo GetBatchInfo(EyesBaseConfig eyes)
        {
            UriBuilder uriBuilder = new UriBuilder(eyes.ServerUrl);
            uriBuilder.Path = $"api/sessions/batches/{eyes.Batch.Id}/bypointerid";
            NameValueCollection query = UrlUtility.ParseQueryString(uriBuilder.Query);
            query["apiKey"] = eyes.ApiKey;
            uriBuilder.Query = query.ToString();
            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            using (HttpResponseMessage batchInfoResponse = client.Get(uriBuilder.ToString()))
            {
                BatchInfo batchInfo = batchInfoResponse.DeserializeBody<BatchInfo>(false);
                return batchInfo;
            }
        }

        public static void compareProcedure(Region actualRegion, Region expectedRegion, string type = null)
        {
            Region[] actualRegions = new Region[] { actualRegion };
            HashSet<Region> expectedRegions = new HashSet<Region> { expectedRegion };
            TestUtils.CompareSimpleRegionsList_(actualRegions, expectedRegions, "Region");
        }

        public static void compareProcedure(FloatingMatchSettings actualRegion, FloatingMatchSettings expectedRegion, string type = "Floating")
        {
            FloatingMatchSettings[] actualRegions = new FloatingMatchSettings[] { actualRegion };
            HashSet<FloatingMatchSettings> expectedRegions = new HashSet<FloatingMatchSettings> { expectedRegion };
            HashSet<FloatingMatchSettings> expectedRegionsClone = new HashSet<FloatingMatchSettings>(expectedRegions);
            if (expectedRegions.Count > 0)
            {
                foreach (FloatingMatchSettings region in actualRegions)
                {
                    if (!expectedRegionsClone.Remove(region))
                    {
                        Assert.Fail("actual {0} region {1} not found in expected regions list", type, region);
                    }
                }
                Assert.IsEmpty(expectedRegionsClone, "not all expected regions found in actual regions list.", type);
            }
        }

        public static void CompareSimpleRegionsList_(Region[] actualRegions, HashSet<Region> expectedRegions, string type)
        {
            HashSet<Region> expectedRegionsClone = new HashSet<Region>(expectedRegions);
            if (expectedRegions.Count > 0)
            {
                foreach (Region region in actualRegions)
                {
                    if (!expectedRegionsClone.Remove(region))
                    {
                        Assert.Fail("actual {0} region {1} not found in expected regions list", type, region);
                    }
                }
                Assert.IsEmpty(expectedRegionsClone, "not all expected {0} regions found in actual regions list.", type);
            }
        }

        //copied from System.Web.HttpUtility code (renamed here) to remove dependency on System.Web.dll
        public static class UrlUtility
        {
            //  Query string parsing support
            public static NameValueCollection ParseQueryString(string query)
            {
                return ParseQueryString(query, Encoding.UTF8);
            }

            public static NameValueCollection ParseQueryString(string query, Encoding encoding)
            {
                if (query == null)
                {
                    throw new ArgumentNullException("query");
                }

                if (encoding == null)
                {
                    throw new ArgumentNullException("encoding");
                }

                if (query.Length > 0 && query[0] == '?')
                {
                    query = query.Substring(1);
                }

                return new HttpValueCollection(query, encoding);
            }

            public static string UrlEncode(string str)
            {
                if (str == null)
                {
                    return null;
                }
                return UrlEncode(str, Encoding.UTF8);
            }

            // URL encodes a path portion of a URL string and returns the encoded string.
            public static string UrlPathEncode(string str)
            {
                if (str == null)
                {
                    return null;
                }

                // recurse in case there is a query string
                int i = str.IndexOf('?');
                if (i >= 0)
                {
                    return UrlPathEncode(str.Substring(0, i)) + str.Substring(i);
                }

                // encode DBCS characters and spaces only
                return UrlEncodeSpaces(UrlEncodeNonAscii(str, Encoding.UTF8));
            }

            public static string UrlEncode(string str, Encoding encoding)
            {
                if (str == null)
                {
                    return null;
                }
                return Encoding.ASCII.GetString(UrlEncodeToBytes(str, encoding));
            }

            public static string UrlEncodeUnicode(string str)
            {
                if (str == null)
                    return null;
                return UrlEncodeUnicodeStringToStringInternal(str, false);

            }

            private static string UrlEncodeUnicodeStringToStringInternal(string s, bool ignoreAscii)
            {
                int l = s.Length;
                StringBuilder sb = new StringBuilder(l);

                for (int i = 0; i < l; i++)
                {
                    char ch = s[i];

                    if ((ch & 0xff80) == 0)
                    {  // 7 bit?
                        if (ignoreAscii || IsSafe(ch))
                        {
                            sb.Append(ch);
                        }
                        else if (ch == ' ')
                        {
                            sb.Append('+');
                        }
                        else
                        {
                            sb.Append('%');
                            sb.Append(IntToHex((ch >> 4) & 0xf));
                            sb.Append(IntToHex((ch) & 0xf));
                        }
                    }
                    else
                    { // arbitrary Unicode?
                        sb.Append("%u");
                        sb.Append(IntToHex((ch >> 12) & 0xf));
                        sb.Append(IntToHex((ch >> 8) & 0xf));
                        sb.Append(IntToHex((ch >> 4) & 0xf));
                        sb.Append(IntToHex((ch) & 0xf));
                    }
                }

                return sb.ToString();
            }

            //  Helper to encode the non-ASCII url characters only
            static string UrlEncodeNonAscii(string str, Encoding e)
            {
                if (string.IsNullOrEmpty(str))
                {
                    return str;
                }
                if (e == null)
                {
                    e = Encoding.UTF8;
                }
                byte[] bytes = e.GetBytes(str);
                bytes = UrlEncodeBytesToBytesInternalNonAscii(bytes, 0, bytes.Length, false);
                return Encoding.ASCII.GetString(bytes);
            }

            //  Helper to encode spaces only
            static string UrlEncodeSpaces(string str)
            {
                if (str != null && str.IndexOf(' ') >= 0)
                {
                    str = str.Replace(" ", "%20");
                }
                return str;
            }

            public static byte[] UrlEncodeToBytes(string str, Encoding e)
            {
                if (str == null)
                {
                    return null;
                }
                byte[] bytes = e.GetBytes(str);
                return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
            }

            //public static string UrlDecode(string str)
            //{
            //    if (str == null)
            //        return null;
            //    return UrlDecode(str, Encoding.UTF8);
            //}

            public static string UrlDecode(string str, Encoding e)
            {
                if (str == null)
                {
                    return null;
                }
                return UrlDecodeStringFromStringInternal(str, e);
            }

            //  Implementation for encoding
            static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
            {
                int cSpaces = 0;
                int cUnsafe = 0;

                // count them first
                for (int i = 0; i < count; i++)
                {
                    char ch = (char)bytes[offset + i];

                    if (ch == ' ')
                    {
                        cSpaces++;
                    }
                    else if (!IsSafe(ch))
                    {
                        cUnsafe++;
                    }
                }

                // nothing to expand?
                if (!alwaysCreateReturnValue && cSpaces == 0 && cUnsafe == 0)
                {
                    return bytes;
                }

                // expand not 'safe' characters into %XX, spaces to +s
                byte[] expandedBytes = new byte[count + cUnsafe * 2];
                int pos = 0;

                for (int i = 0; i < count; i++)
                {
                    byte b = bytes[offset + i];
                    char ch = (char)b;

                    if (IsSafe(ch))
                    {
                        expandedBytes[pos++] = b;
                    }
                    else if (ch == ' ')
                    {
                        expandedBytes[pos++] = (byte)'+';
                    }
                    else
                    {
                        expandedBytes[pos++] = (byte)'%';
                        expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                        expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                    }
                }

                return expandedBytes;
            }


            static bool IsNonAsciiByte(byte b)
            {
                return (b >= 0x7F || b < 0x20);
            }

            static byte[] UrlEncodeBytesToBytesInternalNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
            {
                int cNonAscii = 0;

                // count them first
                for (int i = 0; i < count; i++)
                {
                    if (IsNonAsciiByte(bytes[offset + i]))
                    {
                        cNonAscii++;
                    }
                }

                // nothing to expand?
                if (!alwaysCreateReturnValue && cNonAscii == 0)
                {
                    return bytes;
                }

                // expand not 'safe' characters into %XX, spaces to +s
                byte[] expandedBytes = new byte[count + cNonAscii * 2];
                int pos = 0;

                for (int i = 0; i < count; i++)
                {
                    byte b = bytes[offset + i];

                    if (IsNonAsciiByte(b))
                    {
                        expandedBytes[pos++] = (byte)'%';
                        expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                        expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                    }
                    else
                    {
                        expandedBytes[pos++] = b;
                    }
                }

                return expandedBytes;
            }

            static string UrlDecodeStringFromStringInternal(string s, Encoding e)
            {
                int count = s.Length;
                UrlDecoder helper = new UrlDecoder(count, e);

                // go through the string's chars collapsing %XX and %uXXXX and
                // appending each char as char, with exception of %XX constructs
                // that are appended as bytes

                for (int pos = 0; pos < count; pos++)
                {
                    char ch = s[pos];

                    if (ch == '+')
                    {
                        ch = ' ';
                    }
                    else if (ch == '%' && pos < count - 2)
                    {
                        if (s[pos + 1] == 'u' && pos < count - 5)
                        {
                            int h1 = HexToInt(s[pos + 2]);
                            int h2 = HexToInt(s[pos + 3]);
                            int h3 = HexToInt(s[pos + 4]);
                            int h4 = HexToInt(s[pos + 5]);

                            if (h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0)
                            {   // valid 4 hex chars
                                ch = (char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4);
                                pos += 5;

                                // only add as char
                                helper.AddChar(ch);
                                continue;
                            }
                        }
                        else
                        {
                            int h1 = HexToInt(s[pos + 1]);
                            int h2 = HexToInt(s[pos + 2]);

                            if (h1 >= 0 && h2 >= 0)
                            {     // valid 2 hex chars
                                byte b = (byte)((h1 << 4) | h2);
                                pos += 2;

                                // don't add as char
                                helper.AddByte(b);
                                continue;
                            }
                        }
                    }

                    if ((ch & 0xFF80) == 0)
                    {
                        helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
                    }
                    else
                    {
                        helper.AddChar(ch);
                    }
                }

                return helper.GetString();
            }

            // Private helpers for URL encoding/decoding
            static int HexToInt(char h)
            {
                return (h >= '0' && h <= '9') ? h - '0' :
                (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
                (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
                -1;
            }

            static char IntToHex(int n)
            {
                Debug.Assert(n < 0x10, "n < 0x10");

                if (n <= 9)
                {
                    return (char)(n + (int)'0');
                }
                else
                {
                    return (char)(n - 10 + (int)'a');
                }
            }

            // Set of safe chars, from RFC 1738.4 minus '+'
            internal static bool IsSafe(char ch)
            {
                if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
                {
                    return true;
                }

                switch (ch)
                {
                    case '-':
                    case '_':
                    case '.':
                    case '!':
                    case '*':
                    case '\'':
                    case '(':
                    case ')':
                        return true;
                }

                return false;
            }

            // Internal class to facilitate URL decoding -- keeps char buffer and byte buffer, allows appending of either chars or bytes
            class UrlDecoder
            {
                int _bufferSize;

                // Accumulate characters in a special array
                int _numChars;
                char[] _charBuffer;

                // Accumulate bytes for decoding into characters in a special array
                int _numBytes;
                byte[] _byteBuffer;

                // Encoding to convert chars to bytes
                Encoding _encoding;

                void FlushBytes()
                {
                    if (_numBytes > 0)
                    {
                        _numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
                        _numBytes = 0;
                    }
                }

                internal UrlDecoder(int bufferSize, Encoding encoding)
                {
                    _bufferSize = bufferSize;
                    _encoding = encoding;

                    _charBuffer = new char[bufferSize];
                    // byte buffer created on demand
                }

                internal void AddChar(char ch)
                {
                    if (_numBytes > 0)
                    {
                        FlushBytes();
                    }

                    _charBuffer[_numChars++] = ch;
                }

                internal void AddByte(byte b)
                {
                    // if there are no pending bytes treat 7 bit bytes as characters
                    // this optimization is temp disable as it doesn't work for some encodings

                    //if (_numBytes == 0 && ((b & 0x80) == 0)) {
                    //    AddChar((char)b);
                    //}
                    //else

                    {
                        if (_byteBuffer == null)
                        {
                            _byteBuffer = new byte[_bufferSize];
                        }

                        _byteBuffer[_numBytes++] = b;
                    }
                }

                internal string GetString()
                {
                    if (_numBytes > 0)
                    {
                        FlushBytes();
                    }

                    if (_numChars > 0)
                    {
                        return new String(_charBuffer, 0, _numChars);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            [Serializable]
            class HttpValueCollection : NameValueCollection
            {
                internal HttpValueCollection(string str, Encoding encoding)
                    : base(StringComparer.OrdinalIgnoreCase)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        FillFromString(str, true, encoding);
                    }

                    IsReadOnly = false;
                }

                protected HttpValueCollection(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }

                internal void FillFromString(string s, bool urlencoded, Encoding encoding)
                {
                    int l = (s != null) ? s.Length : 0;
                    int i = 0;

                    while (i < l)
                    {
                        // find next & while noting first = on the way (and if there are more)

                        int si = i;
                        int ti = -1;

                        while (i < l)
                        {
                            char ch = s[i];

                            if (ch == '=')
                            {
                                if (ti < 0)
                                    ti = i;
                            }
                            else if (ch == '&')
                            {
                                break;
                            }

                            i++;
                        }

                        // extract the name / value pair

                        string name = null;
                        string value = null;

                        if (ti >= 0)
                        {
                            name = s.Substring(si, ti - si);
                            value = s.Substring(ti + 1, i - ti - 1);
                        }
                        else
                        {
                            value = s.Substring(si, i - si);
                        }

                        // add name / value pair to the collection

                        if (urlencoded)
                        {
                            base.Add(
                               UrlUtility.UrlDecode(name, encoding),
                               UrlUtility.UrlDecode(value, encoding));
                        }
                        else
                        {
                            base.Add(name, value);
                        }

                        // trailing '&'

                        if (i == l - 1 && s[i] == '&')
                        {
                            base.Add(null, string.Empty);
                        }

                        i++;
                    }
                }

                public override string ToString()
                {
                    return ToString(true, null);
                }

                string ToString(bool urlencoded, IDictionary excludeKeys)
                {
                    int n = Count;
                    if (n == 0)
                        return string.Empty;

                    StringBuilder s = new StringBuilder();
                    string key, keyPrefix, item;

                    for (int i = 0; i < n; i++)
                    {
                        key = GetKey(i);

                        if (excludeKeys != null && key != null && excludeKeys[key] != null)
                        {
                            continue;
                        }
                        if (urlencoded)
                        {
                            key = UrlUtility.UrlEncodeUnicode(key);
                        }
                        keyPrefix = (!string.IsNullOrEmpty(key)) ? (key + "=") : string.Empty;

                        ArrayList values = (ArrayList)BaseGet(i);
                        int numValues = (values != null) ? values.Count : 0;

                        if (s.Length > 0)
                        {
                            s.Append('&');
                        }

                        if (numValues == 1)
                        {
                            s.Append(keyPrefix);
                            item = (string)values[0];
                            if (urlencoded)
                                item = UrlUtility.UrlEncodeUnicode(item);
                            s.Append(item);
                        }
                        else if (numValues == 0)
                        {
                            s.Append(keyPrefix);
                        }
                        else
                        {
                            for (int j = 0; j < numValues; j++)
                            {
                                if (j > 0)
                                {
                                    s.Append('&');
                                }
                                s.Append(keyPrefix);
                                item = (string)values[j];
                                if (urlencoded)
                                {
                                    item = UrlUtility.UrlEncodeUnicode(item);
                                }
                                s.Append(item);
                            }
                        }
                    }

                    return s.ToString();
                }
            }
        }
    }
}