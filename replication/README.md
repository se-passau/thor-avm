There are six statistical companions to the robustness experiment
reported on in Section 4, one for each replication/ model:

* [ToyBox](data/Ptoybox1of3.md) (in particular Section 4.1)
* [axTLS](data/Paxtls1of3.md)
* [eCos](data/Pecos1of3.md)
* [FreeBSD](data/Pfreebsd1of3.md)
* [Fiasco](data/Pfiasco1of3.md)
* [uClinux](data/Puclinux1of3.md)

In addition, there is an _experiment package_ for reproducing and
replicating a) the robustness experiment (for all models) and b) the
original ASE'13 experiment (baseline).

# Dependencies

(for executing the JMetal experiments)
* Java SDK 1.7
* Apache Ant 1.9.6

(for re-generating the companion)
* R 3.3.1 plus `rmarkdown` package
* Pandoc 1.12.3+

# Preparatory steps

* Clone the repo or download its content.
* `cd replication/`
* Make sure that `JAVA_HOME` is set, e.g.:

  ```
  export JAVA_HOME=`/usr/libexec/java_home -v 1.7`
  ```
  
* `ant clean`

# How to reproduce and replicate

## Robustness experiment (Section IV)

This is how to re-run each of the seven robustness experiments
(ToyBox, axTLS):

(There are four factor combinations, each requires a separate experimental run.)

1. `ant run -Dspl=Ptoybox1of3 -Dname=normal-F -Ddatabed=Ptoybox1of3-normal-15-F -Drepeats=50 -Dalgo=IBEA` (normal distribution, without feature interactions)
2. `ant run -Dspl=Ptoybox1of3 -Dname=normal-FI100 -Ddatabed=Ptoybox1of3-normal-15-FI100 -Drepeats=50 -Dalgo=IBEA` (normal distribution, with feature interactions)
3. `ant run -Dspl=Ptoybox1of3 -Dname=x264-F -Ddatabed=Ptoybox1of3-normal-15-F -Drepeats=50 -Dalgo=IBEA` (x264 distribution, without feature interactions)
4. `ant run -Dspl=Ptoybox1of3 -Dname=x264-FI100 -Ddatabed=Ptoybox1of3-normal-15-FI100 -Drepeats=50 -Dalgo=IBEA` (x264 distribution, with feature interactions)
5. [Collect](#how-to-collect-measurement-data) the results from the
   four newly created sub-directories: `normal-F`, `normal-FI100`,
   `x264-F`, and `x264-FI100`

Repeat the above steps for the following pairwise combinations of JMetal problem
classes and databeds:

* [Ptoybox1of3](src/jmetal/problems/dimacs/Ptoybox1of3.java) and Paxtls1of3-* databeds
* ...

## ASE'13 experiment (exact)

1. `ant run` (Note: The defaults correspond to the ASE'13 study setting.)
2. [Collect](#how-to-collect-measurement-data) the results from a newly created sub-directory `NSGAIIDMStudy`.

## How to collect measurement data

The Ant runs result in JMetal 4 output directories (`-Dname=<outdir>`) containing the measurement data on Hypervolume (HV), PCORRECT, TimeTo50C, and TimeToAnyC. To facilitate data postprocessing and analysis, there is a helper to turn the nested dir/file structures into one R data frame (in long format):

`Rscript data/collect.R <outdir> <csv>`

e.g.:

`Rscript data/collect.R x264-FI100 x264-FI100.csv`

The resulting CSV file can be sourced using `read.table` & friends in R.

# Changelog

The original setup by Sayyad et al. is a customized and extended fork of [JMetal 4.0](https://sourceforge.net/projects/jmetal/files/jmetal.4.0.tar.gz) (as released in 2011). To allow us to run a differentiated replication, as well as to parametrize the experimental runs without the need for modifying the Java sources directly, we contributed the following changes:

## Additions
* [build.xml](build.xml): Added an Ant build descriptor allowing for parametrizing experimental runs.
* [Ptoybox1of3](src/jmetal/problems/dimacs/Ptoybox1of3.java): A variant of the Ptoybox problem definition, which allows for injecting feature interactions for attribute COST (see [computeCosts](src/jmetal/problems/dimacs/Ptoybox1of3.java#L155)).
* Datasets provided to and processed by the objective function in [Ptoybox1of3](src/jmetal/problems/dimacs/Ptoybox1of3.java), incl.:

  * [Ptoybox1of3-normal-15-F](src/attrs/1of3normal-15-F/)
  * [Ptoybox1of3-normal-15-FI100](src/attrs/1of3normal-15-FI100/)
  * [Ptoybox1of3-normal-x264-F](src/attrs/1of3x264-15-F/)
  * [Ptoybox1of3-normal-x264-FI100](src/attrs/1of3x264-15-FI100/)
  * [orig-sayyad-ase13](src/attrs/orig-sayyad-ase13/)

(For each model/problem, there is one Java class and the four databeds.)

## Modifications
* [NSGAIIDMStudy](src/jmetal/experiments/NSGAIIDMStudy.java): Has been modified to process Ant parameters, rather than setting hard-coded values on heuristics, data files (attribute-value data, true Pareto fronts), and output locations ([diff](https://github.com/mrcalvin/thor-avm/blame/master/replication/src/jmetal/experiments/NSGAIIDMStudy.java)).

# References

* [A. S. Sayyad, J. Ingram, T. Menzies and H. Ammar (2013): _Scalable product line configuration: A straw to break the camel's back_, Proc. IEEE/ACM 28th Int. Conf. Automated Softw. Eng. (ASE'13), pp. 465-474, IEEE](http://dx.doi.org/10.1109/ASE.2013.6693104)
* [A. S. Sayyad, T. Menzies and H. Ammar (2013): _On the value of user preferences in search-based software engineering: A case study in software product lines_, Proc. 35th Int. Conf. Softw. Eng. (ICSE'13), pp. 492-501, IEEE](http://dx.doi.org/10.1109/ICSE.2013.6606595)


