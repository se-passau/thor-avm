#!/bin/sh

SPLS="Ptoybox PaxTLS Pecos Pfreebsd Pfiasco PuClinux P286"
ALGOS="IBEA NSGAII"
REPEATS=9
DATABED="orig-sayyad-ase13"
TS=$(date "+%Y%m%d-%H%M%S")
DIR=${DATABED}-${TS}

##
## backup and clean-up 
##

for spl in ${SPLS} 
do
    for algo in ${ALGOS}
    do
	i=1
	while [ "$i" -le "${REPEATS}" ]; do
	    # echo "$i ${algo} ${spl}"
	    sbatch -A spl -p chimaira -n 1 --exclusive=user --mem=5900 ./slurm-run.sh ${algo} ${spl} ${DATABED} ${DIR}
	    i=$(($i + 1))
	done
    done		
done
