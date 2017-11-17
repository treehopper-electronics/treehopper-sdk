def constrain(x, maxVal = 1.0, minVal = 0.0):
    return max(min(x, maxVal), minVal)


def chunks(l, n):
    """Yield successive n-sized chunks from l."""
    for i in range(0, len(l), n):
        yield l[i:i + n]
