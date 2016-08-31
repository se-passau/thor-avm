solutionPlot <- function(folder, index) {
        file1 <- paste(folder, "TargetFeatureValues.txt", sep = "/") 
        data1 <- read.table(file1, header = FALSE)
        colnames(data1)[1] = "TargetFeatureValues"
        f <- paste("FeatSolution", index, ".txt", sep = "")
        
        file2 <- paste(folder, f, sep = "/")
        data2 <- read.table(file2, header = FALSE)
        colnames(data2)[1] = "SolutionFeatureValues"
        combined <- data.frame(data1, data2)
        p1 <- ggplot(melt(combined)) + geom_density(aes(colour = variable, x = value, fill = variable), adjust = 0.5, alpha = 0.3) + ggtitle("Features")
        #pdf <- paste("FeatValueComparison", index, ".pdf", sep = "")
        #ggsave(paste(folder, pdf, sep = "/"))

        file3 <- paste(folder, "TargetInteractionValues.txt", sep = "/")
        data3 <- read.table(file3, header = FALSE)
        colnames(data3)[1] = "TargetInteractionValues"
        fi <- paste("InteracSolution", index, ".txt", sep = "")
        file4 <- paste(folder, fi, sep = "/")
        data4 <- read.table(file4, header = FALSE)
        colnames(data4)[1] = "SolutionInteractionValues"
        combined2 <- data.frame(data3, data4)
        p2 <- ggplot(melt(combined2)) + geom_density(aes(colour = variable, x = value, fill = variable), adjust = 0.5, alpha = 0.3) + ggtitle("Interactions")

        ft <- paste("VariantTarget", index, ".txt", sep = "")
        file5 <- paste(folder, ft, sep = "/")
        data5 <- read.table(file5, header = FALSE)
        colnames(data5)[1] = "TargetVariantValues"
        fv <- paste("VariantSolution", index, ".txt", sep = "")
        file6 <- paste(folder, fv, sep = "/")
        data6 <- read.table(file6, header = FALSE)
        colnames(data6)[1] = "SolutionVariantValues"
        combined3 <- data.frame(data5, data6)
        p3 <- ggplot(melt(combined3)) + geom_density(aes(colour = variable, x = value, fill = variable), adjust = 0.5, alpha = 0.3) + ggtitle("Variants")

        pdff <- paste("Solution", index, ".pdf", sep = "")
        pdf(paste(folder, pdff, sep = "/"), width=24, height=5)
        #ggsave(paste(folder, pdf, sep = "/"))
        grid.arrange(p1, p2, p3, ncol = 3)
     
        dev.off()
        
        }
        featvalgraph <- function(x) {
            feat <- read.table(paste(x, "FitnFeat.txt", sep = "/"))
            feat$evolution_step <- seq.int(nrow(feat))
            colnames(feat)[1] <- "CmV_Fitnessvalue"
            ggplot(feat) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle("CmV Fitnessvalue of Features")
            ggsave(filename = paste(x, "FeatFitn.png", sep = "/"))

        }
        interacvalgraph <- function(x) {
            interac <- read.table(paste(x, "FitnInterac.txt", sep = "/"))
            interac$evolution_step <- seq.int(nrow(interac))
            colnames(interac)[1] <- "CmV_Fitnessvalue"
            ggplot(interac) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle("CmV Fitnessvalue of Interactions")
            ggsave(filename = paste(x, "InteracFitn.png", sep = "/"))
        }

        variantvalgraph <- function(x) {
            interac <- read.table(paste(x, "FitnVariant.txt", sep = "/"))
            interac$evolution_step <- seq.int(nrow(interac))
            colnames(interac)[1] <- "CmV_Fitnessvalue"
            ggplot(interac) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle("CmV Fitnessvalue of Variants")
            ggsave(filename = paste(x, "VariantFitn.png", sep = "/"))
        }

