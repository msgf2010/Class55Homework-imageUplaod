using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class55_Homework.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
    }

    public class ImageManager
    {
        private string _connectionString;

        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveImage(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Images (Description, FileName, Password, Views)
                                  VALUES (@desc, @fileName, @password, @views) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@desc", image.Description);
                cmd.Parameters.AddWithValue("@fileName", image.FileName);
                cmd.Parameters.AddWithValue("@password", image.Password);
                cmd.Parameters.AddWithValue("@views", image.Views);
                connection.Open();
                image.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public IEnumerable<Image> Get()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                var list = new List<Image>();
                cmd.CommandText = "SELECT * FROM Images";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        Description = (string)reader["Description"],
                        FileName = (string)reader["FileName"],
                        Password = (string)reader["Password"],
                        Views = (int)reader["Views"]
                    });
                }

                return list;
            }
        }

        public Image GetSingleImage(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                var image = new Image();

                image.Id = (int)reader["Id"];
                image.Description = (string)reader["Description"];
                image.FileName = (string)reader["FileName"];
                image.Password = (string)reader["Password"];
                image.Views = (int)reader["Views"];

                return image;
            }
        }

        public void UpdateImageViews(int id, int views)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Images
                                    SET Views = @views
                                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@views", views);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
