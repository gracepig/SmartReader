namespace SmartReader.ViewModel
{
    public class ModelManager
    {

        private static SearchViewModel SearchResultModel;

        public static SearchViewModel GetSearchResultModel()
        {
            if (SearchResultModel == null )
            {
                SearchResultModel = new SearchViewModel();
            }

            return SearchResultModel;
        }

        private static BookIndexViewModel BookIndexModel;

        public static BookIndexViewModel GetBookIndexModel ()
        {
            if (BookIndexModel == null )
            {
                BookIndexModel = new BookIndexViewModel(null);
            }

            return BookIndexModel;
        }

        private static ChapterViewModel ChapterModel;
        public static ChapterViewModel GetChapterViewModel ()
        {
            if (ChapterModel == null )
            {
                ChapterModel = new ChapterViewModel();
            }

            return ChapterModel;
        }

        private static BookListModel BookListModel;
        public static BookListModel GetBookListModel()
        {
            if (BookListModel == null)
            {
                BookListModel = new BookListModel();
            }

            return BookListModel;
        }
    }
}
