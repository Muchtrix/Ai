#coding=utf8
import itertools as it

def evaluate(text):
    def replace_all(text, dic):
        for i, j in dic.iteritems():
            text = text.replace(i, j)
        return text

    ops = {'v': ' or ', '^': ' and ', '~': ' not '}

    letters = tuple(set(text.translate(None, 'v^()~ ')))
    for tr in it.product(['True', 'False'], repeat=(len(letters))):
        matching = dict(it.izip(letters, tr))
        with_variables = replace_all(text, matching)
        with_operators = replace_all(with_variables, ops)
        if (eval(with_operators)):
            return matching
    return {}

print evaluate('a^(b^~c)')

