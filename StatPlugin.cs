using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DNWS
{
  class StatPlugin : IPlugin
  {
    protected static Dictionary<String, int> statDictionary = null;
    public StatPlugin()
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
        sb.Append("<html><body style=\"font-family: Arial, sans-serif;\">");
        
        // Display Client Information Section
        sb.Append("<h2>Client Information</h2>");
        sb.Append("<pre style=\"background-color: #f0f0f0; padding: 10px; border-radius: 5px;\">");
        
        try
        {
          string remoteEndPointStr = request.getPropertyByKey("remoteendpoint");
          IPEndPoint endpoint = IPEndPoint.Parse(remoteEndPointStr);
          sb.AppendFormat("Client IP: {0}<br/>\n", endpoint.Address);
          sb.AppendFormat("Client Port: {0}<br/>\n", endpoint.Port);
        }
        catch
        {
          sb.Append("Client IP: Not available<br/>\n");
          sb.Append("Client Port: Not available<br/>\n");
        }
        
        string userAgent = request.getPropertyByKey("user-agent");
        sb.AppendFormat("Browser Information: {0}<br/>\n", userAgent != null ? userAgent.Trim() : "Not available");
        
        string acceptLanguage = request.getPropertyByKey("accept-language");
        sb.AppendFormat("Accept Language: {0}<br/>\n", acceptLanguage != null ? acceptLanguage.Trim() : "Not available");
        
        string acceptEncoding = request.getPropertyByKey("accept-encoding");
        sb.AppendFormat("Accept Encoding: {0}<br/>\n", acceptEncoding != null ? acceptEncoding.Trim() : "Not available");
        
        sb.Append("</pre>");
        
        // Display Statistics Section
        sb.Append("<h2>Statistics</h2>");
        sb.Append("<pre style=\"background-color: #e8f4f8; padding: 10px; border-radius: 5px;\">");
        foreach (KeyValuePair<String, int> entry in statDictionary)
        {
          sb.Append(entry.Key + ": " + entry.Value.ToString() + "<br />");
        }
        sb.Append("</pre>");
        
        sb.Append("</body></html>");
        
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
