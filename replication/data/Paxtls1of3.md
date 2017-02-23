Statistical companion
================
n/a
2017-02-23

-   Prerequisites
    -   How to (re-)generate the companion report
    -   R packages
-   Data description
    -   ALGO=IBEA
    -   ALGO=NSGAII
-   Analysis of variances (for ALGO = IBEA)
    -   Checks
    -   Anova table: Hypervolume (HV)
    -   Anova table: PCORRECT
    -   Significant differences: Tukey HSD
-   Detach R packages

Prerequisites
=============

How to (re-)generate the companion report
-----------------------------------------

(This assumes that the R package `rmarkdown` is installed, in doubt, run from within R: `install.packages("rmarkdown")`.)

1.  Clone the repo or download its content
2.  `cd replication/data`
3.  `Rscript -e "rmarkdown::render('companion.Rmd','all', params = list(spl = 'Paxtls1of3'))"`
4.  Open `companion.html`

R packages
----------

``` {.r}
wants <- c("ggplot2", "reshape2", "car", "pander")
has   <- wants %in% rownames(installed.packages())
if(any(!has)) install.packages(wants[!has], repos = "http://cran.us.r-project.org")
```

``` {.r}
library(reshape2)
library(ggplot2)
```

    ## Warning: package 'ggplot2' was built under R version 3.3.2

``` {.r}
library(car)
library(pander)

all <- read.csv2(params$data)
df <- subset(all, SPL == params$spl)
levels(df$DIST) <- c("normal", "uniform", "x264")
```

Data description
================

### ALGO=IBEA

``` {.r}
DATA <- subset(df, ALGO == "IBEA" & FINT %in% c("F","FI20","FI100"))
```

``` {.r}
acast(DATA, DIST ~ FINT, length)
```

    ##           F FI100 FI20
    ## normal  200   200  200
    ## uniform 200   200  200
    ## x264    200   200  200

``` {.r}
ggplot(na.omit(subset(DATA, VARIABLE=="HV")), aes(y=value, x = 1)) +
    geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 3, drop = TRUE) +
            stat_summary(fun.y="median", geom="point") +
                stat_summary(fun.y="mean", geom="point", shape=3) + xlab("HV")
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-3-1.png)

``` {.r}
ggplot(na.omit(subset(DATA, VARIABLE=="PCORRECT")), aes(y=value, x = 1)) +
    geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 3, drop = TRUE) +
            stat_summary(fun.y="median", geom="point") +
                stat_summary(fun.y="mean", geom="point", shape=3) + xlab("PCORRECT")
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-3-2.png)

``` {.r}
tta <- na.omit(subset(DATA, VARIABLE=="TimeToAnyC"))

if (nrow(tta)) {
    ggplot(tta, aes(y=value, x = 1)) +
        geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 3, drop = TRUE) +
        stat_summary(fun.y="median", geom="point") +
        stat_summary(fun.y="mean", geom="point", shape=3) +
        xlab("TimeToAnyC")
}
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-3-3.png)

### ALGO=NSGAII

``` {.r}
DATA <- subset(df, ALGO == "NSGAII" & FINT %in% c("F","FI20","FI100"))
```

``` {.r}
acast(DATA, DIST ~ FINT, length)
```

    ##           F FI100 FI20
    ## normal  200   200  200
    ## uniform 200   200  200
    ## x264    200   200  200

``` {.r}
ggplot(na.omit(subset(DATA, VARIABLE=="HV")), aes(y=value, x = 1)) +
    geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 3, drop = TRUE) +
            stat_summary(fun.y="median", geom="point") +
                stat_summary(fun.y="mean", geom="point", shape=3) + xlab("HV")
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-6-1.png)

``` {.r}
## & value < 1000

ggplot(na.omit(subset(DATA, VARIABLE=="PCORRECT")), aes(y=value, x = 1)) +
    geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 3, drop = TRUE) +
            stat_summary(fun.y="median", geom="point") +
                stat_summary(fun.y="mean", geom="point", shape=3) + xlab("PCORRECT")
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-6-2.png)

``` {.r}
## & value < 50000

tta <- na.omit(subset(DATA, VARIABLE=="TimeToAnyC"))

if (nrow(tta)) {
    ggplot(tta, aes(y=value, x = 1)) +
        geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 3, drop = TRUE) +
        stat_summary(fun.y="median", geom="point") +
        stat_summary(fun.y="mean", geom="point", shape=3) + xlab("TimeToAnyC")
}
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-6-3.png)

Analysis of variances (for ALGO = IBEA)
=======================================

``` {.r}
DATA <- subset(df, ALGO == "IBEA" & FINT %in% c("F","FI100") & DIST %in% c("normal","x264"))
d.hv.glm <- glm(value ~ FINT * DIST, family = gaussian, data = subset(DATA, VARIABLE == "HV"))
d.pc.glm <- glm(value ~ FINT * DIST, family = gaussian, data = subset(DATA, VARIABLE == "PCORRECT"))
```

### Checks

#### Normality of residuals

``` {.r}
hv.res <- residuals(d.hv.glm)
QQplot(hv.res)
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-8-1.png)

``` {.r}
pc.res <- residuals(d.pc.glm)
QQplot(pc.res)
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-8-2.png)

#### Homogeneity of variances

``` {.r}
hv <- subset(DATA, VARIABLE == "HV")
hv$combn <- interaction(hv$FINT,hv$DIST)
pc <- subset(DATA, VARIABLE == "PCORRECT")
pc$combn <- interaction(pc$FINT,pc$DIST)

ggplot(data=hv, aes(y = value, x = 1)) + geom_boxplot() + facet_wrap(~ combn, nrow=1) + theme_bw()
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-9-1.png)

``` {.r}
ggplot(data=pc, aes(y = value, x = 1)) + geom_boxplot() + facet_wrap(~ combn, nrow=1) + theme_bw()
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-9-2.png)

``` {.r}
l.test <- cbind(as.numeric(leveneTest(value ~ FINT * DIST, data = hv)[1,]),
      as.numeric(leveneTest(value ~ FINT * DIST, data = hv, center = mean)[1,]),
                as.numeric(leveneTest(value ~ FINT * DIST, data = pc)[1,]),
      as.numeric(leveneTest(value ~ FINT * DIST, data = pc, center = mean)[1,]))

colnames(l.test) <- c("median","mean","median","mean")
rownames(l.test) <- c("Df", "F", "p-value")
pander(l.test)
```

<table>
<colgroup>
<col width="19%" />
<col width="12%" />
<col width="11%" />
<col width="13%" />
<col width="13%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">median</th>
<th align="center">mean</th>
<th align="center">median</th>
<th align="center">mean</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>Df</strong></td>
<td align="center">3</td>
<td align="center">3</td>
<td align="center">3</td>
<td align="center">3</td>
</tr>
<tr class="even">
<td align="center"><strong>F</strong></td>
<td align="center">3.11</td>
<td align="center">3.25</td>
<td align="center">16.27</td>
<td align="center">17.75</td>
</tr>
<tr class="odd">
<td align="center"><strong>p-value</strong></td>
<td align="center">0.02753</td>
<td align="center">0.02292</td>
<td align="center">1.744e-09</td>
<td align="center">3.12e-10</td>
</tr>
</tbody>
</table>

### Anova table: Hypervolume (HV)

``` {.r}
ggplot(na.omit(subset(DATA, VARIABLE == "HV")), aes(y=value, x = 1)) +
    geom_violin() + geom_boxplot(width = 0.2) +
        facet_wrap(DIST ~ FINT, ncol = 2, drop = TRUE) +
            stat_summary(fun.y="median", geom="point") +
                stat_summary(fun.y="mean", geom="point", shape=3) + xlab("HV")
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-11-1.png)

``` {.r}
panderOptions('digits', 4)
## panderOptions('round', 4)
panderOptions('keep.trailing.zeros', TRUE)
pander(anova(d.hv.glm, test = "F"))
```

<table>
<caption>Analysis of Deviance Table</caption>
<colgroup>
<col width="21%" />
<col width="6%" />
<col width="15%" />
<col width="16%" />
<col width="17%" />
<col width="10%" />
<col width="10%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">Df</th>
<th align="center">Deviance</th>
<th align="center">Resid. Df</th>
<th align="center">Resid. Dev</th>
<th align="center">F</th>
<th align="center">Pr(&gt;F)</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>NULL</strong></td>
<td align="center">NA</td>
<td align="center">NA</td>
<td align="center">199</td>
<td align="center">0.5619</td>
<td align="center">NA</td>
<td align="center">NA</td>
</tr>
<tr class="even">
<td align="center"><strong>FINT</strong></td>
<td align="center">1</td>
<td align="center">0.1119</td>
<td align="center">198</td>
<td align="center">0.45</td>
<td align="center">941866</td>
<td align="center">0</td>
</tr>
<tr class="odd">
<td align="center"><strong>DIST</strong></td>
<td align="center">1</td>
<td align="center">0.2245</td>
<td align="center">197</td>
<td align="center">0.2255</td>
<td align="center">1889151</td>
<td align="center">0</td>
</tr>
<tr class="even">
<td align="center"><strong>FINT:DIST</strong></td>
<td align="center">1</td>
<td align="center">0.2255</td>
<td align="center">196</td>
<td align="center">2.329e-05</td>
<td align="center">1897600</td>
<td align="center">0</td>
</tr>
</tbody>
</table>

``` {.r}
aov.hv <- aov(value ~ FINT * DIST,
              data = droplevels(subset(DATA, VARIABLE == "HV")))
pander(aov.hv)
```

<table>
<caption>Analysis of Variance Model</caption>
<colgroup>
<col width="22%" />
<col width="6%" />
<col width="13%" />
<col width="13%" />
<col width="13%" />
<col width="13%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">Df</th>
<th align="center">Sum Sq</th>
<th align="center">Mean Sq</th>
<th align="center">F value</th>
<th align="center">Pr(&gt;F)</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>FINT</strong></td>
<td align="center">1</td>
<td align="center">0.1119</td>
<td align="center">0.1119</td>
<td align="center">941866</td>
<td align="center">0</td>
</tr>
<tr class="even">
<td align="center"><strong>DIST</strong></td>
<td align="center">1</td>
<td align="center">0.2245</td>
<td align="center">0.2245</td>
<td align="center">1889151</td>
<td align="center">0</td>
</tr>
<tr class="odd">
<td align="center"><strong>FINT:DIST</strong></td>
<td align="center">1</td>
<td align="center">0.2255</td>
<td align="center">0.2255</td>
<td align="center">1897600</td>
<td align="center">0</td>
</tr>
<tr class="even">
<td align="center"><strong>Residuals</strong></td>
<td align="center">196</td>
<td align="center">2.329e-05</td>
<td align="center">1.188e-07</td>
<td align="center">NA</td>
<td align="center">NA</td>
</tr>
</tbody>
</table>

``` {.r}
pander(my.etasq(aov.hv))
```

<table>
<colgroup>
<col width="25%" />
<col width="25%" />
<col width="25%" />
</colgroup>
<tbody>
<tr class="odd">
<td align="center">FINT</td>
<td align="center">DIST</td>
<td align="center">FINT:DIST</td>
</tr>
<tr class="even">
<td align="center">0.199184327307677</td>
<td align="center">0.399514421071929</td>
<td align="center">0.401301251620393</td>
</tr>
</tbody>
</table>

``` {.r}
my.interactionPlot(subset(DATA, VARIABLE == "HV"))
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-14-1.png)

``` {.r}
my.nestedBoxplot(subset(DATA, VARIABLE == "HV"))
```

![](Paxtls1of3_files/figure-markdown_github/hv-1.png)

### Anova table: PCORRECT

``` {.r}
d.pc.glm <- glm(value ~ FINT * DIST, family = gaussian, data = subset(DATA, VARIABLE == "PCORRECT"))

ggplot(na.omit(subset(DATA, VARIABLE == "PCORRECT")), aes(y=value, x = 1)) +
    geom_violin() + geom_boxplot(width = 0.2) +
    facet_wrap(DIST ~ FINT, ncol = 2, drop = TRUE) +
    stat_summary(fun.y="median", geom="point") +
    stat_summary(fun.y="mean", geom="point", shape=3) + xlab("PCORRECT")
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-15-1.png)

``` {.r}
pander(anova(d.pc.glm, test = "F"))
```

<table>
<caption>Analysis of Deviance Table</caption>
<colgroup>
<col width="22%" />
<col width="6%" />
<col width="15%" />
<col width="16%" />
<col width="18%" />
<col width="8%" />
<col width="12%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">Df</th>
<th align="center">Deviance</th>
<th align="center">Resid. Df</th>
<th align="center">Resid. Dev</th>
<th align="center">F</th>
<th align="center">Pr(&gt;F)</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>NULL</strong></td>
<td align="center">NA</td>
<td align="center">NA</td>
<td align="center">199</td>
<td align="center">794.7</td>
<td align="center">NA</td>
<td align="center">NA</td>
</tr>
<tr class="even">
<td align="center"><strong>FINT</strong></td>
<td align="center">1</td>
<td align="center">580.6</td>
<td align="center">198</td>
<td align="center">214.1</td>
<td align="center">667.2</td>
<td align="center">5.133e-65</td>
</tr>
<tr class="odd">
<td align="center"><strong>DIST</strong></td>
<td align="center">1</td>
<td align="center">2.549</td>
<td align="center">197</td>
<td align="center">211.6</td>
<td align="center">2.93</td>
<td align="center">0.08854</td>
</tr>
<tr class="even">
<td align="center"><strong>FINT:DIST</strong></td>
<td align="center">1</td>
<td align="center">41.02</td>
<td align="center">196</td>
<td align="center">170.5</td>
<td align="center">47.15</td>
<td align="center">8.481e-11</td>
</tr>
</tbody>
</table>

``` {.r}
aov.pc <- aov(value ~ FINT * DIST, data = droplevels(subset(DATA, VARIABLE == "PCORRECT")))
pander(aov.pc)
```

<table>
<caption>Analysis of Variance Model</caption>
<colgroup>
<col width="22%" />
<col width="6%" />
<col width="12%" />
<col width="13%" />
<col width="13%" />
<col width="13%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">Df</th>
<th align="center">Sum Sq</th>
<th align="center">Mean Sq</th>
<th align="center">F value</th>
<th align="center">Pr(&gt;F)</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>FINT</strong></td>
<td align="center">1</td>
<td align="center">580.6</td>
<td align="center">580.6</td>
<td align="center">667.2</td>
<td align="center">5.133e-65</td>
</tr>
<tr class="even">
<td align="center"><strong>DIST</strong></td>
<td align="center">1</td>
<td align="center">2.549</td>
<td align="center">2.549</td>
<td align="center">2.93</td>
<td align="center">0.08854</td>
</tr>
<tr class="odd">
<td align="center"><strong>FINT:DIST</strong></td>
<td align="center">1</td>
<td align="center">41.02</td>
<td align="center">41.02</td>
<td align="center">47.15</td>
<td align="center">8.481e-11</td>
</tr>
<tr class="even">
<td align="center"><strong>Residuals</strong></td>
<td align="center">196</td>
<td align="center">170.5</td>
<td align="center">0.8701</td>
<td align="center">NA</td>
<td align="center">NA</td>
</tr>
</tbody>
</table>

``` {.r}
pander(my.etasq(aov.pc))
```

<table>
<colgroup>
<col width="25%" />
<col width="27%" />
<col width="27%" />
</colgroup>
<tbody>
<tr class="odd">
<td align="center">FINT</td>
<td align="center">DIST</td>
<td align="center">FINT:DIST</td>
</tr>
<tr class="even">
<td align="center">0.930187535950312</td>
<td align="center">0.00408437903630415</td>
<td align="center">0.0657280850133835</td>
</tr>
</tbody>
</table>

``` {.r}
my.interactionPlot(subset(DATA, VARIABLE == "PCORRECT"))
```

![](Paxtls1of3_files/figure-markdown_github/unnamed-chunk-18-1.png)

``` {.r}
my.nestedBoxplot(subset(DATA, VARIABLE == "PCORRECT"))
```

![](Paxtls1of3_files/figure-markdown_github/pc-1.png)

### Significant differences: Tukey HSD

``` {.r}
hsd.hv <- TukeyHSD(aov(value ~ FINT * DIST, data = droplevels(subset(DATA, VARIABLE == "HV"))))
pander(hsd.hv$FINT)
```

<table>
<colgroup>
<col width="19%" />
<col width="12%" />
<col width="12%" />
<col width="12%" />
<col width="12%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">diff</th>
<th align="center">lwr</th>
<th align="center">upr</th>
<th align="center">p adj</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>FI100-F</strong></td>
<td align="center">-0.04731</td>
<td align="center">-0.04741</td>
<td align="center">-0.04721</td>
<td align="center">7.572e-14</td>
</tr>
</tbody>
</table>

``` {.r}
pander(hsd.hv$DIST)
```

<table>
<colgroup>
<col width="25%" />
<col width="9%" />
<col width="11%" />
<col width="12%" />
<col width="12%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">diff</th>
<th align="center">lwr</th>
<th align="center">upr</th>
<th align="center">p adj</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>x264-normal</strong></td>
<td align="center">-0.067</td>
<td align="center">-0.0671</td>
<td align="center">-0.06691</td>
<td align="center">7.572e-14</td>
</tr>
</tbody>
</table>

``` {.r}
pander(hsd.hv$`FINT:DIST`)
```

<table>
<colgroup>
<col width="41%" />
<col width="13%" />
<col width="15%" />
<col width="13%" />
<col width="13%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">diff</th>
<th align="center">lwr</th>
<th align="center">upr</th>
<th align="center">p adj</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>FI100:normal-F:normal</strong></td>
<td align="center">0.01984</td>
<td align="center">0.01966</td>
<td align="center">0.02002</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="even">
<td align="center"><strong>F:x264-F:normal</strong></td>
<td align="center">0.0001497</td>
<td align="center">-2.897e-05</td>
<td align="center">0.0003283</td>
<td align="center">0.135</td>
</tr>
<tr class="odd">
<td align="center"><strong>FI100:x264-F:normal</strong></td>
<td align="center">-0.1143</td>
<td align="center">-0.1145</td>
<td align="center">-0.1141</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="even">
<td align="center"><strong>F:x264-FI100:normal</strong></td>
<td align="center">-0.01969</td>
<td align="center">-0.01987</td>
<td align="center">-0.01951</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="odd">
<td align="center"><strong>FI100:x264-FI100:normal</strong></td>
<td align="center">-0.1342</td>
<td align="center">-0.1343</td>
<td align="center">-0.134</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="even">
<td align="center"><strong>FI100:x264-F:x264</strong></td>
<td align="center">-0.1145</td>
<td align="center">-0.1146</td>
<td align="center">-0.1143</td>
<td align="center">7.572e-14</td>
</tr>
</tbody>
</table>

``` {.r}
hsd.pc <- TukeyHSD(aov(value ~ FINT * DIST, data = droplevels(subset(DATA, VARIABLE == "PCORRECT"))))
pander(hsd.pc$FINT)
```

<table>
<colgroup>
<col width="19%" />
<col width="9%" />
<col width="8%" />
<col width="8%" />
<col width="12%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">diff</th>
<th align="center">lwr</th>
<th align="center">upr</th>
<th align="center">p adj</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>FI100-F</strong></td>
<td align="center">3.408</td>
<td align="center">3.147</td>
<td align="center">3.668</td>
<td align="center">7.572e-14</td>
</tr>
</tbody>
</table>

``` {.r}
pander(hsd.pc$DIST)
```

<table>
<colgroup>
<col width="25%" />
<col width="11%" />
<col width="9%" />
<col width="11%" />
<col width="11%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">diff</th>
<th align="center">lwr</th>
<th align="center">upr</th>
<th align="center">p adj</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>x264-normal</strong></td>
<td align="center">-0.2258</td>
<td align="center">-0.486</td>
<td align="center">0.03436</td>
<td align="center">0.08854</td>
</tr>
</tbody>
</table>

``` {.r}
pander(hsd.pc$`FINT:DIST`)
```

<table>
<colgroup>
<col width="41%" />
<col width="9%" />
<col width="9%" />
<col width="11%" />
<col width="12%" />
</colgroup>
<thead>
<tr class="header">
<th align="center"> </th>
<th align="center">diff</th>
<th align="center">lwr</th>
<th align="center">upr</th>
<th align="center">p adj</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td align="center"><strong>FI100:normal-F:normal</strong></td>
<td align="center">4.313</td>
<td align="center">3.83</td>
<td align="center">4.797</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="even">
<td align="center"><strong>F:x264-F:normal</strong></td>
<td align="center">0.68</td>
<td align="center">0.1966</td>
<td align="center">1.163</td>
<td align="center">0.00193</td>
</tr>
<tr class="odd">
<td align="center"><strong>FI100:x264-F:normal</strong></td>
<td align="center">3.182</td>
<td align="center">2.698</td>
<td align="center">3.665</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="even">
<td align="center"><strong>F:x264-FI100:normal</strong></td>
<td align="center">-3.633</td>
<td align="center">-4.117</td>
<td align="center">-3.15</td>
<td align="center">7.572e-14</td>
</tr>
<tr class="odd">
<td align="center"><strong>FI100:x264-FI100:normal</strong></td>
<td align="center">-1.132</td>
<td align="center">-1.615</td>
<td align="center">-0.6482</td>
<td align="center">3.983e-08</td>
</tr>
<tr class="even">
<td align="center"><strong>FI100:x264-F:x264</strong></td>
<td align="center">2.502</td>
<td align="center">2.018</td>
<td align="center">2.985</td>
<td align="center">7.572e-14</td>
</tr>
</tbody>
</table>

Detach R packages
=================

``` {.r}
try(detach(package:ggplot2))
try(detach(package:reshape2))
try(detach(package:car))
try(detach(package:pander))
```
