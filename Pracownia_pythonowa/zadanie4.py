import itertools as it
slownik = [line.rstrip('\n') for line in open('./words_for_ai1.txt')]


def linewise(x, y):
    res = []
    for i in range(y):
        for j in range(x):
            res.append((j,i))
    return res

def columnwise(x, y):
    res = []
    for i in range(x):
        for j in range(y):
            res.append((i,j))
    return res

words = []
crossword = [line.rstrip('\n') for line in open('./crossword.txt')]

act_word = []
for x,y in linewise(len(crossword[0]), len(crossword)):
    if (crossword[y][x] == ' '):
        if (len(act_word) > 1):
            words.append(act_word)
        act_word = []
    elif (crossword[y][x] == '#'):
        act_word.append((x,y))
if (len(act_word) > 1):
    words.append(act_word)


act_word = []
for x,y in columnwise(len(crossword[0]), len(crossword)):
    if (crossword[y][x] == ' '):
        if (len(act_word) > 1):
            words.append(act_word)
        act_word = []
    elif (crossword[y][x] == '#'):
        act_word.append((x,y))
if (len(act_word) > 1):
    words.append(act_word)

points = tuple(set([item for sublist in words for item in sublist]))
mapping = []

def check_mapping(mapping):
    for i in range(len(mapping)):
        if (len(mapping[i]) != len(words[i])):
            return None;
    res = {}
    for idx in range(len(words)):
        for iidx in range(len(words[idx])):
            if (words[idx][iidx] not in res):
                res[words[idx][iidx]] = mapping[idx][iidx]
            elif (res[words[idx][iidx]] != mapping[idx][iidx]):
                return None;
    return res;

mapping = []
for tr in it.product(slownik, repeat=len(words)):
    mapping = check_mapping(tr)
    if (mapping is not None):
        print map(lambda x: ''.join(map(lambda y: mapping[y], x)), words)
        break
solved = []
for i in range(len(crossword)):
    line = []
    for j in range(len(crossword[i])):
        line.append(mapping.get((j,i), ' '))
    solved.append(''.join(line))
for l in solved:
    print l
