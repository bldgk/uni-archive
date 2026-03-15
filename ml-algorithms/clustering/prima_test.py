from prima import *

if __name__=='__main__':
    p=prima()
    c=np.array([[0,0],[1,0],[0,1]])
    a=p.fit(c)
    b=d.distance_matrix(c,c,d.minkovski)
    pass