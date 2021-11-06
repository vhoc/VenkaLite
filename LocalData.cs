using System;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml;

namespace VenkaLite
{

    class LocalData {

        /*
        * LocalData.Get( queries )
        * return Dictionary<string,string>
        *
        * Queries the local SoftRestaurant database using the queries
        * obtained with RemoteQueries.Get() and puts the data in a
        * Dictionary of strings.
        */
        public static Dictionary<string, string> Get ( string queries )
        {
            Dictionary<string, string> queryData = new Dictionary<string, string>();

            List<RemoteQueries> allQueries;

            // Prepare the SQL connection.            
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource              = Settings.Value( "SqlServerInstance" );
            builder.UserID                  = Security.DecryptString("1234567890123456", Settings.Value( "User" ) );
            builder.Password                = Security.DecryptString( "1234567890123456", Settings.Value( "Password" ) );
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

            // Compile all the queries into a List in preparation for their execution.
            allQueries = System.Text.Json.JsonSerializer.Deserialize<List<RemoteQueries>>( queries );

            // Add an index row to identify the establishment or restaurant.
            queryData.Add( "id_sucursal", Settings.Value( "Sucursal" ) );

            //Int32 queryCount = allQueries.Count; // I just realised this is completely useless. Probably a remnant of another algorithm. Commenting out just in case.

            Console.WriteLine( "[" + DateTime.Now + "] Obteniendo datos del sistema..." );

            // Execute every query and populate Dictionary "queryData" with each result.
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

                            /* WHAT'S THIS?
                            *  There are some queries that return a data table in the form of XML, instead of a single result.
                            *  Detect those validating that the first character is a "<"
                            *  and convert every instance into a JSON string.
                            *
                            *  If the validation fails, pass the data as it is.
                            */
                            if( !String.IsNullOrEmpty( tempResult ) && tempResult.TrimStart().StartsWith("<") )
                            {
                                XmlDocument xmlResult = new XmlDocument();
                                xmlResult.LoadXml( tempResult );

                                String jsonResult = JsonConvert.SerializeXmlNode( xmlResult );

                                queryData.Add( query.name, jsonResult );
                            }
                            else
                            {
                                if( String.IsNullOrEmpty( tempResult ) ) { tempResult = "0"; } // Empty cells fallback to "0".
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