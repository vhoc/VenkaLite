using System;
using System.Net;
using System.Net.Http;

namespace VenkaLite
{
    class RemoteData
    {

        public static void Put( dynamic localData, string authKey )
        {

            string uri              = "https://venka.app/api/datalive/";
            string response         = string.Empty;
            string jsonLocalData    = System.Text.Json.JsonSerializer.Serialize( localData );

            WebClient client = new WebClient();
            client.Headers.Add( HttpRequestHeader.Authorization, "Bearer " + authKey );
            client.Headers.Add( HttpRequestHeader.Accept, "application/json" );
            client.Headers.Add( "Content-Type", "text/json" );

            try
            {
                Console.WriteLine( "[" + DateTime.Now + "] Actualizando Venka..." );
                response = client.UploadString( uri, "PUT", jsonLocalData );
                Console.WriteLine( "[" + DateTime.Now + "] Venka Actualizado." );
                Environment.Exit(0);
            }
            catch( WebException )
            {
                Console.WriteLine( "[" + DateTime.Now + "] ERROR: No se pudo actualizar Venka." );
                Environment.Exit(0);
            }

        }

    }

}