# THOR Command Line Parser
A lightweight easy to use command line parsing solution for THOR. Load configuration files and create a attributed variability model as an alternative to the GUI based program.

## Getting Started
Create a text file and fill the file line by line with a command. The parser only accepts the given syntax as valid commands.
In order to run the program just start the Thor Command program with the path of the settings file as the first Command-Line argument.
Or else it's possible to run the program without a Command-Line argument and enter the path in the console application.

## Command List
### Output Folder
Set the Path for the Solution Folder
```
set output [PATH]
```
### Loading Data
Load a Variability Model
```
load model [PATH]
```
Load a Feature Distribution
```
load feature_distribution [PATH]
```
Load a Interaction Distribution
```
load interaction_distribution [PATH]
```
Load a Variant Distribution
```
load variant_distribution [PATH]
```
### Feature Model Setting
Set the number of interations
```
set interaction_count [INT]
```
Set the order of interactions. They will scale to 100% if the sum is not 100.
```
set interaction_degrees [INT] [INT] [INT] ...
```
Activate Feature Fitness Calculation
```
set feature_fitness [BOOL]
```
Activate Interaction Fitness Calculation
```
set interaction_fitness [BOOL]
```
Activate Variant Fitness Calculation
```
set variant_fitness [BOOL]
```
Activate Variant Calculation
```
set variant [BOOL]
```
### Variant Setting
Choose from a set of sampling methods

Set Pure Random Method with Treshold, Modulo and Seconds for Random Variant Generation
```
set random [BOOL]
set treshold [INT]
set modulo [INT]
set random_variant_seconds [INT]
```
Set Featurewise Method with Seconds for Featurewise Generation
```
set featurewise [BOOL]
set featurewise_seconds [INT]
```
Set Negative Featurewise Method with Seconds for Negative Featurewise Generation
```
set nfeaturewise [BOOL]
set nfeaturewise_seconds [INT]
```
Set Pairwise Method with Seconds for Pairwise Generation
```
set pairwise [BOOL]
set pairwise_seconds [INT]
```
Set Pseudo Random Method with Number of Configuration Size
```
set pseudo_random [BOOL]
set number_config_size [INT]
```
Set Random Linear Config Size + Value Method with Number of Configuration Start Size
```
set random_linear [BOOL]
set linear_config_start_size [INT]
```
Set Random Quadratic Config Size + Value Method with Number of Configuration Start Size
```
set random_quadratic [BOOL]
set quadratic_config_start_size [INT]
```
Set the Seconds for the Solver to timeout
```
set solver_timeout_seconds [INT]
```
### Evolution Setting
Activate Variant Calculation
```
set cramer_von_mises [BOOL]
```
Activate Variant Calculation
```
set kolmogorov_smirnov [BOOL]
```
Activate Variant Calculation
```
set euclidean_distance [BOOL]
```
Activate Variant Calculation
```
set chi_squared [BOOL]
```
Set the amount of maximum evaluation steps
```
set max_evaluation [INT]
```
Set the amount of population size
```
set population_size [INT]
```
Activate the NSGA-II Algorithm
```
set parallel_nsga_2 [BOOL]
```
Scale to the Overall Min/Max of calculated Variant Distributions
```
set scale_variant [BOOL]
```
Activate to stop early when the generational distance is lesser than the value in %
```
set early_stop [BOOL]
set early_stop_level [INT]
```
### Weights

Set weight for the Feature Solution
```
set weight_feature [INT]
```
Set weight for the Interaction Solution
```
set weight_interaction [INT]
```
Set weight for the Variant Solution
```
set weight_variant [INT]
```
### Logging
Set the path of the log folder
```
set log_folder [PATH]
```
Activate Logging
```
set logging [BOOL]
```
## Example File
```
set output C:\...\Solution

set log_folder C:\...\Solution
set logging true

load model C:\...\thor-avm-master\models\Apache\model.xml
load feature_distribution C:\...\thor-avm-master\Thor\InteracGenerator\FeatureValues\binarysize\Linux_fv.txt
load interaction_distribution C:\...\thor-avm-master\Thor\InteracGenerator\FeatureValues\binarysize\SQLite_iv.txt
load variant_distribution C:\...\thor-avm-master\Thor\InteracGenerator\FeatureValues\binarysize\variants\SQLite_variants.txt

set interaction_count 10
set interaction_degrees 20 40
set feature_fitness true
set interaction_fitness true
set variant_fitness true
set variant true

set number_config_size 50
set solver_timeout_seconds 3

set parallel_nsga true
set max_evaluation 5000
set population_size 50
set euclidean_distance true

set weight_feature 20
set weight_interaction 30
set weight_variant 50
```