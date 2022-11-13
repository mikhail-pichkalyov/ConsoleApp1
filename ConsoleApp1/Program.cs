using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp4
{
    class Graph
    {
        public int N { get; set; }
        public int [,] Mas { get; set; }
        public Graph(int N, int[,] mas)
        {
            this.N = N;
            this.Mas = mas;
        }
    }

    class Perebor : Graph//алгоритм перебора
    {
        public Perebor(int N, int[,] mas)
            : base(N, mas)
        {

        }

        int idk = 0;
        List<int> cycles = new();//лист с сохранением того, использовалась ли эта вершина
        public void Alg(List<bool> guse,int sum,int a,int b)
        {
            if (guse.Contains(false))//если есть неиспользованные вершины
            {
                for(int i=0; i < N-1; i++)
            {
                if (!guse[i])//если вершина не использована
                {
                        if (!guse.Contains(true))
                        {
                            sum += idk;
                            b = i;//если это первая вершина в графе
                            sum -= Mas[a, i];
                            idk= Mas[a, i];
                        }
                    guse[i] = true;//отмечаем вершину использованной
                    Alg(guse,sum+ Mas[a, i], i,b);//запускаем снова
                        guse[i] = false;//размечаем вершину обратно
                    }
            }
            }
            else
            {
                cycles.Add(sum + Mas[a, N-1] + Mas[N-1,b]);//добавляем длину цикла
            }
        }
        public int Print()
        {
            cycles.Sort();//сортируем цикл
            return(cycles[0]);//выводим самое маленькое
            cycles.Clear();//очищаем
        }
    }
    class Greedy : Graph//жадный алгоритм
    {
        public Greedy(int N, int[,] mas)
            : base(N, mas)
        {

        }
        public bool DFS(List<List<int>> graph, int i, int b, int j)//поиск циклов в графе
        {
            if (graph[b].Count == 2)//если цепь продолжается
            {
                if (graph[b][0] == j || graph[b][1] == j)//если цикл замкнулся
                {
                    return false;
                }
                else
                {
                    if (graph[b][0] == i) DFS(graph, b, graph[b][1], j);//Если не замкнулся, но продолжается цикл
                    else DFS(graph, b, graph[b][0], j);
                }
            }
            return true;
        }
        int sum=0;//величина цикла
        public void AddGraph(List<List<int>> graph,int i,int j,int a)//добавляем рёбра в граф
        {
            if (graph[i].Count()<2&& graph[j].Count() < 2)//если нет рёбр длиной 2
            {
                
                if (graph[i].Count() == 1 && graph[j].Count() == 1)
                {
                    if (DFS(graph, i, graph[i][0], j))//если циклов нет
                    {
                        graph[i].Add(j);
                        graph[j].Add(i);
                        sum += a;
                    }
                }
                else
                {
                    graph[i].Add(j);
                    graph[j].Add(i);
                    sum += a;
                }
            }
        }
        public void Alg(List<List<int>> graph2)
        {
            int a = 0, b = 0, min = 0;
            for (int i = 0; i < N; i++)//поиск минимального ребра
                for (int j = i + 1; j < N; j++)
                {
                    if ((min == 0 && Mas[i, j] != 0) || (Mas[i, j] < min && Mas[i, j] != 0))
                    {
                        min = Mas[i, j];
                        a = i;
                        b = j;
                    }
                }
            if (Mas[a, b] != 0)//если рёбра не закончились
            {
                int i = 0;
                while (i < N && graph2[i].Count() == 2) i++;//пока цикл не замкнулся
                if (i < N)//если цикл не замкнулся
                {
                    AddGraph(graph2, a, b, Mas[a, b]);//добавляем (или не добавляем) ребро
                    Mas[a, b] = 0;//забываем ребро
                    if (graph2[a].Count() == 2)//если вершина замкнулась
                    {
                        for (int j = 0; j < N; j++)//обнуляем все пересечения
                        {
                            Mas[a, j] = 0;
                            Mas[j, a] = 0;
                        }
                    }
                    if (graph2[b].Count() == 2)
                    {
                        for (int j = 0; j < N; j++)
                        {
                            Mas[b, j] = 0;
                            Mas[j, b] = 0;
                        }
                    }
                    Alg(graph2);//
                }
            }
        }
        public int Print()
        {
            return(sum);
        }
    }
    class Little : Graph//алгоритм Литтла
    {
        public Little(int N, int[,] mas)
            : base(N, mas)
        {

        }
        List<int> res=new();
        public void Litl(List<List<int>> graph, int ng,int N)
        {
            int min = 0, a = 0; bool b = false;
            for (int i = 0; i < N; i++)//вычитаем минимальный элемент из каждой строки
            {
                for(int j = 0; j < N; j++)
                    if(graph[i][j]!=-1)
                    {
                        if (graph[i][j] < min||!b)
                        {
                            min = graph[i][j];
                            b = true;
                            //a = i; b = j;
                        }
                            
                    }
                for (int j = 0; j < N; j++)
                    if (graph[i][j] != -1)
                    {
                        graph[i][j] -= min;
                    }
                ng += min;
                min = 0; b = false;
            }
            for (int j = 0; j < N; j++)//и из каждого столбца, где нет нуля
            {
                for (int i = 0; i < N; i++)
                {
                    if (graph[i][j] != -1)
                    {
                        if (!b|| graph[i][j] <= min)
                        {
                            min = graph[i][j];
                            b = true;
                            //a = i; b = j;
                        }

                    }
                }
                if (min != 0)
                {
                    for (int i = 0; i < N; i++)
                    {
                        if (graph[i][j] != -1)
                        {
                            graph[i][j] -= min;
                        }
                    }
                    ng += min;
                    min = 0;
                }
                b=false;
            }
            if (graph.Count == 2)
            {
                res.Add(ng);
            }
            else
            {
                int minr = 0, minc = 0, maxc = 0; bool user=false, usec=false; List<int> a1 = new(), b1 = new();//Для каждого нулевого элемента матрицы cij  рассчитаем коэффициент Гi,j, который равен сумме наименьшего элемента i строки (исключая элемент Сi,j=0) и наименьшего элемента j столбца. 
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                    {

                        if (graph[i][j] == 0)
                        {
                            for (int k = 0; k < N; k++)
                            {
                                if ((graph[i][k] < minr || !user) && graph[i][k] != -1&&k!=j)
                                {
                                    minr = graph[i][k];
                                    a = k;
                                    user = true;
                                }
                                if ((graph[k][j] < minc || !usec) && graph[k][j] != -1&&k!=i)
                                {
                                    minc = graph[k][j];
                                    usec = true;
                                }
                            }
                            if (minc + minr > maxc) //Из всех коэффициентов Гi, j выберем такой, который является максимальным Гk,l = max{ Гi,j}. Если таких элементов несколько, то проверяем их все
                            {
                                maxc = minc + minr;
                                a1.Clear(); b1.Clear();
                                a1.Add(i); b1.Add(j);
                            }
                            else if (minc + minr == maxc)
                            {
                                a1.Add(i); b1.Add(j);
                            }
                            minc = 0; minr = 0; user = false;usec = false;
                        }
                    }
                for (int i = 0; i < a1.Count; i++)//Удаляем k-тую строку и столбец l, поменяем на бесконечность значение элемента Сl,k
                {
                    
                        List<List<int>> graph1 = new();
                        for(int j = 0; j < graph.Count; j++)
                    {
                        graph1.Add(new List<int>());
                        for (int k = 0; k < graph.Count; k++)
                            graph1[j].Add(graph[j][k]);
                    }
                        graph1.RemoveAt(a1[i]);
                        N--;
                        int notr=0;
                        for (int j = 0; j < N; j++)
                        {
                            graph1[j].RemoveAt(b1[i]);
                        if (!graph1[j].Contains(-1)) 
                            notr = j;
                        }
                    int notc = 0,l=0;
                    bool notc1 = true;
                        while(notc1 && l < N)
                    {
                        bool yesc = true;
                        int k = 0;
                        while (yesc&&k<N)
                        {
                            if (graph1[k][l] == -1)
                                yesc = false;
                            k++;
                        }
                        if(yesc)
                        {
                            notc = l;
                            notc1 = false;
                        }
                        l++;
                    }
                    graph1[notr][notc] = -1;
                        Litl(graph1, ng, N);
                    N++;
                }
                a1.Clear();b1.Clear();
            }
        }
        public int Print()
        {
            res.Sort();
                return (res[0]);
        }
        public void Alg()
        {
            List<List<int>> graph = new();
            for(int i = 0; i < N; i++)//тут переделываем граф в более удобный формат
            {
                graph.Add(new List<int>());
                for(int j = 0; j < N; j++)
                {
                    if(i==j) graph[i].Add(-1);
                    else graph[i].Add(Mas[i,j]);
                }
            }
            Litl(graph,0,N);
        }
    }
        class Program
    {
        static void Main(string[] args)
        {
            int N;

            /*StreamReader f = new("graphs.txt");//создали файл
            N = Convert.ToInt32(f.ReadLine());
            int[,] mas = new int[N, N];
            for (int i = 0; i < N; i++)
            { 
                string s=f.ReadLine();
                string[] strings = s.Split(' ');
                for (int j = 0; j < N; j++)
                {

                    mas[i, j] = Convert.ToInt32(strings[j]);
                }
            
        }
            f.Close();*/
            double tocht=0, a,b;
            N=Convert.ToInt32(Console.ReadLine());
            for (int rnds = 0; rnds < 100; rnds++)
            {
                int[,] mas = new int[N, N];
                Random rnd = new();
                for (int i = 0; i < N; i++)
                {
                    mas[i, i] = 0;
                    for (int j = i + 1; j < N; j++)
                    {
                        mas[i, j] = rnd.Next(1, 10);
                        mas[j, i] = mas[i, j];

                    }
                } 
                /*for(int i = 0; i < N; i++)
                { 
                    for(int k = 0; k < N; k++)
                    {
                        Console.Write(mas[i, k]);
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }*/
                //перебор */
                List<bool> guse = new();
                for (int i = 0; i < N-1; i++)
                {
                    guse.Add(false);
                }
                Perebor test = new(N, mas);
                test.Alg(guse,0,0,0);
            a=test.Print();

                //12 максимум
                Little test1 = new(N, mas);
                test1.Alg();
                b=test1.Print();
                if (a == b) tocht++;
                else
                {
                    for (int i = 0; i < N; i++)
                    {
                        for (int k = 0; k < N; k++)
                        {
                            Console.Write(mas[i, k]);
                            Console.Write(' ');
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine(a);
                    Console.WriteLine(b);
                }
            }
            Console.WriteLine(tocht / 10);//это для теста

        }
    }
}