using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibraryManagementSystem
{
    class Program
    {
        static List<Book> library = new List<Book>();

        static void Main(string[] args)
        {
            LoadLibrary();

            while (true)
            {
                Console.WriteLine("\nKütüphane Otomasyon Sistemi");
                Console.WriteLine("1. Yeni Kitap Ekle");
                Console.WriteLine("2. Tüm Kitapları Listele");
                Console.WriteLine("3. Bir kitap ara");
                Console.WriteLine("4. Bir kitap ödünç al");
                Console.WriteLine("5. Kitabı iade et");
                Console.WriteLine("6. Gecikmiş kitapları görüntüle");
                Console.WriteLine("7. Çıkış");
                Console.Write("İstediğiniz işlem için 1-7 arası bir tuşa basın: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        ListBooks();
                        break;
                    case "3":
                        SearchBook();
                        break;
                    case "4":
                        BorrowBook();
                        break;
                    case "5":
                        ReturnBook();
                        break;
                    case "6":
                        ViewOverdueBooks();
                        break;
                    case "7":
                        SaveLibrary();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        static void LoadLibrary()
        {
            if (File.Exists("library.txt"))
            {
                string[] lines = File.ReadAllLines("library.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    Book book = new Book(parts[0], parts[1], DateTime.Parse(parts[2]));
                    library.Add(book);
                }
            }
        }

        static void SaveLibrary()
        {
            using (StreamWriter writer = new StreamWriter("library.txt"))
            {
                foreach (Book book in library)
                {
                    writer.WriteLine($"{book.Title}|{book.Author}|{book.DueDate}");
                }
            }
        }

        static void AddBook()
        {
            Console.Write("Kitap Adı: ");
            string title = Console.ReadLine();
            Console.Write("Yazar Adı: ");
            string author = Console.ReadLine();
            Book newBook = new Book(title, author, DateTime.MinValue);
            library.Add(newBook);
            Console.WriteLine("Kitap başarıyla kaydedildi.");
        }

        static void ListBooks()
        {
            if (library.Count == 0)
            {
                Console.WriteLine("Kitap mevcut değil.");
            }
            else
            {
                foreach (Book book in library)
                {
                    Console.WriteLine($"Kitap Adı:{book.Title}, Yazar: {book.Author}, Teslim Tarihi: {book.DueDate}");
                }
            }
        }

        static void SearchBook()
        {
            Console.Write("Aramak istediğiniz kitabı girin: ");
            string keyword = Console.ReadLine().ToLower();
            var results = library.Where(book => book.Title.ToLower().Contains(keyword) || book.Author.ToLower().Contains(keyword));
            if (results.Count() == 0)
            {
                Console.WriteLine("Kitap bulunamadı.");
            }
            else
            {
                foreach (Book book in results)
                {
                    Console.WriteLine($"Kitap Adı: {book.Title}, Yazar: {book.Author}, Teslim Tarihi: {book.DueDate}");
                }
            }
        }

        static void BorrowBook()
        {
            Console.Write("Ödünç alınacak kitabın başlığını girin:");
            string title = Console.ReadLine();
            Book book = library.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                Console.WriteLine("Kitap bulunamadı.");
            }
            else if (book.DueDate != DateTime.MinValue)
            {
                Console.WriteLine("Kitap zaten ödünç alınmış.");
            }
            else
            {
                book.DueDate = DateTime.Now.AddDays(14); 
                Console.WriteLine("Kitap başarıyla ödünç alındı. Bitiş tarihi: " + book.DueDate);
                SaveLibrary(); 
            }
        }

        static void ReturnBook()
        {
            Console.Write("İade etmek istediğiniz kitabın adını girin: ");
            string title = Console.ReadLine();
            Book book = library.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                Console.WriteLine("Kitap bulunamadı.");
            }
            else if (book.DueDate == DateTime.MinValue)
            {
                Console.WriteLine("Bu kitap ödünç alınamaz.");
            }
            else
            {
                book.DueDate = DateTime.MinValue;
                Console.WriteLine("Kitap başarıyla iade edildi.");
            }
        }

        static void ViewOverdueBooks()
        {
            var overdueBooks = library.Where(book => book.DueDate != DateTime.MinValue && book.DueDate < DateTime.Now);
            if (overdueBooks.Count() == 0)
            {
                Console.WriteLine("Gecikmiş Kitap Yok");
            }
            else
            {
                foreach (Book book in overdueBooks)
                {
                    Console.WriteLine($"Kitap Adı: {book.Title}, Yazar: {book.Author}, Teslim Tarihi: {book.DueDate}");
                }
            }
        }
    }

    class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime DueDate { get; set; }

        public Book(string title, string author, DateTime dueDate)
        {
            Title = title;
            Author = author;
            DueDate = dueDate;
        }
    }
}