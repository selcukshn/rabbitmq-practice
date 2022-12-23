using System.Collections.Generic;

namespace MQ
{
    public static class ExchangeTypes
    {
        public static Dictionary<ETypes, string> Types = new Dictionary<ETypes, string>{
            {ETypes.Direct,"direct"},
            {ETypes.Fanout,"fanout"},
            {ETypes.Topic,"topic"},
        };

        public static bool TypeValid(int type)
        {
            return Types[(ETypes)type] != null ? true : false;
        }

        public static string GetType(int type)
        {
            return Types[(ETypes)type];
        }
    }
    public enum ETypes
    {
        Direct = 1,
        Fanout = 2,
        Topic = 3
    }
}