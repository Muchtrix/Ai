A = [1, 2, 3, 1, 4]

def similar_lists(A):
    bigrams = {}

    for i,v in zip(A[:-1],A[1:]):
        if (i not in bigrams):
            bigrams[i] = set([v])
        else:
            bigrams[i].add(v)

    if (A[len(A) - 1] not in bigrams):
        bigrams[A[len(A) - 1]] = set()

    results = []
    current = []

    def iterate_bigrams(first):
        for big in iter(bigrams[first]):
            current.append(big)
            if (len(current) == len(A)):
                results.append(current[:])
            elif (len(current) < len(A)):
                iterate_bigrams(big)
            current.pop()

    for start in iter(bigrams):
        current.append(start)
        iterate_bigrams(start)
        current.pop()
    
    return results

for res in similar_lists(A):
    print res