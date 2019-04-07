using System.Threading;
using Rincon.Core.Domain;

namespace Rincon.Core
{
    public class HandlerBuilder
    {
        public HandlerBuilder()
        {
        }

        public T Build<T>(CancellationToken token)
            where T : CommandHandler, new()
        {
            var result = new T
            {
                Token = token
            };

            return result;
        }
    }
}