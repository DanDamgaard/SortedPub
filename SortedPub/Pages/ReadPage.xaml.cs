using SortedPub.Classes;
using VersOne.Epub;

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
        EpubBook eBook = EpubReader.ReadBook(book.Path);

        var b = eBook.Content;

        WebView webView = new WebView
        {
            Source = new HtmlWebViewSource
            {
                Html = $"<HTML><BODY><H1>.NET MAUI</H1><P>Welcome to WebView.</P></BODY></HTML>"
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