using System.Threading;
using System.Threading.Tasks;
using LupuServ.Util;

namespace LupuServ.Services
{
    internal interface IStatusReceiver
    {
        Task ProcessMessageAsync(IMessagePacket message, CancellationToken cancellationToken = default);
    }
}
