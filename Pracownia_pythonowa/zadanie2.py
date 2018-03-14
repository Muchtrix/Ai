import itertools as it

text = "send + more = money"

def solve(txt):
    s = txt.split()
    numbers = s[::2]
    letters = tuple(set(''.join(numbers)))
    not_zeros = set(''.join(map(lambda x: x[0], numbers)))
    if (len(letters) > 10):
        return {}
    
    letters_map = {}
    reverse_map = {}

    for p in it.permutations(range(10), len(letters)):
        lets_2_nums = dict(it.izip(letters, p))
        nums_2_lets = dict(it.izip(p, letters))
        if (0 not in p or nums_2_lets[0] not in not_zeros):
            n = map(lambda x: map(lambda y: lets_2_nums[y], x), numbers)
            numbs = map(lambda x: int(''.join(map(str, x))), n)
            if numbs[0] + numbs[1] == numbs[2]:
                return lets_2_nums
    return {}

print(solve(text))

    