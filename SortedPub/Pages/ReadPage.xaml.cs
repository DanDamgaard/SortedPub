using SortedPub.Classes;
using VersOne.Epub;
using VersOne.Epub.Environment;

namespace SortedPub.Pages;

public partial class ReadPage : ContentPage
{
    private readonly BookClass _book;

    public ReadPage(BookClass book)
	{
		InitializeComponent();
        _book = book;
        showBook(book);
    }

    private void showBook(BookClass book)
    {
        EpubBookRef eBook = EpubReader.OpenBook(book.Path);

        IZipFile b = eBook.EpubFile;

        Image i = new Image
        {
            Source = ImageSource.FromFile("dotnet_bot.png")
        };

        WebView webView = new WebView
        {
            Source = new HtmlWebViewSource
            {
                Html = $" <img src={i.Source} >"
            }
        };

        pageGrid.Children.Add(webView);
    }

    //private void prewPageBtn_Clicked(object sender, EventArgs e)
    //{

    //}

    //private void nextPageBtn_Clicked(object sender, EventArgs e)
    //{

    //}

    //private void bookMenuBtn_Clicked(object sender, EventArgs e)
    //{

    //}
}