import numpy as np
from sklearn.datasets import make_blobs

import distance as d
from hclustering import hclustering, node, printtree

from dbscan import dbscan
from kmeans import kmeans
from kmedoids import kmedoids
import matplotlib.pyplot as plt
import seaborn as sns

def plot_clust(data, clust):
    cols='rgbcm'
    for i,v in zip(data,clust):
        plt.plot(i[0],i[1],'.'+cols[v])

if __name__=='__main__':    
    data=np.array([
        [2,4],
        [7,3],
        [3,5],
        [5,3],
        [7,4],
        [6,8],
        [6,5],
        [8,4],
        [2,5],
        [3,7]        
    ])
    db=dbscan(data,1.9,d.eucledian)
    r=db.fit_predict()
    plt.ylim([0,10])
    plt.xlim([0,10])
    plot_clust(data,r)
    #plt.show()

    #h=hclustering(data,d.eucledian)
    #r=h.fit('abcdefghij')
    #plt.ylim([0,10])
    #plt.xlim([0,10])
    #printtree(r)
    #res=h.predict(3)  
    #plot_clust(data,res)
    #plt.show()  
#    data,_=make_blobs()
    k=kmedoids(data,2)
    res=k.fit_predict()
    
    plot_clust(data,res)
    plt.ylim([0,10])
    plt.xlim([0,10])
    plt.plot(k.centers[:,0],k.centers[:,1], 'k*')
    
    plt.show()
    s=5
