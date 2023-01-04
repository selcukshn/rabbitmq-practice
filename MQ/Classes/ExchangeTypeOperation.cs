using System.Collections.Generic;
using MQ.Enums;

namespace MQ.Classes
{
    public static class ExchangeTypeOperation
    {
        public static Dictionary<EExchangeType, string> Types = new Dictionary<EExchangeType, string>{
            {EExchangeType.Direct,"direct"},
            {EExchangeType.Fanout,"fanout"},
            {EExchangeType.Topic,"topic"},
        };

        public static bool TypeValid(int type)
        {
            return Types[(EExchangeType)type] != null ? true : false;
        }

        public static string GetType(int type)
        {
            return Types[(EExchangeType)type];
        }
    }
}