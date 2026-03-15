import numpy as np 
import distance as d
import random

class kmedoids:
    def __init__(self,data,n_centers,distance=d.manheten):
        self.data=data
        self.distance=distance
        self.n_centers=n_centers        
        self.centers=np.asarray(random.sample(list(data),n_centers))
        self.clusters=None

    def fit_predict(self):
        data=self.data
        new_centers=self.centers        
        while True:
            cur_centers=new_centers

            d_matrix=d.distance_matrix(self.data,cur_centers,self.distance)

            clusters=np.asarray([np.argmin(i) for i in d_matrix])

            n_changes=0
            n_center=0
            clusters_data=[data[np.where(clusters==i)] for i in range(self.n_centers)]
            for medoid,cluster in zip(self.centers,clusters_data):
                med_score=np.mean(d.distance_matrix(cluster,medoid[np.newaxis,:],self.distance))
                for point in cluster:
                    if np.any(point!=medoid):
                        new_score=np.mean(d.distance_matrix(cluster,point[np.newaxis,:],self.distance))
                        if new_score<med_score:
                            n_changes+=1
                            med_score=new_score
                            medoid=point
                            self.centers[n_center]=point
                n_center+=1
            clusters=np.asarray([np.argmin(i) for i in d_matrix])

            if n_changes==0:
                break

        self.clusters=clusters
        return self.clusters