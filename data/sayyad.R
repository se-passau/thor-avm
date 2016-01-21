##
## stefan.sobernig@wu.ac.at
##

orig <- read.csv2("ase13.csv", header = FALSE, sep=";", dec=",")
orig <- cbind(orig, run = 1)
rep1 <- read.csv2("replica1.csv", header = FALSE, sep=";", dec=",")
rep1 <- cbind(rep1, run = 2)
rep2 <- read.csv2("replica2.csv", header = FALSE, sep=";", dec=",")
rep2 <- cbind(rep2, run = 3)

dat <- rbind(orig, rep1, rep2)

dat[dat$V3 == "TTANY",]$V4 <- log(dat[dat$V3 == "TTANY",]$V4)

ggplot(na.omit(dat))  +
    facet_wrap(V3 ~ run, ncol = 3, scales = "free_y", drop = FALSE) +
        geom_point(aes(x=V1, y = V4, shape = V2)) + theme_bw() + xlab("SPLs in Sayyad et al. (ASE'13); see Table IV") + ylab("MOOP performance indicators") +
        theme(axis.ticks = element_blank(), axis.text.x =
                  element_text(angle = 45, hjust = 1))


##
##
##

x <- as.numeric(unlist(strsplit("-1
217269
-1
-1
-1
-1
-1
-1
-1", "\n")))

x[x < 0] <- NA

median(x)
