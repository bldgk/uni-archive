import numpy as np
import distance as d

class prima:
    def __init__(self,distance:callable=d.minkovski):
        self.distance=distance
        
    def fit(self, data):
        self.d_matrix=d.distance_matrix(data,data,self.distance)
                
        inf=np.inf
        real_max=np.max(self.d_matrix)
        d_matrix=self.d_matrix.copy()
        
        np.fill_diagonal(d_matrix, real_max+1)
        
        self.graph=np.zeros(self.d_matrix.shape)
        self.selected_points=[0]            
        while set(self.selected_points)^set(range(d_matrix.shape[0])):            
            min_val=real_max+1            
            min_i=0
            min_j=0
            for i in set(self.selected_points):
                for j in range(d_matrix.shape[0]):
                    if d_matrix[i,j]<min_val:
                        min_val=d_matrix[i,j]
                        min_i=i
                        min_j=j                        
            self.selected_points.append(min_j)
            d_matrix[min_i,min_j]=np.inf
            d_matrix[min_j,min_i]=np.inf
            self.graph[min_i,min_j]=min_val
            self.graph[min_j,min_i]=min_val            
        return self.graph