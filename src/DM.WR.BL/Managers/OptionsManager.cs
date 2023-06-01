using DM.WR.BL.Builders;
using System;
using DM.WR.Models.Config;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using HandyStuff;

namespace DM.WR.BL.Managers
{
    public class OptionsManager : IOptionsManager
    {
        private readonly ISessionManager _sessionManager;

        private readonly IOptionsBuilder _optionsBuilder;

        private readonly UserData _userData;

        public OptionsManager(IUserDataManager userDataManager, ISessionManager sessionManager, IOptionsBuilder optionsBuilder)
        {
            _sessionManager = sessionManager;

            _optionsBuilder = optionsBuilder;

            _userData = userDataManager.GetUserData();
        }

        public OptionBook GetOptionBook()
        {
            var optionBook = RetrieveOptionBook();

            if (optionBook == null)
            {
                var optionPage = _optionsBuilder.BuildOptions(new OptionPage(ConfigSettings.XmlAbsolutePath), XMLGroupType._INTERNAL_FIRST_, _userData, 0);
                optionBook = new OptionBook(optionPage);
                StoreOptionBook(optionBook);
            }

            return optionBook;
        }

        public void UpdateOptionBook(OptionBook optionBook)
        {
            StoreOptionBook(optionBook);
        }

        public void DeleteOptionBook()
        {
            _sessionManager.Delete(SessionKey.OptionBookKey);
        }

        public void AddOptionPage()
        {
            var optionBook = RetrieveOptionBook();
            if (optionBook.PagesCount > 9)
                throw new Exception($"Options Manager :: Can't add another options page.  Already {optionBook.PagesCount} pages.");

            var clonedPage = optionBook.GetCurrentPage().Copy();
            optionBook.InsertPage(clonedPage);
            optionBook = _optionsBuilder.SyncUpPagesForMultimeasure(optionBook);

            StoreOptionBook(optionBook);
        }

        public void RemoveOptionPage()
        {
            var optionBook = RetrieveOptionBook();
            optionBook.RemoveCurrentPage();
            optionBook = _optionsBuilder.SyncUpPagesForMultimeasure(optionBook);

            StoreOptionBook(optionBook);
        }

        public void FlipPage(int pageNumber)
        {
            var optionBook = RetrieveOptionBook();
            optionBook.CurrentPageIndex = pageNumber - 1;

            StoreOptionBook(optionBook);
        }

        public void EnableCriteriaEditMode(CriteriaInfo criteria)
        {
            var book = RetrieveOptionBook();
            book.Criteria = new CriteriaInfo
            {
                Id = criteria.Id,
                Name = criteria.Name,
                Summary = criteria.Summary,
                LastUpdated = criteria.LastUpdated
            };
            StoreOptionBook(book);
        }

        public void DisableCriteriaEditMode()
        {
            var book = RetrieveOptionBook();
            book.Criteria = null;
            book.Pages.ForEach(p => p.InvalidGroup = null);
            StoreOptionBook(book);
        }

        private OptionBook RetrieveOptionBook()
        {
            var sessionStoredOptionBook = _sessionManager.Retrieve(SessionKey.OptionBookKey);
            return (OptionBook)sessionStoredOptionBook;
        }

        private void StoreOptionBook(OptionBook optionBook)
        {
            _sessionManager.Store(optionBook, SessionKey.OptionBookKey);
        }
    }
}