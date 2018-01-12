def constrain(x, maxVal = 1.0, minVal = 0.0):
    return max(min(x, maxVal), minVal)


def chunks(l, n):
    """Yield successive n-sized chunks from l."""
    for i in range(0, len(l), n):
        yield l[i:i + n]


def bit_list_to_bytes(bits, byteorder='big'):
    int_value = sum(2 ** i for i, v in enumerate(reversed(bits)) if v)
    return int_value.to_bytes(length=int(len(bits)/8), byteorder=byteorder)

def byte_to_bit_list(byte):
    return [((byte >> i) & 0x01) > 0 for i in range(8)]
