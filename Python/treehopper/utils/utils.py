def constrain(x, maxVal = 1.0, minVal = 0.0):
    return max(min(x, maxVal), minVal)