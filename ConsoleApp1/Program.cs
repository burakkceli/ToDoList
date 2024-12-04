using System;
using System.Data.SQLite;

namespace MiniToDoList
{
    class Program
    {
        // SQLite bağlantı dizesi
        static string connectionString = "Data Source=tasks.db;Version=3;";

        static void Main(string[] args)
        {
            // Veritabanını başlat
            InitializeDatabase();

            // Kullanıcı menüsünü göster
            ShowMenu();
        }

        // Veritabanını başlatma ve tabloyu oluşturma
        static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    IsCompleted INTEGER NOT NULL DEFAULT 0
                )";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Ana menü
        static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\nMini To-Do List");
                Console.WriteLine("1. Görev Ekle");
                Console.WriteLine("2. Görevleri Listele");
                Console.WriteLine("3. Görevi Güncelle (Tamamla)");
                Console.WriteLine("4. Görev Sil");
                Console.WriteLine("5. Çıkış");
                Console.Write("Bir seçenek girin: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        ListTasks();
                        break;
                    case "3":
                        UpdateTask();
                        break;
                    case "4":
                        DeleteTask();
                        break;
                    case "5":
                        Console.WriteLine("Çıkış yapılıyor...");
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçenek. Tekrar deneyin.");
                        break;
                }
            }
        }

        // Görev ekleme
        static void AddTask()
        {
            Console.Write("Görev açıklamasını girin: ");
            string description = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Tasks (Description) VALUES (@Description)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Description", description);
                    command.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Görev başarıyla eklendi!");
        }

        // Görevleri listeleme
        static void ListTasks()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Id, Description, IsCompleted FROM Tasks";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("\nGörev Listesi:");
                        while (reader.Read())
                        {
                            string status = reader.GetInt32(2) == 1 ? "[Tamamlandı]" : "[Devam Ediyor]";
                            Console.WriteLine($"{reader.GetInt32(0)}. {reader.GetString(1)} {status}");
                        }
                    }
                }
            }
        }

        // Görevi güncelleme (tamamlama)
        static void UpdateTask()
        {
            Console.Write("Güncellemek istediğiniz görevin ID'sini girin: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID.");
                return;
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @Id";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        Console.WriteLine("Görev başarıyla tamamlandı!");
                    else
                        Console.WriteLine("Görev bulunamadı.");
                }
            }
        }

        // Görev silme
        static void DeleteTask()
        {
            Console.Write("Silmek istediğiniz görevin ID'sini girin: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID.");
                return;
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Tasks WHERE Id = @Id";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        Console.WriteLine("Görev başarıyla silindi!");
                    else
                        Console.WriteLine("Görev bulunamadı.");
                }
            }
        }
    }
}
