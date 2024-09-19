using SortedPub.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortedPub.Services
{
    public static class LibraryDbService
    {
        static SQLiteAsyncConnection db;
        static async Task Init()
        {
            if (db != null)
            {
                return;
            }

            var databasePath = Path.Combine(FileSystem.Current.AppDataDirectory, "LibraryDb.db");

            // if db exits no need for crating tables
            if (File.Exists(databasePath))
            {
                db = new SQLiteAsyncConnection(databasePath);
                return;
            }

            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<BookClass>();

            await db.DropTableAsync<SeriesClass>();

        }

        public static async Task<List<BookClass>> GetBookList()
        {
            await Init();
            
            var result = await db.Table<BookClass>().ToListAsync(); 
            
            return result;
        }

        public static async Task InsertBook(BookClass book)
        {
            await Init();
            await db.InsertAsync(book);
        }
       
        public static async Task InsertBookList(List<BookClass> bookList)
        {
            await Init();
            await db.InsertAllAsync(bookList);
        }

        public static async Task UpdateBook(BookClass book)
        {
            await Init();
            await db.UpdateAsync(book);
        }

        public static async Task DeleteBook(BookClass book)
        {
            await Init();
            await db.DeleteAsync(book);
        }

        public static async Task DeleteBookList(List<BookClass> booklist)
        {
            await Init();

            foreach (var book in booklist)
            { 
                await db.DeleteAsync(book);
            }
        }


    }
}
