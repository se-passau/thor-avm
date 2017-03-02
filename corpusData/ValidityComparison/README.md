# Comparing kernel density estimation and genetric algorithm (KDE+GA) against pure genetic algorithm (GA)

We use three metrics: 
* Anderson-Darling test: Denotes whether the input distribution and the output distribution are drawn from the same probability distribution. Results are p-value.
* Pearson correlation: Denotes whether the input and output distribution are linearily correlated. Results are correlation coefficients.
* Euclidean distance: Denotes whether the sum all of attribute values of the input distribution equals the sum of all attribute values of the output distribution. Results are real values.

We performed 30 runts with Thor, in which 50 solutins could be generated (depending on the Pareto criterion). See as figures only the best runs. All raw data are available.

## Anderson Darling test

### GA 
![Apache](fig/GA_Apache_summary_best_AD.pdf)
![BDBC](fig/GA_BDBC_summary_best_AD.pdf)
![BDBJ](fig/GA_BDBJ_summary_best_AD.pdf)
![h264](fig/GA_h264_summary_best_AD.pdf)
![LLVM](fig/GA_LLVM_summary_best_AD.pdf)
### KDE+GA
![Apache](fig/KDE_Apache_summary_best_AD.pdf)
![BDBC](fig/KDE_BDBC_summary_best_AD.pdf)
![BDBJ](fig/KDE_BDBJ_summary_best_AD.pdf)
![h264](fig/KDE_h264_summary_best_AD.pdf)
![LLVM](fig/KDE_LLVM_summary_best_AD.pdf)

## Person correlation

### GA 

![Apache](fig/GA_Apache_summary_best_Cor.pdf)
![BDBC](fig/GA_BDBC_summary_best_Cor.pdf)
![BDBJ](fig/GA_BDBJ_summary_best_Cor.pdf)
![h264](fig/GA_h264_summary_best_Cor.pdf)
![LLVM](fig/GA_LLVM_summary_best_Cor.pdf)
### KDE+GA
![Apache](fig/KDE_Apache_summary_best_Cor.pdf)
![BDBC](fig/KDE_BDBC_summary_best_Cor.pdf)
![BDBJ](fig/KDE_BDBJ_summary_best_Cor.pdf)
![h264](fig/KDE_h264_summary_best_Cor.pdf)
![LLVM](fig/KDE_LLVM_summary_best_Cor.pdf)

## Euclidean distance

### GA 
![Apache](fig/GA_Apache_summary_best_Dist.pdf)
![BDBC](fig/GA_BDBC_summary_best_Dist.pdf)
![BDBJ](fig/GA_BDBJ_summary_best_Dist.pdf)
![h264](fig/GA_h264_summary_best_Dist.pdf)
![LLVM](fig/GA_LLVM_summary_best_Dist.pdf)
### KDE+GA
![Apache](fig/KDE_Apache_summary_best_Dist.pdf)
![BDBC](fig/KDE_BDBC_summary_best_Dist.pdf)
![BDBJ](fig/KDE_BDBJ_summary_best_Dist.pdf)
![h264](fig/KDE_h264_summary_best_Dist.pdf)
![LLVM](fig/KDE_LLVM_summary_best_Dist.pdf)