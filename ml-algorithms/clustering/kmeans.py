import numpy as np 
import distance as d
import random

class kmeans:
    def __init__(self,data,n_centers):
        self.data=data
        self.distance=d.euclidean
        self.n_centers=n_centers
        first_center=np.asarray([random.randint(int(np.min(data)),int(np.max(data))),random.randint(int(np.min(data)),int(np.max(data)))])
        self.centers=[first_center]
        for i in range(n_centers-1):
            d_matrix=d.distance_matrix(self.data,np.mean(self.centers,axis=0)[np.newaxis,:],self.distance)
            self.centers.append(data[np.argmax(d_matrix)])
        self.centers=np.asarray(self.centers)        
        #self.centers=np.asarray([np.asarray([random.randint(int(np.min(data)),int(np.max(data)))for i in range(data.shape[1])])\
         #   for j in range(n_centers)])
        self.clusters=None

    def fit_predict(self):
        new_centers=self.centers        
        while True:
            cur_centers=new_centers

            d_matrix=d.distance_matrix(self.data,cur_centers,self.distance)

            clusters=np.asarray([np.argmin(i) for i in d_matrix])

            new_centers=np.asarray([np.mean(self.data[clusters==i],axis=0) for i in range(self.n_centers)])

            if np.allclose(cur_centers, new_centers):
                break
        self.clusters=clusters
        return self.clusters