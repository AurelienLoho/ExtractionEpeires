namespace ExtractionEpeires
{
    using System.Net;
    using System.Text;

    public sealed class HttpRequest
    {
        public readonly CookieContainer CookieJar = new CookieContainer();

        public readonly string hostName;

        public HttpRequest(string hostName)
        {
            this.hostName = hostName;
        }

        public HttpWebRequest GetRequest(string url)
        {
            return this.CreateBaseRequest(url, "GET");
        }

        public HttpWebRequest PostRequest(string url, string data)
        {
            var request = this.CreateBaseRequest(url, "POST");

            var dataBytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = dataBytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return request;
        }

        public HttpWebResponse SendGetRequest(string url)
        {
            var request = this.CreateBaseRequest(url, "GET");

            return (HttpWebResponse)request.GetResponse();
        }

        public HttpWebResponse SendPostRequest(string url, string data)
        {
            var request = this.CreateBaseRequest(url, "POST");

            var dataBytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = dataBytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return (HttpWebResponse)request.GetResponse();
        }

        private HttpWebRequest CreateBaseRequest(string url, string method)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Host = this.hostName;
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            request.CookieContainer = this.CookieJar;
            request.KeepAlive = false;

            return request;
        }
    }

}
