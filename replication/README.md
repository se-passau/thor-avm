There is the [statistical companion](data/) to the robustness experiment reported on in Section IV. 

In addition, there is the complete _experimental package_ for reproducing and replicating a) the robustness experiment and b) the original ASE'13 experiment.

# Dependencies

* Java SDK 1.7+
* Apache Ant 1.9.6
* R 3.3.1 (for re-generating the companion)

# Preparatory steps

* Clone the repo or download its content.
* `cd replication/`
* `ant clean`

# How to reproduce and replicate

## Robustness experiment (Section IV)

(There are four factor combinations, each requires a separate experimental run.)

1. `ant run -Dname=normal-F -Ddatabed=1of3normal-15-F -Drepeats=50 -Dalgo=IBEA` (normal distribution, without feature interactions)
2. `ant run -Dname=normal-FI100 -Ddatabed=1of3normal-15-FI100 -Drepeats=50 -Dalgo=IBEA` (normal distribution, with feature interactions)
3. `ant run -Dname=x264-F -Ddatabed=1of3x264-15-F -Drepeats=50 -Dalgo=IBEA` (x264 distribution, without feature interactions)
4. `ant run -Dname=x264-FI100 -Ddatabed=1of3x264-15-FI100 -Drepeats=50 -Dalgo=IBEA` (x264 distribution, with feature interactions)
5. [Collect](#how-to-collect-measurement-data) the results from the four newly created sub-directories: `normal-F`, `normal-FI100`, `x264-F`, and `x264-FI100`

## ASE'13 experiment (exact)

1. `ant run` (Note: The defaults correspond to the ASE'13 study setting.)
2. [Collect](#how-to-collect-measurement-data) the results from a newly created sub-directory `NSGAIIDMStudy`.

## How to collect measurement data

The ant runs result in JMetal 4 output directories (`-Dname=<outdir>`) containing the measurement data on Hypervolume (HV), PCORRECT, TimeTo50C, and TimeToAnyC. To facilitate data postprocessing and analysis, there is a helper to turn the nested dir/file structures into one R data frame (in long format):

`Rscript data/collect.R <outdir> <csv>`

e.g.:

`Rscript data/collect.R x264-FI100 x264-FI100.csv`

The resulting CSV file can be sourced using `read.table` & friends in R.

# References

* [A. S. Sayyad, J. Ingram, T. Menzies and H. Ammar (2013): _Scalable product line configuration: A straw to break the camel's back_, Proc. IEEE/ACM 28th Int. Conf. Automated Softw. Eng. (ASE'13), pp. 465-474, IEEE](http://dx.doi.org/10.1109/ASE.2013.6693104)
* [A. S. Sayyad, T. Menzies and H. Ammar (2013): _On the value of user preferences in search-based software engineering: A case study in software product lines_, Proc. 35th Int. Conf. Softw. Eng. (ICSE'13), pp. 492-501, IEEE](http://dx.doi.org/10.1109/ICSE.2013.6606595)


