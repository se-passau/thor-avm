#!/bin/sh
ALGO=$1
SPL=$2
DATABED=$3
DIR=$4

USER=sobernig
PREFIX=/home/${USER}

LOCAL=/local/${USER}/${SLURM_JOB_ID}
## staging to nodes
cp -R ${PREFIX}/sayyad ${LOCAL}
(
    cd ${LOCAL}
    ## trigger the run
    ## (assuming `ant compile` was performed once on the SLURM server: debussy).
    ant -Dalgo=${ALGO} -Dspl=${SPL} -Drepeats=1 -Ddatabed=${DATABED} -Dname=${DIR} run
    
    ## copy-merge the result data back to the SLURM server 
    for file in $(find ${DIR}/data -type f ! -name "FUN.*" ! -name "VAR.*"); do
	## echo "${file}"
	mkdir -p "${PREFIX}/$(dirname "${file}")"
	cat "${file}" >> "${PREFIX}/${file}"
    done

    for file in $(find ${DIR}/data -type f -name "FUN.*" -o -name "VAR.*"); do
	## echo "${file}"
	mkdir -p "${PREFIX}/$(dirname "${file}")"
	cat "${file}" > "${PREFIX}/${file}.${SLURM_JOB_ID}"
    done
)

## provide a clean slate
rm -rf ${LOCAL}

