using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Lab3
{
    class V3DataArray : V3Data, IEnumerable<DataItem>
    {
        public float[] left { get; set; }
        public float[] right { get; set; }
        public float[,] calculated_integrals { get; private set; }
        public Vector2[,] dimensions { get; private set; }
        public double stepX { get; private set; }
        public double stepY { get; private set; }
        public int nodesX { get; private set; }
        public int nodesY { get; private set; }
        public V3DataArray(string id, DateTime date) : base(id, date)
        {
            dimensions = new Vector2[0, 0];
            left = null;
            right = null;
            calculated_integrals = null;
        }
        public V3DataArray(string id, DateTime date, int nodesX, int nodesY, double stepX, double stepY, FdblVector2 F) : base(id, date)
        {
            dimensions = new Vector2[nodesY, nodesX];
            this.stepX = stepX;
            this.nodesX = nodesX;
            this.nodesY = nodesY;
            this.stepY = stepY;
            for (int i = 0; i < nodesY; i++)
            {
                for (int j = 0; j < nodesX; j++)
                {
                    dimensions[i, j] = F(j * stepX, i * stepY);
                }
            }
        }
        public override int Count
        {
            get
            {
                return nodesX * nodesY;
            }
        }
        public override double MaxDistance
        {
            get
            {
                return Math.Sqrt((nodesX - 1) * stepX * (nodesX - 1) * stepX + (nodesY - 1) * stepY * (nodesY - 1) * stepY);
            }
        }
        public override string ToString()
        {
            return "V3DataArray\n" + base.ToString() + "Размер шага по X и по Y:" + stepX + " " + stepY + "\n" + "Размер сетки: " + nodesX + "*" + nodesY + "\n";
        }
        public override string ToLongString(string format)
        {
            StringBuilder str = new StringBuilder(ToString());
            for (int i = 0; i < nodesY; i++)
            {
                for (int j = 0; j < nodesX; j++)
                {
                    str.Append($"X = {(j * stepX).ToString(format)}, Y = {(i * stepY).ToString(format)}, Field X component = {dimensions[i, j].X.ToString(format)}, Field Y component = {dimensions[i, j].Y.ToString(format)}, Module of field = { Math.Sqrt(dimensions[i, j].X * dimensions[i, j].X + dimensions[i, j].Y * dimensions[i, j].Y).ToString(format)}\n"); //Тут была ошибка в выводе
                }
            }

            return str.ToString();
        }
        public static explicit operator V3DataList(V3DataArray param)
        {
            V3DataList List = new V3DataList(param.id, param.date);
            for (int i = 0; i < param.nodesY; i++)
            {
                for (int j = 0; j < param.nodesX; j++)
                {
                    List.Add(new DataItem(j * param.stepX, i * param.stepY, param.dimensions[i, j]));
                }
            }
            return List;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            for (int i = 0; i < nodesY; i++)
                for (int j = 0; j < nodesX; j++)
                {
                    double x = stepX * j;
                    double y = stepY * i;
                    yield return new DataItem(x, y, dimensions[i, j]);
                }
        }
        public static bool SaveAsText(string filename, V3DataArray v3)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Truncate);
                StreamWriter stream = new StreamWriter(fs);
                stream.WriteLine(v3.id);
                stream.WriteLine(v3.date);
                stream.WriteLine(v3.stepX);
                stream.WriteLine(v3.stepY);
                stream.WriteLine(v3.nodesX);
                stream.WriteLine(v3.nodesY);
                foreach (var mesure in v3.dimensions)
                {
                    stream.WriteLine(mesure.X + " " + mesure.Y);
                }
                stream.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return true;
        }
        public static bool LoadAsText(string filename, ref V3DataArray v3)
        {
            CultureInfo CI = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                StreamReader stream = new StreamReader(fs);
                v3.id = stream.ReadLine();
                v3.date = DateTime.Parse(stream.ReadLine(), CI);
                v3.stepX = double.Parse(stream.ReadLine(), CI);
                v3.stepY = double.Parse(stream.ReadLine(), CI);
                v3.nodesX = int.Parse(stream.ReadLine(), CI);
                v3.nodesY = int.Parse(stream.ReadLine(), CI);
                v3.dimensions = new Vector2[v3.nodesY, v3.nodesX];
                string[] measure = null;
                for (int i = 0; i < v3.nodesY; i++)
                {
                    for (int j = 0; j < v3.nodesX; j++)
                    {
                        measure = stream.ReadLine().Split(' ');
                        v3.dimensions[i, j] = new Vector2(float.Parse(measure[0], CI), float.Parse(measure[1], CI));
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return true;
        }

        public bool Integrals(float[] left, float[] right)
        {
            bool tf = true;
            int nx = nodesX;
            int ny = nodesY * 2;
            float[] x = new float[2] { 0F, (float)((nx - 1) * stepX) };
            float[] y = new float[ny * nx];
            int nlim = right.Length;
            float[] calculated_integrals = new float[nlim * ny];
            int ret = 0;
            int l = 0;
            for (int i = 0; i < nodesY; i++)
            {
                for (int k = 0; k < nodesX; k++)
                {
                    y[l] = dimensions[i, k].X;
                    y[l + nodesX] = dimensions[i, k].Y;
                    l++;
                }
                l += nodesX;
            }
            if (integrals_computation(nx, ny, x, y, nlim, left, right, calculated_integrals, ref ret) == 0)
            {
                Console.WriteLine($"Error number: {ret}");
                tf = false;
            }
            this.calculated_integrals = new float[nlim, ny];
            l = 0;
            for (int i = 0; i < nlim; i++)
            {
                l = i;
                for (int j = 0; j < ny; j++)
                {
                    this.calculated_integrals[i, j] = calculated_integrals[l];
                    l += nlim;
                }
            }
            return tf;
        }

        [DllImport("..\\..\\..\\..\\x64\\Debug\\CPP_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern
        int integrals_computation(int nx, int ny, float[] x, float[] y, int nlim, float[] left, float[] right, float[] calculated_integrals, ref int ret);
    }

}
