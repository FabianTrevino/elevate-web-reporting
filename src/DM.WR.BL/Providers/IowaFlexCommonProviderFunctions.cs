using System;
using DM.WR.Models.IowaFlex;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DM.WR.BL.Builders;
using DM.WR.GraphQlClient;
using DM.WR.Models.Config;
using DM.WR.Models.GraphqlClient.RangeEndPoint;
using DM.WR.Models.IowaFlex.ViewModels;

namespace DM.WR.BL.Providers
{
    public interface IIowaFlexCommonProviderFunctions
    {
        List<BreadCrumb> MakeBreadCrumbs(IowaFlexFilterPanel filterPanel, string appPath);
        IowaFlexFilterPanel ChangeFiltersSelection(IowaFlexFilterPanel filterPanel, FilterType filterType, List<string> values);
        Task<IowaFlexFilterPanel> RecreateFiltersAsync(IowaFlexFilterPanel currentPanel, FilterType changedFilter, string userId);
        LocationNode MakeRootBreadCrumb(Filter parentLocationsFilter);
        IowaFlexFilterPanel DrillDownLocationsPathAsync(IowaFlexFilterPanel currentPanel, LocationNode node);
        IowaFlexFilterPanel DrillUpLocationsPathAsync(IowaFlexFilterPanel currentPanel, LocationNode node);
        List<Band> BuildBands(StandardScoreRange range);
    }

    public class IowaFlexCommonProviderFunctions : IIowaFlexCommonProviderFunctions
    {
        private readonly IApiClient _adaptiveApiClient;
        private readonly IIowaFlexFiltersBuilder _filtersBuilder;
        private readonly IGraphQlQueryStringBuilder _graphQlQueryStringBuilder;

        public IowaFlexCommonProviderFunctions(IApiClient apiClient, IIowaFlexFiltersBuilder filtersBuilder, IGraphQlQueryStringBuilder graphQlQueryStringBuilder)
        {
            _adaptiveApiClient = apiClient;
            _filtersBuilder = filtersBuilder;
            _graphQlQueryStringBuilder = graphQlQueryStringBuilder;
        }

        public List<BreadCrumb> MakeBreadCrumbs(IowaFlexFilterPanel filterPanel, string controllerUrl)
        {
            var breadCrumbs = new List<BreadCrumb>();
            for (int c = 0; c < filterPanel.BreadCrumbs.Count; ++c)
            {
                var breadCrumb = filterPanel.BreadCrumbs[c];

                if (c + 1 == filterPanel.BreadCrumbs.Count)
                    breadCrumbs.Add(new BreadCrumb
                    {
                        Text = breadCrumb.NodeName,
                        Link = ""
                    });
                else if (c == 0)
                    breadCrumbs.Add(new BreadCrumb
                    {
                        Text = breadCrumb.NodeName,
                        Link = $"{controllerUrl}/GoToRootNode"
                    });
                else
                    breadCrumbs.Add(new BreadCrumb
                    {
                        Text = breadCrumb.NodeName,
                        Link = $"{controllerUrl}/DrillUpLocations?id={breadCrumb.NodeId}&name={breadCrumb.NodeName}&type={breadCrumb.NodeType}"
                    });
            }

            return breadCrumbs;
        }

        public IowaFlexFilterPanel ChangeFiltersSelection(IowaFlexFilterPanel filterPanel, FilterType filterType, List<string> values)
        {
            var filterToUpdate = filterPanel.GetFilterByType(filterType);

            filterToUpdate.Items.ForEach(i => i.IsSelected = false);
            if (values != null)
                filterToUpdate.Items.Where(i => values.Contains(i.Value)).ToList().ForEach(i => i.IsSelected = true);

            if (filterType <= FilterType.Grade)
                filterPanel.BreadCrumbs = null;

            return filterPanel;
        }

        public async Task<IowaFlexFilterPanel> RecreateFiltersAsync(IowaFlexFilterPanel currentPanel, FilterType changedFilter, string userId)
        {
            var query = _graphQlQueryStringBuilder.BuildFiltersQueryString(currentPanel, changedFilter, userId);
            var apiResponse = await _adaptiveApiClient.MakeUserCallAsync(query);

            var newFilterPanel = _filtersBuilder.BuildFilters(currentPanel, apiResponse, changedFilter);
            newFilterPanel.GraphqlQuery = ConfigSettings.IsEnvironmentProd ? "" : query;

            return newFilterPanel;
        }

        public LocationNode MakeRootBreadCrumb(Filter parentLocationsFilter)
        {
            var firstSelectedLocation = parentLocationsFilter.Items.First(i => i.IsSelected);

            return new LocationNode
            {
                NodeId = Convert.ToInt32(firstSelectedLocation.Value),
                NodeName = firstSelectedLocation.Text,
                NodeType = parentLocationsFilter.DisplayName
            };
        }


        public IowaFlexFilterPanel DrillDownLocationsPathAsync(IowaFlexFilterPanel currentPanel, LocationNode node)
        {
            var filterToUpdate = (LocationsFilter)currentPanel.GetFilterByType(FilterType.ParentLocations);

            filterToUpdate.Items.RemoveRange(0, filterToUpdate.Items.Count);
            filterToUpdate.Items.Add(new FilterItem
            {
                Text = node.NodeName,
                Value = node.NodeId.ToString(),
                IsSelected = true
            });
            filterToUpdate.LocationNodeType = node.NodeType;
            filterToUpdate.DisplayName = node.NodeType.ToUpper();

            if (currentPanel.BreadCrumbs.Count == 1)
            {
                var nodeFilterItem = currentPanel.GetFilterByType(FilterType.ChildLocations).Items.Cast<PiledFilterItem>().First(i => i.Value == node.NodeId.ToString());
                currentPanel.BreadCrumbs[0] = new LocationNode { NodeId = Convert.ToInt32(nodeFilterItem.PileKey), NodeName = nodeFilterItem.PileLabel };
            }
            currentPanel.BreadCrumbs.Add(node);

            return currentPanel;
        }

        public IowaFlexFilterPanel DrillUpLocationsPathAsync(IowaFlexFilterPanel currentPanel, LocationNode node)
        {
            var filterToUpdate = (LocationsFilter)currentPanel.GetFilterByType(FilterType.ParentLocations);

            filterToUpdate.Items.RemoveRange(0, filterToUpdate.Items.Count);
            filterToUpdate.Items.Add(new FilterItem
            {
                Text = node.NodeName,
                Value = node.NodeId.ToString(),
                IsSelected = true
            });
            filterToUpdate.LocationNodeType = node.NodeType;
            filterToUpdate.DisplayName = node.NodeType.ToUpper();

            var index = currentPanel.BreadCrumbs.IndexOf(currentPanel.BreadCrumbs.FirstOrDefault(bc => bc.NodeId == node.NodeId));
            currentPanel.BreadCrumbs.RemoveRange(index + 1, currentPanel.BreadCrumbs.Count - 2);

            return currentPanel;
        }

        public List<Band> BuildBands(StandardScoreRange range)
        {
            if (range?.Ranges == null || !range.Ranges.Any())
                throw new Exception("No Data in Standard Score Range.");

            var bands = new List<Band>();
            foreach (var r in range.Ranges)
            {
                bands.Add(new Band
                {
                    Range = r.PerformanceLevel,
                    RangeBand = $"{r.Lower}:{r.Upper}"
                });
            }

            return bands;
        }
    }
}