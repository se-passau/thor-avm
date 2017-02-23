library(reshape2)

args <- commandArgs(TRUE)
dir <- args[1]
path <- file.path(dir, "data")
out <- args[2]

vars <- c("PCORRECT","TimeToAnyC", "TimeTo50C", "HV")

files <- list.files(path = path,
                    pattern = paste("(",paste(vars,collapse="|"),")", sep=""),
                    recursive=TRUE)

data <- do.call("rbind", lapply(files, function(f) {
    d <- readLines(file.path(path,f))
    meta <- unlist(strsplit(f, "/"))
    meta <- rep(meta, each = length(d))
    m <- matrix(c(meta,d), nrow=length(d))
}))

data <- as.data.frame(data, stringsAsFactors = FALSE)
colnames(data) <- c("ALGO","SPL","VARIABLE","value")

data[,4] <- as.numeric(data[,4])
data[data[,4] == -1 & !is.na(data[,4]),4] <- NA
print(out)
write.table(data,
            file = out,
            na="",
            quote = TRUE,
            col.names = TRUE,
            row.names = FALSE,
            sep=",")

q()



