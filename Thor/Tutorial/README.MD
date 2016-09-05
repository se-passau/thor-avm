
# THOR</h1>
A genetic generator for realistic attributed variability models

With the help of:
* [SPLConqueror] (https://github.com/nsiegmun/SPLConqueror)
* [JMetal.Net] (http://jmetalnet.sourceforge.net/)
* [Accord.Net] (http://accord-framework.net/)
* [ILNumerics] (https://ilnumerics.net/)
* [MS Solver Foundation] (https://msdn.microsoft.com/en-us/devlabs/hh145003)

<p align="center">
<img alt="overview" src="https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/overview.png" width="546">
</p>


## Quickstart Guide

This chapter describes the steps to get started.  

### Requirements
* Windows only
* .NET Framework 4.5 or higher
* R (Version 3.2+)
* VisualStudio 2013+ (TODO)
* MS Solver Foundation (Academic License) Version 2.0.2x(retrievable via [MSDNAA / Dreamspark] (https://www.dreamspark.com/Default.aspx))
* ILNumerics (automatically installed via Nuget)
* Accord (automatically installed via Nuget)

### Compilation

* Install [R](https://www.r-project.org/) and the packages
 * ks
 * ggplot2
 * scatterplot3d
 * scales
 * gridExtra
* Install [MS Solver Foundation] (https://msdn.microsoft.com/en-us/devlabs/hh145003) in Version 2.0.2x 
 * It is planned to upgrade to Version 3.1 in future
 * If you have trouble getting the right version, please contact us
* Open solution and restore the Nuget packages
* Make sure that "Microsoft.Solver.Foundation.dll" and "ILNumerics.dll" are place in Thor's main folder.
* Compile (TODO)
 * TODO
 * TODO

### 3. Get Started
#### Variability Model Settings and Fitness Selection
1. Load a Variability Model  (e.g. from [BeTTy](http://www.isa.us.es/betty/betty-online), [SPLOT](http://www.splot-research.org/))
 * Supported formats: SPlConqueror XML, Splot SXFM, .afm
2. Select the desired amount of interactions and the order of interactions
 * Order of interactions must sum up to 100%
3. Declare which types should be optimized (fitness calculation)
 * Features
 * Interactions
 * Variants
 * Any combination of the above (Multi-objective)
![varmod](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/varmod.png)

#### Feature Distribution
1. Chose from the predefined non-functional properties binarysize, mainmemory, performance or a random function
2. Select one system as the target feature distribution
3. Bootstrap the data sample to the targeted variability model size
 * It is possible to repeat the bootstrap process to receive another random draw of the density estimate
4. Select one distribution as target feature distribution
![feat](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/feat.png)

#### Interaction Distribution
1. Same as feature distribution just for the interactions

#### Variant Distribution
1. Same as feature distribution

#### Variant Sampling
1. For larger variability models it is not feasible to generate all variants. We can choose from a set of sampling methods.
![sample](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/sampling.png)
 * Pure Random:
 * FeatureWise
 * NegativeFeatureWise
 * Pairwise
 * ConfigSize Sampling:
  * Static: Same amount of variants for each configuration size
  	![static](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/static.png)
  * Linear: Increases linearly until half of configuration size
    ![lin](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/linear.png)
  * Quadratic: Increases quadratically until half of configuration size
  	![quad](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/quad.png)

We recommend the usage of one of the ConfigSize sampling methods. 

#### Genetic Settings
![evoset](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/evoset.png)
1. Select the test-statistic for the fitness calculation:
* [Kolmogorov-Smirnoff](https://en.wikipedia.org/wiki/Kolmogorov%E2%80%93Smirnov_test)
* [Cramer-von-Mises](https://en.wikipedia.org/wiki/Cram%C3%A9r%E2%80%93von_Mises_criterion)
* Chi-squared distance
* Euclidean distance

2. Select maximum amount of evaluation steps and population size
3. We recommend the usage of the parallel NSGA2 implementation
4. The algorithm can stop early if the distance (best indiviual - worst individual) from the current population to the previous is smaller than the selected percentage.

#### Evolution
![evo](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/evo.png)

1. Select the Plot Stepsize: After how many evolution steps the plot is renewed. (Plotting is slow)
2. Select if kernel density or histogram
3. Select if the fitness values should be plotted
4. Start the algorithm

#### Solutions
![evo](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/solution.png)

1. Select the weights (sum = 100%) for the solution selection of the pareto-front  (if multi-objective)
2. Plot the solution and save to shown folder. 
3. Show the fitness over the evolution steps. 

![fit](https://github.com/leutheus/TeenageMutantFeatureModels/blob/master/docs/fit.png)
