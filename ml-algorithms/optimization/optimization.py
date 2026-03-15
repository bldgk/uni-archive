import math
import random
from itertools import chain

def randomoptimize(domain, costf):
    best = 999999999
    bestr = None
    for i in range(1000):
        # Generate random solution
        r = [random.randint(domain[i][0], domain[i][1])
             for i in range(len(domain))]
        cost = costf(r)
        # Keep if better than current best
        if cost < best:
            best = cost
            bestr = r
            return r

def geneticoptimize(domain, costf, popsize=50, step=1, mutprob=0.2, elite=0.2, maxiter=100):
    # Mutation: tweak one gene by +/- step
    def mutate(vec):
        i = random.randint(0, len(domain) - 1)
        if random.random() < 0.5 and vec[i] > domain[i][0]:
            return vec[0:i] + [vec[i] - step] + vec[i+1:]
        elif vec[i] < domain[i][1]:
            return vec[0:i] + [vec[i] + step] + vec[i+1:]

    # Crossover: single-point splice of two parents
    def crossover(r1, r2):
        i = random.randint(1, len(domain) - 2)
        return r1[0:i] + r2[i:]

    # Build initial population
    pop = []
    for i in range(popsize):
        vec = [random.randint(domain[i][0], domain[i][1])
               for i in range(len(domain))]
        pop.append(vec)

    topelite = int(elite * popsize)

    # Main loop
    for i in range(maxiter):
        scores = [(costf(v), v) for v in pop]
        scores.sort(key=lambda x: x[0])
        ranked = [v for (s, v) in scores]

        # Keep only elite survivors
        pop = ranked[0:topelite]

        # Fill population with mutations and crossovers
        while len(pop) < popsize:
            if random.random() < mutprob:
                c = random.randint(0, topelite)
                pop.append(mutate(ranked[c]))
            else:
                c1 = random.randint(0, topelite)
                c2 = random.randint(0, topelite)
                pop.append(crossover(ranked[c1], ranked[c2]))

        print(scores[0][0])

    return scores[0][1]

def annealingoptimize(domain, costf, T=10000.0, cool=0.95, step=1):
    """Simulated annealing: accept worse solutions with decreasing probability."""
    vec = [float(random.randint(domain[i][0], domain[i][1]))
           for i in range(len(domain))]

    while T > 0.1:
        i = random.randint(0, len(domain) - 1)
        direction = random.randint(-step, step)

        vecb = vec[:]
        vecb[i] += direction
        if vecb[i] < domain[i][0]:
            vecb[i] = domain[i][0]
        elif vecb[i] > domain[i][1]:
            vecb[i] = domain[i][1]

        # Compare costs
        ea = costf(vec)
        eb = costf(vecb)
        p = pow(math.e, (-eb - ea) / T)

        # Accept if better, or probabilistically if worse
        if eb < ea or random.random() < p:
            vec = vecb

        T = T * cool

    return vec

def hillclimb(domain, costf):
    """Hill climbing: greedily move to best neighbor until no improvement."""
    sol = [random.randint(domain[i][0], domain[i][1])
           for i in range(len(domain))]

    while True:
        # Generate all single-step neighbors
        neighbors = []
        for j in range(len(domain)):
            if sol[j] > domain[j][0]:
                neighbors.append(sol[0:j] + [sol[j] + 1] + sol[j+1:])
            if sol[j] < domain[j][1]:
                neighbors.append(sol[0:j] + [sol[j] - 1] + sol[j+1:])

        # Find best neighbor
        current = costf(sol)
        best = current
        for j in range(len(neighbors)):
            cost = costf(neighbors[j])
            if cost < best:
                best = cost
                sol = neighbors[j]

        # No improvement — local optimum reached
        if best == current:
            break

    return sol
