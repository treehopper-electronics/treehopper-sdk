from logging import Logger


def error(logger: Logger, msg: str, fatal: bool):
    logger.critical(msg)
    if fatal:
        raise RuntimeError(msg)
