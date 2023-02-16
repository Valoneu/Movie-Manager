using System;
using ConsoleTables;

namespace APK
{
    public class APK
    {
        public static T GetInput<T>(string text)
        {
            Console.WriteLine(text);
            string input = Console.ReadLine();
            T output;
            try
            {
                output = (T)Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                Console.Write("Invalid input. ");
                return GetInput<T>(text);
            }
            return output;
        }

        public static void Main()
        {
            FileCheck();

            List<Movie> movies = LoadFile("movies.txt");

            Table(movies);

            bool MainLoop = true;
            WrongInput:
            while (MainLoop)
            {
                string input = GetInput<string>("input action: Add, Delete, Change, Save");
                input = input.ToLower();
                switch (input)
                {
                    case "add":
                        movies = Add(movies);
                        break;
                    case "delete":
                        movies = Delete(movies);
                        break;
                    case "change":
                        movies = Change(movies);
                        break;
                    case "save":
                        MainLoop = false;
                        break;
                    default:
                        goto WrongInput;
                }
                Console.Clear();
                Table(movies);
            }

            SaveAndExit(movies);
        }

        public static string GetToday()
        {
            DateTime thisDay = DateTime.Today;
            return thisDay.ToString("d");
        }

        public static List<Movie> LoadFile(string path)
        {
            List<Movie> movies = new List<Movie>();

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    Movie movie = new Movie (int.Parse(parts[0]), parts[1], parts[2], int.Parse(parts[3]), parts[4]);
                    movies.Add(movie);
                }
            }
            return movies;
        }

        public static void SaveAndExit(List<Movie> movies)
        {
            using (StreamWriter writer = new StreamWriter("movies.txt"))
            {
                foreach (Movie movie in movies)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4}", movie.ID, movie.Name, movie.Type, movie.Rating, movie.Time);
                }
            }
            Console.WriteLine("File succesfully saved");
            Thread.Sleep(1500);
            Environment.Exit(0);
        }

        public static void FileCheck()
        {
            if (!File.Exists("movies.txt"))
                using (StreamWriter sw = File.CreateText("movies.txt"));
        }

        public static void Table(List<Movie> movies)
        {
            ConsoleTable table = new ConsoleTable("ID", "Name", "Type", "Rating", "When");
            foreach (var movie in movies)
            {
                table.AddRow(movie.ID, movie.Name, movie.Type, movie.Rating, movie.Time);
            }

            table.Write(Format.MarkDown);
        }

        public static List<Movie> Add(List<Movie> movies)
        {
            Movie movie = new Movie(movies.Count,GetInput<string>("Enter name"), GetInput<string>("Enter type"), GetInput<int>("Enter rating 1-100"), GetToday());
            while (movie.Rating > 100)
                movie.Rating = GetInput<int>("Enter valid rating 1-100");
            movies.Add(movie);
            return movies;
        }

        public static List<Movie> Delete(List<Movie> movies)
        {
            if (movies.Count != 0)
            {
                int idToRemove = GetInput<int>("Enter ID of item you want to remove");
                int indexToRemove = movies.FindIndex(x => x.ID == idToRemove);
                movies.RemoveAt(indexToRemove);
                for (int i = 0; i < movies.Count; i++)
                    movies[i].ID = i; 
            }
            return movies;
        }

        public static List<Movie> Change(List<Movie> movies)
        {
            if (movies.Count != 0)
            {
                int idToChange = GetInput<int>("Enter ID of item you want to remove");
                while (idToChange >= movies.Count)
                    idToChange = GetInput<int>("Enter valid ID of existing item");

                string PropertyToChange = GetInput<string>("Enter what property do you want to change Name, Type, Rating");
                PropertyToChange = PropertyToChange.ToLower();

                if (PropertyToChange == "name" | PropertyToChange == "type" | PropertyToChange == "rating")
                { }
                else
                {
                    while (true)
                    {
                        PropertyToChange = GetInput<string>("Enter property to change Name, Type, Rating");
                        PropertyToChange = PropertyToChange.ToLower();
                        if (PropertyToChange == "name" || PropertyToChange == "type" || PropertyToChange == "rating")
                            break;
                        else
                            Console.Write("Not valid ");
                    }
                }

                foreach (var movie in movies)
                {
                    if (movie.ID == idToChange)
                    {
                        switch (PropertyToChange)
                        {
                            case "name":
                                movie.Name = GetInput<string>("Enter new name");
                                break;
                            case "type":
                                movie.Type = GetInput<string>("Enter new type");
                                break;
                            case "rating":
                                movie.Rating = GetInput<int>("Enter new rating 1-100");
                                while (movie.Rating > 100)
                                    movie.Rating = GetInput<int>("Enter valid rating 1-100");
                                break;
                        }
                    }
                }
            }
            return movies;
        }
    }

    public class Movie
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Rating { get; set; }
        public string Time { get; set; }

        public Movie(int iD, string name, string type, int rating, string time)
        {
            ID = iD;
            Name = name;
            Type = type;
            Rating = rating;
            Time = time;
        }

        public override string ToString()
        {
            return $"{ID},{Name},{Type},{Rating},{Time}";
        }
    }
}