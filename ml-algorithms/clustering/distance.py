import numpy as np

def distance_matrix(matrix_x,matrix_y,distance:callable):
    if matrix_x.shape[1]!=matrix_y.shape[1]:
        raise Exception('Number of features should match')
    distance_m=np.zeros((matrix_x.shape[0],matrix_y.shape[0]))
    for i in range(matrix_x.shape[0]):
        for j in range(matrix_y.shape[0]):
            distance_m[i,j]=distance(matrix_x[i,:],matrix_y[j,:])
    return distance_m
        
def minkovski(x,y,p=2):
    return np.power(np.sum(np.abs((x-y)**p)),1/p)

def manheten(x,y):
    return minkovski(x,y,1)

def eucledian(x,y):
    return minkovski(x,y,2)

def chebyshev(x,y):
    return np.max(np.abs(x-y))