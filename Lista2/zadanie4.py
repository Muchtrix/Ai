import time
from collections import deque

walls = set()
goals = set()
starts = set()

UP = 0
DOWN = 1
LEFT = 2
RIGHT = 3

dirs = ['U', 'D', 'L', 'R']

inp = open('zad_input.txt', 'r')
y = 0
for line in inp:
    y += 1
    x = 0
    for letter in line:
        x += 1
        if (letter == '#'):
            walls.add((x,y))
        elif (letter == 'G'):
            goals.add((x,y))
        elif (letter == 'S'):
            starts.add((x,y))
        elif (letter == 'B'):
            goals.add((x,y))
            starts.add((x,y))

def move(pos, d):
    if (d == UP):
        return (pos[0], pos[1] - 1)
    elif (d == DOWN):
        return (pos[0], pos[1] + 1)
    elif (d == LEFT):
        return (pos[0] - 1, pos[1])
    return (pos[0]+1, pos[0])

def maybe_move(pos, d):
    new = move(pos, d)
    return pos if (new in walls) else new

def move_state(st, d):
    return frozenset(map(lambda x: maybe_move(x, d), st))


fr_start = frozenset(starts)
prevs = {fr_start:(None, 0)}
opened_states = deque()
opened_states.append(fr_start)
finish = None
t0 = time.time()
iters = 0
while(True):
    iters += 1
    if (time.time() - t0 > 1):
        print 'iterations', iters
        t0 = time.time()
    current = opened_states.popleft()
    if (current.issubset(goals)):
        finish = current;
        break;
    for i in [UP, DOWN, LEFT, RIGHT]:
        nxt = move_state(current, i)
        if (nxt not in prevs):
            opened_states.append(nxt)
            prevs[nxt] = (current, i)

finish = prevs[finish]
res = []
while (finish[0] is not None):
    res.insert(0, dirs[finish[1]])
    finish = prevs[finish[0]]
output = open("zad_output.txt", 'w')
output.write(''.join(res))
output.close()

