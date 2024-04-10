namespace GQI_Mediaproxy_GetDpiEvents_1.RealTimeUpdates
{
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Protobuf.Shared.IdObjects.v1;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class DataProvider : IDisposable
    {
        private readonly int _dataminerId;
        private readonly int _elementId;

        private readonly GQIDMS _gqiDms;
        private readonly Connection _connection;

        public DataProvider(Connection connection, GQIDMS gqiDms)
        {
            _connection = connection;
            _gqiDms = gqiDms;

            InstantiateCache();
        }

        public ElementTableCache DpiEventsTableLogServer6 { get; private set; }

        public ElementTableCache DpiEventsTableLogServer7 { get; private set; }

        public ElementTableCache DpiEventsTableLogServer8 { get; private set; }

        public void Dispose()
        {
            DpiEventsTableLogServer6?.Dispose();
            DpiEventsTableLogServer7?.Dispose();
            //DpiEventsTableLogServer8?.Dispose();
        }

        private void InstantiateCache()
        {
            if (DpiEventsTableLogServer6 != null && DpiEventsTableLogServer7 != null && DpiEventsTableLogServer8 != null)
            {
                return;
            }

            if (_connection == null)
            {
                throw new ArgumentNullException(nameof(_connection));
            }

            DpiEventsTableLogServer6 = new ElementTableCache(_connection, _gqiDms, 196201, 4, 1700, "1");
            DpiEventsTableLogServer7 = new ElementTableCache(_connection, _gqiDms, 196201, 6, 1700, "2");
            //DpiEventsTableLogServer8 = new ElementTableCache(_connection, _gqiDms, 196201, 8, 1700, "3");
        }
    }
}
