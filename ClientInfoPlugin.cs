using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DNWS
{
  class ClientInfoPlugin : IPlugin
  {
    protected static Dictionary<String, int> statDictionary = null;
    public ClientInfoPlugin()
    {
      if (statDictionary == null)
      {
        statDictionary = new Dictionary<String, int>();

      }
    }

    public void PreProcessing(HTTPRequest request)
    {
      if (statDictionary.ContainsKey(request.Url))
      {
        statDictionary[request.Url] = (int)statDictionary[request.Url] + 1;
      }
      else
      {
        statDictionary[request.Url] = 1;
      }
    }

    public HTTPResponse GetResponse(HTTPRequest request)
    {
      HTTPResponse response = null;
      StringBuilder sb = new StringBuilder();

      try
      {
        string remoteEndPointStr = request.getPropertyByKey("remoteendpoint");
        IPEndPoint endpoint = IPEndPoint.Parse(remoteEndPointStr);
        
        sb.Append("<html><body><h2>Client Information</h2><pre>");
        sb.AppendFormat("<b>Client IP Address:</b> {0}<br/>\n", endpoint.Address);
        sb.AppendFormat("<b>Client Port:</b> {0}<br/>\n", endpoint.Port);
        
        string userAgent = request.getPropertyByKey("user-agent");
        sb.AppendFormat("<b>Browser Information:</b> {0}<br/>\n", userAgent != null ? userAgent.Trim() : "Not provided");
        
        string acceptLanguage = request.getPropertyByKey("accept-language");
        sb.AppendFormat("<b>Accept-Language:</b> {0}<br/>\n", acceptLanguage != null ? acceptLanguage.Trim() : "Not provided");
        
        string acceptEncoding = request.getPropertyByKey("accept-encoding");
        sb.AppendFormat("<b>Accept-Encoding:</b> {0}<br/>\n", acceptEncoding != null ? acceptEncoding.Trim() : "Not provided");

        sb.Append("</pre></body></html>");

        response = new HTTPResponse(200);
        response.body = Encoding.UTF8.GetBytes(sb.ToString());
      }
      catch(Exception ex)
      {
        response = new HTTPResponse(500);
        response.body = Encoding.UTF8.GetBytes("<h1>500 Error</h1><p>" + ex.Message + "</p>");
      }
      return response;
    }


    public HTTPResponse PostProcessing(HTTPResponse response)
    {
      return response;
    }
  }
}
