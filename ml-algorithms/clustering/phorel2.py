from phorel import *

class phorel2(phorel):
    def __init__(self,disctance:callable=d.minkovski):
        phorel.__init__(self,disctance)

    def fit(self,data,n_classes=3,initial_r=5,step=0.5):
        classes=[]
        while(len(classes)!=n_classes):
           classes=phorel.fit(self,data,n_classes,initial_r)
           if len(classes)>n_classes:
               initial_r+=step
           else:
               initial_r-=step
        return classes