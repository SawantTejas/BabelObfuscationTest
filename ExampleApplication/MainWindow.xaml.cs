using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace ExampleApplication
{
    public partial class MainWindow : Window
    {
        private readonly DataProcessor _processor;

        public MainWindow()
        {
            InitializeComponent();
            _processor = new DataProcessor();
        }

        private void OnProcessDataClick(object sender, RoutedEventArgs e)
        {
            List<Person> people = new List<Person>
            {
                new Person("Alice", 30, "alice@example.com"),
                new Person("Bob", 25, "bob@example.com")
            };

            _processor.ProcessData(people);
            MessageBox.Show("Data processed. Check the log.");
        }
    }

    public class DataProcessor
    {
        public event Action<string> OnProcessingComplete;

        public void ProcessData<T>(List<T> items) where T : IEntity
        {
            foreach (var item in items)
            {
                item.PrintDetails();
            }

            SerializeToFile(items, "output.dat");
            DeserializeFromFile<List<T>>("output.dat");

            OnProcessingComplete?.Invoke("Processing Complete!");
        }

        private void SerializeToFile(object data, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, data);
            }
        }

        private T DeserializeFromFile<T>(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }
    }

    public interface IEntity
    {
        void PrintDetails();
    }

    [Serializable]
    public class Person : IEntity
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Email { get; private set; }

        public Person(string name, int age, string email)
        {
            Name = name;
            Age = age;
            Email = email;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Email: {Email}");
        }
    }

    public static class ReflectionHelper
    {
        public static void DisplayProperties<T>(T obj)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                Console.WriteLine($"{prop.Name}: {prop.GetValue(obj)}");
            }
        }
    }
}
