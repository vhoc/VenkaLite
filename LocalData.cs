using System;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml;

namespace VenkaLite
{

    class LocalData {

        public static Dictionary<string, string> Get ( string queries )
        {
            Dictionary<string, string> queryData = new Dictionary<string, string>();

            List<RemoteQueries> allQueries;
            
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource              = Settings.Value( "SqlServerInstance" );
            builder.UserID                  = Security.DecryptString("origin ladder as", Settings.Value( "User" ) );
            builder.Password                = Security.DecryptString( "origin ladder as", Settings.Value( "Password" ) );
            builder.InitialCatalog          = Settings.Value( "Database" );
            builder.IntegratedSecurity      = Convert.ToBoolean( Settings.Value( "IntegratedSecurity" ) );
            builder.Encrypt                 = Convert.ToBoolean( Settings.Value( "Encrypt" ) );
            builder.ConnectRetryCount       = Convert.ToInt32( Settings.Value( "ConnectRetryCount" ) );
            builder.ConnectRetryInterval    = Convert.ToInt32( Settings.Value( "ConnectRetryInterval" ) );
            builder.ConnectTimeout          = Convert.ToInt32( Settings.Value( "ConnectTimeout" ) );

            if( String.IsNullOrEmpty( queries ) )
            {
                Console.WriteLine( "[" + DateTime.Now + "] ERROR: No queries supplied." );
                return queryData;
            }

            allQueries = System.Text.Json.JsonSerializer.Deserialize<List<RemoteQueries>>( queries );
            //allQueryes = RemoteQueries.Get( "1", "" )

            queryData.Add( "id_sucursal", Settings.Value( "Sucursal" ) );

            Int32 queryCount = allQueries.Count;

            Console.WriteLine( "[" + DateTime.Now + "] Obteniendo datos del sistema..." );

            foreach( dynamic query in allQueries )
            {
                using( SqlConnection connection = new SqlConnection( builder.ConnectionString ) )
                {
                    try { connection.Open(); } catch( Exception ex ) { Console.WriteLine( "[" + DateTime.Now + "] ERROR: " + ex.Message ); Environment.Exit(0); }

                    using( SqlCommand cmd = new SqlCommand( query.query, connection ) )
                    {
                        try
                        {
                            String tempResult = Convert.ToString( cmd.ExecuteScalar() );

                            if( !String.IsNullOrEmpty( tempResult ) && tempResult.TrimStart().StartsWith("<") )
                            {
                                XmlDocument xmlResult = new XmlDocument();
                                xmlResult.LoadXml( tempResult );

                                String jsonResult = JsonConvert.SerializeXmlNode( xmlResult );

                                queryData.Add( query.name, jsonResult );
                            }
                            else
                            {
                                if( String.IsNullOrEmpty( tempResult ) ) { tempResult = "0"; }
                                queryData.Add( query.name, tempResult );
                            }
                        }
                        catch ( Exception ex )
                        {
                            Console.WriteLine( "[" + DateTime.Now + "] ERROR: " + ex.Message + " in [" + query.name + "]" );
                            Environment.Exit(0);
                        }
                    }

                    connection.Close();
                }
            }

            Console.WriteLine( "[" + DateTime.Now + "] Datos recopilados." );

            return queryData;
        }

    }

}