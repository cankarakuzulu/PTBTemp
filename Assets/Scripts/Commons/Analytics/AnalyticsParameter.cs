using UnityEngine;
using System.Collections;

namespace nopact.Commons.Analytics
{
    public class AnalyticsParameter
    {
        private string id;
        private string sValue = null;
        private int? iValue;
        private float? fValue;
        
        public AnalyticsParameter( string id, string value )
        {
            this.id = id;
            this.sValue = value;
        }

        public AnalyticsParameter( string id, int value )
        {
            this.id = id;
            this.iValue = value;
        }

        public AnalyticsParameter( string id, float value )
        {
            this.id = id;
            this.fValue = value;
        }

        public object Get ( out System.Type type )
        {
            type = typeof( object );

            if ( !string.IsNullOrEmpty( sValue )) {
                type = typeof( string );
                return sValue;                
            }

            if ( iValue != null )
            {
                type = typeof( int );
                return iValue.Value;
            }

            if ( fValue != null )
            {
                type = typeof( float );
                return fValue.Value;             
            }

            return null;                
        }

        public string ValueToString()
        {
            System.Type type;
            string output = string.Empty;
            var ctx = Get( out type );
            if ( type == typeof( float ) )
            {
                output = ( ( float ) ctx ).ToString( "n2" );
            }
            else if ( type == typeof( int ) )
            {
                output = ( ( int ) ctx ).ToString();
            }
            else if ( type == typeof( string ) )
            {
                output = ( string ) ctx;
            }
            return output;
        }

        public override string ToString()
        {
            return string.Join(" :: " , new string[ ] { id, ValueToString() } );
        }

        public string Id
        {
            get
            {
                return id;
            }
        }
    }
}
