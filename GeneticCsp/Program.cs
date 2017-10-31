using System;

namespace GeneticCsp {
    class Program {
        static void Main(string[] args) {
            //Random number generator
            Random rnd = new Random();
            int best;
            int stagnated = 0;
            bool stop = false;
            //The graph to be optimized
            int[,] cspGraph = GetMatrix(2);
            int rows = 20;
            //Matix to store solutions
            char[,] solutions = new char[rows, cspGraph.GetLength(0)];
            //Array to store the fitness
            int[] fitness = new int[solutions.GetLength(0)];
            //Make initial random solutions
            solutions = InitSolutions(solutions, rnd);

            fitness = FitnessFind(solutions, cspGraph, fitness);

            SortMat(fitness, solutions);

            while (!stop) { 
                best = fitness[0];
                //make new children of the best solutions
                Mate(solutions, rnd);

                //Find the Fitness of the solutions
                fitness = FitnessFind(solutions, cspGraph, fitness);               
                
                //sort the solutions
                SortMat(fitness, solutions);
                
                if(best == fitness[0]) {
                    stagnated++;
                } else {
                    stagnated = 0;
                }

                if (stagnated > 10)
                    stop = true;
            }
            //Print the solutions
            PrintMat(solutions, fitness);

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
    }
}