using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net;
using System.IO;
using System.Text;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void stpWs_Requisicao(SqlString Ds_Url, SqlString Ds_Metodo, SqlString Ds_Parametros, SqlString Ds_Codificacao, out SqlString Ds_Retorno_OUTPUT)
    {

        var parametros = (Ds_Parametros.IsNull) ? "" : Ds_Parametros.Value;
        var metodo = (Ds_Metodo.IsNull) ? "GET" : Ds_Metodo.Value.ToUpper();
        var url = Ds_Url.Value;
        var feedData = string.Empty;
        var encoding = (Ds_Codificacao.IsNull) ? "UTF-8" : Ds_Codificacao.Value;

        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = metodo;
            request.ContentType = "application/x-www-form-urlencoded";

            if (metodo == "POST" && parametros.Length > 0)
            {
                var data = parametros;
                var dataStream = Encoding.UTF8.GetBytes(data);

                request.ContentLength = dataStream.Length;

                var newStream = request.GetRequestStream();
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream, Encoding.GetEncoding(encoding));
            feedData = streamReader.ReadToEnd();

            response.Close();
            stream?.Dispose();
            streamReader.Dispose();
        }
        catch (Exception ex)
        {
            if (SqlContext.Pipe != null) SqlContext.Pipe.Send(ex.Message);
        }

        Ds_Retorno_OUTPUT = feedData;

    }

}