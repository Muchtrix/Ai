from collections import deque

X = 3
Y = 5

def possible_moves(c):
    move_right = (0 if Y - c[1] > c[0] else c[0] - (Y - c[1]), Y if Y - c[1] <= c[0] else c[0] + c[1])
    move_left = (X if X - c[0] <= c[1] else c[0] + c[1], 0 if X - c[0] > c[1] else c[1] - (X - c[0]))
    return [(0, c[1]), (c[0], 0), (X, c[1]), (c[0], Y), move_left, move_right]

def find_solution(K):
    prevs = {(0,0): None}
    opened = deque()
    opened.append((0,0))
    finish = None

    while(len(opened) > 0):
        current = opened.popleft()
        if (K in current):
            finish = current
            break
        for m in possible_moves(current):
            if (m not in prevs):
                opened.append(m)
                prevs[m] = current
    
    res = []
    while(finish is not None):
        res.insert(0, finish)
        finish = prevs[finish]
    return res

for i in range(7):
    print i, find_solution(i)