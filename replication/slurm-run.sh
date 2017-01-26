#!/bin/bash
ALGO=$1
SPL=$2
DATABED=$3
DIR=$4

USER=sobernig
PREFIX=/home/${USER}

LOCAL=/local/${USER}/${SLURM_JOB_ID}
## staging to nodes
cp -R ${PREFIX}/thor-avm/replication ${LOCAL}
(
    exec 100>${PREFIX}/.slurmlock || exit 1
    cd ${LOCAL}
    ant clean
    ## trigger the run
    ## (assuming `ant compile` was performed once on the SLURM server: debussy).
    ant -Dalgo=${ALGO} -Dspl=${SPL} -Drepeats=1 -Ddatabed=${DATABED} -Dname=${DIR} run

    ## copy-merge the result data back to the SLURM server 

    for file in $(find ${DIR}/data -type f ! -name "FUN.*" ! -name "VAR.*"); do
	## echo "${file}"
	mkdir -p "${PREFIX}/$(dirname "${file}")"
        flock -x 100 || { echo "ERROR: flock() failed." >&2; exit 1; }    
	cat "${file}" >> "${PREFIX}/${file}"
        flock -u 100
    done

    for file in $(find ${DIR}/data -type f -name "FUN.*" -o -name "VAR.*"); do
	## echo "${file}"
	mkdir -p "${PREFIX}/$(dirname "${file}")"
	cat "${file}" > "${PREFIX}/${file}.${SLURM_JOB_ID}"
    done
)

## provide a clean slate
rm -rf ${LOCAL}

