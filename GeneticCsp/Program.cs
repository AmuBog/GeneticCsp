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

            solutions = InitSolutions(solutions);
            //PrintMat(solutions);
            fitness = FitnessFind(solutions, cspGraph, fitness);

            for(int i = 0; i < fitness.Length; i++)
            {
                Console.WriteLine(fitness[i]);
            }

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
            for(int solution = 0; solution < solutions.GetLength(0);solution++)
            {
                char color = solutions[solution, 0];
                int fit = 0;
                for(int i = 0; i < cspGraph.GetLength(0); i++)
                {
                    for(int j = 0;j<cspGraph.GetLength(1); j++)
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
    }
}