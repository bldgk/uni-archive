import numpy as np
import distance as d
import matplotlib.pyplot as plt


class node:
    def __init__(self,left,right,distance,mean=None,node_value=None):
        self.left=left
        self.right=right        
        self.node_value=node_value
        self.distance=None
        if not(mean is None):
            self.node_point=mean
        else:
            self.node_point=(left+right)/2
            self.distance=distance(left,right)
    
    def __sub__(self,value):
        if type(value) is node:
            return self.node_point-value.node_point
        return self.node_point-value

    def __add__(self, value):
        if type(value) is node:
            return self.node_point+value.node_point
        return self.node_point+value

    def __radd__(self,value):
        if type(value) is node:
            return self.node_point+value.node_point
        return self.node_point+value

    def __str__(self):
        if not self.node_value is None:
            return str(self.node_value)
        if self.distance:
            return str(round(self.distance,2))
        return 'NA'

def printtree(root):
    visited=[]    
    visited.append([root])
    str_branches=[]
    n_spaces=1
    while True:
        tmp_next=[]
        tmp_n_branches=[]
        for node in visited[-1]:
            if not (node.left is None):
                tmp_next.append(node.left)
                tmp_n_branches.append('/')
                n_spaces+=1
            else:
                tmp_n_branches.append('')

            if not (node.right is None):
                tmp_next.append(node.right)
                tmp_n_branches.append('\\')
            else:
                tmp_n_branches.append('')

        if not tmp_next:
            break
        visited.append(tmp_next)
        str_branches.append(tmp_n_branches)
    str_branches.append([])

    for nodes,branches in zip(visited,str_branches):
        
        spaces=' '*n_spaces        
        print(spaces+'   '.join(map(str,nodes)))
        print(spaces+'  '.join(branches))        
        n_spaces-=1
    
        

class hclustering:
    def __init__(self,data,distance):
        self.data=data
        self.distance=distance
        self.clusters=[]
        self.clust_dists=[]
        self.tree_root=None
    
    def node_point_to_array(self,nodes):
        data=[i.node_point for i in nodes]
        return np.asarray(data)

    def fit(self,point_names):
        data=np.copy(self.data)
        dist=self.distance
        if point_names:
            nodes=[node(None,None,dist,i,n) for i,n in zip(data,point_names)]
        else:
            nodes=[node(None,None,dist,i) for i in data]        

        while len(nodes)!=1:
            data=self.node_point_to_array(nodes)
            d_matrix=d.distance_matrix(data,data,self.distance)
            np.fill_diagonal(d_matrix,np.inf)
            index=np.argmin(d_matrix)
            y,x=index//data.shape[0],index%data.shape[0]
            new_node=node(nodes[x],nodes[y],dist)
            nodes=list(np.delete(nodes,(x,y)))
            nodes.append(new_node)

        self.tree_root=nodes[0]
        return self.tree_root   

    def predict(self,n_clusters):
        if not self.tree_root:
            raise Exception('Fit first')        
        splited=[self.tree_root]
        for i in range(n_clusters-1):
            max_node=splited[0]            
            for node in splited:
                if node.distance:
                    if max_node.distance<=node.distance:
                        max_node=node
            
            if max_node.left:
                splited.append(max_node.left)
            if max_node.right:
                splited.append(max_node.right)
            splited.remove(max_node)
        
        res={}
        for i,node in enumerate(splited):
            for l in self.leaf_search(node,[]):
                res[tuple(l.node_point)]=i

        return [res[tuple(i)] for i in self.data]

    def leaf_search(self,tree,leafs=None):
        if leafs is None:
            leafs=[]
        if not (tree.right and tree.left):
            if tree not in leafs:
                leafs.append(tree)
        else:
            if tree.right:
                self.leaf_search(tree.right,leafs)
            if tree.left:
                self.leaf_search(tree.left,leafs)
        return leafs                   
                
            