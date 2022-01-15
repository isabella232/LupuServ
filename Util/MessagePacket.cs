using System.Text.RegularExpressions;

namespace LupuServ.Util
{
    /// <summary>
    ///     Decodes a status message received from Alarm Centre.
    /// </summary>
    internal sealed class MessagePacket
    {
        /// <summary>
        ///     Gets the Zone ID (Sensor number).
        /// </summary>
        public int? ZoneId { get; }

        /// <summary>
        ///     Gets the sensor name.
        /// </summary>
        public string SensorName { get; }

        /// <summary>
        ///     Gets the event message content.
        /// </summary>
        public string EventMessage { get; }

        public string Template { get; }

        private static readonly Regex StatusPattern = new(@"Zone\:(\d) (.*), (.*)");
        
        private static readonly Regex ArmPattern = new(@"(.*), (Arm|Disarm)$");

        private MessagePacket(int zoneId, string sensorName, string eventMessage, string template)
        {
            ZoneId = zoneId;
            SensorName = sensorName;
            EventMessage = eventMessage;
            Template = template;
        }

        public override string ToString()
        {
            return string.Format(Template, ZoneId, SensorName, EventMessage);
        }

        /// <summary>
        ///     Decodes a received string message into individual message components.
        /// </summary>
        /// <param name="content">The source message text.</param>
        /// <param name="template">The template to use to create a human readable string.</param>
        /// <returns>The decoded <see cref="MessagePacket"/>.</returns>
        public static MessagePacket DecodeFrom(string content, string template)
        {
            var match = StatusPattern.Match(content);

            return new MessagePacket(
                int.Parse(match.Groups[1].Value),
                match.Groups[2].Value,
                match.Groups[3].Value,
                template
            );
        }
    }
}