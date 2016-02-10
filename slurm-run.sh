#!/bin/sh
ALGO=$1
SPL=$2

USER=sobernig
PREFIX=/home/${USER}

## staging to nodes
cd /local/$USER
cp -R ${PREFIX}/sayyad .
cd sayyad
## trigger the run (assuming `ant compile` was performed once on the SLURM server: debussy).
ant -Dalgo=$1 -Dspl=$2 -Drepeats=1 run

## copy-merge the result data back to the SLURM server

for file in $(find NSGAIIDMStudy/data -type f); do
    echo "${file}"
    mkdir -p "${PREFIX}/$(dirname "${file}")"
    cat "${file}" >> "${PREFIX}/${file}"
done

## provide a clean state
cd ..
rm -rf sayyad/

