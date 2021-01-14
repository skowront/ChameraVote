import threading
from VotingServer import VotingServer
votingServer = VotingServer(16403,True)
lock = threading.Lock()
def rawServer(lock):
    print("Raw server running.")
    votingServer.Run(lock)
rawServerThread = threading.Thread(target=rawServer,args=(lock,))
rawServerThread.start() 

from aiohttp import web
import socketio

sio = socketio.AsyncServer(cors_allowed_origins='*')
app = web.Application()
sio.attach(app)

async def index(request):
    with open('index.html') as f:
        return web.Response(text=f.read(), content_type='text/html')

@sio.on('connect', namespace='/vote')
def connect(sid, environ):
    print("connect", sid)

@sio.on('message', namespace='/vote')
async def message(sid, data):
    print("server received message!", data)
    lock.acquire()
    res = votingServer.BuildResponse(data)
    lock.release()
    print('Response: '+res)
    await sio.emit('reply', res,namespace='/vote')

@sio.on('disconnect', namespace='/vote')
def disconnect(sid):
    print('disconnect', sid)

app.router.add_get('/', index)

if __name__ == '__main__':
    web.run_app(app,host="localhost",port="16402")