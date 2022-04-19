using System;

namespace MsGraph.Classes
{
  /// <summary>
  /// Allows to specify HTTP (or any other) status code along with error message for further forwarding the code to client
  /// </summary>
  [Serializable]
  public class GraphStatusCodeException : Exception
  {
    public int StatusCode { get; }

    public GraphStatusCodeException()
    {
            
    }

    public GraphStatusCodeException(string message):base(message)
    {
            
    }

    public GraphStatusCodeException(string message, Exception innerException) : base(message, innerException)
    {
            
    }

    public GraphStatusCodeException(int statusCode, string message) : base(message)
    {
      StatusCode = statusCode;
    }



    public GraphStatusCodeException(int statusCode, string message, Exception innerException) : base(message, innerException)
    {
      StatusCode = statusCode;
    }

  }
}