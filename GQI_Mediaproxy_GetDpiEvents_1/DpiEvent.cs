namespace GQI_Mediaproxy_GetDpiEvents_1
{
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Net.Messages;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text.RegularExpressions;

    internal class DpiEvent
    {
        public DpiEvent()
        {
        }

        public DpiEvent(object[] row)
        {
        }

        public string Instance { get; set; }

        public string Description { get; set; }

        public string ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string EventId { get; set; }

        public DateTime DetectedTime { get; set; }

        public double PreRoll { get; set; }

        public double Duration { get; set; }

        public string EventType { get; set; }

        public string Category { get; set; }

        public string RawMessage { get; set; }

        public static List<DpiEvent> CreateDpiEvents(ParameterValue[] columns)
        {
            if (columns == null || columns.Length == 0) return new List<DpiEvent>();

            var dpiEvents = new List<DpiEvent>();

            for (int i = 0; i < columns[0].ArrayValue.Length; i++)
            {
                var channelId = columns[1].ArrayValue[i]?.CellValue?.GetAsStringValue();
                if (string.IsNullOrWhiteSpace(channelId)) continue;

                var dpiEvent = new DpiEvent
                {
                    Instance = columns[0].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                    ChannelId = channelId,
                    EventId = columns[2].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                    DetectedTime = DateTime.FromOADate(columns[3].ArrayValue[i].CellValue.DoubleValue),
                    PreRoll = columns[4].ArrayValue[i].CellValue.DoubleValue,
                    Duration = columns[5].ArrayValue[i].CellValue.DoubleValue,
                    EventType = columns[6].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                    Category = columns[7].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                    RawMessage = columns[8].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                    ChannelName = columns[10].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                    Description = columns[11].ArrayValue[i]?.CellValue?.GetAsStringValue(),
                };

                dpiEvents.Add(dpiEvent);
            }

            return dpiEvents;
        }

        public GQIRow ToRow()
        {
            var cells = new[]
            {
                new GQICell { Value = Instance },
                new GQICell { Value = Description },
                new GQICell { Value = EventId },
                new GQICell { Value = Convert.ToString(DetectedTime, CultureInfo.InvariantCulture) },
                new GQICell { Value = String.Format("{0}s", TimeSpan.FromSeconds(Math.Round(PreRoll)).Seconds) },
                new GQICell { Value = String.Format("{0}s", TimeSpan.FromSeconds(Math.Round(Duration)).Seconds) },
                new GQICell { Value = EventType },
                new GQICell { Value = Category },
                new GQICell { Value = RawMessage },
            };

            return new GQIRow(Instance, cells);
        }

        public bool IsValid()
        {
            return true;
        }
    }
}