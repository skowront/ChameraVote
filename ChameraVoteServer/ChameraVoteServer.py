import threading
from VotingServer import VotingServer
from Configuration import Configuration
votingServer = VotingServer(Configuration.ServerPort,True)
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

@sio.on('connect', namespace='')
def connect(sid, environ):
    print("connect", sid)

@sio.on('message', namespace='')
async def message(sid, data):
    print("server received message!", data)
    lock.acquire()
    res = votingServer.BuildResponse(data)
    lock.release()
    print('Response: '+res)
    await sio.emit('reply', res.encode('utf-8') ,namespace='',room=sid)

@sio.on('disconnect', namespace='')
def disconnect(sid):
    print('disconnect', sid)

#app.router.add_get('/', index)

if __name__ == '__main__':
    web.run_app(app,host=Configuration.ServerAddress,port=Configuration.ServerWebPort)