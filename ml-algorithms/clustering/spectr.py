import numpy as np
import distance as d
from itertools import chain

class spectr:
    def __init__(self,distance:callable=d.minkovski):
        self.B=[]  # basis classes
        self.C=[]  # spectrum between neighboring points
        self.distance=distance
    
    def fit(self,data):        
        d_matr=d.distance_matrix(data,data,self.distance)
        self.data=data
        self.B.append([0])
        while len(self.B)!=len(data)+1:
            dists=[]
            for i in self.B[-1]:
                dists.append(d_matr[i,:])  # add pairwise distance rows
                d_matr[i,i]=np.inf
            distance=np.mean(dists,axis=0) # mean pairwise distance to basis
            el=np.argmin(distance)
            self.B.append(self.B[-1]+[el])
            self.C.append(distance[el])
        return self.C

    def set_borders(self,n_borders=3):
        tmp_c=np.roll(self.C,1)
        tmp_c[0]=0
        tmp_c[-1]=0
        diff=tmp_c-self.C
        argsorted=np.argsort(diff)
        self.borders=np.argsort(diff)[1:n_borders]    

    def predict(self,n_borders=3):        
        res=[]        
        bord=np.append(np.sort(self.borders),-1)
        res.append(set(self.B[bord[0]]))
        for i,v in enumerate(bord[:-1]):
            tmp_set=set(self.B[bord[i+1]])^set(self.B[v])
            res.append(tmp_set)
        return res

