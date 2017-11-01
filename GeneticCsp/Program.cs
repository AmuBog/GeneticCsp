using System;
using System.Diagnostics;
using System.IO;

namespace GeneticCsp {
    class Program {
        static void Main(string[] args) {
            //Random number generator
            Random rnd = new Random();            
            Stopwatch time = new Stopwatch();

            int best;
            int i = 0;
            int stagnated = 0;
            bool stop = false;
            //The graph to be optimized
            int[,] cspGraph = MakeGraph(100);
            //PrintGraph(cspGraph);
            int rows = 20;           
            //Matix to store solutions
            char[,] solutions = new char[rows, cspGraph.GetLength(0)];
            //Array to store the fitness
            int[] fitness = new int[solutions.GetLength(0)];

            time.Start();
            //Make initial random solutions
            solutions = InitSolutions(solutions, rnd);

            fitness = FitnessFind(solutions, cspGraph, fitness);

            SortMat(fitness, solutions);

            long elapsedTime = time.ElapsedMilliseconds;
            Statistics(fitness[0], elapsedTime);
            while (!stop) {
                //Save currently best solution
                best = fitness[0];
                //make new children of the best solutions
                Mate(solutions, rnd);

                //Find the Fitness of the solutions
                fitness = FitnessFind(solutions, cspGraph, fitness);               
             
                //sort the solutions
                SortMat(fitness, solutions);

                elapsedTime = time.ElapsedMilliseconds;
                Statistics(fitness[0], elapsedTime);

                //Condition block checking if the algorithm should stop
                if (best == fitness[0]) {
                    stagnated++;
                } else {                    
                    stagnated = 0;
                }
                if (stagnated > 20)
                    stop = true;
                //number of iterations traken
                i++;
            }
            //Print the solutions
            PrintMat(solutions, fitness);
            Console.WriteLine("\nNumber of iterations: " + i);
            Console.Read();
        }
        //Method to make initial solutions
        static char[,] InitSolutions(char[,] solutions, Random rnd) {
            //Save half the length of the solution matrix
            int initSol = solutions.GetLength(0) / 2;

            //Array holding the different colors
            char[] colors = { 'r', 'b', 'w' };

            //Filling half of the solution matrix
            for (int i = 0; i < initSol; i++) {
                //Getting random colors and fill the matrix with them
                for (int j = 0; j < solutions.GetLength(1); j++) {
                    int color = rnd.Next(colors.Length);
                    solutions[i, j] = colors[color];
                }
            }
            return solutions;
        }
        //Method to calculate the fitness of the solutions
        static int[] FitnessFind(char[,] solutions, int[,] cspGraph, int[] fitness) {
            //Outer loop changing what solution is worked on
            for (int solution = 0; solution < solutions.GetLength(0); solution++) {
                //Reset fitnett count for each solution
                int fit = 0;
                //Nested loop construction counting the fitness of each solution
                for (int i = 0; i < cspGraph.GetLength(0); i++) {
                    //Get the color of the node
                    char color = solutions[solution, i];
                    for (int j = i; j < cspGraph.GetLength(1); j++) {
                        //Check if the neighbor has the same color
                        if (cspGraph[i, j] == 1 && solutions[solution, j].Equals(color)) {
                            fit++;
                        }
                    }
                }
                //Storing the fitness inside the fitness array
                fitness[solution] = fit;
            }

            return fitness;
        }
        //Make new solutions of the best solutions
        static void Mate(char[,] solutions, Random rnd) {
            int rng1, rng2, temp, child;

            //Make children starting from the middle of the solution matrix
            for (int i = 0; i < solutions.GetLength(0) / 2; i += 2) {
                //Get two random numbers to set a range for two-point crossover
                rng1 = rnd.Next(solutions.GetLength(1));
                rng2 = rnd.Next(solutions.GetLength(1));
                while (rng1 == rng2) {
                    rng2 = rnd.Next(solutions.GetLength(1));
                }

                //Make sure rng1 is less than rng2
                if (rng2 < rng1) {
                    temp = rng2;
                    rng2 = rng1;
                    rng1 = temp;
                }

                //Set the position in which the child solution gets placed
                child = (solutions.GetLength(0) / 2) + i;
                //Make two children with two-point crossover from two parents
                CopyRow(solutions, i, child, rng1, rng2);
                CopyRow(solutions, i + 1, child + 1, rng1, rng2);

                double mut1 = rnd.NextDouble();
                double mut2 = rnd.NextDouble();

                //mutations
                if (mut1 > 0.9)
                {
                    int place = rnd.Next(solutions.GetLength(1));
                    char color = solutions[child, place];

                    solutions[child, place] = GetColor(rnd, color);
                } 
                if (mut2 > 0.9)
                {
                    int place = rnd.Next(solutions.GetLength(1));
                    char color = solutions[child+1, place];

                    solutions[child+1, place] = GetColor(rnd, color);
                } 
            }
        }
        //Print the solutions matrix
        static void PrintMat(char[,] solutions, int[] fitness) {
            for (int i = 0; i < solutions.GetLength(0); i++) {
                for (int j = 0; j < solutions.GetLength(1); j++) {
                    if (j == 0)
                        Console.Write("\n" + i + ": " + solutions[i, j]);
                    else if (j != 0 && j < solutions.GetLength(1) - 1)
                        Console.Write(solutions[i, j]);
                    else
                        Console.Write(solutions[i, j] + ": " + fitness[i]);
                }
            }
        }
        //Sort the matrix so the fittest solutions is at the top
        static void SortMat(int[] fitness, char[,] solutions) {
            int temp;
            for (int i = 0; i < fitness.Length; i++) {
                for (int j = 1; j < fitness.Length; j++) {
                    //Sorting the fitness array and the 2D matrix using bubble sort
                    if (fitness[j - 1] > fitness[j]) {
                        temp = fitness[j - 1];
                        fitness[j - 1] = fitness[j];
                        fitness[j] = temp;
                        //Sending rows to be swapped in 2D array
                        solutions = SwapRow(solutions, j);
                    }
                }
            }
        }
        //Used by the SortMat method to sort the solutions matrix
        static char[,] SwapRow(char[,] solutions, int swap) {
            char temp;

            //Swapping two rows putting the fittest on top using bubblesort
            for (int i = 0; i < solutions.GetLength(1); i++) {
                temp = solutions[swap - 1, i];
                solutions[swap - 1, i] = solutions[swap, i];
                solutions[swap, i] = temp;
            }

            return solutions;
        }
        //Used by the Mate method to copy the solutions to the child solutions
        static void CopyRow(char[,] solutions, int from, int into, int rng1, int rng2) {
            //Console.WriteLine(rng1 + " " + rng2);

            //Copy row from parent to child included two-point crosover
            for (int i = 0; i < solutions.GetLength(1); i++) {
                //solutions taken from parent 1
                if (i < rng1 || i > rng2)
                    solutions[into, i] = solutions[from, i];
                //Solutions taken from parent 2
                else {
                    //If even number, the range is taken from the solution below
                    if (from % 2 == 0)
                        solutions[into, i] = solutions[from + 1, i];
                    //Else the range is taken from the solution above
                    else
                        solutions[into, i] = solutions[from - 1, i];
                }
            }
        }
        static int[,] GetMatrix(int choice) {
            int[,] graph;
            switch (choice) {
                case 1:
                    graph = new int[,]
                    {
                         { 0, 1, 1, 0, 0, 0 },
                         { 1, 0, 0, 1, 1, 0 },
                         { 1, 0, 0, 1, 1, 0 },
                         { 0, 1, 1, 0, 0, 1 },
                         { 0, 1, 1, 0, 0, 1 },
                         { 0, 0, 0, 1, 1, 0 }
                    };
                    break;
                case 2:
                    graph = new int[,]
                    {
                         { 0, 1, 1, 0, 0, 0, 0, 0 },
                         { 1, 0, 0, 0, 1, 0, 0, 1 },
                         { 1, 0, 0, 1, 1, 0, 0, 0 },
                         { 0, 0, 1, 0, 1, 1, 0, 0 },
                         { 0, 1, 1, 1, 0, 0, 1, 0 },
                         { 0, 0, 0, 1, 0, 0, 0, 1 },
                         { 0, 0, 0, 0, 1, 0, 0, 1 },
                         { 0, 1, 0, 0, 0, 1, 1, 0 },
                    };
                    break;
                default:
                    graph = new int[,]
{
                         { 0, 1, 1, 0, 0, 0 },
                         { 1, 0, 0, 1, 1, 0 },
                         { 1, 0, 0, 1, 1, 0 },
                         { 0, 1, 1, 0, 0, 1 },
                         { 0, 1, 1, 0, 0, 1 },
                         { 0, 0, 0, 1, 1, 0 }
};
                    break;
            }
            return graph;
        }
        static int[,] MakeGraph(int nodes) {
            Random rnd = new Random();
            int random;
            int [] connected = new int[nodes];
            int[,] graph = new int[nodes, nodes];
            
            for (int i = 0; i < nodes; i++) {
                for (int j = i; j < nodes; j++) {
                    if (i == j)
                        graph[i, j] = 0;
                    else {
                        if (nodes % 2 == 0) {
                            if (connected[i] >= (nodes/2))
                                break;
                            }
                        if (nodes % 2 != 0) {
                            if (connected[i] >= (nodes/2) +1 )
                                break;
                        }
                        random = rnd.Next(2);
                        if (random == 1) {
                            connected[i]++;
                            connected[j]++;
                        }

                        graph[i, j] = random;
                        graph[j, i] = random;
                    }
                }
                if (connected[i] == 0 && i != (nodes - 1)) {
                    i--;
                }
                
            }
            return graph;
        }
        static void PrintGraph(int[,] graph) {
            int length = graph.GetLength(0);
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < length; j++) {
                    if (j == 0)
                        Console.Write("\n" + graph[i, j]);
                    else
                         Console.Write(" " + graph[i, j]);
                    }
            }
        }
        static char GetColor(Random rnd, char originColor) {
            char[] colors = { 'r', 'b', 'w' };
            char color;

            color = colors[rnd.Next(colors.Length)];
            while (color.Equals(originColor))
                color = colors[rnd.Next(colors.Length)];

            return color;
        }
        static void Statistics(int best, long elapsedTime) {
            Convert.ToString(elapsedTime);
            using (TextWriter sw = File.AppendText(@"C:\Users\Student\Documents\data3.csv")) {
                sw.WriteLine("{0}ms;{1}", elapsedTime, best);
            }
        }
    }
}