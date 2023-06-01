using System.Collections.Generic;
using System.Linq;
using DM.WR.Models.Types;

namespace DM.WR.Models.Options
{
    public class OptionBook
    {
        public OptionBook(OptionPage firstPage)
        {
            Pages = new List<OptionPage> { firstPage };
            CurrentPageIndex = 0;
        }

        public List<OptionPage> Pages { get; }

        public int PagesCount => Pages.Count;
        public int CurrentPageIndex { get; set; }

        public CriteriaInfo Criteria { get; set; }

        public OptionPage GetFirstPage()
        {
            return Pages.First();
        }

        public OptionPage GetPage(int index)
        {
            return Pages[index];
        }

        public OptionPage GetCurrentPage()
        {
            return Pages[CurrentPageIndex];
        }

        public void UpdateCurrentPage(OptionPage updatedPage)
        {
            Pages[CurrentPageIndex] = updatedPage;
        }

        public void AddPage(OptionPage page)
        {
            Pages.Add(page);
        }

        public void InsertPage(OptionPage page)
        {
            ++CurrentPageIndex;
            Pages.Insert(CurrentPageIndex, page);
        }

        public void RemoveCurrentPage()
        {
            Pages.RemoveAt(CurrentPageIndex);

            if (CurrentPageIndex != 0)
                --CurrentPageIndex;
        }

        public void RemoveAllPages()
        {
            Pages.Clear();
        }
    }
}