using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Parallel;

namespace InteracGenerator.Problem
{
    public class MyMultiThreadedEvaluator : ISynchronousParallelRunner
    {
        private IntergenProblem problem;
        //private List<EvaluationTask> taskList;
        private List<Task<IntergenSolution>> taskList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="threads"></param>
        public MyMultiThreadedEvaluator()
        {
        }
        public void StartParallelRunner(object problem)
        {
            this.problem = (IntergenProblem)problem;
            taskList = null;
        }

        private int counter;
        /// <summary>
        /// Adds a solution to be evaluated to a list of tasks
        /// </summary>
        /// <param name="taskParameters"></param>
        public void AddTaskForExecution(object[] taskParameters)
        {
            IntergenSolution solution = (IntergenSolution)taskParameters[0];
            solution.FoundAtEval = counter;
            counter++;
            if (taskList == null)
            {
                taskList = new List<Task<IntergenSolution>>();
            }

            taskList.Add(new Task<IntergenSolution>(() =>
            {
                problem.Evaluate(solution);
                problem.EvaluateConstraints(solution);
               
                return solution;
            }));
        }

        /// <summary>
        /// Evaluates a list of solutions
        /// </summary>
        /// <returns>A list with the evaluated solutions</returns>
        public object ParallelExecution()
        { 
            try
            {
                foreach (var task in taskList)
                {
                    task.Start();
                    
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error in MultithreadedEvaluator.ParallelExecution", ex);
                Console.Error.WriteLine(ex.StackTrace);
            }

            Task.WaitAll(taskList.ToArray());

            List<IntergenSolution> solutionList = new List<IntergenSolution>();

            foreach (Task<IntergenSolution> task in taskList)
            {
                IntergenSolution solution = null;
                try
                {
                    solution = task.Result;
                    solutionList.Add(solution);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Error in MultithreadedEvaluator.ParallelExecution", ex);
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
            taskList = null;
            return solutionList;
        }
    }
}
/*
namespace JMetalCSharp.Utils.Parallel
{
    /// <summary>
    /// @author Antonio J. Nebro Class for evaluating solutions in parallel using
    /// threads
    /// </summary>
    public class MultithreadedEvaluator : ISynchronousParallelRunner
    {
        private Problem problem;
        //private List<EvaluationTask> taskList;
        private List<Task<Solution>> taskList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="threads"></param>
        public MultithreadedEvaluator()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem">Problem to Solve</param>
        public void StartParallelRunner(object problem)
        {
            this.problem = (Problem)problem;
            taskList = null;
        }

        /// <summary>
        /// Adds a solution to be evaluated to a list of tasks
        /// </summary>
        /// <param name="taskParameters"></param>
        public void AddTaskForExecution(object[] taskParameters)
        {
            Solution solution = (Solution)taskParameters[0];
            if (taskList == null)
            {
                taskList = new List<Task<Solution>>();
            }

            taskList.Add(new Task<Solution>(() =>
            {
                problem.Evaluate(solution);
                problem.EvaluateConstraints(solution);

                return solution;
            }));
        }

        /// <summary>
        /// Evaluates a list of solutions
        /// </summary>
        /// <returns>A list with the evaluated solutions</returns>
        public object ParallelExecution()
        {
            try
            {
                foreach (var task in taskList)
                {
                    task.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error in MultithreadedEvaluator.ParallelExecution", ex);
                Console.Error.WriteLine(ex.StackTrace);
            }

            Task.WaitAll(taskList.ToArray());

            List<Solution> solutionList = new List<Solution>();

            foreach (Task<Solution> task in taskList)
            {
                Solution solution = null;
                try
                {
                    solution = task.Result;
                    solutionList.Add(solution);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Error in MultithreadedEvaluator.ParallelExecution", ex);
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
            taskList = null;
            return solutionList;
        }
    }
} */
