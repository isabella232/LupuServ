using System.Threading;
using LupuServ.Util;
using System.Threading.Tasks;

namespace LupuServ.Services
{
    internal interface IAlarmReceiver
    {
        Task ProcessMessageAsync(IMessagePacket message, CancellationToken cancellationToken = default);
    }
}
