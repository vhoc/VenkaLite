using System;
using System.Net;
using System.Net.Http;
using System.IO;

namespace VenkaLite
{

    class RemoteQueries {

        public string name { get; set; }
        public Int32 db_engine { get; set; }
        public string query { get; set; }

        /*
        *   
        */
        public static string Get ( string dbengine, string authKey )
        {
            Console.WriteLine( "[" + DateTime.Now + "] Obteniendo biblioteca de datos... " );

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add( new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue( "application/json" ) );            
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", authKey );

            HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Get, "https://venka.app/api/queries/" + dbengine );

            try
            {
                var response = client.Send( request );
                using var reader = new StreamReader( response.Content.ReadAsStream() );
                Console.WriteLine( "[" + DateTime.Now + "] Biblioteca de datos obtenida y cargada." );
                return reader.ReadToEnd();
            }
            catch ( Exception )
            {
                Console.WriteLine( "[" + DateTime.Now + "] ERROR: No se pudo obtener la biblioteca de datos." );
                return "";
            }

        }

    }

}