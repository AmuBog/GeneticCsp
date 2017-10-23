using System;

namespace GeneticCsp
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] cspGraph = 
            {
                     { 0, 1, 1, 0, 0, 0 },
                     { 1, 0, 0, 1, 1, 0 },
                     { 1, 0, 0, 1, 1, 0 },
                     { 0, 1, 1, 0, 0, 1 },
                     { 0, 1, 1, 0, 0, 1 },
                     { 0, 0, 0, 1, 1, 0 }
            };
            char[,] solutions = new char[20, 6];
            int[] fitness = new int[solutions.GetLength(0)];

            //Make initial random solutions
            solutions = InitSolutions(solutions);
            //Find the Fitness of the solutions
            fitness = FitnessFind(solutions, cspGraph, fitness);
            //sort the solutions
            SortMat(fitness, solutions);
            Mate(solutions);
            //Print the solutions
            PrintMat(solutions);

            //for (int i = 0; i < fitness.Length; i++)
            //{
            //    Console.WriteLine(fitness[i]);
            //}

            Console.Read();
        }
        static char[,] InitSolutions(char[,] solutions)
        {
            Random rnd = new Random();
            int initSol = solutions.GetLength(0) / 2;

            char[] colors = { 'r', 'b', 'w'};

            for (int i = 0; i<initSol; i++)
            {
                for (int j = 0; j<solutions.GetLength(1); j++)
                {
                    int color = rnd.Next(colors.Length);
                    solutions[i, j] = colors[color];                
                }
            }
            return solutions;
        }
        static int[] FitnessFind(char[,] solutions, int[,] cspGraph, int[] fitness)
        {           
            for (int solution = 0; solution < solutions.GetLength(0);solution++)
            {
                int fit = 0;
                for(int i = 0; i < cspGraph.GetLength(0); i++)
                {
                    char color = solutions[solution, i];
                    for(int j = i;j<cspGraph.GetLength(1); j++)
                    {
                        if (cspGraph[i, j] == 1 && solutions[solution, j].Equals(color))
                        {
                            fit++;
                        }                        
                    }
                }
                fitness[solution] = fit;
            }

            return fitness;
        }
        static void Mate(char[,] solutions)
        {
            Random rnd = new Random();
            int rng1, rng2, child;

            for (int i = 0; i < 16; i += 2)
            {
                child = (solutions.GetLength(0) / 2) + i;
                rng1 = rnd.Next(solutions.GetLength(0));
                do
                    rng2 = rnd.Next(solutions.GetLength(0));
                while (rng1 == rng2);

                CopyRow(solutions, i, child);
                CopyRow(solutions, i + 1, child + 1);


            }
        }
        static void PrintMat(char[,] solutions)
        {
            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                for (int j = 0; j < solutions.GetLength(1); j++)
                {
                    if (j != 0)
                        Console.Write(solutions[i, j]);
                    else
                        Console.Write("\n" + solutions[i, j]);
                }
            }
        }
        static void SortMat(int[] fitness, char[,] solutions)
        {
            int temp;
            for(int i = 0; i < fitness.Length; i++)
            {
                for(int j = 1; j < fitness.Length; j++)
                {
                    if (fitness[j-1] > fitness[j])
                    {
                        temp = fitness[j-1];
                        fitness[j-1] = fitness[j];
                        fitness[j] = temp;
                        solutions = SwapRow(solutions, j);
                    }
                }
            }
        }
        static char[,] SwapRow(char [,] solutions, int swap)
        {
            char temp;

            for(int i = 0; i<solutions.GetLength(1); i++)
            {
                temp = solutions[swap-1, i];
                solutions[swap-1, i] = solutions[swap, i];
                solutions[swap, i] = temp;
            }

            return solutions;
        }
        static void CopyRow(char[,] solutions, int from, int into)
        {
            for(int i = 0; i < solutions.GetLength(1);i++)
            {
                solutions[into, i] = solutions[from, i];
            }
        }
    }
}