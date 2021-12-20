using System;
using System.Text;
using System.Collections.Generic;
using System.Numerics;


namespace Lab3
{
    delegate Vector2 FdblVector2(double x, double y);


    class Program
    {
        private const string Filename = @"C:\Users\romul\source\repos\Lab1v32\TextFile1.txt";

        static void Main(string[] args)
        {
            //            Linq();
            //            ReadWrite();
            Integrals();

        }
        static void Integrals()
        {

            FdblVector2 f = DefaultType.f;
            V3DataArray array = new V3DataArray("2x^2 + x + 1", DateTime.Now, 3, 3, 1.0f, 1.0f, f);
            Console.WriteLine("Data:");
            Console.WriteLine(array.ToLongString("f"));
            float[] left = new float[2] { 0F, 1F };
            float[] right = new float[2] { 1F, 2F };
            Console.WriteLine($"Is ok? {array.Integrals(left, right)}");
            float[,] integrals = array.calculated_integrals;
            output(integrals, left, right);

            left = new float[1] { 0F };
            right = new float[1] { 2F };
            Console.WriteLine($"Is ok? {array.Integrals(left, right)}");
            integrals = array.calculated_integrals;
            output(integrals, left, right);

            left = new float[1] { 1F };
            right = new float[1] { 1F };
            Console.WriteLine($"Is ok? {array.Integrals(left, right)}");
            integrals = array.calculated_integrals;
            output(integrals, left, right);

            Console.WriteLine("Empty array");
            V3DataArray empty = new V3DataArray("2x^2 + x + 1", DateTime.Now, 0, 0, 1.0f, 1.0f, f);
            left = new float[1] { 0F };
            right = new float[1] { 1F };
            Console.WriteLine($"Is ok? {empty.Integrals(left, right)}");
            integrals = empty.calculated_integrals;
            output(integrals, left, right);
        }
        static void output(float[,] integrals, float[] left, float[] right)
        {
            for (int i = 0; i < integrals.GetLength(0); i++)
            {
                Console.Write($"Сегмент ({left[i]}; {right[i]}) :");
                for (int j = 0; j < integrals.GetLength(1); j++)
                {
                    Console.Write($"{ integrals[i, j].ToString("F2")} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        static void ReadWrite()
        {
            FdblVector2 coord = DefaultType.AsCoordinat;
            V3DataArray myArray = new V3DataArray("First", new DateTime(2021, 10, 12), 1, 3, 2, 2, coord);
            Console.WriteLine("Array изначально");
            Console.WriteLine(myArray.ToLongString("f"));
            V3DataArray.SaveAsText(Filename, myArray);
            V3DataArray.LoadAsText(Filename, ref myArray);
            Console.WriteLine("Array после восстановления");
            Console.WriteLine(myArray.ToLongString("f"));
            V3DataArray myArray3 = new V3DataArray("Third", new DateTime(2021, 10, 12), 2, 2, 3, 3, coord);
            V3DataList myList2 = (V3DataList)myArray3;
            V3DataList.SaveBinary(Filename, myList2);
            Console.WriteLine("List изначально");
            Console.WriteLine(myList2.ToLongString("f"));
            V3DataList.LoadBinary(Filename, ref myList2);
            Console.WriteLine(myList2.ToLongString("f"));
            Console.WriteLine("List после восстановления");
            Console.ReadKey();
        }
        static void Linq()
        {
            FdblVector2 coord = DefaultType.AsCoordinat;
            V3DataArray myArray = new V3DataArray("First", new DateTime(2021, 10, 12), 0, 0, 2, 2, coord); //array с нулём элементов
            V3DataList myList = (V3DataList)myArray;
            Console.WriteLine(myList.ToLongString("f"));
            //2.
            V3MainCollection myCollection = new V3MainCollection();
            coord = DefaultType.normalXplate;
            V3DataArray myArray2 = new V3DataArray("Second", new DateTime(2021, 10, 12), 4, 3, 2, 2, coord);
            V3DataArray myArray3 = new V3DataArray("Third", new DateTime(2020, 10, 12), 0, 0, 3, 3, coord); //list с нулём элементов
            V3DataList myList2 = (V3DataList)myArray3;

            coord = DefaultType.zeros;
            V3DataArray myArray4 = new V3DataArray("Forth", new DateTime(2021, 10, 12), 4, 4, 0.5, 0.5, coord);
            V3DataList myList3 = (V3DataList)myArray4;

            myCollection.Add(myArray);
            myCollection.Add(myArray2);
            myCollection.Add(myList2);
            myCollection.Add(myList3);
            Console.WriteLine(myCollection.ToLongString("f"));
            Console.WriteLine("Запрос возвращает элемент с максимальным расстоянием до начала координат.");
            DataItem MaxItem = (DataItem)myCollection.Max;
            Console.WriteLine(MaxItem.ToLongString("f"));
            IEnumerable<double> X = myCollection.NUniqueX;
            Console.WriteLine("Запрос возвращает неуникальные значения координаты X");
            foreach (double item in X)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Запрос возвращает все элементы из V3MainCollection, с минимальной датой.(Идёт печать даты и id этих элементов)");
            IEnumerable<V3Data> MinDate = myCollection.MinDate;
            foreach (V3Data item in MinDate)
            {
                Console.WriteLine(item.id + " " + item.date);
            }
            Console.ReadKey();
        }
    }
}
