# Thor

We implemented Thor to enable the systematic testing of approaches and tools that use attributed variability models. Thor is extensible in the way that one can easily integrate new distribution profiles to the ones we have already added. 

## Technical Basis

Thor uses kernel density estimation (Section V) to scale an existing attribute-value distribution to any given variability model. That is, we adapt the number of values in the selected distribution file to meet the number of features and interactions of the new variability model.

A second key technique is a genetic algorithm (NSGA-II) that solves our multi-objective optimization for generating similar distributions for features, interactions, and system variants as the target (user-selected) ones.

Third, we use different sampling techniques to create a sample set of valid system variants. This sample set is evaluated in each evolutionary step to compute the value distribution of the system variants. We provide the following sampling techniques:

* Feature-wise sampling: Selects one variant per feature with this feature selected and with a minimal number of additionally selected features
* Pair-wise sampling: Selects one variant per pair of features with this pair of features selected and with a minimal number of additionally selected features
* Negative feature-wise sampling: Selects one variant per feature with this feature deselected and a maximal number of additionally selected features
* Random sampling with modulo: Uses a SAT solver to find all valid system variants. The modulo operator introduces a limited randomness in the selection (otherwise the found variants are locally clustered due to the SAT solver internals) and a timeout threshold.
* Pseudo random sampling: Uses a SAT solver to find a given number of valid system variants with a given number of features. The number of features is incremented from 1 to the maximum number of selectable features.
* Random linear sampling: Similar to pseudo random sampling, but linearly increases the number of selected configurations till n/2 (where n is the number of features) and then linearily decreases again the number of selected variants.
* Random quadratic sampling: Similar to random linear sampling, but using a quadratic function to determine the number of selected variants.

## Installation and Dependencies

We rely on a number of libraries. First, we use [R](https://www.r-project.org/) for statistical tests and visualization.
Second, we use [SPL Conqueror](https://github.com/nsiegmun/SPLConqueror) for the internal representation of an attributed variability model. Furthermore, we use the capabilities of SPL Conqueror to generate feature interactions, use the sampling techniques, and call the SAT/CSP solver.
Third, we use via SPL Conqeror the Microsoft Solver Foundation in version 2.0.50727 as a SAT/CSP solver. Microsoft Solver Foundation is free for academic use (do not use the free, but limited version). If you have trouble in getting the right version, just send an email to the contributors of this project.

A detailed installation instruction as well as a short How To can be found at the [Quickstart-Guide](https://github.com/se-passau/thor-avm/tree/master/Thor/Tutorial)
