# ML Algorithms from Scratch

Hand-rolled machine learning algorithms in Python (2016). No scikit-learn for the algorithms themselves — only for generating test data.

## Clustering (`clustering/`)

All algorithms implement `fit` / `fit_predict` and use a common `distance.py` module with Minkowski, Euclidean, Manhattan, and Chebyshev metrics.

| File | Algorithm | How it works |
|------|-----------|-------------|
| `kmeans.py` | **K-Means** | Smart center initialization (farthest-point), iterative reassignment |
| `kmedoids.py` | **K-Medoids** | Like K-Means but centers must be actual data points |
| `dbscan.py` | **DBSCAN** | Density-based clustering, expands clusters by radius |
| `hclustering.py` | **Hierarchical** | Agglomerative with dendrogram tree, split by max distance |
| `phorel.py` | **FOREL** | Hypersphere-based clustering, shifts center to cluster mean |
| `phorel2.py` | **FOREL v2** | Auto-tunes radius to get exact number of clusters |
| `prima.py` | **Prim's MST** | Minimum spanning tree for graph-based clustering |
| `crab.py` | **Crab** | MST-based: builds Prim's tree, cuts longest edges |
| `spectr.py` | **Spectral** | Spectral gap analysis on incremental distance spectrum |
| `distance.py` | Distance metrics | Minkowski, Euclidean, Manhattan, Chebyshev + distance matrix |

```bash
cd clustering
python3 main.py
```

Requires: `numpy`, `matplotlib`, `scikit-learn` (for test data generation only)

## Optimization (`optimization/`)

| File | Algorithms |
|------|-----------|
| `optimization.py` | **Genetic algorithm**, **simulated annealing**, **hill climbing**, random search |
| `traveling.ipynb` | Traveling salesman problem |
| `cars.ipynb` | Vehicle optimization |
| `socialnetwork.ipynb` | Social network analysis |
| `students.ipynb` | Student data analysis |

```bash
pip3 install numpy matplotlib scikit-learn
```
