namespace CobaDatabase;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

class Program
{
    // Dua baris perintah berikut untuk melakukan konektivitas database
    static string ConnectionString = "Data Source=DESKTOP-IBME24N;Initial Catalog=db_hr_sibkm;Integrated Security=True;Connect Timeout=30;";
    static SqlConnection connection;

    private static void Main(string[] args)
    {
        // Deklarasi variabel di awal
        bool endApp = false;
        bool iid;
        string opsi, key, name, code = "";
        int id;

        // Mengulang program hingga endApp bernilai true
        while (!endApp)
        {
            Console.Clear();
            Console.WriteLine("Choose option:");
            Console.WriteLine("1. See all data in table region");
            Console.WriteLine("2. Search region");
            Console.WriteLine("3. Add new region");
            Console.WriteLine("4. Edit existing region");
            Console.WriteLine("5. Delete region");
            Console.WriteLine("6. Exit");
            Console.Write("Your option? ");
            opsi = Console.ReadLine();

            // Pemilihan menu
            switch (opsi)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("List of region\n");
                    GetAllRegion();
                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Search region\n");
                    Console.Write("Keyword: ");
                    key = Console.ReadLine();
                    GetByString(key);
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("Add new region\n");
                    Console.Write("Name: ");
                    name = Console.ReadLine();
                    Console.Write("Code: ");
                    code = Console.ReadLine();
                    InsertRegion(name, code);
                    break;
                case "4":
                    Console.Clear();
                    Console.WriteLine("Edit region\n");
                    Console.Write("Id: ");
                    // id pada tabel region adalah integer, perlu dicek jika tipe data yg dimasukkan benar
                    iid = int.TryParse(Console.ReadLine(), out id);
                    if (GetById(id))
                    {
                        Console.Write("Name: ");
                        name = Console.ReadLine();
                        Console.Write("Code: ");
                        code = Console.ReadLine();
                        UpdateById(id, name, code);
                    }
                    else
                    {
                        Console.WriteLine("Id tidak tersedia, harap lihat kembali daftar");
                    }
                    Console.ReadKey();
                    break;
                case "5":
                    Console.Clear();
                    Console.WriteLine("Delete a region\n");
                    Console.Write("Id: ");
                    // id pada tabel region adalah integer, perlu dicek jika tipe data yg dimasukkan benar
                    iid = int.TryParse(Console.ReadLine(), out id);
                    if (GetById(id))
                    {
                        DeleteById(id);
                    }
                    else
                    {
                        Console.WriteLine("Id tidak tersedia, harap lihat kembali daftar");
                    }
                    Console.ReadKey();
                    break;
                case "6":
                    endApp = true;
                    break;
                default:
                    break;
            }
        }
        /*try {
           connection.Open();
           Console.WriteLine("Koneksi Berhasil Dibuka!");
           connection.Close();
       } catch (Exception e) {
           Console.WriteLine(e.Message);
       }*/
    }


    // Get all data from table region
    public static void GetAllRegion()
    {
        connection = new SqlConnection(ConnectionString);
        
        // SQL command untuk menampilkan semua data
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM region";

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();

        // Tampilkan data jika hasilnya ditemukan, dan respon jika tidak ada data
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Console.WriteLine("Id: " + reader[0]);
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("Code: " + reader[2]);
                Console.WriteLine("-------------------");
            }
        }
        else
        {
            Console.WriteLine("Data not found!");
        }
        Console.ReadKey();
        reader.Close();
        connection.Close();
    }


    // Get by Id, not used, but just in case
    public static void GetRecordById(int id)
    {
        connection = new SqlConnection(ConnectionString);

        // Melakukan SQL query untuk mencari id
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM region where id = @id";
        // Tambah parameter
        command.Parameters.Add("@id", SqlDbType.Int);
        command.Parameters["@id"].Value = id;

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();

        // Tampilkan data jika ditemukan dan respon jika tidak ada data
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Console.WriteLine("Id: " + reader[0]);
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("Code: " + reader[2]);
                Console.WriteLine("-------------------");
            }
        }
        else
        {
            Console.WriteLine("Data not found!");
        }
        Console.ReadKey();
        reader.Close();
        connection.Close();
    }

    // See if id is exist
    public static bool GetById(int id)
    {
        connection = new SqlConnection(ConnectionString);

        // Query untuk mencari berdasarkan id
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM region where id = @id";
        command.Parameters.Add("@id", SqlDbType.Int);
        command.Parameters["@id"].Value = id;

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();

        // Jika ditemukan kembalikan nilai true, jika tidak kembalikan nilai false
        if (reader.HasRows)
        {
            return true;
        }
        else
        {
            return false;
        }
        //reader.Close();
        connection.Close();
    }

    // Search region
    public static void GetByString(string search)
    {
        connection = new SqlConnection(ConnectionString);

        // Query untuk melakukan pencarian berdasarkan nama atau code
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM region WHERE name = @search OR code = @search";
        command.Parameters.Add("@search", SqlDbType.VarChar);
        command.Parameters["@search"].Value = search;

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();

        // Tampilkan data jika ditemukan dan respon jika tidak ada data
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Console.WriteLine("Id: " + reader[0]);
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("Code: " + reader[2]);
                Console.WriteLine("-------------------");
            }
        }
        else
        {
            Console.WriteLine("Data not found!");
        }
        Console.ReadKey();
        reader.Close();
        connection.Close();
    }

    // Insert new data into table region
    public static void InsertRegion(string name, string code)
    {
        connection = new SqlConnection(ConnectionString);

        connection.Open();

        // Melakukan penerapan rollback jika terjadi kesalahan
        SqlTransaction transaction = connection.BeginTransaction(); // open connection before use this

        try
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "INSERT INTO region (name, code) VALUES (@name, @code)";
            command.Transaction = transaction;

            command.Parameters.Add("@name", SqlDbType.VarChar);
            command.Parameters["@name"].Value = name;
            command.Parameters.Add("@code", SqlDbType.VarChar);
            command.Parameters["@code"].Value = code;

            int result = command.ExecuteNonQuery();
            transaction.Commit(); // Titik data dipulihkan ketika rollback dilaksanakan.

            if (result > 0)
            {
                Console.WriteLine("Data berhasil ditambah");
            }
            else
            {
                Console.WriteLine("Data gagal ditambah");
            }

            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback();
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }
        Console.ReadKey();
    }


    // Edit/update table region
    public static void UpdateById(int id, string name, string code)
    {
        connection = new SqlConnection(ConnectionString);

        connection.Open();

        // Menerapkan operasi untuk rollback jika terjadi kesalahan
        SqlTransaction transaction = connection.BeginTransaction(); // open connection before use this

        try
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "UPDATE region SET name = @name, code = @code WHERE id = @id";
            command.Transaction = transaction;

            command.Parameters.Add("@id", SqlDbType.Int);
            command.Parameters["@id"].Value = id;
            command.Parameters.Add("@name", SqlDbType.VarChar);
            command.Parameters["@name"].Value = name;
            command.Parameters.Add("@code", SqlDbType.VarChar);
            command.Parameters["@code"].Value = code;

            int result = command.ExecuteNonQuery();
            transaction.Commit(); // Titik data dipulihkan ketika rollback dilaksanakan.

            if (result > 0)
            {
                Console.WriteLine("Data berhasil diubah");
            }
            else
            {
                Console.WriteLine("Data gagal diubah, pastikan id berupa angka");
            }

            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback();
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }
    }


    // Delete a record from region
    public static void DeleteById(int id)
    {
        connection = new SqlConnection(ConnectionString);

        connection.Open();

        SqlTransaction transaction = connection.BeginTransaction(); // open connection before use this

        try
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "DELETE FROM region WHERE id = @id";
            command.Transaction = transaction;

            command.Parameters.Add("@id", SqlDbType.Int);
            command.Parameters["@id"].Value = id;

            int result = command.ExecuteNonQuery();
            transaction.Commit(); // Titik data dipulihkan ketika rollback dilaksanakan.

            if (result > 0)
            {
                Console.WriteLine("Data berhasil dihapus");
            }
            else
            {
                Console.WriteLine("Data gagal dihapus, pastikan id berupa angka");
            }

            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback();
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }
    }
}