using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GQI_Mediaproxy_GetDpiEvents_1.RealTimeUpdates;
using Skyline.DataMiner.Analytics.GenericInterface;
using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Net.Messages.Advanced;
using Skyline.DataMiner.Net.Ticketing;

[GQIMetaData(Name = "Mediaproxy Get DPI Events")]
public class GQI_Mediaproxy_GetDpiEvents : IGQIDataSource, IGQIOnInit, IGQIInputArguments, IGQIUpdateable
{
    private readonly GQIStringArgument _fullElementIdArgument = new GQIStringArgument("Element ID") { IsRequired = true };
    private readonly GQIStringArgument _channelIdArgument = new GQIStringArgument("Channel ID") { IsRequired = true };

    private GQIDMS dms;

    private int dataminerId;
    private int elementId;
    private string channelId;

    private DataProvider _dataProvider;
    private MediaproxyFilter mediaproxyFilter;

    private ICollection<GQIRow> _currentRows = Array.Empty<GQIRow>();
    private IGQIUpdater _updater;

    public OnInitOutputArgs OnInit(OnInitInputArgs args)
    {
        dms = args.DMS;

        StaticDataProvider.Initialize(dms);
        _dataProvider = StaticDataProvider.Instance;

        return new OnInitOutputArgs();
    }

    public GQIArgument[] GetInputArguments()
    {
        return new GQIArgument[]
        {
            _fullElementIdArgument,
            _channelIdArgument,
        };
    }

    public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
    {
        var rawElementId = args.GetArgumentValue(_fullElementIdArgument).Split(new[] { '/' });
        if (rawElementId.Length == 2)
        {
            if (!int.TryParse(rawElementId[0], out dataminerId)) throw new ArgumentException("Script input is not valid.");
            if (!int.TryParse(rawElementId[1], out elementId)) throw new ArgumentException("Script input is not valid.");
        }

        channelId = Convert.ToString(args.GetArgumentValue(_channelIdArgument));

        return new OnArgumentsProcessedOutputArgs();
    }

    public GQIColumn[] GetColumns()
    {
        return new GQIColumn[]
        {
            new GQIStringColumn("Instance"),
            new GQIStringColumn("Description"),
            new GQIStringColumn("Event ID"),
            new GQIStringColumn("Detected Time"),
            new GQIStringColumn("Preroll"),
            new GQIStringColumn("Duration"),
            new GQIStringColumn("Event Type"),
            new GQIStringColumn("Category"),
            new GQIStringColumn("Raw Message"),
        };
    }

    public GQIPage GetNextPage(GetNextPageInputArgs args)
    {
        var newRows = CalculateNewRows();

        try
        {
            return new GQIPage(newRows)
            {
                HasNextPage = false,
            };
        }
        finally
        {
            _currentRows = newRows;
        }
    }

    public void OnStartUpdates(IGQIUpdater updater)
    {
        _updater = updater;
        _dataProvider.DpiEventsTableLogServer6.Changed += TableData_OnChanged;
        _dataProvider.DpiEventsTableLogServer7.Changed += TableData_OnChanged;
        //_dataProvider.DpiEventsTableLogServer8.Changed += TableData_OnChanged;
    }

    public void OnStopUpdates()
    {
        _dataProvider.DpiEventsTableLogServer6.Changed -= TableData_OnChanged;
        _dataProvider.DpiEventsTableLogServer7.Changed -= TableData_OnChanged;
        //_dataProvider.DpiEventsTableLogServer8.Changed -= TableData_OnChanged;
        _updater = null;
    }

    private void TableData_OnChanged(object sender, ParameterTableUpdateEventMessage e)
    {
        var newRows = CalculateNewRows().ToList();

        try
        {
            var comparison = new GqiTableComparer(_currentRows, newRows);

            foreach (var row in comparison.RemovedRows)
            {
                _updater.RemoveRow(row.Key);
            }

            foreach (var row in comparison.UpdatedRows)
            {
                _updater.UpdateRow(row);
            }

            foreach (var row in comparison.AddedRows)
            {
                _updater.AddRow(row);
            }
        }
        finally
        {
            _currentRows = newRows;
        }
    }

    private GQIRow[] CalculateNewRows()
    {
        mediaproxyFilter = new MediaproxyFilter(_dataProvider, dataminerId, elementId, channelId);

        var dpiEvent = mediaproxyFilter.GetLatestDpiEvent().Last();

        return new GQIRow[] { dpiEvent.ToRow() };
    }
}