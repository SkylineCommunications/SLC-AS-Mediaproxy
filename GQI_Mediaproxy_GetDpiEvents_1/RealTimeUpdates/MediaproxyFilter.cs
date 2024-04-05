namespace GQI_EVSCerebrum_GetEndpoints_1.RealTimeUpdates
{
    using GQI_EVSCerebrum_GetRoutesForDestination_1;
    using Skyline.DataMiner.Net.Messages;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class MediaproxyFilter
    {
        private readonly DataProvider _dataProvider;
        private readonly string _channelId;
        private readonly int _dataminerId;
        private readonly int _elementId;

        public MediaproxyFilter(DataProvider dataProvider, int dataminerId, int elementId , string channelId)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _channelId = channelId;
            _dataminerId = dataminerId;
            _elementId = elementId;

            Initialize();
        }

        public List<DpiEvent> AllDpiEvents { get; set; } = new List<DpiEvent>();

        public List<DpiEvent> GetLatestDpiEvent()
        {
            return Initialize();
        }

        private List<DpiEvent> Initialize()
        {
            return CreateDpiEvents();
        }

        private List<DpiEvent> CreateDpiEvents()
        {
            ParameterValue[] levelColumnData = new ParameterValue[0];
            if (_dataminerId == 196201 && _elementId == 4)
            {
                levelColumnData = _dataProvider.DpiEventsTableLogServer6.GetData();
            }
            else if (_dataminerId == 196201 && _elementId == 6)
            {
                levelColumnData = _dataProvider.DpiEventsTableLogServer7.GetData();
            }
            else if (_dataminerId == 196201 && _elementId == 8)
            {
                levelColumnData = _dataProvider.DpiEventsTableLogServer8?.GetData();
            }

            AllDpiEvents = DpiEvent.CreateDpiEvents(levelColumnData);

            return AllDpiEvents.Where(dpiEvent => dpiEvent.ChannelId == _channelId).OrderBy(x => x.DetectedTime).ToList();
        }

        private void Log(int items)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(@"C:\Skyline_Data\RealTimeUpdates.txt"))
                {
                    sw.WriteLine($"Category: {_channelId}, Filtered items: {items}");
                }
            }
            catch (Exception)
            {

            }

        }
    }
}
