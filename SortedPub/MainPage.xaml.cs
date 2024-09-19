using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Dispatching;
using SortedPub.Classes;
using System.Text;
using System.Xml.Serialization;
using VersOne.Epub;
using SortedPub.Services;
using CommunityToolkit.Maui.Core.Primitives;
using System.Collections.Generic;
using Microsoft.Maui.Controls.Shapes;

namespace SortedPub
{
    public partial class MainPage : ContentPage
    {
        // init variables
        IDispatcherTimer menuTimer;
        int col1Width = 10;
        int col2Width = 90;

        bool hidden = true;
        bool hasKey = Preferences.Default.ContainsKey("books_path");


        public MainPage()
        {
            InitializeComponent();
            askPermision();
            loadingGrid.IsVisible = true;
            selectFolderBtnGrid.IsVisible = hasKey ? false : true;

            if (hasKey)
            {
                checkForNewBooks();
                //BuildLibraryUi();
            };

            loadingGrid.IsVisible = false;
            menuTimer = Dispatcher.CreateTimer();
            menuTimer.Interval = TimeSpan.FromMilliseconds(15);
            menuTimer.Tick += MenuTimer_Tick;
        }

        // ask permissions for android platform
        private async void askPermision()
        {
            if (DeviceInfo.Current.Platform != DevicePlatform.Android) return;

            var readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            var writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (readStatus == PermissionStatus.Granted && writeStatus == PermissionStatus.Granted) return;

            if (Permissions.ShouldShowRationale<Permissions.StorageRead>())
            {
                await Shell.Current.DisplayAlert("Needs read permision", "SortedPub need read permision for scaning epub library", "Ok");
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                await Shell.Current.DisplayAlert("Needs write permision", "SortedPub need write permision for building the app epub library", "Ok");
            }

            readStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
            writeStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

        }

        // timer for side menu pop in pop out animation
        private void MenuTimer_Tick(object? sender, EventArgs e)
        {
            if (hidden)
            {
                col1Width += 1;
                col2Width -= 1;
                col1.Width = new GridLength(col1Width, GridUnitType.Star);
                col2.Width = new GridLength(col2Width, GridUnitType.Star);
                if (col1Width == 20)
                {
                    menuTimer.Stop();
                    hidden = false;
                    //minMenuBtn.IsVisible = true;
                }
            }
            else
            {
                col1Width -= 1;
                col2Width += 1;
                col1.Width = new GridLength(col1Width, GridUnitType.Star);
                col2.Width = new GridLength(col2Width, GridUnitType.Star);
                if (col1Width == 0)
                {
                    menuTimer.Stop();
                    hidden = true;
                    maxMenuBtn.IsVisible = true;
                    sideMenuGrid.IsVisible = false;
                }

            }
        }


        // menu pop out animation
        private void maxMenuBtn_Clicked(object sender, EventArgs e)
        {
            sideMenuGrid.IsVisible = true;
            col1.Width = new GridLength(0, GridUnitType.Star);
            col2.Width = new GridLength(100, GridUnitType.Star);
            menuTimer.Start();
        }


        // create xml file to save library
        private void selectFolderBtn_Clicked(object sender, EventArgs e)
        {
            buildLibraryDb();
        }

        private async void buildLibraryDb()
        {
            try
            {
                var folder = await FolderPicker.Default.PickAsync(default);

                if (folder.IsSuccessful)
                {
                    Preferences.Default.Set("books_path", $"{folder.Folder.Path}");
                    selectFolderBtnGrid.IsVisible = false;

                    string[] fileArray = Directory.GetFiles($"{folder.Folder.Path}", "*.epub", SearchOption.AllDirectories);

                    List<BookClass> BookList = new List<BookClass>();

                    int i = 0;

                    foreach (string s in fileArray)
                    {
                        EpubBook eBook = EpubReader.ReadBook(s);

                        BookClass book = new BookClass();
                        book.Path = s;
                        book.Title = eBook.Title;
                        if (eBook.Description != null)
                        {
                            book.Description = eBook.Description;
                        }
                        book.Auther = eBook.Author;
                        BookList.Add(book);

                    }

                    await LibraryDbService.InsertBookList(BookList);

                    BuildLibraryUi();

                }
            }
            catch (Exception ex)
            {

            }
        }

        //$"<HTML><BODY>{book.Description}</BODY></HTML>"
        private void BuildDetailBookUi(BookClass book)
        {
            EpubBook epub = EpubReader.ReadBook(book.Path);

            BookDetailsGrid.IsVisible = true;
            BookDtailTitleLabel.Text = book.Title;
            DetailBookLabel.Text = book.Description;
            detailBookImage.Source = ImageSource.FromStream(() => new MemoryStream(epub.CoverImage));
        }


        private async void BuildLibraryUi()
        {
            List<BookClass> booklist = await LibraryDbService.GetBookList();
            AllBooksGrid.IsVisible = true;

            int count = 0;

            Grid views = new Grid
            {
                
                ColumnDefinitions =
                    {
                        new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                        new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                        new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                        new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                        new ColumnDefinition( new GridLength(20, GridUnitType.Star))
                    },
                ColumnSpacing = 15
            };

            BookListScroll.Add(views);
            foreach (BookClass book in booklist)
            {

                EpubBook epub = EpubReader.ReadBook(book.Path);
                ImageButton cover  = new ImageButton();

                Border border = new Border
                {
                    Stroke = Color.FromArgb("#252525"),
                    StrokeThickness = 4,
                    HorizontalOptions = LayoutOptions.Center,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(15, 15, 15, 15)
                    }
                };

                Grid BookGrid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(80, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(20, GridUnitType.Star) }
                    }
                };

                border.Content = BookGrid;


                cover.Source = ImageSource.FromStream(() => new MemoryStream(epub.CoverImage));
                cover.VerticalOptions = LayoutOptions.Start;

                Button BookButton = new Button();
                BookButton.Background = Colors.Transparent;
                BookButton.Clicked += (s, arg) =>
                {
                    BuildDetailBookUi(book);
                };

                Label bookTitleLabel = new Label();
                bookTitleLabel.Text = book.Title;
                bookTitleLabel.HorizontalOptions = LayoutOptions.Center;
                bookTitleLabel.VerticalOptions = LayoutOptions.Center;
                bookTitleLabel.Padding = 15;

                BookGrid.Add(cover, 0,0);
                BookGrid.Add(bookTitleLabel, 0, 1);
                BookGrid.AddWithSpan(BookButton, 0, 0, 2);

                views.Add(border, count, 0);

                count += 1;

                if (count >= 5)
                {
                    
                    count = 0;
                    views = new Grid
                    {
                        
                        ColumnDefinitions =
                        {
                            new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                            new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                            new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                            new ColumnDefinition( new GridLength(20, GridUnitType.Star)),
                            new ColumnDefinition( new GridLength(20, GridUnitType.Star))
                        },
                        ColumnSpacing = 15
                    };
                    BookListScroll.Add(views);
                }
            }

        }

        private async void checkForNewBooks()
        {
            List<BookClass> booklist = await LibraryDbService.GetBookList();

            string[] fileArray = Directory.GetFiles ( Preferences.Default.Get("books_path", "Unknown"), "*.epub", SearchOption.AllDirectories);

            List<BookClass> newBookList = new List<BookClass>();

            int i = 0;

            foreach (string s in fileArray)
            {
                EpubBook eBook = EpubReader.ReadBook(s);

                BookClass book = new BookClass();
                book.Path = s;
                book.Title = eBook.Title;
                if (eBook.Description != null)
                {
                    book.Description = eBook.Description;
                }
                book.Auther = eBook.Author;
                newBookList.Add(book);

            }

            if (booklist.Count == newBookList.Count)
            {
                BuildLibraryUi();
                return;
            }

            if (booklist.Count < newBookList.Count)
            {
                List<BookClass> booksToAdd = newBookList.Where(x => !booklist.Any(z => z.Path == x.Path)).ToList();
                await LibraryDbService.InsertBookList(booksToAdd);
            }

            if (booklist.Count > newBookList.Count)
            {
                List<BookClass> booksToAdd = booklist.Where(x => !newBookList.Any(z => z.Path == x.Path)).ToList();
                await LibraryDbService.DeleteBookList(booksToAdd);
            }

            BuildLibraryUi();
        }

        


        private void selectNewFolderBtn_Clicked(object sender, EventArgs e)
        {
            buildLibraryDb();
        }

        private void settingsBtn_Clicked(object sender, EventArgs e)
        {

        }

        private void HideMenuBtn_Clicked(object sender, EventArgs e)
        {
            sideMenuGrid.IsVisible = false;
            hidden = true;
            col1.Width = new GridLength(0, GridUnitType.Star);
            col2.Width = new GridLength(100, GridUnitType.Star);

        }

        private void minMenuBtn_Clicked(object sender, EventArgs e)
        {
            menuTimer.Start();

        }

        private void closeBookDetailsGridBtn_Clicked(object sender, EventArgs e)
        {
            BookDetailsGrid.IsVisible = false;
        }
    }

}
