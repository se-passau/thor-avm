There is the [statistical companion](data/) to the robustness experiment reported on in Section IV. In addition, there is the complete _experimental package_ for reproducing and replicating a) the robustness experiment and b) the original ASE'13 experiment.

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

```
ant run
```

## ASE'13 experiment (exact)

1. `ant run`
2. Collect the results from a newly created sub-directory `NSGAIIDMStudy`.

# References

* [A. S. Sayyad, J. Ingram, T. Menzies and H. Ammar (2013): _Scalable product line configuration: A straw to break the camel's back_, Proc. IEEE/ACM 28th Int. Conf. Automated Softw. Eng. (ASE'13), pp. 465-474, IEEE](http://dx.doi.org/10.1109/ASE.2013.6693104)
* [A. S. Sayyad, T. Menzies and H. Ammar (2013): _On the value of user preferences in search-based software engineering: A case study in software product lines_, Proc. 35th Int. Conf. Softw. Eng. (ICSE'13), pp. 492-501, IEEE](http://dx.doi.org/10.1109/ICSE.2013.6606595)


