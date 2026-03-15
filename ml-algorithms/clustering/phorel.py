import numpy as np
import distance as d
from random import sample
from functools import reduce
from itertools import chain

class phorel:
    def __init__(self,distance:callable=d.minkovski):
        self.distance=distance

    def fit(self,data,n_classes=3, initial_r=5):
        self.data=data
        tmp_data=np.copy(data)
        self.d_matrix=d.distance_matrix(data,data,self.distance)

        tmp=np.random.randint(0,self.data.shape[0])
        tmp_point=tmp_data[tmp,:]
        d_matrix=np.copy(self.d_matrix)

        classes=[]
        while True:
            tmp_class=[]           # points in current cluster
            tmp_class_ind=[]       # their indices
            tmp_unused={}
            for ind,val in enumerate(tmp_data):
                if self.distance(tmp_point,val)<=initial_r and ind not in chain.from_iterable(classes):  # check if point falls in hypersphere
                    tmp_class.append(val)
                    tmp_class_ind.append(ind)
            if np.allclose(np.mean(tmp_class,axis=0),tmp_point): # cluster center converged
                classes.append(set(tmp_class_ind))
                unused=set(chain.from_iterable(classes))^set(range(len(tmp_data)))
                if unused: # find unassigned points
                    tmp_point=tmp_data[sample(unused,1)] # pick random unassigned point
                else:
                    break  # all points assigned
            else:
                tmp_point=np.mean(tmp_class,axis=0) # shift center to cluster mean

        return classes
