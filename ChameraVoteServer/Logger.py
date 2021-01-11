from datetime import datetime

class Logger:
    def Log(message):
        f = open("log.txt", "a")
        now = datetime.now()
        timestamp = datetime.timestamp(now)
        f.write(str(timestamp)+": "+message+'\n')
        f.close()