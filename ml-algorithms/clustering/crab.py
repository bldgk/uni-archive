import numpy as np
import distance as d
from prima import *
from random import sample
from itertools import chain
from functools import reduce

class crab(prima):
    def __init__(self,distance:callable=d.minkovski):
        prima.__init__(self,distance)        

    def fit(self, data, n_slices=3):
        self.data=data
        prima.fit(self,data)
        tmp_graph=np.copy(self.graph)        
        #i_max=[0]
        self.classes=[]        
           
        for i in range(n_slices):            
            x,y=np.argmax(tmp_graph)//tmp_graph.shape[0],np.argmax(tmp_graph)%tmp_graph.shape[0]

            tmp_graph[y,x]=0
            tmp_graph[x,y]=0
        
        for i in range(n_slices-1):
            r=sample(list(range(len(data))),1)
            while r in chain.from_iterable(self.classes):
                r=sample(list(range(len(data))),1)
            tmp_class=r
            for i in tmp_class:
                for j in range(len(tmp_graph[i])):
                    if tmp_graph[i,j]!=0:
                        if j not in tmp_class:
                            tmp_class.append(j)
            self.classes.append(set(tmp_class))
        rest=set(chain.from_iterable(self.classes))^set(range(data.shape[0]))
        self.classes.append(rest)
        return self.classes
        
        """ for i in range(n_slices):
            max=0
            tmp_i=0
            for i in range(len(self.selected_points)-1):
                if tmp_graph[self.selected_points[i],self.selected_points[i+1]]>max: 
                    max=tmp_graph[self.selected_points[i],self.selected_points[i+1]]
                    tmp_i=i
            tmp_graph[self.selected_points[tmp_i],self.selected_points[tmp_i+1]]=0
            tmp_graph[self.selected_points[tmp_i+1],self.selected_points[tmp_i]]=0
            i_max.append(tmp_i)
        for i in range(len(i_max)-1):
            self.classes.append(set(self.selected_points[i_max[i]:i_max[i+1]]))
        return self.classes"""
             