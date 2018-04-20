def B(i,j):
    return 'B_%d_%d' % (i,j)

def domains(Vs):
    return [ q + ' in 0..1' for q in Vs ]

def sums(rows, cols):
    def get_row(j):
        return [B(i,j) for i in xrange(len(cols))] 
            
    def get_column(i):
        return [B(i,j) for j in xrange(len(rows))] 

    p = [(get_row(i), cols[i]) for i in xrange(len(rows))] + [(get_column(i), rows[i]) for i in xrange(len(cols))]
    return ['sum([%s], #=, %d)' % (', '.join(l), s) for l, s in p]

def tripletsInColumn(col, R):
    return ['[{0}, {1}, {2}]'.format(B(x, col), B(y, col), B(z, col)) for x, y, z in zip(range(R - 2), range(1, R - 1), range(2, R))]

def tripletsInRow(row, C):
    return ['[{0}, {1}, {2}]'.format(B(row, x), B(row, y), B(row, z)) for x, y, z in zip(range(C - 2), range(1, C - 1), range(2, C))]

def triplets(R, C):
    res = []
    for i in xrange(R):
        res += tripletsInRow(i, C)
    for i in xrange(C):
        res += tripletsInColumn(i, R)
    return res

def tripletsConstraint(R, C):
    return 'tuples_in([%s], [[0,0,0], [1,0,0], [1,1,0], [0,1,1], [0,0,1], [1,0,1], [1,1,1]])' % ', '.join(triplets(R, C))

def pairs(R):
    return zip(range(R - 1), range(1, R))

def quartets(R, C):
    rw = pairs(R)
    cl = pairs(C)
    return ['[%s, %s, %s, %s]' % (B(x,a), B(y, a), B(x, b), B(y, b)) for x, y in cl for a, b in rw]

def quartetsConstraint(R, C):
    return 'tuples_in([%s], [[1,1,0,0], [0,0,1,1], [1,0,1,0], [0,1,0,1], [1,0,0,0], [0,1,0,0], [0,0,1,0], [0,0,0,1], [0,0,0,0], [1,1,1,1]])' % ', '.join(quartets(R, C))
    
def storms(raws, cols, triples):
    writeln(':- use_module(library(clpfd)).')
    
    R = len(raws)
    C = len(cols)
    
    bs = [ B(i,j) for i in range(R) for j in range(C)]
    
    writeln('solve([' + ', '.join(bs) + ']) :- ')

    for i in domains(bs):
        writeln('    %s,' % i)

    for t in triples:
        writeln('    ' + B(t[0], t[1]) + ' #= ' + str(t[2]) + ',')
    
    cs = [tripletsConstraint(R, C), quartetsConstraint(R, C)] + sums(raws, cols)
    for i in cs:
        writeln('    %s,' % i)
    
    #TODO: add some constraints
    
    #writeln('    [%s] = [1,1,0,1,1,0,1,1,0,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0],' % (', '.join(bs),)) #only for test 1

    writeln('    labeling([ff], [' +  ', '.join(bs) + ']).' )
    writeln('')
    writeln(":- tell('prolog_result.txt'), solve(X), write(X), nl, told.")

def writeln(s):
    output.write(s + '\n')

txt = open('zad_input.txt').readlines()
output = open('zad_output.txt', 'w')

raws = map(int, txt[0].split())
cols = map(int, txt[1].split())
triples = []

for i in range(2, len(txt)):
    if txt[i].strip():
        triples.append(map(int, txt[i].split()))

storms(raws, cols, triples)            
        

