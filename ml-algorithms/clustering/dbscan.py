import random
import numpy as np
import distance as d

class dbscan:
    def __init__(self,data,r,distance):
        self.data=data
        self.distance=distance
        self.r=r
        self.clusters=[]

    def fit_predict(self):
        data=np.copy(self.data)
        d_matrix=d.distance_matrix(data,data,self.distance)
        np.fill_diagonal(d_matrix,np.inf)
        used_inds=list()
        while len(set(used_inds))!=len(data):
            initial_point=random.sample(set(range(len(data)))-set(used_inds),1)[0]
            to_visit=[]
            visited=[]
            to_visit.append(initial_point)    
            for i in to_visit:                    
                tmp_to_visit=list(np.argwhere(d_matrix[i]<=self.r).flatten())

                visited.append(i)                
                if len(tmp_to_visit)>0:
                    d_matrix[i,tmp_to_visit]=np.inf*np.ones(len(tmp_to_visit))
                    d_matrix[tmp_to_visit,i]=np.inf*np.ones(d_matrix[tmp_to_visit,i].shape)
                #else:continue
                #to_visit.remove(i)
                to_visit+=tmp_to_visit
            self.clusters.append(visited)
            used_inds=used_inds+visited
        self.clusters=self.__predict()    
        return self.clusters

    def __predict(self):
        clust=[]
        for i,v in enumerate(self.data):
            for j,l in enumerate(self.clusters):
                if i in l:
                    clust.append(j)
                    break
        return clust