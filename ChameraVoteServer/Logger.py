from datetime import datetime

class Logger:
    def Log(message):
        f = open("storage/log.txt", "a")
        now = datetime.now()
        timestamp = datetime.timestamp(now)
        f.write(str(datetime.utcfromtimestamp(timestamp).strftime('%Y-%m-%d %H:%M:%S'))+": "+message+'\n')
        f.close()