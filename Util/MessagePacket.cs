using System.Text.RegularExpressions;

namespace LupuServ.Util
{
    internal interface IMessagePacket
    {
        public string MessageText { get; }

        public string Template { get; }
    }

    internal abstract class MessagePacketBase : IMessagePacket
    {
        public abstract string MessageText { get; }

        public abstract string Template { get; }

        private static readonly Regex StatusPattern = new(@"Zone\:(\d) (.*), (.*)");

        private static readonly Regex ArmPattern = new(@"(.*), (Arm|Disarm)$");

        public static IMessagePacket DecodeFrom(string content, string template)
        {
            var statusMatch = StatusPattern.Match(content);

            if (statusMatch.Success)
                return new StatusMessage(
                    int.Parse(statusMatch.Groups[1].Value),
                    statusMatch.Groups[2].Value,
                    statusMatch.Groups[3].Value,
                    template
                );

            var armMatch = ArmPattern.Match(content);

            if (armMatch.Success)
                return new ArmStatusMessage(
                    armMatch.Groups[1].Value,
                    armMatch.Groups[2].Value,
                    template
                );

            return null;
        }
    }

    internal sealed class StatusMessage : MessagePacketBase
    {
        /// <summary>
        ///     Gets the Zone ID (Sensor number).
        /// </summary>
        public int ZoneId { get; }

        /// <summary>
        ///     Gets the sensor name.
        /// </summary>
        public string SensorName { get; }

        /// <summary>
        ///     Gets the event message content.
        /// </summary>
        public string EventMessage { get; }

        public override string Template { get; }

        public StatusMessage(int zoneId, string sensorName, string eventMessage, string template)
        {
            ZoneId = zoneId;
            SensorName = sensorName;
            EventMessage = eventMessage;
            Template = template;
        }

        public override string MessageText => string.Format(Template, ZoneId, SensorName, EventMessage);
    }

    internal sealed class ArmStatusMessage : MessagePacketBase
    {
        public ArmStatusMessage(string user, string eventMessage, string template)
        {
            User = user;
            EventMessage = eventMessage;
            Template = template;
        }

        public string User { get; }

        /// <summary>
        ///     Gets the event message content.
        /// </summary>
        public string EventMessage { get; }

        public override string MessageText => string.Format(Template, User, EventMessage);

        public override string Template { get; }
    }
}