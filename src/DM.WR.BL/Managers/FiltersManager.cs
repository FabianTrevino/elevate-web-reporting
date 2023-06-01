using DM.WR.BL.Builders;
using DM.WR.Models.Config;
using DM.WR.Models.Dashboard;
using DM.WR.Models.Types;

namespace DM.WR.BL.Managers
{
    public class FiltersManager : IFiltersManager
    {
        private readonly ISessionManager _sessionManager;

        private readonly IFiltersBuilder _filtersBuilder;

        private readonly UserData _userData;

        public FiltersManager(IUserDataManager userDataManager, ISessionManager sessionManager, IFiltersBuilder filtersBuilder)
        {
            _sessionManager = sessionManager;

            _filtersBuilder = filtersBuilder;

            _userData = userDataManager.GetUserData();
        }

        public FilterPanel GetFilterPanel()
        {
            var filterPanel = RetrieveFilterPanel();

            if (filterPanel == null)
            {
                filterPanel = _filtersBuilder.BuildPanel(new FilterPanel(), FilterType._INTERNAL_FIRST_, _userData);
                StoreFilterPanel(filterPanel);
            }

            return filterPanel;
        }

        public void UpdateFilterPanel(FilterPanel filterPanel)
        {
            StoreFilterPanel(filterPanel);
        }

        public void DeleteFilterPanel()
        {
            _sessionManager.Delete(SessionKey.DashboardFilters);
        }



        private FilterPanel RetrieveFilterPanel()
        {
            var sessionStoredFilterPanel = _sessionManager.Retrieve(SessionKey.DashboardFilters);
            return (FilterPanel)sessionStoredFilterPanel;
        }

        private void StoreFilterPanel(FilterPanel filterPanel)
        {
            _sessionManager.Store(filterPanel, SessionKey.DashboardFilters);
        }
    }
}